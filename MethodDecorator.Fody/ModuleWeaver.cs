using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MethodDecorator.Fody;

using Mono.Cecil;
using MethodDecoratorInterfaces;
using System.Text.RegularExpressions;
using Mono.Collections.Generic;

public class ModuleWeaver {
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public Action<string> LogError { get; set; }


    public void Execute() {
        this.LogInfo = s => { };
        this.LogWarning = s => { };

        var decorator = new MethodDecorator.Fody.MethodDecorator(this.ModuleDefinition);

        foreach (var x in this.ModuleDefinition.AssemblyReferences) AssemblyResolver.Resolve(x);

        //this.DecorateDirectlyAttributed(decorator);
        this.DecorateAttributedByImplication(decorator);
		this.DecorateByType(decorator);

        if(this.ModuleDefinition.AssemblyReferences.Count(r => r.Name == "mscorlib") > 1) {
            throw new Exception(
                String.Format(
                    "Error occured during IL weaving. The new assembly is now referencing more than one version of mscorlib: {0}",
                    String.Join(", ", this.ModuleDefinition.AssemblyReferences.Where(r => r.Name == "mscorlib").Select(r => r.FullName))
                )
            );
        }
    }

	private void DecorateByType(MethodDecorator.Fody.MethodDecorator decorator)
	{
		var referenceFinder = new ReferenceFinder(this.ModuleDefinition);
		var markerTypeDefinitions = this.FindMarkerTypes();

		var rulesStack = new Stack<IEnumerable<MulticastThingy>>();

		rulesStack.Push(ParseTypeDecorators(this.ModuleDefinition.Assembly.CustomAttributes, 4));
		rulesStack.Push(ParseTypeDecorators(this.ModuleDefinition.CustomAttributes, 3));


		// Read the top-level and nested types from this module
		foreach(var type in this.ModuleDefinition.Types.SelectMany(x => GetAllTypes(x)))
		{
			rulesStack.Push(ParseTypeDecorators(type.CustomAttributes, 2));
			rulesStack.Push(FindAttributedMethods2(markerTypeDefinitions, type.CustomAttributes, 2));

			foreach(var method in type.Methods.Where(x => x.HasBody))
			{
				rulesStack.Push(ParseTypeDecorators(method.CustomAttributes, 1));
				rulesStack.Push(FindAttributedMethods2(markerTypeDefinitions, method.CustomAttributes, 1));

				var allRules = rulesStack.SelectMany(x => x);

				// Group the rules by the aspect type
				foreach(var aspectSet in
					allRules.ToLookup(x => x.MethodDecoratorAttribute.AttributeType))
				{
					var rule = aspectSet
						.Where(x => x.Match(type, method) || x.ScopePriority == 1)
						.OrderBy(x => x.AttributePriority)
						.ThenBy(x => x.ScopePriority)
						.FirstOrDefault();

					if(rule != null && !rule.AttributeExclude)
					{
						decorator.Decorate(
							type,
							method,
							rule.MethodDecoratorAttribute);
					}
				}

				rulesStack.Pop();
				rulesStack.Pop();
			}

			rulesStack.Pop();
			rulesStack.Pop();
		}


	}

	private IEnumerable<MulticastThingy> FindAttributedMethods2(
		IEnumerable<TypeDefinition> markerTypeDefinitions,
		Collection<CustomAttribute> customAttributes,
		int scopePriority)
	{
		foreach(var attr in customAttributes)
		{
			var attributeTypeDef = attr.AttributeType.Resolve();

			foreach(var markerTypeDefinition in markerTypeDefinitions)
			{
				if(attributeTypeDef.Implements(markerTypeDefinition)
					|| attributeTypeDef.DerivesFrom(markerTypeDefinition)
					|| this.AreEquals(attributeTypeDef, markerTypeDefinition))
				{
					yield return new MulticastThingy()
					{
						MethodDecoratorAttribute = attr,
						AttributeExclude = false,
						AttributePriority = 0,
						ScopePriority = scopePriority,
						SuperMatch = true
					};
				}
			}
		}
	}

