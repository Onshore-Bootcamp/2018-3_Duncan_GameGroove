using GameGrooveDAL.Mapping;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace GameGrooveDAL
{
    public class RoleDAO
    {
        /*Set up variables needed for the class. My DAOs need a logger in order to fill out an error log, a mapper for objects read by SqlDataReader,
         and a variable to apply the connection string stored in WebConfig.*/
        private static Logger _Logger;
        private readonly string _ConnectionString;

        /// <summary>
        /// RoleDAO holds a method that will pull information from the Roles table in the GAMEGROOVE database. 
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt found in WebConfig</param>
        /// <param name="connectionString">Connection string for GAMEGROOVE database found in WebConfig</param>
        public RoleDAO(string logPath, string connectionString)
        {
            _ConnectionString = connectionString;
            _Logger = new Logger(logPath);
        }

        //initialize mapper
        private readonly RoleMapper _RoleMapper = new RoleMapper();

        /// <summary>
        /// Pull the information for one record in the Role table in the GAMEGROOVE database. Runs the VIEW_ROLE_BY_ID stored procedure.
        /// </summary>
        /// <param name="roleID">ID of the role needing to be retrieved</param>
        /// <returns>Returns a RoleDO filled with information retrieved from the database</returns>
        public RoleDO ViewRoleByID(int roleID)
        {
            RoleDO role = new RoleDO();

            //catch errors while accessing the database
            try
            {
                //connect to sql server database, run VIEW_ROLE_BY_ID
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_ROLE_BY_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for stored procedure
                    command.Parameters.AddWithValue("@RoleID", roleID);

                    connection.Open();

                    //use SqlDataReader to pull one record from the database, [if] statement only selects one record from database
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            role = _RoleMapper.MapReaderToSingle(reader);
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

            return role;
        }
    }
}
