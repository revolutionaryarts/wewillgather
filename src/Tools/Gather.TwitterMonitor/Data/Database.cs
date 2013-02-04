using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Gather.ApplicationMonitor.Data
{
    public class Database : IDisposable
    {

        public Database()
        {
            Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            Connection.Open();
        }

        internal SqlConnection Connection { get; private set; }

        void IDisposable.Dispose()
        {
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                Connection.Close();
                Connection.Dispose();
            }
            Connection = null;
        }

    }
}

