using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data.Common;
using ArabazonPlatform.Generic.Enum;
using Dapper;
using static ArabazonPlatform.Generic.Utility.DatabaseEngineException;

namespace ArabazonPlatform.Generic.Utility
{
    public class DatabaseEngine : IDisposable
    {
        private ConnectionType connectionType = ConnectionType.SQL;

        private SqlConnection sqlConnection;

        private string connectionString = string.Empty;

        private bool disposed;

        private string lastError = string.Empty;

        private SqlCommand cmd;

        public ConnectionType ConnectionType
        {
            get
            {
                return connectionType;
            }
            set
            {
                connectionType = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
            }
        }

        public string LastError
        {
            get
            {
                return lastError;
            }
            set
            {
                lastError = value;
            }
        }

        public DatabaseEngine()
        {
        }

        public DatabaseEngine(string connectionString)
        {
            this.connectionString = connectionString;
            sqlConnection = new SqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            lastError = string.Empty;
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                return true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                throw new DBConncetionException(ex.Message, ex.InnerException);
            }
        }

        private bool CloseConnection()
        {
            lastError = string.Empty;
            try
            {
                sqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                return false;
            }
        }

        public DataSet ExecuteSp(string spName)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteSp(spName, null);
            }

            return null;
        }

        public DataSet ExecuteSp(string spName, List<SqlParameter> sqlParameters)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                DataSet dataSet = new DataSet();
                SqlCommand sqlCommand = null;
                try
                {
                    if (!OpenConnection())
                    {
                        return null;
                    }

                    sqlCommand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = spName,
                        Connection = sqlConnection,
                        CommandTimeout = 500
                    };
                    if (sqlParameters != null)
                    {
                        for (int i = 0; i < sqlParameters.Count; i++)
                        {
                            sqlCommand.Parameters.Add(sqlParameters[i]);
                        }
                    }

                    new SqlDataAdapter(sqlCommand).Fill(dataSet);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
                finally
                {
                    CloseConnection();
                }

                return dataSet;
            }

            return null;
        }

        public SqlDataReader ExecuteSpReader(string spName)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteSpReader(spName, null);
            }

            return null;
        }

        public SqlDataReader ExecuteSpReader(string spName, List<SqlParameter> sqlParams)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                SqlDataReader sqlDataReader = null;
                try
                {
                    if (!OpenConnection())
                    {
                        return null;
                    }

                    cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = spName,
                        Connection = sqlConnection,
                        CommandTimeout = 500
                    };
                    if (sqlParams != null)
                    {
                        foreach (SqlParameter sqlParam in sqlParams)
                        {
                            cmd.Parameters.Add(sqlParam);
                        }
                    }

                    return cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception inner)
                {
                    throw new DBException("this SP '" + spName + "' encounter error", inner);
                }
            }

            return null;
        }

        public bool ExecuteNonQuerySp(string spName)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteNonQuerySp(spName, null);
            }

            return false;
        }

        public bool ExecuteNonQuerySp(string spName, List<SqlParameter> sqlParams)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                SqlCommand sqlCommand = null;
                try
                {
                    if (!OpenConnection())
                    {
                        return false;
                    }

                    sqlCommand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = spName,
                        Connection = sqlConnection,
                        CommandTimeout = 500
                    };
                    if (sqlParams != null)
                    {
                        for (int i = 0; i < sqlParams.Count; i++)
                        {
                            sqlCommand.Parameters.Add(sqlParams[i]);
                        }
                    }

                    sqlCommand.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    CloseConnection();
                }
            }

            return false;
        }

        public DataSet ExecuteQuery(string query)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteQuery(query, null);
            }

            return null;
        }

        public DataSet ExecuteQuery(string query, List<SqlParameter> sqlParameters)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                DataSet dataSet = new DataSet();
                SqlCommand sqlCommand = null;
                try
                {
                    if (!OpenConnection())
                    {
                        return null;
                    }

                    sqlCommand = new SqlCommand
                    {
                        CommandType = CommandType.Text,
                        CommandText = query,
                        Connection = sqlConnection,
                        CommandTimeout = 500
                    };
                    if (sqlParameters != null)
                    {
                        for (int i = 0; i < sqlParameters.Count; i++)
                        {
                            sqlCommand.Parameters.Add(sqlParameters[i]);
                        }
                    }

                    new SqlDataAdapter(sqlCommand).Fill(dataSet);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
                finally
                {
                    CloseConnection();
                }

                return dataSet;
            }

            return null;
        }

        public bool ExecuteNonQuery(string query)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteNonQuery(query, null);
            }

            return false;
        }

        public bool ExecuteNonQuery(string query, List<SqlParameter> sqlParams)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                SqlCommand sqlCommand = null;
                try
                {
                    if (!OpenConnection())
                    {
                        return false;
                    }

                    sqlCommand = new SqlCommand
                    {
                        CommandType = CommandType.Text,
                        CommandText = query,
                        Connection = sqlConnection,
                        CommandTimeout = 500
                    };
                    if (sqlParams != null)
                    {
                        for (int i = 0; i < sqlParams.Count; i++)
                        {
                            sqlCommand.Parameters.Add(sqlParams[i]);
                        }
                    }

                    sqlCommand.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    CloseConnection();
                }
            }

            return false;
        }

        public object GetOutParamValue(string Name)
        {
            return cmd.Parameters[Name].Value;
        }

        public IEnumerable<T> ExecuteDapperSp<T>(string spName)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteDapperSp<T>(spName, null);
            }

            return null;
        }

        public IEnumerable<T> ExecuteDapperSp<T>(string spName, DapperParameters dapperParameters)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                try
                {
                    if (!OpenConnection())
                    {
                        Task.Delay(500).Wait();
                    }
                    return sqlConnection.Query<T>(spName, dapperParameters?.DynamicParameters, null, buffered: true, null, CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
                finally
                {
                    CloseConnection();
                }
                return null;
            }
            return null;
        }

        public async Task<IEnumerable<T>> ExecuteDapperSpAsync<T>(string spName)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return await ExecuteDapperSpAsync<T>(spName, null);
            }

            return null;
        }

        public Task<IEnumerable<T>> ExecuteDapperSpAsync<T>(string spName, DapperParameters dapperParameters)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                try
                {
                    if (!OpenConnection())
                    {
                        return null;
                    }

                    return sqlConnection.QueryAsync<T>(spName, dapperParameters?.DynamicParameters, null, null, CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
                finally
                {
                    CloseConnection();
                }

                return null;
            }

            return null;
        }

        public IEnumerable<T> ExecuteDapperQuery<T>(string query)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return ExecuteDapperQuery<T>(query, null);
            }

            return null;
        }

        public IEnumerable<T> ExecuteDapperQuery<T>(string query, DapperParameters dapperParameters)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                try
                {
                    if (!OpenConnection())
                    {
                        return null;
                    }

                    return sqlConnection.Query<T>(query, dapperParameters?.DynamicParameters);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
                finally
                {
                    CloseConnection();
                }

                return null;
            }

            return null;
        }

        public async Task<IEnumerable<T>> ExecuteDapperQueryAsync<T>(string query)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                return await ExecuteDapperQueryAsync<T>(query, null);
            }

            return null;
        }

        public async Task<IEnumerable<T>> ExecuteDapperQueryAsync<T>(string query, DapperParameters dapperParameters)
        {
            ConnectionType connectionType = this.connectionType;
            if (connectionType == ConnectionType.SQL)
            {
                lastError = string.Empty;
                try
                {
                    if (!OpenConnection())
                    {
                        return null;
                    }

                    return await sqlConnection.QueryAsync<T>(query, dapperParameters?.DynamicParameters);
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
                finally
                {
                    CloseConnection();
                }

                return null;
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }
    }
}
