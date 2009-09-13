using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.Database
{
    public class Database: IDisposable
    {

        private SQLiteConnection _connection;

        public Database(string fileName)
            : this(Path.GetDirectoryName(Utils.GetExecutablePath()), fileName)
        { }

        public Database(string dirName, string fileName)
        {
            if (!fileName.ToLower().EndsWith(".sqlite"))
                fileName += ".sqlite";
            string fullName = Path.Combine(dirName, fileName);
            _connection = new SQLiteConnection(String.Format("Data Source={0}", fullName));
            _connection.Open();
        }

        public void Dispose()
        {
            _connection.Close();
        }

        public int ExecuteCommand(string sql, params object[] parameters)
        {
            var command = new SQLiteCommand(sql, _connection);
            foreach (var p in parameters)
                command.Parameters.Add(GetParameter(p));
            return command.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(string sql, params object[] parameters)
        {
            var command = new SQLiteCommand(sql, _connection);
            foreach (var p in parameters)
                command.Parameters.Add(GetParameter(p));
            return command.ExecuteReader();
        }

        private SQLiteParameter GetParameter(object parameter)
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
            return null;
        }


    }
}
