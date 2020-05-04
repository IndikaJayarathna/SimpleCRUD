using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleCRUD
{
    public class Database
    {
        
        public static SqlSelect Select(string sqlQuery)
        {
            SqlSelect sqlSelect = new SqlSelect();

            sqlSelect.Details = sqlQuery;

            DataTable dataResults = new DataTable();

            SqlConnection sqlConn = Database.Open();

            try
            {
                SqlDataAdapter sqlCommand = new SqlDataAdapter(sqlQuery, sqlConn);

                sqlCommand.Fill(dataResults);

                sqlSelect.Successful = true;
                sqlSelect.Rows = dataResults.Rows.Count;
            }
            catch (Exception ex)
            {
                sqlSelect.Exception = ex;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();

                    sqlConn.Dispose();
                }
            }

            sqlSelect.DataView = new DataView(dataResults);

            return sqlSelect;

        }

        public static SqlInsert Insert(string sqlQuery)
        {
            byte[] bytNothing = new byte[0];

            return Insert(sqlQuery, bytNothing);
        }

        public static SqlInsert Insert(string sqlQuery, byte[] Data)
        {
            SqlInsert sqlInsert = new SqlInsert();

            sqlInsert.Details = sqlQuery;

            if (sqlQuery.Length > 6)
            {
                if (sqlQuery.Substring(0, sqlQuery.IndexOf(" ")).ToLower().Trim() == "insert")
                {
                    SqlConnection sqlConn = Open();

                    string guidID = string.Empty;
                    string dataTable = string.Empty;

                    // Extract the table name from the SQL statement.
                    if (sqlQuery.ToLower().IndexOf(" into [") > -1)
                    {
                        dataTable = sqlQuery.Replace("\r\n", string.Empty).Substring(0, sqlQuery.Replace("\r\n", string.Empty).ToLower().IndexOf(" into [") + 7);
                        dataTable = sqlQuery.Replace("\r\n", string.Empty).Replace(dataTable, string.Empty);
                        dataTable = dataTable.Substring(0, dataTable.IndexOf("]"));
                    }
                    else
                    {
                        dataTable = sqlQuery.Replace("\r\n", string.Empty).Substring(0, sqlQuery.Replace("\r\n", string.Empty).ToLower().IndexOf(" into ") + 6);
                        dataTable = sqlQuery.Replace("\r\n", string.Empty).Replace(dataTable, string.Empty);
                        dataTable = dataTable.Substring(0, dataTable.IndexOf(" "));
                    }

                    // Remove any trailling characters we don't need.
                    dataTable = dataTable.Replace("[", string.Empty).Replace("]", string.Empty).Trim();

                    // If a new ID has been requested in the SQL statement, then generate the GUID and add it to the statement.
                    if (sqlQuery.IndexOf("NEWID()") > -1)
                    {
                        guidID = Guid.NewGuid().ToString();

                        // Replace the SQL function NEWID() with the GUID we generated.
                        sqlQuery = sqlQuery.Replace("NEWID()", "'" + guidID + "'");

                        // Append the SQL statement to return the GUID if it was inserted.
                        sqlQuery += ";\r\n" +
                            "SELECT \r\n" +
                            "   [ID] \r\n" +
                            "FROM [" + dataTable + "] \r\n" +
                            "WHERE [ID] = '" + guidID + "'\r\n";
                    }

                    IDbCommand dbCommand = new SqlCommand();

                    dbCommand.CommandText = sqlQuery;
                    dbCommand.Connection = sqlConn;
                    dbCommand.CommandTimeout = 200;

                    // If there is data to be inserted then add it to the command as a parameter.
                    if (Data.Length > 0)
                    {
                        SqlParameter spParameter = new SqlParameter("@DATA", SqlDbType.Image, Data.Length);
                        spParameter.Value = Data;

                        dbCommand.Parameters.Add(spParameter);
                    }

                    sqlConn.Open();

                    try
                    {

                        IDataReader idrReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);


                        idrReader.Close();

                        sqlInsert.Successful = true;

                    }
                    catch (Exception exceptionError)
                    {

                        sqlInsert.Exception = exceptionError;
                    }
                    finally
                    {
                        if (sqlConn.State == ConnectionState.Open)
                        {
                            sqlConn.Close();

                            sqlConn.Dispose();
                        }
                    }
                }
            }


            return sqlInsert;
        }

        public static SqlUpdate Update(string sqlQuery)
        {
            byte[] bytNothing = new byte[0];

            return Update(sqlQuery, bytNothing);
        }

        public static SqlUpdate Update(string sqlQuery, byte[] sqlData)
        {
            SqlUpdate SqlUpdate = new SqlUpdate();

            SqlUpdate.Details = sqlQuery;

            SqlConnection sqlConn = Open();

            IDbCommand dbCommand = new SqlCommand();

            dbCommand.CommandText = sqlQuery;
            dbCommand.Connection = sqlConn;
            dbCommand.CommandTimeout = 200;

            if (sqlData.Length > 0)
            {
                SqlParameter spParameter = new SqlParameter("@DATA", SqlDbType.Image, sqlData.Length);
                spParameter.Value = sqlData;

                dbCommand.Parameters.Add(spParameter);
            }

            sqlConn.Open();

            SqlUpdate.Rows = dbCommand.ExecuteNonQuery();

            SqlUpdate.Successful = true;

            try
            {
            }
            catch (Exception ex)
            {
                SqlUpdate.Exception = ex;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();

                    sqlConn.Dispose();
                }
            }

            return SqlUpdate;
        }

        public static SqlDelete Delete(string sqlQuery)
        {
            SqlDelete SqlDelete = new SqlDelete();

            SqlDelete.Details = sqlQuery;

            SqlConnection sqlConn = Open();

            IDbCommand dbCommand = new SqlCommand();

            dbCommand.CommandText = sqlQuery;
            dbCommand.Connection = sqlConn;
            dbCommand.CommandTimeout = 200;

            sqlConn.Open();

            try
            {
                SqlDelete.Rows = dbCommand.ExecuteNonQuery();

                SqlDelete.Details = string.Empty;
                SqlDelete.Successful = true;
            }
            catch (Exception ex)
            {
                SqlDelete.Exception = ex;
            }
            finally
            {
                if (sqlConn.State == ConnectionState.Open)
                {
                    sqlConn.Close();

                    sqlConn.Dispose();
                }
            }

            return SqlDelete;
        }

        private static SqlConnection Open()
        {

            string connectionString = "Data Source=desktop-enhhncd;Initial Catalog=simpleCRUD;Integrated Security=True";


            SqlConnection scConnection = new SqlConnection(connectionString);

            // Test the active connection to see if it's working.
            try
            {
                scConnection.Open();
            }
            catch (Exception exConnection)
            {

            }
            finally
            {
                // If the connection was opened succesfully, close it.
                if (scConnection.State == ConnectionState.Open)
                {
                    scConnection.Close();
                }
            }

            return scConnection;
        }
        public struct SqlSelect
        {
            /// <summary>
            /// DataView containing the requested data.
            /// </summary>
            public DataView DataView;

            /// <summary>
            /// The number of rows returned from the query.
            /// </summary>
            public int Rows;

            /// <summary>
            /// Whether the query was successful or not.
            /// </summary>
            public bool Successful;

            /// <summary>
            /// The details of the query.
            /// </summary>
            public string Details;

            /// <summary>
            /// An exception for the query as a result of any errors.
            /// </summary>
            public Exception Exception;
        }
        public struct SqlInsert
        {

            /// <summary>
            /// Whether the query was successful or not.
            /// </summary>
            public bool Successful;

            /// <summary>
            /// The details of the query.
            /// </summary>
            public string Details;

            /// <summary>
            /// An exception for the query as a result of any errors.
            /// </summary>
            public Exception Exception;
        }
        public struct SqlUpdate
        {
            /// <summary>
            /// the number of rows updated.
            /// </summary>
            public int Rows;

            /// <summary>
            /// Whether the query was successful or not.
            /// </summary>
            public bool Successful;

            /// <summary>
            /// The details of the query.
            /// </summary>
            public string Details;

            /// <summary>
            /// An exception for the query as a result of any errors.
            /// </summary>
            public Exception Exception;
        }
        public struct SqlDelete
        {
            /// <summary>
            /// the number of rows deleted.
            /// </summary>
            public int Rows;

            /// <summary>
            /// Whether the query was successful or not.
            /// </summary>
            public bool Successful;

            /// <summary>
            /// The details of the query.
            /// </summary>
            public string Details;

            /// <summary>
            /// An exception for the query as a result of any errors.
            /// </summary>
            public Exception Exception;
        }
    }
}

    
