using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using DataAccess.Core.Data;
using DataAccess.Core.Attributes;
using System.Reflection;
using System.Runtime.CompilerServices;
using DataAccess.Core.Events;
using System.Linq.Expressions;

namespace DataAccess.Core
{
    /// <summary>
    /// A caching type information parser, the type will be parsed once and then stored for later retrieval,
    /// NOTE: Type parsing is expensive, so make sure the cache does not invalidate very often
    /// </summary>
    public class TypeParser
    {
        private DictionaryCacheProvider<Type, DatabaseTypeInfo> _cache = new DictionaryCacheProvider<Type, DatabaseTypeInfo>();
        private IDataConnection _connection;

        /// <summary>
        /// Occurs when a type is parsed
        /// </summary>
        public event EventHandler<TypeParsedEventArgs> OnTypeParsed;

        /// <summary>
        /// Triggers the OnTypeParsed event.
        /// </summary>
        public virtual void FireOnTypeParsed(TypeParsedEventArgs ea)
        {
            OnTypeParsed?.Invoke(this, ea);
        }

        public TypeParser(IDataConnection connection)
        {
            _connection = connection;
        }

        public void ClearCache()
        {
            _cache.ClearCache();
        }

        /// <summary>
        /// Gets the types fields.
        /// </summary>
        /// <param name="dataType">The type to parse</param>
        /// <returns></returns>
        public virtual IEnumerable<DataFieldInfo> GetTypeFields(Type dataType)
        {
            return GetTypeInfo(dataType).DataFields;
        }

        /// <summary>
        /// Gets the primary keys for a type
        /// </summary>
        /// <param name="dataType">The type to parse</param>
        /// <returns></returns>
        public virtual IEnumerable<DataFieldInfo> GetPrimaryKeys(Type dataType)
        {
            return GetTypeInfo(dataType).PrimaryKeys;
        }

        /// <summary>
        /// Gets a lot of information from a type
        /// </summary>
        /// <param name="type">The type to parse</param>
        /// <param name="bypassValidate">If true, type will not validate against datastore</param>
        /// <returns></returns>
        public virtual DatabaseTypeInfo GetTypeInfo(Type type, bool bypassValidate = false)
        {
            DatabaseTypeInfo toReturn = _cache.GetObject(type);

            if (toReturn == null)
            {
                lock (_cache)
                {
                    toReturn = _cache.GetObject(type);

                    if (toReturn == null)
                    {
                        toReturn = new DatabaseTypeInfo(type);
                        toReturn.IsDynamic = type.IsDynamic();

                        if (!type.IsSystemType())
                        {
                            ParseBypass(type, toReturn);
                            ParseDataInfo(type, toReturn);
                        }

                        StoreTypeInfo(bypassValidate, type, toReturn);
                    }
                }
            }

            return toReturn;
        }

        private void StoreTypeInfo(bool bypassValidate, Type type, DatabaseTypeInfo toAdd)
        {
            FireOnTypeParsed(new TypeParsedEventArgs(toAdd, type, bypassValidate));

            lock (_cache)
            {
                if (!_cache.ContainsKey(type))
                    _cache.StoreObject(type, toAdd);
            }
        }

        private void ParseDataInfo(Type type, DatabaseTypeInfo toAdd)
        {
            ParseTableName(type, toAdd);
            ParseView(type, toAdd);
            ParseDataFields(type, toAdd);
            ParseFunctionAttributes(type, toAdd);
            CheckForMultipleKeys(toAdd);
        }

        private void ParseView(Type type, DatabaseTypeInfo toAdd)
        {
            ViewAttribute dField = type.GetCustomAttributes(typeof(ViewAttribute), true).FirstOrDefault() as ViewAttribute;
            toAdd.IsView = dField != null ? true : false;
        }

        private void CheckForMultipleKeys(DatabaseTypeInfo toAdd)
        {
            if (toAdd.PrimaryKeys.Count > 1)
                toAdd.PrimaryKeys.ForEach(R => R.SetOnInsert = true);
        }

