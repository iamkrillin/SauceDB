using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using System.Reflection;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using DataAccess.Core.Data.Results;

namespace DataAccess.Core
{
    /// <summary>
    /// Contains functions that will map a data row to an object
    /// </summary>
    public static class ObjectBuilder
    {
        /// <summary>
        /// Builds a return object
        /// </summary>
        /// <param name="dstore">The datastore.</param>
        /// <param name="dt">The query data to build with</param>
        /// <param name="ti">The parsed  type info for the object</param>
        /// <returns></returns>
        public static object BuildObject(this IDataStore dstore, IQueryRow row, DatabaseTypeInfo ti)
        {
            object toAdd;
            ConstructorInfo ci = ti.DataType.GetConstructor(new Type[] { });
            object[] parms;

            if (ci == null)
            {
                ci = ti.DataType.GetConstructors().First();
                ParameterInfo[] parminfo = ci.GetParameters();
                toAdd = ci.Invoke(SetConstructorArguments(dstore, parminfo, ti, row));
            }
            else
            {
                parms = new object[0];
                toAdd = ci.Invoke(parms);
                SetFieldData(dstore, ti, row, toAdd);
            }

            return toAdd;
        }

        /// <summary>
        /// Gets a array of constructor arguments to build an object with
        /// </summary>
        /// <param name="dstore">The datastore.</param>
        /// <param name="parminfo">The parms for the constructor.</param>
        /// <param name="ti">The type info</param>
        /// <param name="row">The row in the result set to use</param>
        /// <param name="dt">The query result set</param>
        /// <returns></returns>
        public static object[] SetConstructorArguments(this IDataStore dstore, ParameterInfo[] parminfo, DatabaseTypeInfo ti, IQueryRow dt)
        {
            object[] toReturn = new object[parminfo.Length];
            for (int i = 0; i < parminfo.Length; i++)
            {
                ParameterInfo curr = parminfo[i];
                if (ti.IsCompilerGenerated)
                {
                    if (curr.ParameterType.IsSystemType())
                        toReturn[i] = dstore.Connection.CLRConverter.ConvertToType(dt.GetDataForRowField(curr.Name), parminfo[i].ParameterType);
                    else
                        toReturn[i] = BuildObject(dstore, dt, dstore.TypeInformationParser.GetTypeInfo(curr.ParameterType));
                }
                else
                {
                    //most system classes are handled just nicely by the type converter, only interested in user defined classes
                    if (dt.FieldHasMapping(ti.DataFields[i].FieldName) && curr.ParameterType.IsSystemType())
                        toReturn[i] = dstore.Connection.CLRConverter.ConvertToType(dt.GetDataForRowField(parminfo[i].Name), parminfo[i].ParameterType);
                    else
                        toReturn[i] = BuildObject(dstore, dt, dstore.TypeInformationParser.GetTypeInfo(curr.ParameterType));
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Sets the fields on an object
        /// </summary>
        /// <param name="dstore">The datastore.</param>
        /// <param name="row">The row to pull from</param>
        /// <param name="dataItem">The object to set the data on</param>
        public static void SetFieldData(this IDataStore dstore, IQueryRow row, object dataItem)
        {
            if (row != null)
            {
                DatabaseTypeInfo ti = dstore.TypeInformationParser.GetTypeInfo(dataItem.GetType());
                SetFieldData(dstore, ti, row, dataItem);
            }
        }

        /// <summary>
        /// Sets the fields on an object
        /// </summary>
        /// <param name="dstore">The datastore.</param>
        /// <param name="info">The information for the type</param>
        /// <param name="row">The row to pull from</param>
        /// <param name="dataItem">The object to set the data on</param>
        public static void SetFieldData(this IDataStore dstore, DatabaseTypeInfo info, IQueryRow row, object dataItem)
        {
            foreach (DataFieldInfo dfi in info.DataFields)
            {
                object item = row.GetDataForRowField(dfi.FieldName);
                if (item != null)
                {
                    if (dfi.PropertyType.IsAssignableFrom(item.GetType()))
                    {
                        dfi.Setter(dataItem, item);
                    }
                    else
                    {
                        try
                        {
                            if (!dfi.PropertyType.IsSystemType() && !dfi.PropertyType.IsEnum)
                                dfi.Setter(dataItem, BuildObject(dstore, row, dstore.TypeInformationParser.GetTypeInfo(dfi.PropertyType)));
                            else
                            {
                                object fieldValue = dstore.Connection.CLRConverter.ConvertToType(item, dfi.PropertyType);
                                if (fieldValue == null)
                                {//if the value comes back null, lets use the default for the property type (null, zero, whatever)
                                    SetDefaultValue(dataItem, dfi);
                                }
                                else
                                {
                                    dfi.Setter(dataItem, fieldValue);
                                }
                            }
                        }
                        catch
                        {//attempt to set to default
                            SetDefaultValue(dataItem, dfi);
                        }
                    }
                }
            }
            if (info.AdditionalInit != null) info.AdditionalInit.ForEach(R => R.Invoke(dstore, dataItem));
        }

        private static void SetDefaultValue(object dataItem, DataFieldInfo dfi)
        {
            ConstructorInfo ci = dfi.PropertyType.GetConstructors().Where(R => R.GetParameters().Count() == 0).FirstOrDefault();
            if (ci != null) dfi.Setter(dataItem, ci.Invoke(null));
        }
    }
}