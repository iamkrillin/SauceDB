﻿using DataAccess.Core.Data;
using System;

namespace DataAccess.Core.Events
{
    public class TypeParsedEventArgs : EventArgs
    {
        public TypeParsedEventArgs(DatabaseTypeInfo dType, Type type, bool bypassValidation)
        {
            this.Data = dType;
            this.Type = type;
            this.BypassValidation = bypassValidation;
        }

        public DatabaseTypeInfo Data { get; private set; }
        public Type Type { get; private set; }
        public bool BypassValidation { get; private set; }
    }
}
