using GameGroove.Mapping;
using GameGroove.Models;
using GameGrooveBLL;
using GameGrooveDAL;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace GameGroove.Controllers
{
    public class HomeController : Controller
    {
        #region Build
        //instantiate mappers
        private readonly ReviewMapper _ReviewMapper = new ReviewMapper();
        private readonly GameMapper _GameMapper = new GameMapper();

        //create variables for constructor
        private static ReviewDAO _ReviewDataAccess;
        private static GameDAO _GameDataAccess;
        private readonly BLO _BusinessLogic;
        private readonly Logger _Logger;

        /// <summary>
        /// This controller's constructor instantiates a ReviewDAO, GameDAO, BLO, and Logger. Each has parameters found in WebConfig.
        /// </summary>
        public HomeController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GameGroove"].ConnectionString;
            string logPath = ConfigurationManager.AppSettings["LogPath"];

            _ReviewDataAccess = new ReviewDAO(logPath, connectionString);
            _GameDataAccess = new GameDAO(logPath, connectionString);
            _BusinessLogic = new BLO(logPath);
            _Logger = new Logger(logPath);
        } 
        #endregion

        /// <summary>
        /// Displays the homepage. Homepage contains two sections to display top game and top category.
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        public ActionResult Index()
        {
            ActionResult response;
            ReviewVM  viewModel = new ReviewVM();

            try
            {
                //instantiate lists
                viewModel.ReviewsList = new List<ReviewPO>();
                List<ReviewDO> reviewDOs = new List<ReviewDO>();

                //call DAO to get list of reviews
                reviewDOs = _ReviewDataAccess.ViewReviews();

                //call BLL for most recommended category
                viewModel.Review = _ReviewMapper.MapDOtoPO(_ReviewDataAccess.TopCategory());

                //call DAO and BLL for most recommended game
                int topGameID = _BusinessLogic.TopGame(reviewDOs);
                viewModel.Game = _GameMapper.MapDOtoPO(_GameDataAccess.ViewGameByID(topGameID));

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
        /// Displays information about the website.
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Displays contact information. Allows registered users to submit requests.
        /// </summary>
        /// <returns></returns>
        public ActionResult Contact()
        {
            return View();
        }

        /// <summary>
        /// Displays a screen notifying the user of an error.
        /// </summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }
    }
}