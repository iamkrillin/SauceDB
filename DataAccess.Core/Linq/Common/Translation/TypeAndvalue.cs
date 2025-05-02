using System;

namespace DataAccess.Core.Linq.Common.Translation
{
    internal struct TypeAndValue : IEquatable<TypeAndValue>
    {
        Type type;
        object value;
        int hash;

        public TypeAndValue(Type type, object value)
        {
            this.type = type;
            this.value = value;
            this.hash = type.GetHashCode() + (value != null ? value.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TypeAndValue))
                return false;
            return this.Equals((TypeAndValue)obj);
        }

        public bool Equals(TypeAndValue vt)
        {
            return vt.type == this.type && object.Equals(vt.value, this.value);
        }

        public override int GetHashCode()
        {
            return this.hash;
        }
    }
}
