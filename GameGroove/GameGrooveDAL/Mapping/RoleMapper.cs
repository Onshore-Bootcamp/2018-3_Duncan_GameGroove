using GameGrooveDAL.Models;
using System;
using System.Data.SqlClient;

namespace GameGrooveDAL.Mapping
{
    class RoleMapper
    {
        /// <summary>
        /// Filters data while reading from the database
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns a RoleDO with no null values</returns>
        public RoleDO MapReaderToSingle(SqlDataReader reader)
        {
            RoleDO roleDO = new RoleDO();

            if (reader["RoleID"] != DBNull.Value)
            {
                roleDO.RoleID = (int)reader["RoleID"];
            }
            if (reader["RoleName"] != DBNull.Value)
            {
                roleDO.RoleName = (string)reader["RoleName"];
            }

            return roleDO;
        }
    
        

    }       
}