        /// <summary>
        /// Parses the additional init and OnTableCreate attributes
        /// </summary>
        /// <param name="type">The type to parse</param>
        /// <param name="toAdd">What to add the data to</param>
        protected void ParseFunctionAttributes(Type type, DatabaseTypeInfo toAdd)
        {
            toAdd.AdditionalInit = new List<AdditionalInitFunction>();
            toAdd.OnTableCreate = new List<AdditionalInitFunction>();

            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
            foreach (MethodInfo mi in methods)
            {
                QueryPredicateAttribute qpred = mi.GetCustomAttributes(typeof(QueryPredicateAttribute), true).FirstOrDefault() as QueryPredicateAttribute;
                if (qpred != null)
                    toAdd.QueryPredicate = new QueryPredicateFunction(mi);

                AdditionalInitAttribute aAtt = mi.GetCustomAttributes(typeof(AdditionalInitAttribute), true).FirstOrDefault() as AdditionalInitAttribute;
                if (aAtt != null)
                    toAdd.AdditionalInit.Add(new AdditionalInitFunction(mi));

                OnTableCreateAttribute tcAtt = mi.GetCustomAttributes(typeof(OnTableCreateAttribute), true).FirstOrDefault() as OnTableCreateAttribute;
                if (tcAtt != null)
                    toAdd.OnTableCreate.Add(new AdditionalInitFunction(mi));
            }
        }

        /// <summary>
        /// Parses the bypass validation attribute
        /// </summary>
        /// <param name="type">The type to parse</param>
        /// <param name="toAdd">What to add the data to</param>
        protected void ParseBypass(Type type, DatabaseTypeInfo toAdd)
        {
            toAdd.BypassValidation = false;

            BypassValidationAttribute dField = type.GetCustomAttributes(typeof(BypassValidationAttribute), true).FirstOrDefault() as BypassValidationAttribute;
            if (dField != null)
                toAdd.BypassValidation = true;

            //also don't validate compiler generated objects i.e. anon objects and so forth
            CompilerGeneratedAttribute cga = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).FirstOrDefault() as CompilerGeneratedAttribute;
            if (cga != null)
            {
                toAdd.BypassValidation = true;
                toAdd.IsCompilerGenerated = true;
            }
        }

        /// <summary>
        /// Parses data field information from a type
        /// </summary>
        /// <param name="type">The type to parse</param>
        /// <param name="toAdd">What to add the data to</param>
        protected void ParseDataFields(Type type, DatabaseTypeInfo toAdd)
        {
            toAdd.DataFields = new List<DataFieldInfo>();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Public);

