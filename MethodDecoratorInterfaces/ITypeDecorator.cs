using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MethodDecoratorInterfaces
{
	public interface ITypeDecorator
	{
		string AttributeTargetTypes { get; set; }
		bool AttributeExclude { get; set; }
		int AttributePriority { get; set; }
	}
}
