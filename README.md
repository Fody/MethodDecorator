![Icon](https://raw.github.com/Fody/MethodDecorator/master/Icons/package_icon.png)

## The nuget package  [![NuGet Status](http://img.shields.io/nuget/v/MethodDecorator.Fody.svg?style=flat)](https://www.nuget.org/packages/MethodDecorator.Fody/)

https://nuget.org/packages/MethodDecorator.Fody/

    PM> Install-Package MethodDecorator.Fody

## This is an add-in for [Fody](https://github.com/Fody/Fody/)

Compile time decorator pattern via IL rewriting

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

### Your Code

```c#
// Atribute should be "registered" by adding as module or assembly custom attribute
[module: Interceptor]

// Any attribute which provides OnEntry/OnExit/OnException with proper args
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
public class InterceptorAttribute : Attribute, IMethodDecorator	{
    // instance, method and args can be captured here and stored in attribute instance fields
	// for future usage in OnEntry/OnExit/OnException
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
```

### What's gets compiled

```c#
public class Sample {
	public void Method(int value) {
	    InterceptorAttribute attribute =
	        (InterceptorAttribute) Activator.CreateInstance(typeof(InterceptorAttribute));

		// in c# __methodref and __typeref don't exist, but you can create such IL
		MethodBase method = MethodBase.GetMethodFromHandle(__methodref (Sample.Method),
														   __typeref (Sample));

		object[] args = new object[1] { (object) value };

		attribute.Init((object)this, method, args);

		attribute.OnEntry();
	    try {
	        Debug.WriteLine("Your Code");
	        attribute.OnExit();
	    }
	    catch (Exception exception) {
	        attribute.OnException(exception);
	        throw;
	    }
	}
}
```

**NOTE:** *this* is replaced by *null* when the decorated method is static or a constructor.

### IntersectMethodsMarkedByAttribute

This is supposed to be used as

```c#
// all MSTest methods will be intersected by the code from IntersectMethodsMarkedBy
[module:IntersectMethodsMarkedBy(typeof(TestMethod))]
```

You can pass as many marker attributes to `IntersectMethodsMarkedBy` as you want

```c#
[module:IntersectMethodsMarkedBy(typeof(TestMethod), typeof(Fact), typeof(Obsolete))]
```

Example of `IntersectMethodsMarkedByAttribute` implementation

```c#
[AttributeUsage(AttributeTargets.Module | AttributeTargets.Assembly)]
public class IntersectMethodsMarkedByAttribute : Attribute {
	// Required
	public IntersectMethodsMarkedByAttribute() {}

	public IntersectMethodsMarkedByAttribute(params Type[] types) {
		if (types.All(x => typeof(Attribute).IsAssignableFrom(x))) {
			throw new Exception("Meaningfull configuration exception");
		}
	}
	public void Init(object instance, MethodBase method, object[] args) {}
	public void OnEntry() {}
	public void OnExit() {}
	public void OnException(Exception exception) {}
    // Optional
    //public void OnTaskContinuation(Task task) {}
}
```

Now all your code marked by [TestMethodAttribute] will be intersected by IntersectMethodsMarkedByAttribute methods.
You can have multiple IntersectMethodsMarkedByAttributes applied if you want (don't have idea why).
MethodDecorator searches IntersectMethodsMarkedByAttribute by predicate StartsWith("IntersectMethodsMarkedByAttribute")

In case of exception in async method you "OnException" will not be called, OnTaskContinuation will be called instead.

### Recent changes

- 2016-04-18 .net2 support added by https://github.com/dterziev, old package name is avaliable through nuget again, no more Ex.
- 2015-10-30 Async support added by https://github.com/KonstantinFinagin
- 2015-10-04 Mono Cecil package udapted to work with Visual Studio 2015

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)
