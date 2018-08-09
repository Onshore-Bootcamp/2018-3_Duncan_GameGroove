using GameGrooveDAL.Mapping;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace GameGrooveDAL
{
    public class GameDAO
    {
        #region Build
        //give GameDAO parameters, config variables
        private static Logger _Logger;
        private readonly string _ConnectionString;

        /// <summary>
        /// My DAOs require a log path and connection string, both found in WebConfig
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt found in WebConfig</param>
        /// <param name="connectionString">Connection string for the GAMEGROOVE database</param>
        public GameDAO(string logPath, string connectionString)
        {
            _ConnectionString = connectionString;
            _Logger = new Logger(logPath);
        }

        //initialize mapper
        private readonly GameMapper _Mapper = new GameMapper(); 
        #endregion

        #region Create
        /// <summary>
        /// Writes a record into the Games table in the GAMEGROOVE database. Runs the CREATE_GAME stored procedure
        /// </summary>
        /// <param name="game">GameDO filled with information supplied by the user</param>
        public void CreateGame(GameDO game)
        {
            //catch errors while accessing the database
            try
            {
                //connect to SQL, use stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("CREATE_GAME", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameters for the stored procedure
                    command.Parameters.AddWithValue("@Title", game.Title);
                    command.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate);
                    command.Parameters.AddWithValue("@Developer", game.Developer);
                    command.Parameters.AddWithValue("@Platform", game.Platform);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch sqlexceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch further exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }
        }
        #endregion

        #region Read
        /// <summary>
        /// Reads all records from the Games table in the GAMEGROOVE database. Runs the VIEW_ALL_GAMES stored procedure.
        /// </summary>
        /// <returns>Returns a list of GameDOs filled with information retrieved from the database</returns>
        public List<GameDO> ViewGames()
        {
            List<GameDO> games = new List<GameDO>();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_ALL_GAMES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    connection.Open();

                    //read from the database, uses [while] statement to read every record in the table
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            GameDO game = _Mapper.MapReaderToSingle(reader);
                            games.Add(game);
                        }
                    }
                }
            }
            //catch sqlexceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch further exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return games;
        }

        /// <summary>
        /// Reads one record from the Games table in the GAMEGROOVE database
        /// </summary>
        /// <param name="gameID">ID of the game needing to be viewed</param>
        /// <returns></returns>
        public GameDO ViewGameByID(int gameID)
        {
            GameDO game = new GameDO();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_GAME_BY_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for stored procedure
                    command.Parameters.AddWithValue("@GameID", gameID);

                    connection.Open();

                    //read from the database, uses [if] statement to read only one record
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            game = _Mapper.MapReaderToSingle(reader);
                        }
                    }
                }
            }
            //catch sqlexceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch further exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return game;
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates one record in the Games table in the GAMEGROOVE database. Runs the UPDATE_GAME stored procedure.
        /// </summary>
        /// <param name="game">GameDO filled with information provided by the user</param>
        public void UpdateGame(GameDO game)
        {
            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("UPDATE_GAME", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameters for the stored procedure
                    command.Parameters.AddWithValue("@GameID", game.GameID);
                    command.Parameters.AddWithValue("@Title", game.Title);
                    command.Parameters.AddWithValue("@ReleaseDate", game.ReleaseDate);
                    command.Parameters.AddWithValue("@Developer", game.Developer);
                    command.Parameters.AddWithValue("@Platform", game.Platform);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch sqlexceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch further exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes one record from the Games table in the GAMEGROOVE database. Runs the DELETE_GAME stored procedure
        /// </summary>
        /// <param name="gameID">ID of the game needing to be deleted</param>
        public void DeleteGame(int gameID)
        {
            //catch errors while accessing the database
            try
            {
                //connect to SQL, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("DELETE_GAME", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set the parameter for the stored procedure
                    command.Parameters.AddWithValue("@GameID", gameID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch sqlexceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch further errors
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }
        }
        #endregion
    }
}
