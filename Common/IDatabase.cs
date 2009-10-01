using System.Data;

namespace Temnenkov.SJB.Common
{
    public interface IDatabase
    {
        int ExecuteCommand(string sql, params object[] parameters);
        IDataReader ExecuteReader(string sql, params object[] parameters);
        void CloseReader();
        bool TableExists(string tanleName);
    }

}
