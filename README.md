## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/MethodDecorator/master/Icons/package_icon.png)

Compile time decorator pattern via IL rewriting

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget

Nuget package http://nuget.org/packages/MethodDecorator.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package MethodDecorator.Fody
    
### Your Code

	public interface IMethodDecorator
	{
	    void OnEntry(MethodBase method);
	    void OnExit(MethodBase method);
	    void OnException(MethodBase method, Exception exception);
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
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
		    object[] args = new object[1]
		    {
        		(object) value
      		    };
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

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)



