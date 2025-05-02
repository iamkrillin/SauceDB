﻿namespace Tests.DataObjects
{
    public class TestItemWithEnum
    {
        public int ID { get; set; }
        public Data ValueOne { get; set; }
        public Data? ValueTwo { get; set; }
        public int? AnotherValue { get; set; }
        public int? AnotherValueTwo { get; set; }
    }

    public enum Data : int
    {
        Var1 = 1,
        Var2 = 2
    }
}
