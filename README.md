## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/MethodDecorator/master/Icons/package_icon.png)

Compile time decorator pattern via IL rewriting

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/MethodDecorator.Fody.svg?style=flat)](https://www.nuget.org/packages/MethodDecorator.Fody/)

https://nuget.org/packages/MethodDecorator.Fody/

    PM> Install-Package MethodDecorator.Fody

### Your Code

Define the ````IMethodDecorator```` interface (exact name) _without_ a namespace:

	public interface IMethodDecorator
	{
	    void OnEntry(MethodBase method);
	    void OnExit(MethodBase method);
	    void OnException(MethodBase method, Exception exception);
	}
	
Define your method decorators by deriving from ````Attribute```` and implementing ````IMethodDecorator````:

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
	public class InterceptorAttribute : Attribute, IMethodDecorator
	{
	    public void OnEntry(MethodBase method)
	    {
	        TestMessages.Record(string.Format("OnEntry: {0}", method.DeclaringType.FullName + "." + method.Name));
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
		public void Method()
		{
		    MethodBase method = methodof(Sample.Method, Sample);
		    InterceptorAttribute attribute = (InterceptorAttribute) method.GetCustomAttributes(typeof(InterceptorAttribute), false)[0];
		    attribute.OnEntry(method);
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



