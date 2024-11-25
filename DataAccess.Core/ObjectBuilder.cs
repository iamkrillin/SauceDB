using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Data;
using System.Reflection;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Events;
using DataAccess.Core.Data.Results;
using System.Threading.Tasks;

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
        public static async Task<object> BuildObject(this IDataStore dstore, IQueryRow row, DatabaseTypeInfo ti)
        {
            object toAdd;
            ConstructorInfo ci = ti.DataType.GetConstructor(new Type[] { });
            object[] parms;

            if (ci == null)
            {
                ci = ti.DataType.GetConstructors().First();
                ParameterInfo[] parminfo = ci.GetParameters();
                var args = await SetConstructorArguments(dstore, parminfo, ti, row);

                toAdd = ci.Invoke(args);
            }
            else
            {
                parms = new object[0];
                toAdd = ci.Invoke(parms);
                await SetFieldData(dstore, ti, row, toAdd);
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
        public static async Task<object[]> SetConstructorArguments(this IDataStore dstore, ParameterInfo[] parminfo, DatabaseTypeInfo ti, IQueryRow dt)
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
                    {
                        var tinfo = await dstore.TypeParser.GetTypeInfo(curr.ParameterType);
                        toReturn[i] = BuildObject(dstore, dt, tinfo);
                    }
                }
                else
                {
                    //most system classes are handled just nicely by the type converter, only interested in user defined classes
                    if (dt.FieldHasMapping(ti.DataFields[i].FieldName) && curr.ParameterType.IsSystemType())
                        toReturn[i] = dstore.Connection.CLRConverter.ConvertToType(dt.GetDataForRowField(parminfo[i].Name), parminfo[i].ParameterType);
                    else
                    {
                        var tinfo = await dstore.TypeParser.GetTypeInfo(curr.ParameterType);
                        toReturn[i] = BuildObject(dstore, dt, tinfo);
                    }
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
        public static async Task SetFieldData(this IDataStore dstore, IQueryRow row, object dataItem)
        {
            if (row != null)
            {
                DatabaseTypeInfo ti = await dstore.TypeParser.GetTypeInfo(dataItem.GetType());
                await SetFieldData(dstore, ti, row, dataItem);
            }
        }

        /// <summary>
        /// Sets the fields on an object
        /// </summary>
        /// <param name="dstore">The datastore.</param>
        /// <param name="info">The information for the type</param>
        /// <param name="row">The row to pull from</param>
        /// <param name="dataItem">The object to set the data on</param>
        public static async Task SetFieldData(this IDataStore dstore, DatabaseTypeInfo info, IQueryRow row, object dataItem)
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
                            {
                                var tinfo = await dstore.TypeParser.GetTypeInfo(dfi.PropertyType);
                                dfi.Setter(dataItem, BuildObject(dstore, row, tinfo));
                            }
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
                else
                {
                    int i = 1;
                    i++;

                }
            }

            if (info.AdditionalInit != null)
                info.AdditionalInit.ForEach(R => R.Invoke(dstore, dataItem));
        }

        private static void SetDefaultValue(object dataItem, DataFieldInfo dfi)
        {
            ConstructorInfo ci = dfi.PropertyType.GetConstructors().Where(R => R.GetParameters().Count() == 0).FirstOrDefault();
            if (ci != null) dfi.Setter(dataItem, ci.Invoke(null));
        }
    }
}