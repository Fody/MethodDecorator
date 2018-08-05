using MethodDecorator.Fody.Interfaces;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleTest
{
	[AttributeUsage( AttributeTargets.Method 
				   | AttributeTargets.Constructor
				   | AttributeTargets.Class 
				   | AttributeTargets.Module 
				   | AttributeTargets.Assembly,
				   AllowMultiple=true)]
	public class MatchingDecoratorAttribute : Attribute, IAspectMatchingRule, IMethodDecorator
	{
		public virtual string AttributeTargetTypes { get; set; }
		public virtual bool AttributeExclude { get; set; }
		public virtual int AttributePriority { get; set; }
        public virtual int AspectPriority { get; set; }
        public MatchingDecoratorAttribute()
		{
		}

		public virtual void Init(object instance, MethodBase method, object[] args) { }

		public virtual void OnEntry() { }

		public virtual void OnExit() { }

		public virtual void OnException(Exception exception) { }

		public virtual void OnTaskContinuation(Task task) { }
	}
}
