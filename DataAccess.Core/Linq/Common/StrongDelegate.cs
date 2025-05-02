// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
// Original code created by Matt Warren: http://iqtoolkit.codeplex.com/Release/ProjectReleases.aspx?ReleaseId=19725

using System;
using System.Reflection;

namespace DataAccess.Core.Linq
{
    /// <summary>
    /// Make a strongly-typed delegate to a weakly typed method (one that takes single object[] argument)
    /// (up to 8 arguments)
    /// </summary>
    public class StrongDelegate
    {
        Func<object[], object> fn;

        private StrongDelegate(Func<object[], object> fn)
        {
            this.fn = fn;
        }

        private static MethodInfo[] _meths;

        static StrongDelegate()
        {
            _meths = new MethodInfo[9];

            var meths = typeof(StrongDelegate).GetMethods();
            for (int i = 0, n = meths.Length; i < n; i++)
            {
                var gm = meths[i];
                if (gm.Name.StartsWith("M"))
                {
                    var tas = gm.GetGenericArguments();
                    _meths[tas.Length - 1] = gm;
                }
            }
        }

        /// <summary>
        /// Create a strongly typed delegate over a method with a weak signature
        /// </summary>
        /// <param name="delegateType">The strongly typed delegate's type</param>
        /// <param name="target"></param>
        /// <param name="method">Any method that takes a single array of objects and returns an object.</param>
        /// <returns></returns>
        public static Delegate CreateDelegate(Type delegateType, object target, MethodInfo method)
        {
            return CreateDelegate(delegateType, (Func<object[], object>)Delegate.CreateDelegate(typeof(Func<object[], object>), target, method));
        }

        /// <summary>
        /// Create a strongly typed delegate over a Func delegate with weak signature
        /// </summary>
        /// <param name="delegateType"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public static Delegate CreateDelegate(Type delegateType, Func<object[], object> fn)
        {
            MethodInfo invoke = delegateType.GetMethod("Invoke");
            var parameters = invoke.GetParameters();
            Type[] typeArgs = new Type[1 + parameters.Length];
            for (int i = 0, n = parameters.Length; i < n; i++)
            {
                typeArgs[i] = parameters[i].ParameterType;
            }
            typeArgs[typeArgs.Length - 1] = invoke.ReturnType;
            if (typeArgs.Length <= _meths.Length)
            {
                var gm = _meths[typeArgs.Length - 1];
                var m = gm.MakeGenericMethod(typeArgs);
                return Delegate.CreateDelegate(delegateType, new StrongDelegate(fn), m);
            }
            throw new NotSupportedException("Delegate has too many arguments");
        }

        /// <summary>
        /// Ms this instance.
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <returns></returns>
        public R M<R>()
        {
            return (R)fn(null);
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <returns></returns>
        public R M<A1, R>(A1 a1)
        {
            return (R)fn(new object[] { a1 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <returns></returns>
        public R M<A1, A2, R>(A1 a1, A2 a2)
        {
            return (R)fn(new object[] { a1, a2 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="A3">The type of the 3.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a3">The a3.</param>
        /// <returns></returns>
        public R M<A1, A2, A3, R>(A1 a1, A2 a2, A3 a3)
        {
            return (R)fn(new object[] { a1, a2, a3 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="A3">The type of the 3.</typeparam>
        /// <typeparam name="A4">The type of the 4.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a3">The a3.</param>
        /// <param name="a4">The a4.</param>
        /// <returns></returns>
        public R M<A1, A2, A3, A4, R>(A1 a1, A2 a2, A3 a3, A4 a4)
        {
            return (R)fn(new object[] { a1, a2, a3, a4 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="A3">The type of the 3.</typeparam>
        /// <typeparam name="A4">The type of the 4.</typeparam>
        /// <typeparam name="A5">The type of the 5.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a3">The a3.</param>
        /// <param name="a4">The a4.</param>
        /// <param name="a5">The a5.</param>
        /// <returns></returns>
        public R M<A1, A2, A3, A4, A5, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5)
        {
            return (R)fn(new object[] { a1, a2, a3, a4, a5 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="A3">The type of the 3.</typeparam>
        /// <typeparam name="A4">The type of the 4.</typeparam>
        /// <typeparam name="A5">The type of the 5.</typeparam>
        /// <typeparam name="A6">The type of the 6.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a3">The a3.</param>
        /// <param name="a4">The a4.</param>
        /// <param name="a5">The a5.</param>
        /// <param name="a6">The a6.</param>
        /// <returns></returns>
        public R M<A1, A2, A3, A4, A5, A6, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6)
        {
            return (R)fn(new object[] { a1, a2, a3, a4, a5, a6 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="A3">The type of the 3.</typeparam>
        /// <typeparam name="A4">The type of the 4.</typeparam>
        /// <typeparam name="A5">The type of the 5.</typeparam>
        /// <typeparam name="A6">The type of the 6.</typeparam>
        /// <typeparam name="A7">The type of the 7.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a3">The a3.</param>
        /// <param name="a4">The a4.</param>
        /// <param name="a5">The a5.</param>
        /// <param name="a6">The a6.</param>
        /// <param name="a7">The a7.</param>
        /// <returns></returns>
        public R M<A1, A2, A3, A4, A5, A6, A7, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7)
        {
            return (R)fn(new object[] { a1, a2, a3, a4, a5, a6, a7 });
        }

        /// <summary>
        /// Ms the specified a1.
        /// </summary>
        /// <typeparam name="A1">The type of the 1.</typeparam>
        /// <typeparam name="A2">The type of the 2.</typeparam>
        /// <typeparam name="A3">The type of the 3.</typeparam>
        /// <typeparam name="A4">The type of the 4.</typeparam>
        /// <typeparam name="A5">The type of the 5.</typeparam>
        /// <typeparam name="A6">The type of the 6.</typeparam>
        /// <typeparam name="A7">The type of the 7.</typeparam>
        /// <typeparam name="A8">The type of the 8.</typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="a1">The a1.</param>
        /// <param name="a2">The a2.</param>
        /// <param name="a3">The a3.</param>
        /// <param name="a4">The a4.</param>
        /// <param name="a5">The a5.</param>
        /// <param name="a6">The a6.</param>
        /// <param name="a7">The a7.</param>
        /// <param name="a8">The a8.</param>
        /// <returns></returns>
        public R M<A1, A2, A3, A4, A5, A6, A7, A8, R>(A1 a1, A2 a2, A3 a3, A4 a4, A5 a5, A6 a6, A7 a7, A8 a8)
        {
            return (R)fn(new object[] { a1, a2, a3, a4, a5, a6, a7, a8 });
        }
    }
}