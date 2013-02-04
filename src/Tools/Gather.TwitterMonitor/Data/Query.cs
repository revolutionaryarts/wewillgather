using System.Data;
using System.Data.SqlClient;

namespace Gather.ApplicationMonitor.Data
{
    public class Query
    {

        private readonly SqlCommand _connection;

        public Query(string sql, Database db)
        {
            _connection = new SqlCommand(sql, db.Connection)
            {
                CommandType = CommandType.Text
            };
        }

        public Resultset Get()
        {
            var sdr = _connection.ExecuteReader();
            return new Resultset(sdr);
        }

        public int ExecuteNonQuery()
        {
            return _connection.ExecuteNonQuery();
        }

    }
}

