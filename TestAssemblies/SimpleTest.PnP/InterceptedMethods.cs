using System;

namespace SimpleTest.PnP
{
    public class InterceptedMethods
    {
        public InterceptedMethods()
        {
        }

        [InterceptorWithParams(11, "parameter", StringProperty = "property", StringField = "field")]
        public InterceptedMethods(string test)
        {
            TestRecords.RecordBody("InterceptedMethods(test)");
        }

        [InterceptorWithParams(12, "parameter", StringProperty = "property", StringField = "field")]
        public InterceptedMethods(int i): this(i.ToString())
        {
            TestRecords.RecordBody("InterceptedMethods(i)");
        }

        [InterceptorWithParams(15, "parameter" , StringProperty = "property", StringField = "field")]
        public void ExplicitIntercepted()
        {
            TestRecords.RecordBody("ExplicitIntercepted");
        }

        [InterceptorWithParams(1, "parameter", StringProperty = "property", StringField = "field")]
        public void OuterMethod()
        {
            TestRecords.RecordBody("OuterMethod");
            InnerMethod();
        }

        [InterceptorWithParams(1, "parameter", StringProperty = "property", StringField = "field")]
        void InnerMethod()
        {
            TestRecords.RecordBody("InnerMethod");
        }

        [InterceptorWithPriority("attr3", AttributePriority = 3)]
        [InterceptorWithPriority("attr5", AttributePriority = 5)]
        [InterceptorWithPriority("attr1", AttributePriority = 1)]
        [InterceptorWithPriority("attr2", AttributePriority = 2)]
        public void MultipleInterceptedWithPriority()
        {
            TestRecords.RecordBody("MultipleInterceptedWithPriority");
        }

        [InterceptorWithPriority("attr5")]
        [InterceptorWithPriority("attr2")]
        [InterceptorWithPriority("attr1")]
        [InterceptorWithPriority("attr3")]
        public void MultipleIntercepted()
        {
            TestRecords.RecordBody("MultipleIntercepted");
        }

        [InterceptorWithParams]
        public int InterceptedReturns10()
        {
            return 10;
        }

        [InterceptorWithParams]
        public string InterceptedReturnsString()
        {
            return "Intercepted";
        }

        [InterceptorWithParams]
        public object InterceptedReturnsType()
        {
            return typeof(InterceptedMethods);
        }

        class MyClass1: IDisposable
        {
            public void Dispose() { }
        }

        class MyClass2 : IDisposable
        {
            public void Dispose() { }
        }

        [InterceptorWithParams]
        public IDisposable InterceptedReturnsImplicitCasted()
        {
            var rnd = new Random(5);
            if(rnd.Next()%2>0)
            {
                return new MyClass1();
            }
            else
            {
                return new MyClass2();
            }
        }

        [InterceptorWithParams]
        public int SomeLongMethod()
        {
            var i = 0;

            if((++i)%13 != 0)
            {
                if ((++i) % 13 != 0)
                {
                    if ((++i) % 13 != 0)
                    {
                        if ((++i) % 13 != 0)
                        {
                            if ((++i) % 13 != 0)
                            {
                                if ((++i) % 13 != 0)
                                {
                                    if ((++i) % 13 != 0)
                                    {
                                        if ((++i) % 13 != 0)
                                        {
                                            if ((++i) % 13 != 0)
                                            {
                                                if ((++i) % 13 != 0)
                                                {
                                                    if ((++i) % 13 != 0)
                                                    {
                                                        if ((++i) % 13 != 0)
                                                        {
                                                            if ((++i) % 13 != 0)
                                                            {
                                                                if ((++i) % 13 != 0)
                                                                {
                                                                    if ((++i) % 13 != 0)
                                                                    {
                                                                        if ((++i) % 13 != 0)
                                                                        {
                                                                            if ((++i) % 13 != 0)
                                                                            {
                                                                                return i;
                                                                            }
                                                                            return i;
                                                                        }
                                                                        return i;
                                                                    }
                                                                    return i;
                                                                }
                                                                return i;
                                                            }
                                                            return i;
                                                        }
                                                        return i;
                                                    }
                                                    return i;
                                                }
                                                return i;
                                            }
                                            return i;
                                        }
                                        return i;
                                    }
                                    return i;
                                }
                                return i;
                            }
                            return i;
                        }
                        return i;
                    }
                    return i;
                }
                return i;
            }

            return 0;
        }