            foreach (PropertyInfo pi in properties)
            {
                DataFieldAttribute dField = pi.GetCustomAttributes(typeof(DataFieldAttribute), true).FirstOrDefault() as DataFieldAttribute;
                IgnoredFieldAttribute ignoredField = pi.GetCustomAttributes(typeof(IgnoredFieldAttribute), true).FirstOrDefault() as IgnoredFieldAttribute;

                if (ignoredField == null)
                {
                    DataFieldInfo dfi = new DataFieldInfo();
                    dfi.PropertyType = pi.PropertyType;

                    ParseWithNoDefaults(dfi, dField);
                    ParseFieldName(dfi, dField, pi);
                    ParsePrimaryFieldType(dfi, pi);
                    ParseSetOnInsert(dfi, dField);
                    ParseLoad(dfi, dField);
                    ParsePropertyInfo(type, dfi, pi);
                    toAdd.DataFields.Add(dfi);
                }
            }
        }

        private void ParseWithNoDefaults(DataFieldInfo dfi, DataFieldAttribute dField)
        {
            if (dField != null)
            {
                dfi.PrimaryKeyType = dField.PrimaryKeyType;
                dfi.ForeignKeyType = dField.RelationshipType;
                dfi.DataFieldString = dField.FieldTypeString;
                dfi.DataFieldType = dField.FieldType;

                if (dField.FieldLength != -1) //wanted to use nullable for the attribute to, but not allowed :(
                    dfi.FieldLength = dField.FieldLength;
            }
        }

        /// <summary>
        /// Determines if a field should be loaded
        /// </summary>
        /// <param name="dfi">The field to check</param>
        /// <param name="dField">The data attribute if present, null otherwise</param>
        protected void ParseLoad(DataFieldInfo dfi, DataFieldAttribute dField)
        {
            dfi.LoadField = true;

            if (dField != null)
                dfi.LoadField = dField.LoadField;
        }

        /// <summary>
        /// Determines if a field should be inserted
        /// </summary>
        /// <param name="dfi">The field to check</param>
        /// <param name="dField">The data attribute if present, null otherwise</param>
        protected void ParseSetOnInsert(DataFieldInfo dfi, DataFieldAttribute dField)
        {
            dfi.SetOnInsert = true;

            if (dField != null)
                dfi.SetOnInsert = dField.SetOnInsert;
            else if (dfi.PrimaryKey)
            { // by default keys are not inserted
                dfi.SetOnInsert = false;
            }
        }

        /// <summary>
        /// Determines a fields name to use on the data store
        /// </summary>
        /// <param name="dfi">The field to check</param>
        /// <param name="dField">The data attribute if present, null otherwise</param>
        /// <param name="pi">The property information for the field</param>
        protected void ParseFieldName(DataFieldInfo dfi, DataFieldAttribute dField, PropertyInfo pi)
        {
            if (dField != null)
                dfi.FieldName = dField.FieldName;

            if (string.IsNullOrEmpty(dfi.FieldName))
                dfi.FieldName = pi.Name;

            dfi.EscapedFieldName = string.Concat(_connection.LeftEscapeCharacter, dfi.FieldName, _connection.RightEscapeCharacter);
        }

        /// <summary>
        /// Gets some stuff out of the property info
        /// </summary>
        /// <param name="dfi">The field to check</param>
        /// <param name="pi">The property information for the field</param>
        protected void ParsePropertyInfo(Type type, DataFieldInfo dfi, PropertyInfo pi)
        {
            dfi.Setter = GetSetter(type, pi);
            dfi.Getter = GetGetter(type, pi);
            dfi.PropertyName = pi.Name;
        }

        //generates a delegate like Func<object, object> foo = (obj, value) => ((ObjType)obj).Property;
        public Func<object, object> GetGetter(Type objType, PropertyInfo property)
        {
            //source prop types
            ParameterExpression objSourceType = Expression.Parameter(typeof(object));

            //step one create the casting expressions, this is to convert the obj param back to the source type
            Expression objCast = Expression.Convert(objSourceType, objType);

            //call the cast set method on the caste-d object with the casted value
            Expression action = Expression.Call(objCast, property.GetMethod);

            //we need to cast the return of action above to object to return
            Expression castedAction = Expression.Convert(action, typeof(object));

            //make it a lambda
            LambdaExpression function = Expression.Lambda(castedAction, objSourceType);

            return (Func<object, object>)function.Compile();
        }

        //generates a delegate like Action<object, object> foo = (obj, value) => ((ObjType)obj).Property = (valueType)value;
        public Action<object, object> GetSetter(Type objType, PropertyInfo property)
        {
            if (property.SetMethod != null)
            {
                //source prop types
                ParameterExpression valueSourceType = Expression.Parameter(typeof(object));
                ParameterExpression objSourceType = Expression.Parameter(typeof(object));

                //step one create the casting expressions
                Expression valueCast = Expression.Convert(valueSourceType, property.PropertyType);
                Expression objCast = Expression.Convert(objSourceType, objType);

                //call the cast set method on the caste-d object with the casted value
                Expression action = Expression.Call(objCast, property.SetMethod, valueCast);

                //make it a lambda
                LambdaExpression function = Expression.Lambda(action, objSourceType, valueSourceType);

                return (Action<object, object>)function.Compile();
            }
            else // no setter for this property, do a noop, more or less..
            {
                return (a, b) => { };
            }
        }

        /// <summary>
        /// If a field has a primary key type defined, gets the information for it
        /// </summary>
        /// <param name="dfi">The field to check</param>
        /// <param name="pi">The property information for the field</param>
        protected void ParsePrimaryFieldType(DataFieldInfo dfi, PropertyInfo pi)
        {
            dfi.PrimaryKey = false;

            if (pi.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                dfi.PrimaryKey = true;

            KeyAttribute ka = pi.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() as KeyAttribute;
            if (ka != null)
                dfi.PrimaryKey = true;
        }

        /// <summary>
        /// Determines the table name to use on the data store
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="toAdd">What to add the data to</param>
        protected void ParseTableName(Type type, DatabaseTypeInfo toAdd)
        {
            TableNameAttribute tName = type.GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            if (tName != null)
            {
                toAdd.UnescapedTableName = tName.TableName;
                toAdd.UnEscapedSchema = tName.Schema;
            }

            if (string.IsNullOrEmpty(toAdd.UnEscapedSchema))
                toAdd.UnEscapedSchema = _connection.DefaultSchema;

            if (string.IsNullOrEmpty(toAdd.UnescapedTableName))
                toAdd.UnescapedTableName = GenTableName(type);

            //escape table name
            toAdd.TableName = string.Concat(_connection.LeftEscapeCharacter, toAdd.UnescapedTableName, _connection.RightEscapeCharacter);

            if (!string.IsNullOrEmpty(toAdd.UnEscapedSchema))
                toAdd.Schema = string.Concat(_connection.LeftEscapeCharacter, toAdd.UnEscapedSchema, _connection.RightEscapeCharacter);
        }

        /// <summary>
        /// Adds an s to the table name if there is not one there already
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns></returns>
        protected string GenTableName(Type type)
        {
            string toReturn = type.Name;
            if (!toReturn.EndsWith("s"))
                toReturn = string.Concat(toReturn, "s");

            return toReturn;
        }
    }
}