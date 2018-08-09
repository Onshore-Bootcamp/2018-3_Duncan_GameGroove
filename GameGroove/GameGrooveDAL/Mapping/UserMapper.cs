using GameGrooveDAL.Models;
using System;
using System.Data.SqlClient;

namespace GameGrooveDAL.Mapping
{
    public class UserMapper
    {
        /// <summary>
        /// Filters data while reading from the database
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns a UserDO with no null values</returns>
        public UserDO MapReaderToSingle(SqlDataReader reader)
        {
            UserDO userDO = new UserDO();

            if (reader["UserID"] != DBNull.Value)
            {
                userDO.UserID = (int)reader["UserID"];
            }
            if (reader["FirstName"] != DBNull.Value)
            {
                userDO.FirstName = (string)reader["FirstName"];
            }
            if (reader["LastName"] != DBNull.Value)
            {
                userDO.LastName = (string)reader["LastName"];
            }
            if (reader["Username"] != DBNull.Value)
            {
                userDO.Username = (string)reader["Username"];
            }
            if (reader["Password"] != DBNull.Value)
            {
                userDO.Password = (string)reader["Password"];
            }
            if (reader["Email"] != DBNull.Value)
            {
                userDO.Email = (string)reader["Email"];
            }
            if (reader["RoleID"] != DBNull.Value)
            {
                userDO.RoleID = (int)reader["RoleID"];
            }
            return userDO;
        }
    }
}
