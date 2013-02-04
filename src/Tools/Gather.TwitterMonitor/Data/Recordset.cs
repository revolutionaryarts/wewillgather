using System;
using System.Data.SqlClient;

namespace Gather.ApplicationMonitor.Data
{
    public class Resultset : IDisposable
    {

        SqlDataReader _sdr;

        internal Resultset(SqlDataReader sdr)
        {
            _sdr = sdr;
        }

        public bool Read()
        {
            return _sdr.Read();
        }

        public bool NextResult()
        {
            return _sdr.NextResult();
        }

        public string GetString(string sFieldName)
        {
            if (_sdr[sFieldName] == DBNull.Value)
                return "";
            return (string)_sdr[sFieldName];
        }

        public decimal GetDecimal(string sFieldName)
        {
            decimal decimalOut;
            if (_sdr[sFieldName] == DBNull.Value)
                return 0;

            if (decimal.TryParse(_sdr[sFieldName].ToString(), out decimalOut))
                return decimalOut;

            return 0;
        }

        public int GetInt(string sFieldName)
        {
            if (_sdr[sFieldName] == DBNull.Value)
                return -1;
            return (int)_sdr[sFieldName];
        }

        public bool GetBool(string sFieldName)
        {
            if (_sdr[sFieldName] == DBNull.Value)
                return false;
            return (bool)_sdr[sFieldName];
        }

        public DateTime GetDateTime(string sFieldName)
        {
            if (_sdr[sFieldName] == DBNull.Value)
                return DateTime.MinValue;
            return (DateTime)_sdr[sFieldName];
        }

        void IDisposable.Dispose()
        {
            if (_sdr != null && !_sdr.IsClosed)
            {
                _sdr.Close();
                _sdr.Dispose();
            }
            _sdr = null;
        }

    }
}

