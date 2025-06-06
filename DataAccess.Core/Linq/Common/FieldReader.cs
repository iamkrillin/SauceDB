﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FieldReader
    {
        TypeCode[] typeCodes;
        private const TypeCode tcGuid = (TypeCode)20;
        private const TypeCode tcByteArray = (TypeCode)21;
        private const TypeCode tcCharArray = (TypeCode)22;
        static Dictionary<Type, MethodInfo> _readerMethods;
        static MethodInfo _miReadValue;
        static MethodInfo _miReadNullableValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReader"/> class.
        /// </summary>
        public FieldReader()
        {
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        protected void Init()
        {
            this.typeCodes = new TypeCode[this.FieldCount];
        }

        /// <summary>
        /// Gets the field count.
        /// </summary>
        protected abstract int FieldCount { get; }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Type GetFieldType(int ordinal);

        /// <summary>
        /// Determines whether [is DB null] [the specified ordinal].
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns>
        ///   <c>true</c> if [is DB null] [the specified ordinal]; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsDBNull(int ordinal);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract T GetValue<T>(int ordinal);

        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Byte GetByte(int ordinal);

        /// <summary>
        /// Gets the char.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Char GetChar(int ordinal);

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract DateTime GetDateTime(int ordinal);

        /// <summary>
        /// Gets the decimal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Decimal GetDecimal(int ordinal);

        /// <summary>
        /// Gets the double.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Double GetDouble(int ordinal);

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Single GetSingle(int ordinal);

        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Guid GetGuid(int ordinal);

        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Int16 GetInt16(int ordinal);

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Int32 GetInt32(int ordinal);

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract Int64 GetInt64(int ordinal);

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        protected abstract String GetString(int ordinal);

        /// <summary>
        /// Reads the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public T ReadValue<T>(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(T);
            }
            return this.GetValue<T>(ordinal);
        }

        /// <summary>
        /// Reads the nullable value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public T? ReadNullableValue<T>(int ordinal) where T : struct
        {
            if (this.IsDBNull(ordinal))
            {
                return default(T?);
            }
            return this.GetValue<T>(ordinal);
        }

        /// <summary>
        /// Reads the byte.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Byte ReadByte(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Byte);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Byte)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Byte)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Byte)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Byte)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Byte)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Byte)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Byte>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable byte.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Byte? ReadNullableByte(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Byte?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Byte)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Byte)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Byte)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Byte)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Byte)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Byte)this.GetDecimal(ordinal);
                    default:
                        return (Byte)this.GetValue<Byte>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the char.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Char ReadChar(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Char);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Char)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Char)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Char)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Char)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Char)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Char)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Char)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Char>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable char.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Char? ReadNullableChar(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Char?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Char)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Char)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Char)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Char)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Char)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Char)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Char)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Char>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the date time.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public DateTime ReadDateTime(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(DateTime);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.DateTime:
                        return this.GetDateTime(ordinal);
                    default:
                        return this.GetValue<DateTime>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable date time.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public DateTime? ReadNullableDateTime(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(DateTime?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.DateTime:
                        return this.GetDateTime(ordinal);
                    default:
                        return this.GetValue<DateTime>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the decimal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Decimal ReadDecimal(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Decimal);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Decimal)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Decimal)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Decimal)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Decimal)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Decimal)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Decimal)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Decimal>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable decimal.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Decimal? ReadNullableDecimal(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Decimal?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Decimal)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Decimal)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Decimal)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Decimal)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Decimal)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Decimal)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Decimal>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the double.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Double ReadDouble(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Double);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Double)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Double)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Double)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Double)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Double)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Double)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Double>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable double.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Double? ReadNullableDouble(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Double?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Double)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Double)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Double)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Double)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Double)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Double)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Double>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the single.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Single ReadSingle(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Single);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Single)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Single)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Single)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Single)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Single)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Single)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Single>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable single.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Single? ReadNullableSingle(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Single?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Single)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Single)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Single)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Single)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Single)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Single)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Single>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the GUID.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Guid ReadGuid(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Guid);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case tcGuid:
                        return this.GetGuid(ordinal);
                    default:
                        return this.GetValue<Guid>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable GUID.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Guid? ReadNullableGuid(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Guid?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case tcGuid:
                        return this.GetGuid(ordinal);
                    default:
                        return this.GetValue<Guid>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the int16.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Int16 ReadInt16(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Int16);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Int16)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Int16)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Int16)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Int16)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Int16)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Int16)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Int16)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Int16>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable int16.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Int16? ReadNullableInt16(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Int16?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Int16)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Int16)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Int16)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Int16)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Int16)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Int16)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Int16)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Int16>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the int32.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Int32 ReadInt32(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Int32);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Int32)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Int32)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Int32)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Int32)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Int32)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Int32)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Int32)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Int32>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable int32.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Int32? ReadNullableInt32(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Int32?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Int32)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Int32)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Int32)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Int32)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Int32)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Int32)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Int32)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Int32>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the int64.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Int64 ReadInt64(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Int64);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Int64)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Int64)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Int64)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Int64)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Int64)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Int64)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Int64)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Int64>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the nullable int64.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Int64? ReadNullableInt64(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Int64?);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return (Int64)this.GetByte(ordinal);
                    case TypeCode.Int16:
                        return (Int64)this.GetInt16(ordinal);
                    case TypeCode.Int32:
                        return (Int64)this.GetInt32(ordinal);
                    case TypeCode.Int64:
                        return (Int64)this.GetInt64(ordinal);
                    case TypeCode.Double:
                        return (Int64)this.GetDouble(ordinal);
                    case TypeCode.Single:
                        return (Int64)this.GetSingle(ordinal);
                    case TypeCode.Decimal:
                        return (Int64)this.GetDecimal(ordinal);
                    default:
                        return this.GetValue<Int64>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public String ReadString(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(String);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = Type.GetTypeCode(this.GetFieldType(ordinal));
                        continue;
                    case TypeCode.Byte:
                        return this.GetByte(ordinal).ToString();
                    case TypeCode.Int16:
                        return this.GetInt16(ordinal).ToString();
                    case TypeCode.Int32:
                        return this.GetInt32(ordinal).ToString();
                    case TypeCode.Int64:
                        return this.GetInt64(ordinal).ToString();
                    case TypeCode.Double:
                        return this.GetDouble(ordinal).ToString();
                    case TypeCode.Single:
                        return this.GetSingle(ordinal).ToString();
                    case TypeCode.Decimal:
                        return this.GetDecimal(ordinal).ToString();
                    case TypeCode.DateTime:
                        return this.GetDateTime(ordinal).ToString();
                    case tcGuid:
                        return this.GetGuid(ordinal).ToString();
                    case TypeCode.String:
                        return this.GetString(ordinal);
                    default:
                        return this.GetValue<String>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the byte array.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Byte[] ReadByteArray(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Byte[]);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Byte:
                        return new Byte[] { this.GetByte(ordinal) };
                    default:
                        return this.GetValue<Byte[]>(ordinal);
                }
            }
        }

        /// <summary>
        /// Reads the char array.
        /// </summary>
        /// <param name="ordinal">The ordinal.</param>
        /// <returns></returns>
        public Char[] ReadCharArray(int ordinal)
        {
            if (this.IsDBNull(ordinal))
            {
                return default(Char[]);
            }
            while (true)
            {
                switch (typeCodes[ordinal])
                {
                    case TypeCode.Empty:
                        typeCodes[ordinal] = GetTypeCode(ordinal);
                        continue;
                    case TypeCode.Char:
                        return new Char[] { this.GetChar(ordinal) };
                    default:
                        return this.GetValue<Char[]>(ordinal);
                }
            }
        }

        private TypeCode GetTypeCode(int ordinal)
        {
            Type type = this.GetFieldType(ordinal);
            TypeCode tc = Type.GetTypeCode(type);
            if (tc == TypeCode.Object)
            {
                if (type == typeof(Guid))
                    tc = tcGuid;
                else if (type == typeof(Byte[]))
                    tc = tcByteArray;
                else if (type == typeof(Char[]))
                    tc = tcCharArray;
            }
            return tc;
        }

        /// <summary>
        /// Gets the reader method.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static MethodInfo GetReaderMethod(Type type)
        {
            if (_readerMethods == null)
            {
                var meths = typeof(FieldReader).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.Name.StartsWith("Read")).ToList();
                _readerMethods = meths.ToDictionary(m => m.ReturnType);
                _miReadValue = meths.Single(m => m.Name == "ReadValue");
                _miReadNullableValue = meths.Single(m => m.Name == "ReadNullableValue");
            }

            MethodInfo mi;
            _readerMethods.TryGetValue(type, out mi);
            if (mi == null)
            {
                if (TypeHelper.IsNullableType(type))
                {
                    mi = _miReadNullableValue.MakeGenericMethod(TypeHelper.GetNonNullableType(type));
                }
                else
                {
                    mi = _miReadValue.MakeGenericMethod(type);
                }
            }
            return mi;
        }
    }
}


