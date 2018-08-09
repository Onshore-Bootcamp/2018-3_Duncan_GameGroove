using GameGroove.Mapping;
using GameGroove.Models;
using GameGrooveDAL;
using GameGrooveDAL.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;

namespace GameGroove.Controllers
{
    public class RequestController : Controller
    {
        #region Build
        //initialize mapper
        private readonly RequestMapper _RequestMapper = new RequestMapper();

        //initialize variables for constructor
        private readonly RequestDAO _RequestDataAccess;
        private readonly UserDAO _UserDataAccess;
        private readonly Logger _Logger;

        /// <summary>
        /// This controller's constructor instantiates a UserDAO, a RequestDAO, and a logger. Each has parameters found in WebConfig.
        /// </summary>
        public RequestController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GameGroove"].ConnectionString;
            string logPath = ConfigurationManager.AppSettings["LogPath"];

            _Logger = new Logger(logPath);
            _RequestDataAccess = new RequestDAO(logPath, connectionString);
            _UserDataAccess = new UserDAO(logPath, connectionString);
        }
        #endregion

        #region Create
        /// <summary>
        /// Displays a form to gather information from the user.
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult CreateRequest()
        {
            ActionResult response;

            //check if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, view the form
                response = View();
            }
            else
            {
                //if not logged in, redirect to the login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Uses information supplied by the user to create a request record in the database.
        /// </summary>
        /// <param name="requestPO">RequestPO filled with information provided by the user</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult CreateRequest(RequestPO requestPO)
        {
            ActionResult response;

            //check if logged in
            if (Session["RoleID"] != null)
            {
                //if logged in, check the model state
                try
                {
                    //check model
                    if (ModelState.IsValid)
                    {
                        //if model is valid, get user information for username
                        UserDO user = _UserDataAccess.ViewUserByID((int)Session["UserID"]);

                        //map from view model to a DO
                        RequestDO request = new RequestDO()
                        {
                            RequestText = requestPO.RequestText,
                            Username = user.Username,
                            Date = DateTime.Now.ToString(),
                        };

                        //access database
                        _RequestDataAccess.CreateRequest(request);

                        //show confirmation screen
                        response = RedirectToAction("RequestSubmitted", "Request");
                    }
                    else
                    {
                        //if model is not valid, return to form
                        response = View(requestPO);
                    }
                }
                catch (Exception ex)
                {
                    //log error
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                    response = View(requestPO);
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

        /// <summary>
        /// Displays a success screen after submitting a request.
        /// </summary>
        /// <returns>Returns a view</returns>
        public ActionResult RequestSubmitted()
        {
            return View();
        }
        #endregion

        #region Read
        /// <summary>
        /// Displays a list of all requests in the database.
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        public ActionResult Index()
        {
            ActionResult response;
            List<RequestPO> mappedRequests = new List<RequestPO>();

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if user is an admin, access the database
                try
                {
                    //access database
                    List<RequestDO> allRequests = _RequestDataAccess.ViewRequests();

                    //map to presentation layer
                    foreach (RequestDO request in allRequests)
                    {
                        mappedRequests.Add(_RequestMapper.MapDOtoPO(request));
                    }

                    response = View(mappedRequests);
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
                //if the user is not an admin, redirect to the login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Displays the information for one specific request
        /// </summary>
        /// <param name="id">ID of the request needing to be displayed</param>
        /// <returns>Returns a result based on status</returns>
        public ActionResult RequestDetails(int id)
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if user is an admin, check id
                if (id > 0)
                {
                    //if id is valid, access database
                    try
                    {
                        //access database, map to view model
                        RequestDO requestDO = _RequestDataAccess.ViewRequestByID(id);

                        RequestPO requestPO = _RequestMapper.MapDOtoPO(requestDO);

                        response = View(requestPO);
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
                    //if id is not valid, return to list of requests
                    response = RedirectToAction("Index", "Request");
                }
            }
            else
            {
                //if user is not an admin, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes a request from the database
        /// </summary>
        /// <param name="id">ID of the request needing to be deleted</param>
        /// <returns>Returns a result based on status</returns>
        public ActionResult DeleteRequest(int id)
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if the user is an admin, check id
                if (id > 0)
                {
                    //if id is valid, access the database
                    try
                    {
                        //pull request data
                        RequestDO requestDO = _RequestDataAccess.ViewRequestByID(id);
                        
                        //delete from database
                        _RequestDataAccess.DeleteRequest(requestDO.RequestID);
                        
                        response = RedirectToAction("Index", "Request");
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
                    //if id is not valid, return to request details
                    response = RedirectToAction("RequestDetails", "Request");
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
    }
}