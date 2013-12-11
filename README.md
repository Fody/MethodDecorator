## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Compile time decorator pattern via IL rewriting.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

This version is fork of [Fody/MethodDecorator](https://github.com/Fody/MethodDecorator) with changes I found useful

Differneces from original Fody/MethodDecorator:
* No attributes or interfaces in root namespace (actually without namespace) required
* Interceptor attribute can be declared and implemented in separate assembly
* OnEntry receiving method parameters

### Your Code
	//Atribute should be "registred" by adding as module custom attribute (assembly attributes registration is on the way)
	[module: Interceptor]
	
	//Any attribute which provide OnEntry/OnExit/OnException with proper args
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
	public class InterceptorAttribute : Attribute, IMethodDecorator
	{
	    public void OnEntry(MethodBase method, object[] args)
	    {
	        TestMessages.Record(string.Format("OnEntry: {0} [{1}]", method.DeclaringType.FullName + "." + method.Name, args.Length));
	    }
	
	    public void OnExit(MethodBase method)
	    {
	        TestMessages.Record(string.Format("OnExit: {0}", method.DeclaringType.FullName + "." + method.Name));
	    }
	
	    public void OnException(MethodBase method, Exception exception)
	    {
	        TestMessages.Record(string.Format("OnException: {0} - {1}: {2}", method.DeclaringType.FullName + "." + method.Name, exception.GetType(), exception.Message));
	    }
	}
	
	public class Sample
	{
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
      		    attribute.OnEntry(methodFromHandle, args);
		    try
		    {
		        Debug.WriteLine("Your Code");
		        attribute.OnExit(method);
		    }
		    catch (Exception exception)
		    {
		        attribute.OnException(method, exception);
		        throw;
		    }
		}
	}