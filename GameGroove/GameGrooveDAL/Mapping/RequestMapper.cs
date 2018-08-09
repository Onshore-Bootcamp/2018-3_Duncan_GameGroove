using GameGrooveDAL.Models;
using System;
using System.Data.SqlClient;

namespace GameGrooveDAL.Mapping
{
    public class RequestMapper
    {
        /// <summary>
        /// Filters data while reading from the database
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns a RequestDO with no null values</returns>
        public RequestDO MapReaderToSingle(SqlDataReader reader)
        {
            RequestDO requestDO = new RequestDO();

            if (reader["RequestID"] != DBNull.Value)
            {
                requestDO.RequestID = (int)reader["RequestID"];
            }
            if (reader["RequestText"] != DBNull.Value)
            {
                requestDO.RequestText = (string)reader["RequestText"];
            }
            if (reader["Username"] != DBNull.Value)
            {
                requestDO.Username = (string)reader["Username"];
            }
            if (reader["Date"] != DBNull.Value)
            {
                requestDO.Date = (string)reader["Date"];
            }

            return requestDO;
        }   
    }
}
