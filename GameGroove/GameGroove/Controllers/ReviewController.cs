using GameGroove.Mapping;
using GameGroove.Models;
using GameGrooveDAL;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace GameGroove.Controllers
{
    public class ReviewController : Controller
    {
        #region Build
        //initialize mappers
        private readonly ReviewMapper _ReviewMapper = new ReviewMapper();
        private readonly GameMapper _GameMapper = new GameMapper();
        private readonly UserMapper _UserMapper = new UserMapper();

        //variables for constructor
        private readonly ReviewDAO _ReviewDataAccess;
        private readonly UserDAO _UserDataAccess;
        private readonly GameDAO _GameDataAccess;
        private readonly Logger _Logger;


        /// <summary>
        /// Review Controller handles all of the views focused on reviews. This controller needs a ReviewDAO, UserDAO, and GameDAO that connect to the
        /// GAMEGROOVE database, as well as a logger to fill out an error log. Retrieve the parameters needed for each of the above from WebConfig.
        /// </summary>
        public ReviewController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GameGroove"].ConnectionString;
            string logPath = ConfigurationManager.AppSettings["LogPath"];

            _Logger = new Logger(logPath);
            _ReviewDataAccess = new ReviewDAO(logPath, connectionString);
            _UserDataAccess = new UserDAO(logPath, connectionString);
            _GameDataAccess = new GameDAO(logPath, connectionString);
        }
        #endregion

        #region Create
        /// <summary>
        /// A form that gathers information from the user and writes it to the database.
        /// </summary>
        /// <param name="gameID">ID of the game being reviewed</param>
        /// <returns>Returns a result based on role or success</returns>
        [HttpGet]
        public ActionResult CreateReview(int gameID)
        {
            ActionResult response;

            //check if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, view the form

                ReviewPO reviewPO = new ReviewPO();

                try
                {
                    //access database, map to PO
                    GameDO gameDO = _GameDataAccess.ViewGameByID(gameID);
                    reviewPO.GameID = gameDO.GameID;
                    reviewPO.GameTitle = gameDO.Title;

                    response = View(reviewPO);
                }
                //catch general exceptions
                catch (Exception ex)
                {
                    //log errors
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                    response = RedirectToAction("Error", "Home");
                }
                finally { }
            }
            else
            {
                //not logged in, redirect to login
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Writes gathered information to the database.
        /// </summary>
        /// <param name="reviewPO">ReviewPO filled with information from the form</param>
        /// <returns>Returns a result based on role or success</returns>
        [HttpPost]
        public ActionResult CreateReview(ReviewPO reviewPO)
        {
            ActionResult response;

            //check if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, check model, then write to database
                try
                {
                    //check model
                    if (ModelState.IsValid)
                    {
                        //if model is valid, map from model to a DO
                        ReviewDO review = new ReviewDO()
                        {
                            ReviewText = reviewPO.ReviewText,
                            DatePosted = DateTime.Now.ToString(),
                            Category = reviewPO.Category,
                            UserID = (int)Session["UserID"],
                            GameID = reviewPO.GameID,
                        };

                        //access database, write to the Reviews table
                        _ReviewDataAccess.CreateReview(review);

                        response = RedirectToAction("GameDetails", "Game", new { id = review.GameID });
                    }
                    else
                    {
                        //if model is not valid, return to the form

                        response = View(reviewPO);
                    }
                }
                catch (Exception ex)
                {
                    //log errors
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                    response = View(reviewPO);
                }
                finally { } 
            }
            else
            {
                //if not logged in, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Read
        /// <summary>
        /// Shows a list of all reviews in the database.
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        public ActionResult Index()
        {
            ActionResult response;
            ReviewVM viewModel = new ReviewVM();

            try
            {
                //access database
                List<ReviewDO> allReviews = _ReviewDataAccess.ViewReviews();

                viewModel.ReviewsList = new List<ReviewPO>();

                //add to view model
                foreach (ReviewDO reviewDO in allReviews)
                {
                    //set review information
                    ReviewPO reviewPO = _ReviewMapper.MapDOtoPO(reviewDO);

                    //get game title
                    GameDO gameDO = _GameDataAccess.ViewGameByID(reviewPO.GameID);
                    reviewPO.GameTitle = gameDO.Title;

                    //get username
                    UserDO userDO = _UserDataAccess.ViewUserByID(reviewPO.UserID);
                    reviewPO.Username = userDO.Username;

                    viewModel.ReviewsList.Add(reviewPO);
                }

                response = View(viewModel);
            }
            catch (Exception ex)
            {
                //log errors
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                response = RedirectToAction("Error", "Home");
            }
            finally { }

            return response;
        }

        /// <summary>
        /// Displays a list of reviews for a specific category.
        /// </summary>
        /// <param name="category">Category of a review to filter list</param>
        /// <returns>Returns a result based on status</returns>
        public ActionResult ReviewsByCategory(string category)
        {
            ActionResult response;
            ReviewVM viewModel = new ReviewVM();

            try
            {
                //access database
                List<ReviewDO> allReviews = _ReviewDataAccess.ViewReviews();

                //instantiate new lists
                List<ReviewPO> mappedReviews = new List<ReviewPO>();
                viewModel.ReviewsList = new List<ReviewPO>();

                //add to view model
                foreach (ReviewDO reviewDO in allReviews)
                {
                    //get review information
                    ReviewPO reviewPO = _ReviewMapper.MapDOtoPO(reviewDO);

                    //get game title
                    GameDO gameDO = _GameDataAccess.ViewGameByID(reviewPO.GameID);
                    reviewPO.GameTitle = gameDO.Title;

                    //get username
                    UserDO userDO = _UserDataAccess.ViewUserByID(reviewPO.UserID);
                    reviewPO.Username = userDO.Username;

                    mappedReviews.Add(reviewPO);
                }

                //sort through list of reviews, add to view model
                viewModel.ReviewsList = mappedReviews.Where(n => n.Category == category).ToList();

                response = View(viewModel);
            }
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                response = RedirectToAction("Error", "Home");
            }
            finally { }

            return response;
        }

        /// <summary>
        /// Display unique information for a specific review.
        /// </summary>
        /// <param name="id">ID of Review needing to be displayed</param>
        /// <returns></returns>
        public ActionResult ReviewDetails(int id)
        {
            ActionResult response;
            ReviewVM viewModel = new ReviewVM();

            //check id
            if (id > 0)
            {
                //if id is valid, retrieve information from the database
                try
                {
                    //access database, map to view model
                    ReviewDO reviewDO = _ReviewDataAccess.ViewReviewByID(id);

                    viewModel.Review = _ReviewMapper.MapDOtoPO(reviewDO);
                    viewModel.User = _UserMapper.MapDOtoPO(_UserDataAccess.ViewUserByID(viewModel.Review.UserID));
                    viewModel.Game = _GameMapper.MapDOtoPO(_GameDataAccess.ViewGameByID(viewModel.Review.GameID));

                    response = View(viewModel);
                }
                catch (Exception ex)
                {
                    //log error
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                    response = RedirectToAction("Error", "Home");
                }
                finally { }
            }
            else
            {
                //if id is not valid, return to the list
                response = RedirectToAction("Index", "Review");
            }

            return response;
        }
        #endregion

        #region Update
        /// <summary>
        /// Displays a form to receive input from the user.
        /// </summary>
        /// <param name="id">ID of the review needing to be edited</param>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult UpdateReview(int id)
        {
            ActionResult response;
            ReviewVM viewModel = new ReviewVM();
            
            //check if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, check id
                if (id > 0)
                {
                    //if id is valid, access the database
                    try
                    {
                        //access database, map retrieved information
                        ReviewDO reviewDO = _ReviewDataAccess.ViewReviewByID(id);

                        //check permissions
                        if (Session["UserID"] != null && (int)Session["UserID"] == reviewDO.UserID || (int)Session["RoleID"] == 4 || (int)Session["RoleID"] == 6)
                        {
                            //if allowed, set information to the view model

                            //set review
                            viewModel.Review = _ReviewMapper.MapDOtoPO(reviewDO);

                            //set user
                            viewModel.User = _UserMapper.MapDOtoPO(_UserDataAccess.ViewUserByID(viewModel.Review.UserID));

                            //set game
                            viewModel.Game = _GameMapper.MapDOtoPO(_GameDataAccess.ViewGameByID(viewModel.Review.GameID));

                            response = View(viewModel); 
                        }
                        else
                        {
                            //if not allowed, redirect to login
                            response = RedirectToAction("Login", "Account");
                        }
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                        response = RedirectToAction("Error", "Home");
                    }
                    finally { }
                }
                else
                {
                    //if id is not valid, return to list
                    response = RedirectToAction("Index", "Review");
                }
            }
            else
            {
                //if not logged in, redirect to login page
                response = RedirectToAction("Login", "Account");
            }
            
            return response;
        }

        /// <summary>
        /// Updates the database with information provided by the user.
        /// </summary>
        /// <param name="viewModel">View Model used for the form</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult UpdateReview(ReviewVM viewModel)
        {
            ActionResult response;

            //check to see if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, check the users permissions
                if (Session["UserID"] != null && (int)Session["UserID"] == viewModel.Review.UserID || (int)Session["RoleID"] == 4 || (int)Session["RoleID"] == 6)
                {
                    //if allowed, check the model state
                    if (ModelState.IsValid)
                    {
                        //if model is valid, map review, then access database
                        try
                        {
                            //map form, send to database
                            ReviewDO reviewDO = _ReviewMapper.MapPOtoDO(viewModel.Review);
                            _ReviewDataAccess.UpdateReview(reviewDO);

                            //redirect to review details to show changes
                            response = RedirectToAction("ReviewDetails", "Review", new { id = viewModel.Review.ReviewID });
                        }
                        catch (Exception ex)
                        {
                            //log error
                            _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                            response = RedirectToAction("Error", "Home");
                        }
                        finally { }
                    }
                    else
                    {
                        //if model is not valid, return to the form
                        response = View(viewModel);
                    }
                }
                else
                {
                    //if not allowed, redirect to login page
                    response = RedirectToAction("Login", "Account");
                }
            }
            else
            {
                //if not logged in, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes a record from the database.
        /// </summary>
        /// <param name="id">ID of review needing to be deleted</param>
        /// <returns>Returns a result based on status</returns>
        public ActionResult DeleteReview(int id)
        {
            ActionResult response;

            //check if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, check id
                if (id > 0)
                {
                    //if id is valid, access database
                    try
                    {
                        //pull review data
                        ReviewDO reviewDO = _ReviewDataAccess.ViewReviewByID(id);

                        //check permissions
                        if (Session["UserID"] != null && (int)Session["UserID"] == reviewDO.UserID || (int)Session["RoleID"] == 6)
                        {
                            //if allowed, map, then run data access method
                            ReviewPO reviewPO = _ReviewMapper.MapDOtoPO(reviewDO);
                            _ReviewDataAccess.DeleteReview(reviewPO.ReviewID);

                            response = RedirectToAction("Index", "Review");
                        }
                        else
                        {
                            //if not allowed, redirect to login page
                            response = RedirectToAction("Login", "Account");
                        }
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                        response = RedirectToAction("Error", "Home");
                    }
                    finally { }
                }
                else
                {
                    //if id is invalid, redirect to details
                    response = RedirectToAction("ReviewDetails", "Review");
                }
            }
            else
            {
                //if not logged in, redirect to login page
                response = RedirectToAction("Login", "Account");
            }
            
            return response;
        }
        #endregion
    }
}