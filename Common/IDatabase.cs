using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Temnenkov.SJB.Common
{
    public interface IDatabase
    {
        int ExecuteCommand(string sql, params object[] parameters);
        IDataReader ExecuteReader(string sql, params object[] parameters);
        bool TableExists(string tanleName);
    }

}
