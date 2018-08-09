using GameGrooveDAL.Models;
using System;
using System.Data.SqlClient;

namespace GameGrooveDAL.Mapping
{
    class CalcMapper
    {
        /// <summary>
        /// Checks for data while reading from the database. Maps the Category column/property. Displays on the homepage.
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns ReviewDO with the top category</returns>
        public ReviewDO MapCategoryToSingle(SqlDataReader reader)
        {
            ReviewDO reviewDO = new ReviewDO();

            if (reader["Category"] != DBNull.Value)
            {
                reviewDO.Category = (string)reader["Category"];
            }

            reviewDO.ReviewID = 0;
            reviewDO.ReviewText = null;
            reviewDO.DatePosted = null;
            reviewDO.UserID = 0;
            reviewDO.GameID = 0;

            return reviewDO;
        }

        /// <summary>
        /// Checks for data while reading from the database. Maps the GameID column/property. Displays on the homepage.
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns ReviewDO with the top game</returns>
        public ReviewDO MapGameToSingle(SqlDataReader reader)
        {
            ReviewDO reviewDO = new ReviewDO();

            if (reader["GameID"] != DBNull.Value)
            {
                reviewDO.GameID = (int)reader["GameID"];
            }

            reviewDO.ReviewID = 0;
            reviewDO.ReviewText = null;
            reviewDO.DatePosted = null;
            reviewDO.UserID = 0;
            reviewDO.Category = null;

            return reviewDO;
        }
    }
}
