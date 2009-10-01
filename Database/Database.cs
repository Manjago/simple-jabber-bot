using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.Database
{
    public class Database : IDatabase
    {
        private readonly SQLiteConnection _connection;

        public Database(string fileName)
            : this(Path.GetDirectoryName(Utils.GetExecutablePath()), fileName)
        { }

        public Database(string dirName, string fileName)
        {
            if (!fileName.ToLower().EndsWith(".sqlite"))
                fileName += ".sqlite";
            var fullName = Path.Combine(dirName, fileName);
            _connection = new SQLiteConnection(String.Format("Data Source={0}", fullName));
        }

        private static SQLiteParameter GetParameter(object parameter)
        {
            if (parameter is string)
                return new SQLiteParameter(DbType.String, parameter);
            if (parameter is int)
                return new SQLiteParameter(DbType.Int32, parameter);
            if (parameter is long)
                return new SQLiteParameter(DbType.Int64, parameter);
            if (parameter is DateTime)
                return new SQLiteParameter(DbType.DateTime, parameter);
            if (parameter is bool)
                return new SQLiteParameter(DbType.Boolean, parameter);
            if (parameter is char)
                return new SQLiteParameter(DbType.StringFixedLength, 1) { Value = parameter };
            return null;
        }

        private int ExecuteCommand(SQLiteConnection conn, SQLiteTransaction trans, string sql, params object[] parameters)
        {
            var command = new SQLiteCommand(sql, conn, trans);
            foreach (var p in parameters)
                command.Parameters.Add(GetParameter(p));
            return command.ExecuteNonQuery();
        }

        private SQLiteDataReader ExecuteReader(SQLiteConnection conn, SQLiteTransaction trans, string sql, params object[] parameters)
        {
            var command = new SQLiteCommand(sql, _connection);
            foreach (var p in parameters)
                command.Parameters.Add(GetParameter(p));
            return command.ExecuteReader();
        }

        #region IDatabase Members

        public int ExecuteCommand(string sql, params object[] parameters)
        {
            _connection.Open();
            try
            {
                return ExecuteCommand(_connection, null, sql, parameters);
            }
            finally
            {
                _connection.Close();
            }
        }

        public IDataReader ExecuteReader(string sql, params object[] parameters)
        {
            _connection.Open();
                return ExecuteReader(_connection, null, sql, parameters);
        }

        public void CloseReader()
        {
            _connection.Close();
        }

        public bool TableExists(string tableName)
        {
            _connection.Open();
            try
            {
                var test = _connection.GetSchema("Tables").Select(string.Format("Table_Name = '{0}'", tableName));
                return test.Length != 0;
            }
            finally
            {
                _connection.Close();
            }
        }

        #endregion


    }
}