	private IEnumerable<AttributeMethodInfo> FindAttributedMethods(IEnumerable<TypeDefinition> markerTypeDefintions)
	{
		return from topLevelType in this.ModuleDefinition.Types
			   from type in GetAllTypes(topLevelType)
			   from method in type.Methods
			   where method.HasBody
			   from attribute in method.CustomAttributes.Concat(method.DeclaringType.CustomAttributes)
			   let attributeTypeDef = attribute.AttributeType.Resolve()
			   from markerTypeDefinition in markerTypeDefintions
			   where attributeTypeDef.Implements(markerTypeDefinition) ||
					 attributeTypeDef.DerivesFrom(markerTypeDefinition) ||
					 this.AreEquals(attributeTypeDef, markerTypeDefinition)
			   select new AttributeMethodInfo
			   {
				   CustomAttribute = attribute,
				   TypeDefinition = type,
				   MethodDefinition = method
			   };
	}

	private void DecorateDirectlyAttributed(MethodDecorator.Fody.MethodDecorator decorator)
	{
		var markerTypeDefinitions = this.FindMarkerTypes();

		var methods = this.FindAttributedMethods(markerTypeDefinitions.ToArray());
		foreach(var x in methods)
			decorator.Decorate(x.TypeDefinition, x.MethodDefinition, x.CustomAttribute);
	}

	private IEnumerable<TypeDefinition> FindMarkerTypes()
	{
		var allAttributes = this.GetAttributes();

		var markerTypeDefinitions = (from type in allAttributes
									 where HasCorrectMethods(type)
									 select type).ToList();

		if(!markerTypeDefinitions.Any())
		{
			if(null != LogError)
				LogError("Could not find any method decorator attribute");
			throw new WeavingException("Could not find any method decorator attribute");
		}

		return markerTypeDefinitions;
	}


	private IEnumerable<MulticastThingy> ParseTypeDecorators(
		IEnumerable<CustomAttribute> attrs,
		int scopePriority)
	{
		return attrs
			.Where(attr => IsTypeDecorator(attr))
			.Select(attr => new MulticastThingy()
		{
			AttributeTargetTypes = GetAttributeProperty<string>(attr, "AttributeTargetTypes"),
			AttributeExclude = GetAttributeProperty<bool>(attr, "AttributeExclude"),
			AttributePriority = GetAttributeProperty<int>(attr, "AttributePriority"),
			ScopePriority = scopePriority,
			MethodDecoratorAttribute = attr
		});
	}

	private T GetAttributeProperty<T>(CustomAttribute attr, string propertyName)
	{
		if(!attr.Properties.Any(x => x.Name == propertyName))
			return default(T);

		return (T)attr.Properties.First(x => x.Name == propertyName).Argument.Value;
	}

	private bool IsTypeDecorator(CustomAttribute x)
	{
		var typeDefinition = x.AttributeType.Resolve();

		// Avoid problem on initial load of types where mscorlib not loaded - the Implements()
		// method crashes if this happens.
		if(!typeDefinition.Module.AssemblyReferences.Any(a => a.Name == "mscorlib"))
			return false;

		return typeDefinition.Implements(typeof(ITypeDecorator));
	}

	private void DecorateAttributedByImplication(MethodDecorator.Fody.MethodDecorator decorator) {
        var inderectAttributes = this.ModuleDefinition.CustomAttributes
                                     .Concat(this.ModuleDefinition.Assembly.CustomAttributes)
                                     .Where(x => x.AttributeType.Name.StartsWith("IntersectMethodsMarkedByAttribute"))
                                     .Select(ToHostAttributeMapping)
                                     .Where(x=>x!=null)
                                     .ToArray();

        foreach (var inderectAttribute in inderectAttributes) {
            var methods = this.FindAttributedMethods(inderectAttribute.AttribyteTypes);
            foreach (var x in methods)
                decorator.Decorate(x.TypeDefinition, x.MethodDefinition, inderectAttribute.HostAttribute);
        }
    }

    private HostAttributeMapping ToHostAttributeMapping(CustomAttribute arg) {
        var prms = arg.ConstructorArguments.First().Value as CustomAttributeArgument[];
        if (null == prms)
            return null;
        return new HostAttributeMapping {
            HostAttribute = arg,
            AttribyteTypes = prms.Select(c => ((TypeReference)c.Value).Resolve()).ToArray()
        };
    }




    private IEnumerable<TypeDefinition> GetAttributes() {
        
        var res = new List<TypeDefinition>();

        res.AddRange(this.ModuleDefinition.CustomAttributes.Select(c => c.AttributeType.Resolve()));
        res.AddRange(this.ModuleDefinition.Assembly.CustomAttributes.Select(c => c.AttributeType.Resolve()));

        if (this.ModuleDefinition.Runtime >= TargetRuntime.Net_4_0) {
            //will find if assembly is loaded
            var methodDecorator = Type.GetType("MethodDecoratorInterfaces.IMethodDecorator, MethodDecoratorInterfaces");

            //make using of MethodDecoratorEx assembly optional because it can break exists code
            if (null != methodDecorator) {
                
                res.AddRange(this.ModuleDefinition.Types.Where(c => c.Implements(methodDecorator)));
            }
        }

        return res;
    }

