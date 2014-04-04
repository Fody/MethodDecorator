## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Compile time decorator pattern via IL rewriting.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

This version is fork of [Fody/MethodDecorator](https://github.com/Fody/MethodDecorator) with changes I found useful

Differences from original Fody/MethodDecorator:
* No attributes or interfaces in root namespace (actually without namespace) required
* Interceptor attribute can be declared and implemented in separate assembly
* Init method called before any method and receiving method reference and method args 
* OnEntry/OnExit/OnException methods don't receiving method reference anymore
* IntersectMethodsMarkedByAttribute attribute allows you to interserct method marked by any attribute

### Your Code
	//Atribute should be "registered" by adding as module or assembly custom attribute
	[module: Interceptor]
	
	//Any attribute which provide OnEntry/OnExit/OnException with proper args
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
	public class InterceptorAttribute : Attribute, IMethodDecorator	{
	    public void Init(object instance, MethodBase method, object[] args) {
			TestMessages.Record(string.Format("Init: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length));
		}
		public void OnEntry() {
	        TestMessages.Record("OnEntry");
	    }
	
	    public void OnExit() {
	        TestMessages.Record("OnExit");
	    }
	
	    public void OnException(Exception exception) {
	        TestMessages.Record(string.Format("OnException: {0}: {1}", exception.GetType(), exception.Message));
	    }
	}
	
	public class Sample	{
		[Interceptor]
		public void Method()
		{
		    Debug.WriteLine("Your Code");
		}
	}

### What gets compiled
	
	public class Sample
	{
		public void Method(int value)
		{
		    MethodBase method = methodof(Sample.Method, Sample);
		    InterceptorAttribute attribute = (InterceptorAttribute) method.GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
		    object[] args = new object[1] { (object) value };
			attribute.Init((object)this, methodFromHandle, args);

			attribute.OnEntry();
		    try
		    {
		        Debug.WriteLine("Your Code");
		        attribute.OnExit();
		    }
		    catch (Exception exception)
		    {
		        attribute.OnException(exception);
		        throw;
		    }
		}
	}

**NOTE:** *this* is replaced by *null* when the decorated method is static or a constructor.

### IntersectMethodsMarkedByAttribute

This supposed to used as	
	//all ms test methods will be intersected by code from IntersectMethodsMarkedBy 
	[module:IntersectMethodsMarkedBy(typeof(TestMethod))] 

Example of implementation of IntersectMethodsMarkedByAttribute

	[AttributeUsage(AttributeTargets.Module | AttributeTargets.Assembly)]
	public class IntersectMethodsMarkedByAttribute : Attribute {
		//Must have
		public IntersectMethodsMarkedByAttribute() {}

		public IntersectMethodsMarkedByAttribute(params Type[] types) {
			if (types.All(x => typeof(Attribute).IsAssignableFrom(x))) {
				throw new Exception("Meaningfull configuration exception");
			}
		}
		public void OnEntry() {}
		public void OnExit() {}
		public void OnException(Exception exception) {}
	}

Then all your code marked by attribyte [TestMethod] will be intersected by IntersectMethodsMarkedByAttribute methods.
You can have many IntersectMethodsMarkedByAttribute if you want (don't have idea why). 
MethodDecorator search IntersectMethodsMarkedByAttribute by predicate StartsWith("IntersectMethodsMarkedByAttribute")

### How to get it

NuGet: https://www.nuget.org/packages/MethodDecoratorEx.Fody/
	
### Planned

- [x] Make Init method optional
- [x] Add "this" as parameter to Init method if method is not static
- [ ] Pass return value to "OnExit" if method returns any

Fill free to request for features you want to see in this plugin.