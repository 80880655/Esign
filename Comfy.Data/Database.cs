using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Transactions;
using Comfy.Data.Core;

namespace Comfy.Data
{
    /// <summary>
    /// A DAAB style database object.
    /// </summary>
    public sealed class Database
    {
        #region Static Members

        /// <summary>
        /// Get the default database, a default database is mapping the last connection string in config file, and can be modified manually.
        /// </summary>
        public static Database Default;

        /// <summary>
        /// Initializes the <see cref="T:Database"/> class.
        /// </summary>
        static Database()
        {
            if (DbProviders.DbProviderFactory.Default == null)
            {
                Default = null;
            }
            else
            {
                Default = new Database(DbProviders.DbProviderFactory.Default);
            }
        }

        #endregion

        #region Private Members

        private readonly ISqlQueryFactory queryFactory;
        private readonly DbProviders.DbProvider dbProvider;

        private DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
        {
            DbCommand command = dbProvider.DbProviderFactory.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;

            return command;
        }
        private void DoLoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return;
            }

            using (DbDataAdapter adapter = GetDataAdapter())
            {
                ((IDbDataAdapter)adapter).SelectCommand = command;

                try
                {
                    string systemCreatedTableNameRoot = "Table";
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemCreatedTableName = (i == 0)
                             ? systemCreatedTableNameRoot
                             : systemCreatedTableNameRoot + i;

                        adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
                    }

                    adapter.Fill(dataSet);
                }
                catch
                {
                    throw;
                }
            }
        }
        private object DoExecuteScalar(DbCommand command)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return null;
            }

            try
            {
                object returnValue = command.ExecuteScalar();
                return returnValue;
            }
            catch
            {
                throw;
            }
            finally
            {
                CloseConnection(command);
            }
        }
        private int DoExecuteNonQuery(DbCommand command)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return 0;
            }

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
            catch
            {
                throw;
            }
        }
        private IDataReader DoExecuteReader(DbCommand command, CommandBehavior cmdBehavior)
        {
            if (IsBatchConnection && batchCommander.batchSize > 1)
            {
                batchCommander.Process(command);
                return null;
            }

            try
            {
                IDataReader reader = command.ExecuteReader(cmdBehavior);
                return reader;
            }
            catch
            {
                throw;
            }
        }
        private DbTransaction BeginTransaction(DbConnection connection)
        {
            return connection.BeginTransaction();
        }
        private IDbTransaction BeginTransaction(DbConnection connection, System.Data.IsolationLevel il)
        {
            return connection.BeginTransaction(il);
        }
        private void PrepareCommand(DbCommand command, DbConnection connection)
        {
            command.Connection = connection;

            if (this.dbProvider.GetType() == typeof(DbProviders.MsAccess.AccessDbProvider))
            {
                command.CommandText = FilterNTextPrefix(command.CommandText);
            }
        }
        private void PrepareCommand(DbCommand command, DbTransaction transaction)
        {
            PrepareCommand(command, transaction.Connection);
            command.Transaction = transaction;

            if (this.dbProvider.GetType() == typeof(DbProviders.MsAccess.AccessDbProvider))
            {
                command.CommandText = FilterNTextPrefix(command.CommandText);
            }
        }
        private static void ConfigureParameter(DbParameter param, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            param.DbType = dbType;
            param.Size = size;
            param.Value = (value == null) ? DBNull.Value : value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
        }
        private DbParameter CreateParameter(string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter param = CreateParameter(name);
            ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }
        private DbParameter CreateParameter(string name)
        {
            DbParameter param = dbProvider.DbProviderFactory.CreateParameter();
            param.ParameterName = name;

            return param;
        }
        private string FilterNTextPrefix(string sql)
        {
            if (sql == null)
            {
                return sql;
            }

            return sql.Replace(" N'", " '");
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        private void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText,
            DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> within the given 
        /// <paramref name="transaction" /> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            LoadDataSet(command, dataSet, new string[] { tableName });
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" /> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"/> to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string tableName, DbTransaction transaction)
        {
            LoadDataSet(command, dataSet, new string[] { tableName }, transaction);
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (IsBatchConnection)
            {
                PrepareCommand(command, GetConnection(true));
                DoLoadDataSet(command, dataSet, tableNames);
            }
            else
            {
                using (DbConnection connection = GetConnection())
                {
                    PrepareCommand(command, connection);
                    DoLoadDataSet(command, dataSet, tableNames);
                }
            }
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in  a transaction.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command to execute to fill the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command in.</para>
        /// </param>
        private void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction)
        {
            PrepareCommand(command, transaction);
            DoLoadDataSet(command, dataSet, tableNames);
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        private void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        #endregion

        #region Close Connection

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="command">The command.</param>
        public void CloseConnection(DbCommand command)
        {
            if (command != null && command.Connection.State != ConnectionState.Closed && batchConnection == null)
            {
                if (command.Transaction == null)
                {
                    CloseConnection(command.Connection);
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="conn">The conn.</param>
        public void CloseConnection(DbConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Closed)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="tran">The tran.</param>
        public void CloseConnection(DbTransaction tran)
        {
            if (tran.Connection != null)
            {
                CloseConnection(tran.Connection);
                tran.Dispose();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="dbProvider">The db provider.</param>
        public Database(DbProviders.DbProvider dbProvider)
        {
            //Check.Require(dbProvider != null, "dbProvider could not be null.");

            this.dbProvider = dbProvider;
            this.queryFactory = dbProvider.GetQueryFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="connectionString">The connection string.</param>
        public Database(DatabaseType type, string connectionString)
        {
            //Check.Require(!string.IsNullOrEmpty(connectionString), "connectionString could not be null or empty.");
            switch (type)
            {
                case DatabaseType.MsAccess:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider(null, typeof(DbProviders.MsAccess.AccessDbProvider).ToString(), connectionString);
                    break;
                case DatabaseType.SqlServer:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider(null, typeof(DbProviders.SqlServer.SqlDbProvider).ToString(), connectionString);
                    break;
                case DatabaseType.SqlServer9:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider(null, typeof(DbProviders.SqlServer.SqlDbProvider9).ToString(), connectionString);
                    break;
                case DatabaseType.MySql:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider("Comfy.Data.AdditionalDbProviders", "Comfy.Data.DbProviders.MySql.MySqlDbProvider", connectionString);
                    break;
                case DatabaseType.Oracle:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider(null, typeof(DbProviders.Oracle.OracleDbProvider).ToString(), connectionString);
                    break;
                case DatabaseType.Sqlite:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider("Comfy.Data.AdditionalDbProviders", "Comfy.Data.DbProviders.Sqlite.SqliteDbProvider", connectionString);
                    break;
                case DatabaseType.PostgreSql:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider("Comfy.Data.AdditionalDbProviders", "Comfy.Data.DbProviders.PostgreSql.PostgreSqlDbProvider", connectionString);
                    break;
                case DatabaseType.DB2:
                    this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider("Comfy.Data.AdditionalDbProviders", "Comfy.Data.DbProviders.DB2.DB2DbProvider", connectionString);
                    break;
                default:
                    throw new NotSupportedException("Unknow DatabaseType.");
            }

            this.queryFactory = dbProvider.GetQueryFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public Database(string connectionStringName)
        {
            this.dbProvider = DbProviders.DbProviderFactory.CreateDbProvider(connectionStringName);
            this.queryFactory = dbProvider.GetQueryFactory();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connect string.
        /// </summary>
        /// <value>The connect string.</value>
        public string ConnectionString
        {
            get
            {
                return dbProvider.ConnectionString;
            }
        }

        /// <summary>
        /// Get the QueryFactory, which can be used to construct complex CRUD command.
        /// </summary>
        public ISqlQueryFactory QueryFactory
        {
            get { return this.queryFactory; }
        }

        /// <summary>
        /// Gets the db provider.
        /// </summary>
        /// <value>The db provider.</value>
        public DbProviders.DbProvider DbProvider
        {
            get
            {
                return dbProvider;
            }
        }

        #endregion

        #region Public Methods

        #region Factory Methods

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            if (batchConnection == null)
            {
                return CreateConnection();
            }
            else
            {
                return batchConnection;
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="tryOpen">if set to <c>true</c> [try open].</param>
        /// <returns></returns>
        public DbConnection GetConnection(bool tryOpen)
        {
            if (batchConnection == null)
            {
                return CreateConnection(tryOpen);
            }
            else
            {
                return batchConnection;
            }
        }

        /// <summary>
        /// <para>When overridden in a derived class, gets the connection for this database.</para>
        /// <seealso cref="DbConnection"/>        
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DbConnection"/> for this database.</para>
        /// </returns>
        public DbConnection CreateConnection()
        {
            DbConnection newConnection = dbProvider.DbProviderFactory.CreateConnection();
            newConnection.ConnectionString = ConnectionString;

            return newConnection;
        }

        /// <summary>
        /// <para>When overridden in a derived class, gets the connection for this database.</para>
        /// <seealso cref="DbConnection"/>        
        /// </summary>
        /// <returns>
        /// <para>The <see cref="DbConnection"/> for this database.</para>
        /// </returns>
        public DbConnection CreateConnection(bool tryOpenning)
        {
            if (!tryOpenning)
            {
                return CreateConnection();
            }

            DbConnection connection = null;
            try
            {
                connection = CreateConnection();
                connection.Open();
            }
            catch (DataException)
            {
                CloseConnection(connection);

                throw;
            }

            return connection;
        }

        /// <summary>
        /// <para>When overridden in a derived class, creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
        public DbCommand GetStoredProcCommand(string storedProcedureName)
        {
            //Check.Require(!string.IsNullOrEmpty(storedProcedureName), "storedProcedureName could not be null.");

            return CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
        }

        /// <summary>
        /// <para>When overridden in a derived class, creates an <see cref="DbCommand"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
        public DbCommand GetSqlStringCommand(string query)
        {
            //Check.Require(!string.IsNullOrEmpty(query), "query could not be null.");

            return CreateCommandByCommandType(CommandType.Text, query);
        }

        /// <summary>
        /// Gets a DbDataAdapter with Standard update behavior.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        public DbDataAdapter GetDataAdapter()
        {
            return dbProvider.DbProviderFactory.CreateDataAdapter();
        }

        #endregion

        #region Basic Execute Methods

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public DataSet ExecuteDataSet(DbCommand command)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.Locale = CultureInfo.InvariantCulture;
                LoadDataSet(command, dataSet, "Table");
                return dataSet;
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command"><para>The <see cref="DbCommand"/> to execute.</para></param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction)
        {
            try
            {
                DataSet dataSet = new DataSet();
                dataSet.Locale = CultureInfo.InvariantCulture;
                LoadDataSet(command, dataSet, "Table", transaction);
                return dataSet;
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public DataSet ExecuteDataSet(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public DataSet ExecuteDataSet(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteDataSet(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbCommand command)
        {
            try
            {
                if (IsBatchConnection)
                {
                    PrepareCommand(command, GetConnection(true));
                    return ExecuteScalar(command);
                }
                else
                {
                    using (DbConnection connection = GetConnection(true))
                    {
                        PrepareCommand(command, connection);
                        return DoExecuteScalar(command);
                    }
                }
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbCommand command, DbTransaction transaction)
        {
            try
            {
                PrepareCommand(command, transaction);
                return DoExecuteScalar(command);
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" />  and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
        /// within the given <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbCommand command)
        {
            try
            {
                if (IsBatchConnection)
                {
                    PrepareCommand(command, GetConnection(true));
                    return DoExecuteNonQuery(command);
                }
                else
                {
                    using (DbConnection connection = GetConnection(true))
                    {
                        PrepareCommand(command, connection);
                        return DoExecuteNonQuery(command);
                    }
                }
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbCommand command, DbTransaction transaction)
        {
            try
            {
                PrepareCommand(command, transaction);
                return DoExecuteNonQuery(command);
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteNonQuery(command, transaction);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(DbCommand command)
        {
            try
            {
                if (IsBatchConnection)
                {
                    PrepareCommand(command, GetConnection(true));
                    return DoExecuteReader(command, CommandBehavior.Default);
                }
                else
                {
                    DbConnection connection = GetConnection(true);
                    PrepareCommand(command, connection);

                    try
                    {
                        return DoExecuteReader(command, CommandBehavior.CloseConnection);
                    }
                    catch (DataException)
                    {
                        CloseConnection(connection);

                        throw;
                    }
                }
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a transaction and returns an <see cref="IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="IDataReader"/> object.</para>
        /// </returns>        
        public IDataReader ExecuteReader(DbCommand command, DbTransaction transaction)
        {
            try
            {
                PrepareCommand(command, transaction);
                return DoExecuteReader(command, CommandBehavior.Default);
            }
            catch (Exception ex) { throw new SqlException(ex, DataUtils.ToString(command)); }
        }

        #endregion

        #region Extended Execute Methods

        /// <summary>
        /// Executes the batch insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnNames">The column names.</param>
        /// <param name="columnTypes">The column types.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="tran">The tran.</param>
        /// <returns></returns>
        public int ExecuteBatchInsert(string tableName, string[] columnNames, DbType[] columnTypes, object[][] rows,
            int batchSize, DbTransaction tran)
        {
            //Check.Require(!string.IsNullOrEmpty(tableName), "tableName could not be null or empty.");
            //Check.Require(columnNames != null && columnNames.Length > 0, "columnNames could not be null or empty.");
            //Check.Require(columnTypes != null && columnNames.Length > 0, "columnNames could not be null or empty.");
            //Check.Require(rows != null && columnNames.Length > 0, "columnNames could not be null or empty.");
            //Check.Require(columnNames.Length == columnTypes.Length && columnNames.Length == rows[0].Length,
            //    "length of column's names, types and values must equal.");
            //Check.Require(batchSize > 0, "batchSize must > 0.");

            int columnCount = columnNames.Length;
            int retCount = 0;

            if (typeof(DbProviders.SqlServer.SqlDbProvider).IsAssignableFrom(dbProvider.GetType()))
            {
                int insertedRowCount = 0;
                int i = 0;
                while (insertedRowCount < rows.Length)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO ");
                    sb.Append(tableName);
                    sb.Append(" (");
                    for (i = 0; i < columnCount; ++i)
                    {
                        if (i > 0)
                        {
                            sb.Append(',');
                        }
                        sb.Append(columnNames[i]);
                    }
                    sb.Append(") exec('");
                    for (i = 0; i < batchSize && insertedRowCount < rows.Length; ++i)
                    {
                        sb.Append("SELECT ");
                        for (int j = 0; j < columnCount; ++j)
                        {
                            if (j > 0)
                            {
                                sb.Append(',');
                            }
                            sb.Append(DataUtils.ToString(columnTypes[j], rows[insertedRowCount][j]).Replace("'", "''"));
                        }
                        sb.Append(';');

                        ++insertedRowCount;
                    }
                    sb.Append("')");

                    DbCommand cmd = GetSqlStringCommand(sb.ToString());
                    retCount += (tran == null ? ExecuteNonQuery(cmd) : ExecuteNonQuery(cmd, tran));
                }
            }
            else
            {
                DbProviders.DbProviderOptions options = dbProvider.Options;
                if (options.SupportADO20Transaction && options.UseADO20TransactionAsDefaultIfSupport && tran == null)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
                    {
                        for (int i = 0; i < rows.Length; ++i)
                        {
                            DbCommand cmd = queryFactory.CreateInsertCommand(tableName, columnNames, columnTypes, rows[i]);
                            retCount += ExecuteNonQuery(cmd);
                        }

                        scope.Complete();
                    }
                }
                else
                {
                    if (tran != null)
                    {
                        for (int i = 0; i < rows.Length; ++i)
                        {
                            DbCommand cmd = queryFactory.CreateInsertCommand(tableName, columnNames, columnTypes, rows[i]);
                            retCount += ExecuteNonQuery(cmd, tran);
                        }
                    }
                    else
                    {
                        DbTransaction t = BeginTransaction();
                        try
                        {
                            for (int i = 0; i < rows.Length; ++i)
                            {
                                DbCommand cmd = queryFactory.CreateInsertCommand(tableName, columnNames, columnTypes, rows[i]);
                                retCount += ExecuteNonQuery(cmd, t);
                            }

                            t.Commit();
                        }
                        catch
                        {
                            t.Rollback();

                            throw;
                        }
                        finally
                        {
                            CloseConnection(t);
                        }
                    }
                }
            }

            return retCount;
        }

        /// <summary>
        /// Executes the batch insert.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnNames">The column names.</param>
        /// <param name="columnTypes">The column types.</param>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public int ExecuteBatchInsert(string tableName, string[] columnNames, DbType[] columnTypes, object[][] rows, int batchSize)
        {
            return ExecuteBatchInsert(tableName, columnNames, columnTypes, rows, batchSize, null);
        }

        /// <summary>
        /// Executes the insert return auto increment ID.
        /// </summary>
        /// <param name="basicInsertCmd">The basic insert CMD.</param>
        /// <param name="tran">The tran.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="autoIncrementColumn">The auto increment column.</param>
        /// <returns></returns>
        public int ExecuteInsertReturnAutoIncrementID(DbCommand basicInsertCmd,
            DbTransaction tran, string tableName, string autoIncrementColumn)
        {
            //Check.Require(basicInsertCmd != null, "basicInsertCmd could not be null.");
            //Check.Require(!string.IsNullOrEmpty(tableName), "tableName could not be null or empty.");

            string selectLastInsertIDSql = null;
            if (!string.IsNullOrEmpty(autoIncrementColumn))
            {
                Dictionary<string, string> additionalOptions = null;
                if (this.dbProvider is DbProviders.Oracle.OracleDbProvider && (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["OracleGlobalAutoIncrementSeqeunceName"])))
                {
                    additionalOptions = new Dictionary<string, string>();
                    additionalOptions.Add("OracleGlobalAutoIncrementSeqeunceName", System.Configuration.ConfigurationManager.AppSettings["OracleGlobalAutoIncrementSeqeunceName"]);
                }
                selectLastInsertIDSql = dbProvider.Options.GetSelectLastInsertAutoIncrementIDSql(tableName, autoIncrementColumn, additionalOptions);
            }
            object retVal = 0;

            if ((!IsBatchConnection) && autoIncrementColumn != null && selectLastInsertIDSql != null)
            {

                if (dbProvider is DbProviders.SqlServer.SqlDbProvider || dbProvider.Options.SupportMultiSqlStatementInOneCommand)
                {
                    basicInsertCmd.CommandText = basicInsertCmd.CommandText + ';' + selectLastInsertIDSql;
                    if (tran == null)
                    {
                        retVal = ExecuteScalar(basicInsertCmd);
                    }
                    else
                    {
                        retVal = ExecuteScalar(basicInsertCmd, tran);
                    }

                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else if (dbProvider is DbProviders.Oracle.OracleDbProvider || selectLastInsertIDSql.Contains(".CURRVAL FROM DUAL"))
                {
                    if (tran == null)
                    {
                        ExecuteNonQuery(basicInsertCmd);
                        retVal = ExecuteScalar(CommandType.Text, selectLastInsertIDSql);
                    }
                    else
                    {
                        ExecuteNonQuery(basicInsertCmd, tran);
                        retVal = ExecuteScalar(tran, CommandType.Text, selectLastInsertIDSql);
                    }

                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else if (!dbProvider.Options.SupportADO20Transaction)
                {
                    DbTransaction t = (tran == null ? BeginTransaction() : tran);
                    try
                    {
                        ExecuteNonQuery(basicInsertCmd, t);
                        retVal = ExecuteScalar(t, CommandType.Text, selectLastInsertIDSql);

                        if (tran == null)
                        {
                            t.Commit();
                        }
                    }
                    catch (DbException)
                    {
                        if (tran == null)
                        {
                            t.Rollback();
                        }
                        throw;
                    }
                    finally
                    {
                        if (tran == null)
                        {
                            CloseConnection(t);
                            t.Dispose();
                        }
                    }

                    if (retVal != DBNull.Value)
                    {
                        return Convert.ToInt32(retVal);
                    }
                }
                else
                {
                    if (tran == null)
                    {
                        ExecuteNonQuery(basicInsertCmd);
                    }
                    else
                    {
                        ExecuteNonQuery(basicInsertCmd, tran);
                    }
                }
            }
            else
            {
                if (tran == null)
                {
                    ExecuteNonQuery(basicInsertCmd);
                }
                else
                {
                    ExecuteNonQuery(basicInsertCmd, tran);
                }
            }

            return Convert.ToInt32(retVal);
        }

        /// <summary>
        /// Executes the insert return auto increment ID.
        /// </summary>
        /// <param name="basicInsertCmd">The basic insert CMD.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="autoIncrementColumn">The auto increment column.</param>
        /// <returns></returns>
        public int ExecuteInsertReturnAutoIncrementID(DbCommand basicInsertCmd,
            string tableName, string autoIncrementColumn)
        {
            return ExecuteInsertReturnAutoIncrementID(basicInsertCmd, null, tableName, autoIncrementColumn);
        }

        #endregion

        #region Extended Query Methods

        /// <summary>
        /// Query from specified custom sql.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public CustomSqlSection CustomSql(string sql)
        {
            //Check.Require(!string.IsNullOrEmpty(sql), "sql could not be null or empty!");

            return new CustomSqlSection(this, sql);
        }

        /// <summary>
        /// Query from specified stored procedure.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public StoredProcedureSection StoredProcedure(string spName)
        {
            //Check.Require(!string.IsNullOrEmpty(spName), "spName could not be null or empty!");

            return new StoredProcedureSection(this, spName);
        }

        /// <summary>
        /// Insert to specified table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public InsertSqlSection Insert(QueryTable table)
        {
            //Check.Require(table != null, "table could not be null.");

            return new InsertSqlSection(this, table);
        }

        /// <summary>
        /// Update specified table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public UpdateSqlSection Update(QueryTable table)
        {
            //Check.Require(table != null, "table could not be null.");

            return new UpdateSqlSection(this, table);
        }

        /// <summary>
        /// Delete from specified table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DeleteSqlSection Delete(QueryTable table)
        {
            //Check.Require(table != null, "table could not be null.");

            return new DeleteSqlSection(this, table);
        }

        /// <summary>
        /// Select from specified table. Supports select with order by, where, group by, inner join, top, skip.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public SelectSqlSection Select(QueryTable table, params ExpressionClip[] columns)
        {
            //Check.Require(table != null, "table could not be null.");

            return new SelectSqlSection(this, table, columns);
        }

        public int Save(DbCommand selectCommand, int batchSize, params DataRow[] rows)
        {
            if (rows == null || rows.Length == 0)
                return 0;

            //Check.Require(selectCommand != null, "selectCommand could not be null.");
            //Check.Require(batchSize >= 0, "batchSize MUST >= 1.");

            using (DbDataAdapter adapter = GetDataAdapter())
            {
                if (IsBatchConnection)
                {
                    DbConnection conn = GetConnection(true);
                    PrepareCommand(selectCommand, conn);
                    adapter.SelectCommand = selectCommand;
                    adapter.UpdateBatchSize = batchSize;
                    DbCommandBuilder builder = dbProvider.DbProviderFactory.CreateCommandBuilder();
                    builder.DataAdapter = adapter;
                    return adapter.Update(rows);
                }
                else
                {
                    using (DbConnection conn = GetConnection(true))
                    {
                        PrepareCommand(selectCommand, conn);
                        adapter.SelectCommand = selectCommand;
                        adapter.UpdateBatchSize = batchSize;
                        DbCommandBuilder builder = dbProvider.DbProviderFactory.CreateCommandBuilder();
                        builder.DataAdapter = adapter;
                        return adapter.Update(rows);
                    }
                }
            }
        }

        public int Save(DbCommand selectCommand, params DataRow[] rows)
        {
            return Save(selectCommand, 1, rows);
        }

        public int Save(DbCommand selectCommand, DbTransaction tran, int batchSize, params DataRow[] rows)
        {
            if (rows == null || rows.Length == 0)
                return 0;

            //Check.Require(selectCommand != null, "selectCommand could not be null.");
            //Check.Require(tran != null, "tran could not be null.");
            //Check.Require(batchSize >= 0, "batchSize MUST >= 1.");

            using (DbDataAdapter adapter = GetDataAdapter())
            {
                PrepareCommand(selectCommand, tran);
                adapter.SelectCommand = selectCommand;
                adapter.UpdateBatchSize = batchSize;
                DbCommandBuilder builder = dbProvider.DbProviderFactory.CreateCommandBuilder();
                builder.DataAdapter = adapter;
                return adapter.Update(rows);
            }
        }

        public int Save(DbCommand selectCommand, DbTransaction tran, params DataRow[] rows)
        {
            return Save(selectCommand, tran, 1, rows);
        }

        public int Save(DbCommand selectCommand, int batchSize, DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            //Check.Require(selectCommand != null, "selectCommand could not be null.");
            //Check.Require(batchSize >= 0, "batchSize MUST >= 1.");

            using (DbDataAdapter adapter = GetDataAdapter())
            {
                using (DbConnection conn = GetConnection(true))
                {
                    PrepareCommand(selectCommand, conn);
                    adapter.SelectCommand = selectCommand;
                    adapter.UpdateBatchSize = batchSize;
                    DbCommandBuilder builder = dbProvider.DbProviderFactory.CreateCommandBuilder();
                    builder.DataAdapter = adapter;
                    return adapter.Update(table);
                }
            }
        }

        public int Save(DbCommand selectCommand, DataTable table)
        {
            return Save(selectCommand, 1, table);
        }

        public int Save(DbCommand selectCommand, DbTransaction tran, int batchSize, DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return 0;

            //Check.Require(selectCommand != null, "selectCommand could not be null.");
            //Check.Require(tran != null, "tran could not be null.");
            //Check.Require(batchSize >= 0, "batchSize MUST >= 1.");

            using (DbDataAdapter adapter = GetDataAdapter())
            {
                PrepareCommand(selectCommand, tran);
                adapter.SelectCommand = selectCommand;
                adapter.UpdateBatchSize = batchSize;
                DbCommandBuilder builder = dbProvider.DbProviderFactory.CreateCommandBuilder();
                builder.DataAdapter = adapter;
                return adapter.Update(table);
            }
        }

        public int Save(DbCommand selectCommand, DbTransaction tran, DataTable table)
        {
            return Save(selectCommand, tran, 1, table);
        }

        #endregion

        #region ASP.NET 1.1 style Transactions

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public DbTransaction BeginTransaction()
        {
            return GetConnection(true).BeginTransaction();
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="il">The il.</param>
        /// <returns></returns>
        public DbTransaction BeginTransaction(System.Data.IsolationLevel il)
        {
            return GetConnection(true).BeginTransaction(il);
        }

        #endregion

        #region DbCommand Parameter Methods

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        public void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter parameter = CreateParameter(name, dbType == DbType.Object ? DbType.String : dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        public void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        public void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
        {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public void AddInParameter(DbCommand command, string name, DbType dbType)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddInParameter(DbCommand command, string name, object value)
        {
            AddParameter(command, name, DbType.Object, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        #endregion

        #endregion

        #region Batch Connection

        internal DbConnection batchConnection = null;
        private BatchCommander batchCommander = null;

        /// <summary>
        /// Gets the size of a batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public int BatchSize
        {
            get
            {
                return batchCommander == null ? 0 : batchCommander.batchSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is batch connection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is batch connection; otherwise, <c>false</c>.
        /// </value>
        public bool IsBatchConnection
        {
            get
            {
                return (batchConnection != null);
            }
        }

        /// <summary>
        /// Begins the batch connection.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        public void BeginBatchConnection(int batchSize)
        {
            batchConnection = CreateConnection(true);
            //this.batchSize = batchSize;
            batchCommander = new BatchCommander(this, batchSize);
        }

        /// <summary>
        /// Begins the batch connection.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="tran">The tran.</param>
        public void BeginBatchConnection(int batchSize, DbTransaction tran)
        {
            batchConnection = CreateConnection(true);
            //this.batchSize = batchSize;
            batchCommander = new BatchCommander(this, batchSize, tran);
        }

        /// <summary>
        /// Begins the batch connection.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <param name="il">The il.</param>
        public void BeginBatchConnection(int batchSize, System.Data.IsolationLevel il)
        {
            batchConnection = CreateConnection(true);
            //this.batchSize = batchSize;
            batchCommander = new BatchCommander(this, batchSize, il);
        }

        /// <summary>
        /// Ends the batch connection.
        /// </summary>
        public void EndBatchConnection()
        {
            batchCommander.Close();
            CloseConnection(batchConnection);
            batchConnection = null;
            batchCommander = null;
        }

        /// <summary>
        /// Executes the pending batch operations.
        /// </summary>
        public void ExecutePendingBatchOperations()
        {
            batchCommander.ExecuteBatch();
        }

        /// <summary>
        /// Begins a un disconnect connection.
        /// </summary>
        public void BeginUnDisconnectConnection()
        {
            BeginBatchConnection(1);
        }

        /// <summary>
        /// Ends the un disconnect connection.
        /// </summary>
        public void EndUnDisconnectConnection()
        {
            EndBatchConnection();
        }

        #endregion
    }
}
