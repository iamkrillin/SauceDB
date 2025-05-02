// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccess.Core.Linq.Common
{
    /// <summary>
    /// Type related helper methods
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Finds the I enumerable.
        /// </summary>
        /// <param name="seqType">Type of the seq.</param>
        /// <returns></returns>
        public static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }

        /// <summary>
        /// Gets the type of the sequence.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <returns></returns>
        public static Type GetSequenceType(Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="seqType">Type of the seq.</param>
        /// <returns></returns>
        public static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        /// <summary>
        /// Determines whether [is nullable type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is nullable type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determines whether [is null assignable] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is null assignable] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        /// <summary>
        /// Gets the type of the non nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        /// <summary>
        /// Gets the type of the null assignable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type GetNullAssignableType(Type type)
        {
            if (!IsNullAssignable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        /// <summary>
        /// Gets the null constant.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ConstantExpression GetNullConstant(Type type)
        {
            return Expression.Constant(null, GetNullAssignableType(type));
        }

        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <param name="mi">The mi.</param>
        /// <returns></returns>
        public static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null) return fi.FieldType;
            PropertyInfo pi = mi as PropertyInfo;
            if (pi != null) return pi.PropertyType;
            EventInfo ei = mi as EventInfo;
            if (ei != null) return ei.EventHandlerType;
            MethodInfo meth = mi as MethodInfo;  // property getters really
            if (meth != null) return meth.ReturnType;
            return null;
        }

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object GetDefault(Type type)
        {
            bool isNullable = !type.IsValueType || TypeHelper.IsNullableType(type);
            if (!isNullable)
                return Activator.CreateInstance(type);
            return null;
        }

        /// <summary>
        /// Determines whether [is read only] [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if [is read only] [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsReadOnly(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)member;
                    return !pi.CanWrite || pi.GetSetMethod() == null;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Determines whether the specified type is integer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
    }
}
