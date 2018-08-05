﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SimpleTest.PnP
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module, AllowMultiple =true)]
    internal class InterceptorWithParamsAttribute : Attribute
    {
        public string StringProperty { get; set; }
        public string StringField;

        private int _Int;
        private string _String;

        public InterceptorWithParamsAttribute() { }

        public InterceptorWithParamsAttribute( int iInt, string iString)
        {
            _Int = iInt;
            _String = iString;
        }

        public void Init(object instance, MethodBase method, object[] args)
        {
            if (null == method) throw new ArgumentNullException("method");
            TestRecords.Record(Method.Init, new object[] { _Int, _String,  StringProperty, StringField });
            ++_Int;
        }

        public void OnEntry()
        {
            TestRecords.RecordOnEntry();
        }

        public void OnExit()
        {
            TestRecords.Record(Method.OnExit, new object[] { _Int, _String, StringProperty, StringField });
        }

        public void OnExit(object retval)
        {
            TestRecords.Record(Method.OnExit, new object[] { retval });
        }

        public void OnException(Exception exception)
        {
            TestRecords.RecordOnException(exception.GetType(), exception.Message);
        }
    }
}
