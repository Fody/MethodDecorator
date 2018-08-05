﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTest.PnP
{
    public enum Method
    {
        Init = 0,
        OnEnter = 1,
        Body = 2,
        OnExit = 3,
        OnException = 4,
        OnContinuation = 5
    }

    public static class TestRecords
    {
        private static readonly IList<Tuple<int, object[]>> _records = new List<Tuple<int, object[]>>();

        public static void Clear()
        {
            _records.Clear();
        }

        public static void RecordOnEntry()
        {
            Record(Method.OnEnter);
        }

        public static void RecordOnExit()
        {
            Record(Method.OnExit);
        }

        public static void RecordOnException(Type exType, string exMessage)
        {
            Record(Method.OnException, new object[] { exType, exMessage });
        }

        public static void RecordInit(object instance, string methodName, int argLength)
        {
            Record(Method.Init, new object[] { instance, methodName, argLength });
        }

        public static void RecordBody(string name, string extraInfo = null)
        {
            Record(Method.Body, new object[] { name, extraInfo });
        }

        internal static void Record(Method method, object[] args = null)
        {
            _records.Add( new Tuple<int,object[]> ( (int) method, args ));
        }

        public static IList<Tuple<int, object[]>> Records => _records;

        public static void RecordOnContinuation()
        {
            Record(Method.OnContinuation);
        }
    }
}
