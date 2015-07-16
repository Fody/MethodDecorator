namespace MethodDecorator.Fody.Tests
{
    using System;
    using System.Dynamic;
    using System.Reflection;

    // With thanks to David Ebbo
    // http://blogs.msdn.com/b/davidebb/archive/2009/10/23/using-c-dynamic-to-call-static-members.aspx
    public class StaticMembersDynamicWrapper : DynamicObject
    {
        private readonly Type type;

        public StaticMembersDynamicWrapper(Type type)
        {
            this.type = type;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var prop = this.type.GetProperty(
                binder.Name,
                BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (prop == null)
            {
                result = null;
                return false;
            }

            result = prop.GetValue(null, null);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var method = this.type.GetMethod(
                binder.Name,
                BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public);
            if (method == null)
            {
                result = null;
                return false;
            }

            result = method.Invoke(null, args);
            return true;
        }
    }
}