using GameGrooveDAL.Mapping;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace GameGrooveDAL
{
    public class RequestDAO
    {
        #region Build
        //instantiate mapper
        private readonly RequestMapper _RequestMapper = new RequestMapper();

        //instantiate constructor variables
        private readonly Logger _Logger;
        private readonly string _ConnectionString;

        /// <summary>
        /// My DAOs require a connection string and a logpath when instantiated. Both are found in the WebConfig file. 
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt</param>
        /// <param name="connectionString">Connection string for the GAMEGROOVE database</param>
        public RequestDAO(string logPath, string connectionString)
        {
            _ConnectionString = connectionString;
            _Logger = new Logger(logPath);
        }
        #endregion

        #region Create
        /// <summary>
        /// Writes a record in the Requests table in the GAMEGROOVE database. Runs the CREATE_REQUEST stored procedure.
        /// </summary>
        /// <param name="request">RequestDO filled out with information supplied by the user</param>
        public void CreateRequest(RequestDO request)
        {
            //catch errors while accessing the database
            try
            {
                //connect to SQL, use stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("CREATE_REQUEST", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameters for the stored procedure
                    command.Parameters.AddWithValue("@RequestText", request.RequestText);
                    command.Parameters.AddWithValue("@Username", request.Username);
                    command.Parameters.AddWithValue("@Date", request.Date);

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
        
        #region Read
        /// <summary>
        /// Reads all records in the Requests table in the GAMEGROOVE database. Runs the VIEW_ALL_REQUESTS stored procedure.
        /// </summary>
        /// <returns>Returns a list of RequestDOs filled with information retrieved from the database.</returns>
        public List<RequestDO> ViewRequests()
        {
            List<RequestDO> requests = new List<RequestDO>();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_ALL_REQUESTS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    connection.Open();

                    //read from the database
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RequestDO request = _RequestMapper.MapReaderToSingle(reader);
                            requests.Add(request);
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
            // catch further exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return requests;
        }

        /// <summary>
        /// Reads one record from the Requests table in the GAMEGROOVE database. Runs the VIEW_REQUEST_BY_ID stored procedure.
        /// </summary>
        /// <param name="requestID">ID of the request wanting to be viewed.</param>
        /// <returns>Returns a RequestDO filled with information retrieved from the database.</returns>
        public RequestDO ViewRequestByID(int requestID)
        {
            RequestDO request = new RequestDO();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_REQUEST_BY_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for the stored procedure
                    command.Parameters.AddWithValue("@RequestID", requestID);

                    connection.Open();

                    //read from the database, uses [if] statement to only read one record
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            request = _RequestMapper.MapReaderToSingle(reader);
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
            //catch further errors
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return request;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes a record from the Requests table in the GAMEGROOVE database. Runs the DELETE_REQUEST stored procedure.
        /// </summary>
        /// <param name="requestID">ID of the Request needing to be deleted</param>
        public void DeleteRequest(int requestID)
        {
            //catch errors while accessing the database
            try
            {
                //connect to SQL, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("DELETE_REQUEST", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for the stored procedure
                    command.Parameters.AddWithValue("@RequestID", requestID);

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
