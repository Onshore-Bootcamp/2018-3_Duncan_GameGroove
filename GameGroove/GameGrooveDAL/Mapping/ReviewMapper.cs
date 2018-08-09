using GameGrooveDAL.Models;
using System;
using System.Data.SqlClient;

namespace GameGrooveDAL.Mapping
{
    public class ReviewMapper
    {
        /// <summary>
        /// Filters data while reading from the database
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns a ReviewDO with no null values</returns>
        public ReviewDO MapReaderToSingle(SqlDataReader reader)
        {
            ReviewDO reviewDO = new ReviewDO();

            if (reader["ReviewID"] != DBNull.Value)
            {
                reviewDO.ReviewID = (int)reader["ReviewID"];
            }
            if (reader["ReviewText"] != DBNull.Value)
            {
                reviewDO.ReviewText = (string)reader["ReviewText"];
            }
            if (reader["DatePosted"] != DBNull.Value)
            {
                reviewDO.DatePosted = (string)reader["DatePosted"];
            }
            if (reader["Category"] != DBNull.Value)
            {
                reviewDO.Category = (string)reader["Category"];
            }
            if (reader["UserID"] != DBNull.Value)
            {
                reviewDO.UserID = (int)reader["UserID"];
            }
            if (reader["GameID"] != DBNull.Value)
            {
                reviewDO.GameID = (int)reader["GameID"];
            }
            return reviewDO;
        }
    }
}
