# <img src="/package_icon.png" height="30px"> MethodDecorator.Fody

[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)
[![NuGet Status](https://img.shields.io/nuget/v/MethodDecorator.Fody.svg)](https://www.nuget.org/packages/MethodDecorator.Fody/)

Compile time decorator pattern via IL rewriting.


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody either [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059), or have a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-fody?utm_source=nuget-fody&utm_medium=referral&utm_campaign=enterprise). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [MethodDecorator.Fody NuGet package](https://nuget.org/packages/MethodDecorator.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package MethodDecorator.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Your Code

```c#
// Attribute should be "registered" by adding as module or assembly custom attribute
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


### What gets compiled

```c#
public class Sample {
    public void Method(int value) {
        InterceptorAttribute attribute =
            (InterceptorAttribute) Activator.CreateInstance(typeof(InterceptorAttribute));

        // in c# __methodref and __typeref don't exist, but you can create such IL
        MethodBase method = MethodBase.GetMethodFromHandle(__methodref (Sample.Method), __typeref (Sample));

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


## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)