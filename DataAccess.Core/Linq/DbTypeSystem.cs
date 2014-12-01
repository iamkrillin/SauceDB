// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DataAccess.Core.Linq
{
    using Common;
    using DataAccess.Core.Linq.Common.Language;

    /// <summary>
    /// 
    /// </summary>
    public class DbTypeSystem : QueryTypeSystem
    {
        ///// <summary>
        ///// Parses the specified type declaration.
        ///// </summary>
        ///// <param name="typeDeclaration">The type declaration.</param>
        ///// <returns></returns>
        //public override QueryType Parse(string typeDeclaration)
        //{
        //    string[] args = null;
        //    string typeName = null;
        //    string remainder = null;
        //    int openParen = typeDeclaration.IndexOf('(');
        //    if (openParen >= 0)
        //    {
        //        typeName = typeDeclaration.Substring(0, openParen).Trim();

        //        int closeParen = typeDeclaration.IndexOf(')', openParen);
        //        if (closeParen < openParen) closeParen = typeDeclaration.Length;

        //        string argstr = typeDeclaration.Substring(openParen + 1, closeParen - (openParen + 1));
        //        args = argstr.Split(',');
        //        remainder = typeDeclaration.Substring(closeParen + 1);
        //    }
        //    else
        //    {
        //        int space = typeDeclaration.IndexOf(' ');
        //        if (space >= 0)
        //        {
        //            typeName = typeDeclaration.Substring(0, space);
        //            remainder = typeDeclaration.Substring(space + 1).Trim();
        //        }
        //        else
        //        {
        //            typeName = typeDeclaration;
        //        }
        //    }

        //    bool isNotNull = (remainder != null) ? remainder.ToUpper().Contains("NOT NULL") : false;

        //    return this.GetQueryType(typeName, args, isNotNull);
        //}

        ///// <summary>
        ///// Gets the type of the query.
        ///// </summary>
        ///// <param name="typeName">Name of the type.</param>
        ///// <param name="args">The args.</param>
        ///// <param name="isNotNull">if set to <c>true</c> [is not null].</param>
        ///// <returns></returns>
        //public virtual QueryType GetQueryType(string typeName, string[] args, bool isNotNull)
        //{
        //    if (String.Compare(typeName, "rowversion", StringComparison.OrdinalIgnoreCase) == 0)
        //    {
        //        typeName = "Timestamp";
        //    }

        //    if (String.Compare(typeName, "numeric", StringComparison.OrdinalIgnoreCase) == 0)
        //    {
        //        typeName = "Decimal";
        //    }

        //    if (String.Compare(typeName, "sql_variant", StringComparison.OrdinalIgnoreCase) == 0)
        //    {
        //        typeName = "Variant";
        //    }

        //    SqlDbType dbType = this.GetSqlType(typeName);

        //    int length = 0;
        //    short precision = 0;
        //    short scale = 0;

        //    switch (dbType)
        //    {
        //        case SqlDbType.Binary:
        //        case SqlDbType.Char:
        //        case SqlDbType.Image:
        //        case SqlDbType.NChar:
        //        case SqlDbType.NVarChar:
        //        case SqlDbType.VarBinary:
        //        case SqlDbType.VarChar:
        //            if (args == null || args.Length < 1)
        //            {
        //                length = 80;
        //            }
        //            else if (string.Compare(args[0], "max", true) == 0)
        //            {
        //                length = Int32.MaxValue;
        //            }
        //            else
        //            {
        //                length = Int32.Parse(args[0]);
        //            }
        //            break;
        //        case SqlDbType.Money:
        //            if (args == null || args.Length < 1)
        //            {
        //                precision = 29;
        //            }
        //            else
        //            {
        //                precision = Int16.Parse(args[0]);
        //            }
        //            if (args == null || args.Length < 2)
        //            {
        //                scale = 4;
        //            }
        //            else
        //            {
        //                scale = Int16.Parse(args[1]);
        //            }
        //            break;
        //        case SqlDbType.Decimal:
        //            if (args == null || args.Length < 1)
        //            {
        //                precision = 29;
        //            }
        //            else
        //            {
        //                precision = Int16.Parse(args[0]);
        //            }
        //            if (args == null || args.Length < 2)
        //            {
        //                scale = 0;
        //            }
        //            else
        //            {
        //                scale = Int16.Parse(args[1]);
        //            }
        //            break;
        //        case SqlDbType.Float:
        //        case SqlDbType.Real:
        //            if (args == null || args.Length < 1)
        //            {
        //                precision = 29;
        //            }
        //            else
        //            {
        //                precision = Int16.Parse(args[0]);
        //            }
        //            break;
        //    }

        //    return NewType(isNotNull, length, precision, scale);
        //}

//        /// <summary>
//        /// News the type.
//        /// </summary>
//        /// <param name="type">The type.</param>
//        /// <param name="isNotNull">if set to <c>true</c> [is not null].</param>
//        /// <param name="length">The length.</param>
//        /// <param name="precision">The precision.</param>
//        /// <param name="scale">The scale.</param>
//        /// <returns></returns>
//        public virtual QueryType NewType(bool isNotNull, int length, short precision, short scale)
//        {
//            return new DbQueryType(isNotNull, length, precision, scale);
//        }

//        /// <summary>
//        /// Gets the type of the SQL.
//        /// </summary>
//        /// <param name="typeName">Name of the type.</param>
//        /// <returns></returns>
//        public virtual SqlDbType GetSqlType(string typeName)
//        {
//            return (SqlDbType)Enum.Parse(typeof(SqlDbType), typeName, true);
//        }

//        /// <summary>
//        /// Gets the default size of the string.
//        /// </summary>
//        /// <value>
//        /// The default size of the string.
//        /// </value>
//        public virtual int StringDefaultSize
//        {
//            get { return Int32.MaxValue; }
//        }

//        /// <summary>
//        /// Gets the default size of the binary.
//        /// </summary>
//        /// <value>
//        /// The default size of the binary.
//        /// </value>
//        public virtual int BinaryDefaultSize
//        {
//            get { return Int32.MaxValue; }
//        }

//        /// <summary>
//        /// Gets the type of the column.
//        /// </summary>
//        /// <param name="type">The type.</param>
//        /// <returns></returns>
//        public override QueryType GetColumnType(Type type)
//        {
//            bool isNotNull = type.IsValueType && !TypeHelper.IsNullableType(type);
//            type = TypeHelper.GetNonNullableType(type);
//            switch (Type.GetTypeCode(type))
//            {
//                case TypeCode.Boolean:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.SByte:
//                case TypeCode.Byte:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.Int16:
//                case TypeCode.UInt16:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.Int32:
//                case TypeCode.UInt32:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.Int64:
//                case TypeCode.UInt64:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.Single:
//                case TypeCode.Double:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.String:
//                    return NewType(isNotNull, this.StringDefaultSize, 0, 0);
//                case TypeCode.Char:
//                    return NewType(isNotNull, 1, 0, 0);
//                case TypeCode.DateTime:
//                    return NewType(isNotNull, 0, 0, 0);
//                case TypeCode.Decimal:
//                    return NewType(isNotNull, 0, 29, 4);
//                default:
//                    if (type == typeof(byte[]))
//                        return NewType(isNotNull, this.BinaryDefaultSize, 0, 0);
//                    else if (type == typeof(Guid))
//                        return NewType(isNotNull, 0, 0, 0);
//#if MSNET
//                    else if (type == typeof(DateTimeOffset))
//                        return NewType(isNotNull, 0, 0, 0);
//#endif
//                    else if (type == typeof(TimeSpan))
//                        return NewType(isNotNull, 0, 0, 0);
//                    return null;
//            }
//        }

//        /// <summary>
//        /// Gets the type of the db.
//        /// </summary>
//        /// <param name="dbType">Type of the db.</param>
//        /// <returns></returns>
//        public static DbType GetDbType(SqlDbType dbType)
//        {
//            switch (dbType)
//            {
//                case SqlDbType.BigInt:
//                    return DbType.Int64;
//                case SqlDbType.Binary:
//                    return DbType.Binary;
//                case SqlDbType.Bit:
//                    return DbType.Boolean;
//                case SqlDbType.Char:
//                    return DbType.AnsiStringFixedLength;
//                case SqlDbType.Date:
//                    return DbType.Date;
//                case SqlDbType.DateTime:
//                case SqlDbType.SmallDateTime:
//                    return DbType.DateTime;
//#if MSNET
//                case SqlDbType.DateTime2:
//                    return DbType.DateTime2;
//                case SqlDbType.DateTimeOffset:
//                    return DbType.DateTimeOffset;
//#endif
//                case SqlDbType.Decimal:
//                    return DbType.Decimal;
//                case SqlDbType.Float:
//                case SqlDbType.Real:
//                    return DbType.Double;
//                case SqlDbType.Image:
//                    return DbType.Binary;
//                case SqlDbType.Int:
//                    return DbType.Int32;
//                case SqlDbType.Money:
//                case SqlDbType.SmallMoney:
//                    return DbType.Currency;
//                case SqlDbType.NChar:
//                    return DbType.StringFixedLength;
//                case SqlDbType.NText:
//                case SqlDbType.NVarChar:
//                    return DbType.String;
//                case SqlDbType.SmallInt:
//                    return DbType.Int16;
//                case SqlDbType.Text:
//                    return DbType.AnsiString;
//                case SqlDbType.Time:
//                    return DbType.Time;
//                case SqlDbType.Timestamp:
//                    return DbType.Binary;
//                case SqlDbType.TinyInt:
//                    return DbType.SByte;
//                case SqlDbType.Udt:
//                    return DbType.Object;
//                case SqlDbType.UniqueIdentifier:
//                    return DbType.Guid;
//                case SqlDbType.VarBinary:
//                    return DbType.Binary;
//                case SqlDbType.VarChar:
//                    return DbType.AnsiString;
//                case SqlDbType.Variant:
//                    return DbType.Object;
//                case SqlDbType.Xml:
//                    return DbType.String;
//                default:
//                    throw new InvalidOperationException(string.Format("Unhandled sql type: {0}", dbType));
//            }
//        }

//        /// <summary>
//        /// Determines whether [is variable length] [the specified db type].
//        /// </summary>
//        /// <param name="dbType">Type of the db.</param>
//        /// <returns>
//        ///   <c>true</c> if [is variable length] [the specified db type]; otherwise, <c>false</c>.
//        /// </returns>
//        public static bool IsVariableLength(SqlDbType dbType)
//        {
//            switch (dbType)
//            {
//                case SqlDbType.Image:
//                case SqlDbType.NText:
//                case SqlDbType.NVarChar:
//                case SqlDbType.Text:
//                case SqlDbType.VarBinary:
//                case SqlDbType.VarChar:
//                case SqlDbType.Xml:
//                    return true;
//                default:
//                    return false;
//            }
//        }

        ///// <summary>
        ///// Gets the variable declaration.
        ///// </summary>
        ///// <param name="type">The type.</param>
        ///// <param name="suppressSize">if set to <c>true</c> [suppress size].</param>
        ///// <returns></returns>
        //public override string GetVariableDeclaration(QueryType type, bool suppressSize)
        //{
        //    var sqlType = (DbQueryType)type;
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(sqlType.SqlDbType.ToString().ToUpper());
        //    if (sqlType.Length > 0 && !suppressSize)
        //    {
        //        if (sqlType.Length == Int32.MaxValue)
        //        {
        //            sb.Append("(max)");
        //        }
        //        else
        //        {
        //            sb.AppendFormat("({0})", sqlType.Length);
        //        }
        //    }
        //    else if (sqlType.Precision != 0)
        //    {
        //        if (sqlType.Scale != 0)
        //        {
        //            sb.AppendFormat("({0},{1})", sqlType.Precision, sqlType.Scale);
        //        }
        //        else
        //        {
        //            sb.AppendFormat("({0})", sqlType.Precision);
        //        }
        //    }
        //    return sb.ToString();
        //}
    }
}