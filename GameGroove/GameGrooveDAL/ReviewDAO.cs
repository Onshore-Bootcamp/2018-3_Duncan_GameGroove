using GameGrooveDAL.Mapping;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace GameGrooveDAL
{
    public class ReviewDAO
    {
        #region Build
        /*Set up variables needed for the class. My DAOs need a logger in order to fill out an error log, a mapper for objects read by SqlDataReader,
         and a variable to apply the connection string stored in WebConfig.*/
        private readonly Logger _Logger;
        private readonly string _ConnectionString;

        /// <summary>
        /// ReviewDAO holds methods that access the GAMEGROOVE database specifically dealing with the Reviews table. 
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt found in WebConfig</param>
        /// <param name="connectionString">Connection string for the GAMEGROOVE database found in WebConfig</param>
        public ReviewDAO(string logPath, string connectionString)
        {
            _ConnectionString = connectionString;
            _Logger = new Logger(logPath);
        }

        //instantiate mappers
        private readonly ReviewMapper _ReviewMapper = new ReviewMapper();
        private readonly CalcMapper _CalcMapper = new CalcMapper(); 
        #endregion

        #region Create
        /// <summary>
        /// Create review is a method that writes a record to the Reviews table in the GAMEGROOVE database. Uses a form filled out by the user.
        /// Runs the CREATE_REVIEW stored procedure.
        /// </summary>
        /// <param name="review">ReviewDO filled out with information provided by a user</param>
        public void CreateReview(ReviewDO review)
        {
            //catch errors while accessing the database
            try
            {
                //connect to SQL, run the stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("CREATE_REVIEW", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set the parameters for the stored procedure
                    command.Parameters.AddWithValue("@ReviewText", review.ReviewText);
                    command.Parameters.AddWithValue("@DatePosted", review.DatePosted);
                    command.Parameters.AddWithValue("@Category", review.Category);
                    command.Parameters.AddWithValue("@UserID", review.UserID);
                    command.Parameters.AddWithValue("@GameID", review.GameID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch SQL exceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions other than sql
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
        /// Method to pull every record from the Reviews table in the GAMEGROOVE database and put them in a list. Runs the VIEW_ALL_REVIEWS stored procedure.
        /// </summary>
        /// <returns>Returns a list of ReviewDOs filled with data retrieved from the database.</returns>
        public List<ReviewDO> ViewReviews()
        {
            List<ReviewDO> reviews = new List<ReviewDO>();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_ALL_REVIEWS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    connection.Open();

                    //SqlDataReader to get information from the Reviews table. Uses [while] statement to read multiple records in the table.
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReviewDO review = _ReviewMapper.MapReaderToSingle(reader);
                            reviews.Add(review);
                        }
                    }
                }
            }
            //catch SqlExceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions for further error handling
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return reviews;
        }

        /// <summary>
        /// Pulls the information from one record in the Reviews table in the GAMEGROOVE database. Runs the VIEW_REVIEW_BY_ID stored procedure.
        /// </summary>
        /// <param name="reviewID">ID of the review needing to be read</param>
        /// <returns>Returns a ReviewDO filled with information retrieved from the database</returns>
        public ReviewDO ViewReviewByID(int reviewID)
        {
            ReviewDO review = new ReviewDO();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("VIEW_REVIEW_BY_ID", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for the stored procedure
                    command.Parameters.AddWithValue("@ReviewID", reviewID);

                    connection.Open();

                    //read from the database, uses [if] statement to pull one record
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            review = _ReviewMapper.MapReaderToSingle(reader);
                        }
                    }
                }
            }
            //catch SqlExceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch general exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return review;
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates a record in the Reviews table in the GAMEGROOVE database with information supplied by a user. 
        /// Runs the UPDATE_REVIEW stored procedure.
        /// </summary>
        /// <param name="review">ReviewDO with information retrieved from the user</param>
        public void UpdateReview(ReviewDO review)
        {
            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("UPDATE_REVIEW", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameters for the stored procedure
                    command.Parameters.AddWithValue("@ReviewID", review.ReviewID);
                    command.Parameters.AddWithValue("@ReviewText", review.ReviewText);
                    command.Parameters.AddWithValue("@DatePosted", review.DatePosted);
                    command.Parameters.AddWithValue("@Category", review.Category);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch SqlExceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch any further general exceptions
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
        /// Removes one record from the Reviews table in the GAMEGROOVE database. Runs the DELETE_REVIEW stored procedure.
        /// </summary>
        /// <param name="reviewID">ReviewID of the review needing to be deleted</param>
        public void DeleteReview(int reviewID)
        {
            //catch errors while accessing database
            try
            {
                //connect to SQL, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("DELETE_REVIEW", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for the stored procedure
                    command.Parameters.AddWithValue("@ReviewID", reviewID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            //catch SqlExceptions for accurate logging
            catch (SqlException ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            //catch further general exceptions
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }
        }
        #endregion

        #region Calculations
        /// <summary>
        /// Accesses the database and calculates the top category accross all reviews in the Reviews table in the GAMEGROOVE database.
        /// Runs the CALCULATE_TOP_CATEGORY stored procedure.
        /// </summary>
        /// <returns>Returns a ReviewDO filled with information retrieved from the database</returns>
        public ReviewDO TopCategory()
        {
            ReviewDO topCategory = new ReviewDO();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("CALCULATE_TOP_CATEGORY", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    connection.Open();

                    //read from the database, [if] statment pulls one record
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            topCategory = _CalcMapper.MapCategoryToSingle(reader);
                        }
                    }
                }
            }
            //catch SqlExceptions for accurate logging
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

            return topCategory;
        }
        
        /// <summary>
        /// Calculates the most frequent category based on user. Displays on a user's profile. Runs the CALCULATE_TOP_CATEGORY_FOR_USER stored procedure.
        /// </summary>
        /// <param name="userID">ID of user whose profile is being displayed</param>
        /// <returns>Returns a ReviewDO filled with information retrieved from the database</returns>
        public ReviewDO UserFavoriteCategory(int userID)
        {
            ReviewDO userFavCategory = new ReviewDO();

            //catch errors while accessing the database
            try
            {
                //connect to sql, run stored procedure
                using (SqlConnection connection = new SqlConnection(_ConnectionString))
                using (SqlCommand command = new SqlCommand("CALCULATE_TOP_CATEGORY_FOR_USER", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 60;

                    //set parameter for the stored procedure
                    command.Parameters.AddWithValue("@UserID", userID);

                    connection.Open();

                    //read from the database, [if] statement only reads one record
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userFavCategory = _CalcMapper.MapCategoryToSingle(reader);
                        }
                    }
                }
            }
            //catch SqlExceptions for accurate logging
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

            return userFavCategory;
        } 
        #endregion
    }
}
