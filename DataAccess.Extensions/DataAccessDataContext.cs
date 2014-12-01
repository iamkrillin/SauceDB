using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.Core.Interfaces;
using System.Data;
using System.Collections;
using System.Linq.Expressions;
using DataAccess.Core.Data;

/// <summary>
/// This class implements a global datastore context as a set of extension methods
/// </summary>
public static class DataAccessDataContext
{
    private static IDataStore _dstore;

    /// <summary>
    /// This will set the global datastore context
    /// </summary>
    /// <param name="o"></param>
    /// <param name="dstore"></param>
    public static void SetDataStore(this object o, IDataStore dstore)
    {
        _dstore = dstore;
    }

    private static void CheckStore()
    {
        if (_dstore == null)
        {
            throw new Exception("Please call SetDataStore() before using these extension methods");
        }
    }

    /// <summary>
    /// Will execute a query and return the raw data
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static IQueryData ExecuteQuery(this IDbCommand command)
    {
        CheckStore();
        return _dstore.ExecuteQuery(command);
    }

    /// <summary>
    /// Returns the inuse data store
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IDataStore GetDataStore(this object item)
    {
        return _dstore;
    }

    /// <summary>
    /// Inserts an object
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns></returns>
    public static bool InsertObject(this object item)
    {
        CheckStore();
        return _dstore.InsertObject(item);
    }

    /// <summary>
    /// Loads a list of objects from the data store
    /// </summary>
    /// <typeparam name="T">The type to load</typeparam>
    /// <param name="o"></param>
    /// <param name="Ids">The primary key(s)</param>
    /// <returns></returns>
    public static IEnumerable<T> LoadObjects<T>(this object o, IEnumerable Ids)
    {
        CheckStore();
        return _dstore.LoadObjects<T>(Ids);
    }

    /// <summary>
    /// Loads an object
    /// </summary>
    /// <typeparam name="T">The type to load</typeparam>
    /// <param name="PrimaryKey">The primary key</param>
    /// /// <param name="o"></param>
    /// <param name="LoadAllFields">if set to <c>true</c> [The load field attribute tag is ignored].</param>
    public static T LoadObject<T>(this object o, object PrimaryKey, bool LoadAllFields)
    {
        CheckStore();
        return _dstore.LoadObject<T>(PrimaryKey, LoadAllFields);
    }

    /// <summary>
    /// Loads an object
    /// </summary>
    /// <typeparam name="T">The type to load</typeparam>
    /// <param name="PrimaryKey">The primary key</param>
    /// /// <param name="o"></param>
    public static T LoadObject<T>(this object o, object PrimaryKey)
    {
        CheckStore();
        return _dstore.LoadObject<T>(PrimaryKey);
    }

    /// <summary>
    /// Execute a command an loads an object
    /// </summary>
    /// <typeparam name="T">The type to load</typeparam>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static T ExecuteCommandLoadObject<T>(this IDbCommand command)
    {
        CheckStore();
        return _dstore.ExecuteCommandLoadObject<T>(command);
    }

    /// <summary>
    /// Loads an entire table
    /// </summary>
    /// <param name="item">The type to load</param>
    public static IEnumerable<object> LoadEntireTable(this Type item)
    {
        CheckStore();
        return _dstore.LoadEntireTable(item);
    }

    /// <summary>
    /// Loads an entire table
    /// </summary>
    /// <typeparam name="T">The type to load</typeparam>
    public static IEnumerable<T> LoadEntireTable<T>(this object o)
    {
        CheckStore();
        return _dstore.LoadEntireTable<T>();
    }

    /// <summary>
    /// Returns the resolved table name for a type
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns></returns>
    public static string GetTableName(this Type t)
    {
        CheckStore();
        return _dstore.GetTableName(t);
    }

    /// <summary>
    /// Returns the resolved table name for a type
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <returns></returns>
    public static string GetTableName<T>(this object o)
    {
        CheckStore();
        return _dstore.GetTableName<T>();
    }

    /// <summary>
    /// Executes a command and loads a list
    /// </summary>
    /// <param name="objectType">The type to load in the list</param>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static IEnumerable<object> ExecuteCommandLoadList(this Type objectType, IDbCommand command)
    {
        CheckStore();
        return _dstore.ExecuteCommandLoadList(objectType, command);
    }

    /// <summary>
    /// Executes a command and loads a list
    /// </summary>
    /// <typeparam name="T">The type to load in the list</typeparam>
    /// <param name="command">The command to execute</param>
    /// <returns></returns>
    public static IEnumerable<T> ExecuteCommandLoadList<T>(this IDbCommand command)
    {
        CheckStore();
        return _dstore.ExecuteCommandLoadList<T>(command);
    }

