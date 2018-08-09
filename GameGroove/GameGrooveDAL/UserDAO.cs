using GameGrooveDAL.Mapping;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace GameGrooveDAL
{
    public class UserDAO
    {
        #region Build
        /*Set up variables needed for the class. My DAOs need a logger in order to fill out an error log, a mapper for objects read by SqlDataReader,
         and a variable to apply the connection string stored in WebConfig.*/
        private Logger _Logger;
        private readonly UserMapper _Mapper = new UserMapper();
        private readonly string _ConnectionString;

        /// <summary>
        /// UserDAO holds methods that connect to the Users table in the GAMEGROOVE database.
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt found in WebConfig</param>
        /// <param name="connectionstring">Connection string for GAMEGROOVE SQL server found in WebConfig</param>
        public UserDAO(string logPath, string connectionstring)
        {
            _ConnectionString = connectionstring;
            _Logger = new Logger(logPath);
        }

        #endregion

        #region Create
        /// <summary>
        /// Method that adds a record to the Users table in the GAMEGROOVE SQL server. Runs the stored procedure CREATE_USER.
        /// </summary>
        /// <param name="userToRegister">UserDO filled out with information for a new account. MUST have a unique Username and Email.</param>
        /// <returns>Returns a boolean based on rows affected in the database. TRUE = Success, FALSE = Failure</returns>
        public bool RegisterUser(UserDO userToRegister)
        {
            //set a boolean to determine if the registration was successful
            bool registerSuccess = false;

            //catch errors when accesssing the database
            try
            {
                //connect to sql, then run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("CREATE_USER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //parameters for CREATE_USER
                    command.Parameters.AddWithValue("@FirstName", userToRegister.FirstName);
                    command.Parameters.AddWithValue("@LastName", userToRegister.LastName);
                    command.Parameters.AddWithValue("@Username", userToRegister.Username);
                    command.Parameters.AddWithValue("@Password", userToRegister.Password);
                    command.Parameters.AddWithValue("@Email", userToRegister.Email);
                    command.Parameters.AddWithValue("@RoleID", userToRegister.RoleID);

                    connection.Open();

                    //check for success, set boolean to be returned
                    int rowsAffected = command.ExecuteNonQuery();
                    registerSuccess = rowsAffected == 1;
                }
            }
            //catch SQL Exceptions for accurate error logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than errors thrown while accessing SQL
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }
            
            return registerSuccess;
        }
        #endregion

        #region Read
        /// <summary>
        /// Method to pull account details based on the username supplied. Runs stored procedure VIEW_USER_BY_USERNAME. Used for logging in.
        /// </summary>
        /// <param name="username">Username supplied by user during login</param>
        /// <returns>Returns a UserDO filled with information retrieved from the database</returns>
        public UserDO ViewUserByUsername(string username)
        {
            UserDO user = new UserDO();

            //catch errors when accessing the database
            try
            {
                //connect to SQL, run VIEW_USER_BY_USERNAME
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_USER_BY_USERNAME", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set the username for the stored procedure's parameter
                    command.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = _Mapper.MapReaderToSingle(reader);
                        }
                    }
                }
            }
            //catch SQL Exceptions for accurate error logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than errors thrown while accessing SQL
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return user;
        }

        /// <summary>
        /// Get one User's info from database
        /// </summary>
        /// <param name="userID">ID of User</param>
        /// <returns>Returns a UserDO filled out with information retrieved from the database</returns>
        public UserDO ViewUserByID(int userID)
        {
            UserDO user = new UserDO();

            //catch errors while accessing the database
            try
            {
                //connect to SQL, run VIEW_USER_BY_ID
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_USER_BY_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for the stored procedure
                    command.Parameters.AddWithValue("@UserID", userID);

                    connection.Open();

                    //read from the database, [if] statement for reader only reads the first record matching the parameter
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = _Mapper.MapReaderToSingle(reader);
                        }
                    }
                }
            }
            //catch SQL Exceptions for accurate error logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than errors thrown while accessing SQL
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return user;
        }

        /// <summary>
        /// Lists every record in the User table from the GAMEGROOVE database. Runs the VIEW_ALL_USERS stored procedure.
        /// </summary>
        /// <returns>Returns all users as items in a list</returns>
        public List<UserDO> ViewUsers()
        {
            List<UserDO> displayUsers = new List<UserDO>();

            //catch errors
            try
            {
                //connect to SQL, run VIEW_ALL_USERS
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_ALL_USERS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //pull user, map to object, add to list
                        while (reader.Read())
                        {
                            UserDO user = _Mapper.MapReaderToSingle(reader);
                            displayUsers.Add(user);
                        }
                    }
                }
            }
            //catch SQL Exceptions for accurate error logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than errors thrown while accessing SQL
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return displayUsers;
        }
        #endregion
        
        #region Update
        /// <summary>
        /// Changes a User's information in the database. Runs the UPDATE_USER stored procedure.
        /// </summary>
        /// <param name="user">UserDO filled with information supplied by the user</param>
        public void UpdateUser(UserDO user)
        {
            //catch errors while accessing database
            try
            {
                //connect to SQL, run UPDATE_USER
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("UPDATE_USER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameters for the stored procedure
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch SQL Exceptions for accurate error logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than errors thrown while accessing SQL
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
        /// Deletes a User's record from the GAMEGROOVE database. Runs the DELETE_USER stored procedure.
        /// </summary>
        /// <param name="userID">ID of the user that needs to be deleted</param>
        public void DeleteUser(int userID)
        {
            //catch errors while accessing the database
            try
            {
                //connect to sql, run DELETE_USER
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("DELETE_USER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set the parameter for the stored procedure
                    command.Parameters.AddWithValue("UserID", userID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch SQL Exceptions for accurate error logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than errors thrown while accessing SQL
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
