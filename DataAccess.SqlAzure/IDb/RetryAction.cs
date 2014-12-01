using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.SqlAzure.IDb
{
    public static class RetryAction
    {
        private const int RETRY_TIMES = 5;
        private const int RETRY_WAIT = 200;

        public static void RunRetryableAction(DbConnection connection, Action Action)
        {
            List<SqlException> agg = null;

            while (true)
            {
                try
                {
                    if (connection != null)
                    {
                        if (connection.State != System.Data.ConnectionState.Open)
                            connection.Open();
                    }

                    Action();
                    break;
                }
                catch (SqlException sqlex)
                {
                    if(IsTransient(sqlex))
                    {
                        if (agg == null)
                            agg = new List<SqlException>();

                        agg.Add(sqlex);
                        Thread.Sleep(RETRY_WAIT);

                        if (agg.Count >= RETRY_TIMES)
                            throw new AggregateException(agg);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static Return RunRetryableAction<Return>(DbConnection connection, Func<Return> Action)
        {
            Return toReturn = default(Return);
            List<SqlException> agg = null;

            while (true)
            {
                try
                {
                    if (connection != null)
                    {
                        if (connection.State != System.Data.ConnectionState.Open)
                            connection.Open();
                    }

                    toReturn = Action();
                    break;
                }
                catch (SqlException sqlex)
                {
                    if (IsTransient(sqlex))
                    {
                        if (agg == null)
                            agg = new List<SqlException>();

                        agg.Add(sqlex);
                        Thread.Sleep(RETRY_WAIT);

                        if (agg.Count >= RETRY_TIMES)
                            throw new AggregateException(agg);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return toReturn;
        }


        #region license
        //===============================================================================
        // Microsoft patterns & practices Enterprise Library
        // Enterprise Application Block Library
        //===============================================================================
        // Copyright © Microsoft Corporation.  All rights reserved.
        // THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
        // OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
        // LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
        // FITNESS FOR A PARTICULAR PURPOSE.
        // http://www.nuget.org/packages/EnterpriseLibrary.Source.WindowsAzure
        //===============================================================================
        #endregion
        public static bool IsTransient(Exception ex)
        {
            if (ex != null)
            {
                SqlException sqlException;
                if ((sqlException = ex as SqlException) != null)
                {
                    // Enumerate through all errors found in the exception.
                    foreach (SqlError err in sqlException.Errors)
                    {
                        switch (err.Number)
                        {
                            // SQL Error Code: 40501
                            // The service is currently busy. Retry the request after 10 seconds. Code: (reason code to be decoded).
                            case 40501:
                                return true;
                            // SQL Error Code: 40197
                            // The service has encountered an error processing your request. Please try again.
                            case 40197:
                            // SQL Error Code: 10053
                            // A transport-level error has occurred when receiving results from the server.
                            // An established connection was aborted by the software in your host machine.
                            case 10053:
                            // SQL Error Code: 10054
                            // A transport-level error has occurred when sending the request to the server. 
                            // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                            case 10054:
                            // SQL Error Code: 10060
                            // A network-related or instance-specific error occurred while establishing a connection to SQL Server. 
                            // The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server 
                            // is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed 
                            // because the connected party did not properly respond after a period of time, or established connection failed 
                            // because connected host has failed to respond.)"}
                            case 10060:
                            // SQL Error Code: 40613
                            // Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer 
                            // support, and provide them the session tracing ID of ZZZZZ.
                            case 40613:
                            // SQL Error Code: 40143
                            // The service has encountered an error processing your request. Please try again.
                            case 40143:
                            // SQL Error Code: 233
                            // The client was unable to establish a connection because of an error during connection initialization process before login. 
                            // Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy 
                            // to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. 
                            // (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)
                            case 233:
                            // SQL Error Code: 64
                            // A connection was successfully established with the server, but then an error occurred during the login process. 
                            // (provider: TCP Provider, error: 0 - The specified network name is no longer available.) 
                            case 64:
                            // DBNETLIB Error Code: 20
                            // The instance of SQL Server you attempted to connect to does not support encryption.
                            case 20:
                                return true;
                        }
                    }
                }
                else if (ex is TimeoutException)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