        [InterceptorWithParams]
        public int MethodWith255Locals()
        {
            int var1  = 1, var2  = 1,var3  = 1,var4  = 1, var5  = 1,var6  = 1,var7  = 1, var8  = 1, var9  = 1, var10  = 1;
            int var11  = 1, var12  = 1, var13  = 1, var14  = 1, var15  = 1, var16  = 1, var17  = 1, var18  = 1, var19  = 1, var20  = 1;
            int var21  = 1, var22  = 1, var23  = 1, var24  = 1, var25  = 1, var26  = 1, var27  = 1, var28  = 1, var29  = 1, var30  = 1;
            int var31  = 1, var32  = 1, var33  = 1, var34  = 1, var35  = 1, var36  = 1, var37  = 1, var38  = 1, var39  = 1, var40  = 1;
            int var41  = 1, var42  = 1, var43  = 1, var44  = 1, var45  = 1, var46  = 1, var47  = 1, var48  = 1, var49  = 1, var50  = 1;
            int var51  = 1, var52  = 1, var53  = 1, var54  = 1, var55  = 1, var56  = 1, var57  = 1, var58  = 1, var59  = 1, var60  = 1;
            int var61  = 1, var62  = 1, var63  = 1, var64  = 1, var65  = 1, var66  = 1, var67  = 1, var68  = 1, var69  = 1, var70  = 1;
            int var71  = 1, var72  = 1, var73  = 1, var74  = 1, var75  = 1, var76  = 1, var77  = 1, var78  = 1, var79  = 1, var80  = 1;
            int var81  = 1, var82  = 1, var83  = 1, var84  = 1, var85  = 1, var86  = 1, var87  = 1, var88  = 1, var89  = 1, var90  = 1;
            int var91  = 1, var92  = 1, var93  = 1, var94  = 1, var95  = 1, var96  = 1, var97  = 1, var98  = 1, var99  = 1, var100  = 1;
            int var101  = 1, var102  = 1, var103  = 1, var104  = 1, var105  = 1, var106  = 1, var107  = 1, var108  = 1, var109  = 1, var110  = 1;
            int var111  = 1, var112  = 1, var113  = 1, var114  = 1, var115  = 1, var116  = 1, var117  = 1, var118  = 1, var119  = 1, var120  = 1;
            int var121  = 1, var122  = 1, var123  = 1, var124  = 1, var125  = 1, var126  = 1, var127  = 1, var128  = 1, var129  = 1, var130  = 1;
            int var131  = 1, var132  = 1, var133  = 1, var134  = 1, var135  = 1, var136  = 1, var137  = 1, var138  = 1, var139  = 1, var140  = 1;
            int var141  = 1, var142  = 1, var143  = 1, var144  = 1, var145  = 1, var146  = 1, var147  = 1, var148  = 1, var149  = 1, var150  = 1;
            int var151  = 1, var152  = 1, var153  = 1, var154  = 1, var155  = 1, var156  = 1, var157  = 1, var158  = 1, var159  = 1, var160  = 1;
            int var161  = 1, var162  = 1, var163  = 1, var164  = 1, var165  = 1, var166  = 1, var167  = 1, var168  = 1, var169  = 1, var170  = 1;
            int var171  = 1, var172  = 1, var173  = 1, var174  = 1, var175  = 1, var176  = 1, var177  = 1, var178  = 1, var179  = 1, var180  = 1;
            int var181  = 1, var182  = 1, var183  = 1, var184  = 1, var185  = 1, var186  = 1, var187  = 1, var188  = 1, var189  = 1, var190  = 1;
            int var191  = 1, var192  = 1, var193  = 1, var194  = 1, var195  = 1, var196  = 1, var197  = 1, var198  = 1, var199  = 1, var200  = 1;
            int var201  = 1, var202  = 1, var203  = 1, var204  = 1, var205  = 1, var206  = 1, var207  = 1, var208  = 1, var209  = 1, var210  = 1;
            int var211  = 1, var212  = 1, var213  = 1, var214  = 1, var215  = 1, var216  = 1, var217  = 1, var218  = 1, var219  = 1, var220  = 1;
            int var221  = 1, var222  = 1, var223  = 1, var224  = 1, var225  = 1, var226  = 1, var227  = 1, var228  = 1, var229  = 1, var230  = 1;
            int var231  = 1, var232  = 1, var233  = 1, var234  = 1, var235  = 1, var236  = 1, var237  = 1, var238  = 1, var239  = 1, var240  = 1;
            int var241  = 1, var242  = 1, var243  = 1, var244  = 1, var245  = 1, var246  = 1, var247  = 1, var248  = 1, var249  = 1, var250  = 1;
            int var251  = 1, var252  = 1, var253  = 1, var254  = 1, var255  = 1, var256  = 1, var257  = 1, var258  = 1, var259  = 1, var260  = 1;


            return  var1 + var2 + var3 + var4 + var5 + var6 + var7 + var8 + var9 + var10 +
                    var11 + var12 + var13 + var14 + var15 + var16 + var17 + var18 + var19 + var20 +
                    var21 + var22 + var23 + var24 + var25 + var26 + var27 + var28 + var29 + var30 +
                    var31 + var32 + var33 + var34 + var35 + var36 + var37 + var38 + var39 + var40 +
                    var41 + var42 + var43 + var44 + var45 + var46 + var47 + var48 + var49 + var50 +
                    var51 + var52 + var53 + var54 + var55 + var56 + var57 + var58 + var59 + var60 +
                    var61 + var62 + var63 + var64 + var65 + var66 + var67 + var68 + var69 + var70 +
                    var71 + var72 + var73 + var74 + var75 + var76 + var77 + var78 + var79 + var80 +
                    var81 + var82 + var83 + var84 + var85 + var86 + var87 + var88 + var89 + var90 +
                    var91 + var92 + var93 + var94 + var95 + var96 + var97 + var98 + var99 + var100 +
                    var101 + var102 + var103 + var104 + var105 + var106 + var107 + var108 + var109 + var110 +
                    var111 + var112 + var113 + var114 + var115 + var116 + var117 + var118 + var119 + var120 +
                    var121 + var122 + var123 + var124 + var125 + var126 + var127 + var128 + var129 + var130 +
                    var131 + var132 + var133 + var134 + var135 + var136 + var137 + var138 + var139 + var140 +
                    var141 + var142 + var143 + var144 + var145 + var146 + var147 + var148 + var149 + var150 +
                    var151 + var152 + var153 + var154 + var155 + var156 + var157 + var158 + var159 + var160 +
                    var161 + var162 + var163 + var164 + var165 + var166 + var167 + var168 + var169 + var170 +
                    var171 + var172 + var173 + var174 + var175 + var176 + var177 + var178 + var179 + var180 +
                    var181 + var182 + var183 + var184 + var185 + var186 + var187 + var188 + var189 + var190 +
                    var191 + var192 + var193 + var194 + var195 + var196 + var197 + var198 + var199 + var200 +
                    var201 + var202 + var203 + var204 + var205 + var206 + var207 + var208 + var209 + var210 +
                    var211 + var212 + var213 + var214 + var215 + var216 + var217 + var218 + var219 + var220 +
                    var221 + var222 + var223 + var224 + var225 + var226 + var227 + var228 + var229 + var230 +
                    var231 + var232 + var233 + var234 + var235 + var236 + var237 + var238 + var239 + var240 +
                    var241 + var242 + var243 + var244 + var245 + var246 + var247 + var248 + var249 + var250 +
                    var251 + var252 + var253 + var254 + var255 + var256 + var257 + var258 + var259 + var260;
        }

