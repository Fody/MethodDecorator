using MethodDecoratorInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTest
{
	[AttributeUsage( AttributeTargets.Method 
				   | AttributeTargets.Constructor
				   | AttributeTargets.Class 
				   | AttributeTargets.Module 
				   | AttributeTargets.Assembly)]
	public class MatchingDecoratorAttribute : Attribute, IAspectMatchingRule, IMethodDecorator
	{
		public string AttributeTargetTypes { get; set; }
		public bool AttributeExclude { get; set; }
		public int AttributePriority { get; set; }
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
