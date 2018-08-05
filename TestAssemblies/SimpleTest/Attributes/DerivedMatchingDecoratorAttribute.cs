﻿using System;
using System.Threading.Tasks;
using System.Reflection;

namespace SimpleTest
{
	public class DerivedMatchingDecoratorAttribute : MatchingDecoratorAttribute
	{
		public void Init(object instance, MethodBase method, object[] args)
		{
			if(null == method) throw new ArgumentNullException("method");
			TestRecords.RecordInit(instance, method.DeclaringType.FullName + "." + method.Name, args.Length);
		}
		public void OnEntry()
		{
			TestRecords.RecordOnEntry();
		}

		public void OnExit()
		{
			TestRecords.RecordOnExit();
		}

		public void OnException(Exception exception)
		{
			TestRecords.RecordOnException(exception.GetType(), exception.Message);
		}

		public void OnTaskContinuation(Task t)
		{
			TestRecords.RecordOnContinuation();
		}
	}
}
