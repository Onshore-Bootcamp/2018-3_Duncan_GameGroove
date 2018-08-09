using GameGrooveDAL.Models;
using System;
using System.Data.SqlClient;

namespace GameGrooveDAL.Mapping
{
    public class GameMapper
    {
        /// <summary>
        /// Filters data while reading from the database.
        /// </summary>
        /// <param name="reader">SqlDataReader to read from the database</param>
        /// <returns>Returns a GameDO with filtered information</returns>
        public GameDO MapReaderToSingle(SqlDataReader reader)
        {
            GameDO gameDO = new GameDO();

            if (reader["GameID"] !=DBNull.Value)
            {
                gameDO.GameID = (int)reader["GameID"];
            }
            if (reader["Title"] != DBNull.Value)
            {
                gameDO.Title = (string)reader["Title"];
            }
            if (reader["ReleaseDate"] != DBNull.Value)
            {
                gameDO.ReleaseDate = (string)reader["ReleaseDate"];
            }
            if (reader["Developer"] != DBNull.Value)
            {
                gameDO.Developer = (string)reader["Developer"];
            }
            if (reader["Platform"] != DBNull.Value)
            {
                gameDO.Platform = (string)reader["Platform"];
            }
            return gameDO;
        }
    }
}
