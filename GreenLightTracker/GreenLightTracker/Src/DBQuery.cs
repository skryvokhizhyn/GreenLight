using System.Data;

using Mono.Data.Sqlite;

namespace GreenLightTracker.Src
{
    class DBQuery
    {
        SqliteConnection m_connection;

        public DBQuery(IDbConnection connection)
        {
            m_connection = (SqliteConnection)connection;
        }

        public void ExecuteCommand(string commandText)
        {
            using (var command = m_connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        public T ExecuteScalar<T>(string commandText)
        {
            object val;
            using (var command = m_connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.Text;
                val = command.ExecuteScalar();
            }

            return (T)val;
        }

        public IDataReader ExecuteQuery(string commandText)
        {
            using (var command = m_connection.CreateCommand())
            {
                command.CommandText = commandText;
                command.CommandType = CommandType.Text;
                return command.ExecuteReader();
            }
        }
    }
}