    private static bool HasCorrectMethods(TypeDefinition type) {
        return type.Methods.Any(IsOnEntryMethod) &&
               type.Methods.Any(IsOnExitMethod) &&
               type.Methods.Any(IsOnExceptionMethod);
    }

    private static bool IsOnEntryMethod(MethodDefinition m) {
        return m.Name == "OnEntry" &&
               m.Parameters.Count == 0;
    }

    private static bool IsOnExitMethod(MethodDefinition m) {
        return m.Name == "OnExit" &&
               m.Parameters.Count == 0;
    }

    private static bool IsOnExceptionMethod(MethodDefinition m) {
        return m.Name == "OnException" && m.Parameters.Count == 1 &&
               m.Parameters[0].ParameterType.FullName == typeof(Exception).FullName;
    }

    private static bool IsOnTaskContinuationMethod(MethodDefinition m) {
        return m.Name == "OnTaskContinuation" && m.Parameters.Count == 1
            && m.Parameters[0].ParameterType.FullName == typeof(Task).FullName;
    }



    private bool AreEquals(TypeDefinition attributeTypeDef, TypeDefinition markerTypeDefinition) {
        return attributeTypeDef.FullName == markerTypeDefinition.FullName;
    }

    private static IEnumerable<TypeDefinition> GetAllTypes(TypeDefinition type) {
        yield return type;

        var allNestedTypes = from t in type.NestedTypes
                             from t2 in GetAllTypes(t)
                             select t2;

        foreach (var t in allNestedTypes)
            yield return t;
    }

    private class HostAttributeMapping {
        public TypeDefinition[] AttribyteTypes { get; set; }
        public CustomAttribute HostAttribute { get; set; }
    }

    private class AttributeMethodInfo {
        public TypeDefinition TypeDefinition { get; set; }
        public MethodDefinition MethodDefinition { get; set; }
        public CustomAttribute CustomAttribute { get; set; }
    }

	private class MulticastThingy {
		public string AttributeTargetTypes { get; set; }
		public bool AttributeExclude { get; set; }
		public int AttributePriority { get; set; }
		public CustomAttribute MethodDecoratorAttribute { get; set; }
		public int ScopePriority { get; internal set; }
		public bool SuperMatch { get; internal set; }

		internal bool Match(TypeDefinition type, MethodDefinition method)
		{
			var wildcard = this.AttributeTargetTypes;

			if(wildcard == null)
				return this.SuperMatch;

			string regexPrefix = "regex:";

			string pattern;
			if(wildcard.StartsWith(regexPrefix))
			{
				pattern = wildcard.Substring(regexPrefix.Length);
			}
			else
			{
				pattern = String.Join(".*",
					wildcard.Split(new[] { '*' })
					.Select(x => Regex.Escape(x)));
			}

			var result = Regex.IsMatch(type.Namespace + "." + method.Name, pattern);
			return result;
		}
	}
}


//var attrs = this.ModuleDefinition.CustomAttributes
//	.Concat(this.ModuleDefinition.Assembly.CustomAttributes)
//	.Where(x => IsTypeDecorator(x))
//	.ToArray();

//var rules = ParseTypeDecorators(attrs).ToArray();

//if(rules.Length == 0)
//	return;

//var methods = (from topLevelType in this.ModuleDefinition.Types
//			   from type in GetAllTypes(topLevelType)
//			   from method in type.Methods
//			   where method.HasBody
//			   select new
//			   {
//				   TypeDefinition = type,
//				   MethodDefinition = method
//			   }).ToArray();

//var methodsByNamespace = methods.ToLookup(x => x.TypeDefinition.Namespace);

//foreach(var item in methodsByNamespace)
//{
//	string ns = item.Key;

//	if(ns == "SimpleTest.MarkedWithTypeNS")
//	{
//		foreach(var method in item)
//		{
//			method.MethodDefinition.CustomAttributes.Where(x => IsTypeDecorator(x))

//		decorator.Decorate(
//			method.TypeDefinition,
//			method.MethodDefinition,
//			rules[0].MethodDecoratorAttribute);
//	}
//}
//}