    /// <summary>
    /// Returns the key value for an object
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <param name="item">The object to extract from</param>
    /// <returns></returns>
    public static object GetKeyForItemType(this Type type, object item)
    {
        CheckStore();
        return _dstore.GetKeyForItemType(type, item);
    }

    /// <summary>
    /// Loads an object from the data store, the key must be set
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool LoadObject(this object item)
    {
        CheckStore();
        return _dstore.LoadObject(item);
    }

    /// <summary>
    /// Loads an object from the data store, the key must be set
    /// </summary>
    /// <param name="item">The object to load</param>
    /// <param name="LoadAllFields">If true the loadfield=false will be ignored</param>
    /// <returns></returns>
    public static bool LoadObject(this object item, bool LoadAllFields)
    {
        CheckStore();
        return _dstore.LoadObject(item, LoadAllFields);
    }

    /// <summary>
    /// Inserts a list of items into the data store
    /// </summary>
    /// <param name="items">The items to insert</param>
    /// <returns></returns>
    public static bool InsertObjects(this IList items)
    {
        CheckStore();
        return _dstore.InsertObjects(items);
    }

    /// <summary>
    /// Updates all items in the list to the datastore, primary keys must be set
    /// </summary>
    /// <param name="items"></param>
    /// <returns>The items that failed to update</returns>
    public static IEnumerable<T> UpdateObjects<T>(this IEnumerable<T> items)
    {
        CheckStore();
        return _dstore.UpdateObjects<T>(items);
    }

    /// <summary>
    /// Determines if an object already exists in the data store, based on the primary key
    /// </summary>
    /// <param name="item">The object to check</param>
    /// <returns></returns>
    public static bool IsNew(this object item)
    {
        CheckStore();
        return _dstore.IsNew(item);
    }

    /// <summary>
    /// Updates an object on the data store, primary key must be set
    /// </summary>
    /// <param name="item">The item to update</param>
    /// <returns></returns>
    public static bool UpdateObject(this object item)
    {
        CheckStore();
        return _dstore.UpdateObject(item);
    }

    /// <summary>
    /// Deletes an objet from the data store, primary key must be set
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns></returns>
    public static bool DeleteObject(this object item)
    {
        CheckStore();
        return _dstore.DeleteObject(item);
    }

    /// <summary>
    /// Executes a command on the data store
    /// </summary>
    /// <param name="command">The command to execute</param>
    public static int ExecuteCommand(this IDbCommand command)
    {
        CheckStore();
        return _dstore.ExecuteCommand(command);
    }

    /// <summary>
    /// Returns a comma separated list of the fields on an object
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns></returns>
    public static string GetSelectList(this Type t)
    {
        CheckStore();
        return _dstore.GetSelectList(t);
    }

    /// <summary>
    /// This function will return an IQueryable appropriate for using with LINQ
    /// </summary>
    /// <typeparam name="T">The type to query</typeparam>
    /// <returns></returns>
    public static IQueryable<T> Query<T>(this object o)
    {
        CheckStore();
        return _dstore.Query<T>();
    }
    /// <summary>
    /// Will delete a list of items from the datastore based on a given expression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public static int DeleteObjects<T>(this object t, Expression<Func<T, bool>> criteria)
    {
        CheckStore();
        return _dstore.DeleteObjects(criteria);
    }

    /// <summary>
    /// Will add a parameter to the command appropriate for the global data store
    /// </summary>
    /// <param name="cmd">The command</param>
    /// <param name="ParmName">The parm name</param>
    /// <param name="value">The value you want to add</param>
    public static void AddParameter(this IDbCommand cmd, string ParmName, object value)
    {
        CheckStore();
        IDbDataParameter parm = _dstore.Connection.GetParameter(ParmName, value);
        cmd.Parameters.Add(parm);
    }

    /// <summary>
    /// Will add a parameter to the command appropriate for the datastore you specify
    /// </summary>
    /// <param name="cmd">The command</param>
    /// <param name="ParmName">The parm name</param>
    /// <param name="value">The value you want to add</param>
    /// <param name="dstore">The datastore</param>
    public static void AddParameter(this IDbCommand cmd, IDataStore dstore, string ParmName, object value)
    {
        CheckStore();
        IDbDataParameter parm = dstore.Connection.GetParameter(ParmName, value);
        cmd.Parameters.Add(parm);
    }
}
