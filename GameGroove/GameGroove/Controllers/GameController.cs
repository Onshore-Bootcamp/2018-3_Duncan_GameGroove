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
    public class GameController : Controller
    {
        #region Build
        //instantiate mappers
        private readonly GameMapper _GameMapper = new GameMapper();
        private readonly ReviewMapper _ReviewMapper = new ReviewMapper();

        //create variables for constructor
        private readonly GameDAO _GameDataAccess;
        private readonly ReviewDAO _ReviewDataAccess;
        private readonly UserDAO _UserDataAccess;
        private readonly Logger _Logger;

        /// <summary>
        /// This controller's constructor instantiates a logger, GameDAO, ReviewDAO, and UserDAO. Each of these has parameters found in WebConfig.
        /// </summary>
        public GameController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GameGroove"].ConnectionString;
            string logPath = ConfigurationManager.AppSettings["LogPath"];

            _Logger = new Logger(logPath);
            _GameDataAccess = new GameDAO(logPath, connectionString);
            _ReviewDataAccess = new ReviewDAO(logPath, connectionString);
            _UserDataAccess = new UserDAO(logPath, connectionString);
        }
        #endregion

        #region Create
        /// <summary>
        /// Displays a form to gather information from the user.
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult CreateGame()
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if the user is an admin, display the form
                response = View();
            }
            else
            {
                //if the user is not an admin, redirect to the login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Creates a record in the database with information in the form.
        /// </summary>
        /// <param name="newGame">GamePO filled with information provided by the user</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult CreateGame(GamePO newGame)
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if the user is an admin, check the model
                if (ModelState.IsValid)
                {
                    //if the model is valid, access the database
                    try
                    {
                        //map to data layer, access database
                        GameDO game = _GameMapper.MapPOtoDO(newGame);
                        _GameDataAccess.CreateGame(game);

                        response = RedirectToAction("Index", "Game");
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                        TempData["GameError"] = "There was an error processing this game, please check the information you entered and try again.";
                        response = View(newGame);
                    }
                    finally { }
                }
                else
                {
                    //if the model is not valid, return to the form
                    response = View(newGame);
                }
            }
            else
            {
                //if the user is not an admin, redirect to the login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Read
        /// <summary>
        /// Displays a list of all games in the database
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        public ActionResult Index()
        {
            ActionResult response;
            List<GamePO> mappedGames = new List<GamePO>();

            try
            {
                //access database
                List<GameDO> allGames = _GameDataAccess.ViewGames();

                //map to presentation layer
                foreach (GameDO game in allGames)
                {
                    mappedGames.Add(_GameMapper.MapDOtoPO(game));
                }

                response = View(mappedGames);
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
        /// Displays the information for a specific game
        /// </summary>
        /// <param name="id">ID of the game needing to be displayed</param>
        /// <returns>Returns a result based on status</returns>
        public ActionResult GameDetails(int id)
        {
            ActionResult response;
            ReviewVM viewModel = new ReviewVM();

            //check id
            if (id > 0)
            {
                //if id is valid, access the database
                try
                {
                    //set game
                    GameDO gameDO = _GameDataAccess.ViewGameByID(id);
                    viewModel.Game = _GameMapper.MapDOtoPO(gameDO);

                    //instantiate lists
                    List<ReviewDO> gameReviews = _ReviewDataAccess.ViewReviews();
                    List<ReviewPO> mappedReviews = new List<ReviewPO>();
                    viewModel.ReviewsList = new List<ReviewPO>();

                    //map reviews to a list of POs
                    foreach (ReviewDO reviewDO in gameReviews)
                    {
                        //get review information
                        ReviewPO reviewPO = _ReviewMapper.MapDOtoPO(reviewDO);

                        //get game information for title
                        GameDO reviewGame = _GameDataAccess.ViewGameByID(reviewPO.GameID);
                        reviewPO.GameTitle = gameDO.Title;

                        //get user information for username
                        UserDO userDO = _UserDataAccess.ViewUserByID(reviewPO.UserID);
                        reviewPO.Username = userDO.Username;

                        mappedReviews.Add(reviewPO);
                    }

                    //filter list and map to view model
                    viewModel.ReviewsList = mappedReviews.Where(n => n.GameID == viewModel.Game.GameID).ToList();

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
                //if id is not valid, return to game list
                response = RedirectToAction("Index", "Game");
            }

            return response;
        }
        #endregion

        #region Update
        /// <summary>
        /// Displays a form to gather information from the user
        /// </summary>
        /// <param name="id">ID of the game needing to be updated</param>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult UpdateGame(int id)
        {
            ActionResult response;

            //check for admin or mod
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6 || Session["RoleID"] != null && (int)Session["RoleID"] == 4)
            {
                //if the user is an admin or a mod, check id
                if (id > 0)
                {
                    //if id is valid, access the database
                    try
                    {
                        //access database, map to presentation layer
                        GameDO gameDO = _GameDataAccess.ViewGameByID(id);
                        GamePO gamePO = _GameMapper.MapDOtoPO(gameDO);

                        response = View(gamePO);
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
                    //if id is not valid, return to list of games
                    response = RedirectToAction("Index", "Game");
                }
            }
            else
            {
                //if the user is not an admin or a mod, redirect to the login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Updates the database with information provided by the user.
        /// </summary>
        /// <param name="updatedGame">GamePO filled with information in the form</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult UpdateGame(GamePO updatedGame)
        {
            ActionResult response;

            //check for admin or mod
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6 || Session["RoleID"] != null && (int)Session["RoleID"] == 4)
            {
                //if the user is an admin or a mod, check the model
                if (ModelState.IsValid)
                {
                    //if the model is valid, access the database
                    try
                    {
                        //map form, send to database
                        GameDO gameDO = _GameMapper.MapPOtoDO(updatedGame);
                        _GameDataAccess.UpdateGame(gameDO);

                        response = RedirectToAction("GameDetails", "Game", new { id = updatedGame.GameID });
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);

                        //set tempdata for an error
                        TempData["GameError"] = "There was an error processing this game, please check the information you entered and try again.";
                        response = View(updatedGame);
                    }
                    finally { }
                }
                else
                {
                    //if the model is not valid, return to form
                    response = View(updatedGame);
                }
            }
            else
            {
                //if the user is not an admin or a mod, redirect to the login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes a game from the database
        /// </summary>
        /// <param name="id">ID of game needing to be deleted</param>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult DeleteGame(int id)
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if the user is an admin, access the database
                try
                {
                    //access database, map to presentation layer
                    GameDO gameDO = _GameDataAccess.ViewGameByID(id);
                    GamePO gamePO = _GameMapper.MapDOtoPO(gameDO);
                    
                    //check id
                    if ( gamePO.GameID > 0)
                    {
                        //if id is valid, delete from database
                        _GameDataAccess.DeleteGame(gamePO.GameID);

                        response = RedirectToAction("Index", "Game");
                    }
                    else
                    {
                        //if id is not valid, show error screen
                        response = RedirectToAction("Error", "Home");
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
                //if the user is not an admin, return to the details page
                response = RedirectToAction("GameDetails", "Game", new { id });
            }

            return response;
        }
        #endregion
    }
}