        [InterceptorInit1]
        public int InterceptedInit1( int iDummy)
        {
            TestRecords.Record(Method.Body);
            return ++iDummy;
        }

        [InterceptorInit2]
        public int InterceptedInit2(int iDummy)
        {
            TestRecords.Record(Method.Body);
            return ++iDummy;
        }

        [InterceptorInit3]
        public int InterceptedInit3(int iDummy)
        {
            TestRecords.Record(Method.Body);
            return ++iDummy;
        }

        [InterceptorEntry]
        public int InterceptedEntry(int iDummy)
        {
            TestRecords.Record(Method.Body);
            return ++iDummy;
        }

        [InterceptorExit]
        public int InterceptedExit(int iDummy)
        {
            TestRecords.Record(Method.Body);
            return ++iDummy;
        }

        [InterceptorExit1]
        public int InterceptedExit1(int iDummy)
        {
            TestRecords.Record(Method.Body);
            return ++iDummy;
        }

        [InterceptorException]
        public int InterceptedException(int iDummy)
        {
            TestRecords.Record(Method.Body);
            throw new Exception("test");
            return ++iDummy;
        }

        [InterceptorExit1Exception]
        public int InterceptedExit1Exception(int iDummy)
        {
            TestRecords.Record(Method.Body);
            if(iDummy == 0) throw new Exception("test");
            return ++iDummy;
        }

        [InterceptorWithPriority("Attr1")]
        [InterceptorWithParams(1, "Attr2")]
        public void InterceptedWithoutPriorities()
        {
            TestRecords.Record(Method.Body);
        }

        [InterceptorWithPriority("Attr1", AspectPriority = -1)]
        [InterceptorWithParams(1,"Attr2")]
        public void InterceptedWithPriorities()
        {
            TestRecords.Record(Method.Body);
        }

        [InterceptorWithParams(1, "Attr1")]
        public T GenericMethod<T>()
        {
            TestRecords.Record(Method.Body);
            return (T)( (object)"string" );
        }

        [InterceptorBypass(true)]
        public void BypassedMethod()
        {
            TestRecords.Record(Method.Body);
        }
        [InterceptorBypass(false)]
        public void NotBypassedMethod()
        {
            TestRecords.Record(Method.Body);
        }
        [InterceptorBypass(true)]
        public bool BypassedMethodRetTrue()
        {
            TestRecords.Record(Method.Body);
            return true;
        }
        [InterceptorAlterRetval("altered")]
        public string AlteredMethodString()
        {
            TestRecords.Record(Method.Body);
            return "original";
        }
        [InterceptorAlterRetval(2)]
        public int AlteredMethodInt()
        {
            TestRecords.Record(Method.Body);
            return 1;
        }
        [InterceptorBypassReturn("altered")]
        public string AlteredBypassedMethodString()
        {
            TestRecords.Record(Method.Body);
            return "original";
        }
        [InterceptorBypassReturn(2)]
        public int AlteredBypassedMethodInt()
        {
            TestRecords.Record(Method.Body);
            return 1;
        }
        [InterceptorBypassReturn(null)]
        public void AlteredBypassedMethodVoid()
        {
            TestRecords.Record(Method.Body);
        }
    }
}
