using GameGroove.Mapping;
using GameGroove.Models;
using GameGrooveBLL;
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
    public class AccountController : Controller
    {
        #region Build
        //instance for mappers
        private readonly UserMapper _Mapper = new UserMapper();
        private readonly ReviewMapper _reviewMapper = new ReviewMapper();

        //variables for constructor
        private readonly UserDAO _UserDataAccess;
        private readonly GameDAO _GameDataAccess;
        private readonly ReviewDAO _ReviewDataAccess;
        private readonly RoleDAO _RoleDataAccess;
        private readonly BLO _businessLogic;
        private readonly Logger _Logger;

        /// <summary>
        /// This controller's constructor instantiates a logger, BLO, UserDAO, GameDAO, ReviewDAO, and RoleDAO.
        /// Each of these has parameters that are found in WebConfig.
        /// </summary>
        public AccountController()
        {
            string logPath = ConfigurationManager.AppSettings["LogPath"];
            string connectionString = ConfigurationManager.ConnectionStrings["GameGroove"].ConnectionString;

            _Logger = new Logger(logPath);
            _businessLogic = new BLO(logPath);
            _UserDataAccess = new UserDAO(logPath, connectionString);
            _GameDataAccess = new GameDAO(logPath, connectionString);
            _ReviewDataAccess = new ReviewDAO(logPath, connectionString);
            _RoleDataAccess = new RoleDAO(logPath, connectionString);
        }
        #endregion

        #region Login
        /// <summary>
        /// Displays a form for the user to input their information
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult Login()
        {
            ActionResult response;
            Login login = new Login();

            //check if logged in
            if (Session["Username"] == null)
            {
                //if not logged in, go to form
                try
                {
                    //check to see if the user just registered
                    if (TempData.ContainsKey("Username"))
                    {
                        login.Username = (string)TempData["Username"];
                    }

                    response = View(login);
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
                //if logged in, send to user details
                response = RedirectToAction("UserDetails", "Account", new { id = (int)Session["UserID"] });
            }

            return response;
        }

        /// <summary>
        /// Sets the users information in session. Checks what the user entered into the login form against what is in the database.
        /// </summary>
        /// <param name="login">Login form that the user has filled out</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult Login(Login login)
        {
            ActionResult response;

            //check to see if logged in
            if (Session["Username"] == null)
            {
                //if not logged in, check model
                if (ModelState.IsValid)
                {
                    //if model is valid, access the database
                    try
                    {
                        //access database, get account details
                        UserDO userDO = _UserDataAccess.ViewUserByUsername(login.Username);

                        //check ID
                        if (userDO.UserID > 0)
                        {
                            //if id is valid, check password
                            if (userDO.Password.Equals(login.Password))
                            {
                                //if password is correct, set details in session
                                Session["Username"] = userDO.Username;
                                Session["UserID"] = userDO.UserID;
                                Session["RoleID"] = userDO.RoleID;

                                response = RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                //if password is wrong, return to form
                                TempData["LoginError"] = "Username or Password is incorrect";
                                response = View(login);
                            }
                        }
                        else
                        {
                            //if id is not valid, return to form
                            TempData["LoginError"] = "Username or Password is incorrect";
                            response = View(login);
                        }
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);

                        //redirect to error page
                        response = RedirectToAction("Error", "Home");
                    }
                    finally { }
                }
                else
                {
                    //if model is not valid, return to form
                    response = View(login);
                }
            }
            else
            {
                //if user is logged in, redirect to user details
                response = RedirectToAction("UserDetails", "Account", new { id = (int)Session["UserID"] });
            }

            return response;
        }
        #endregion

        #region Logout
        /// <summary>
        /// Logout abandons session so that the user's information is no longer available to access certain pages.
        /// </summary>
        /// <returns>Returns a result</returns>
        public ActionResult Logout()
        {
            //remove details from session
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region Create
        /// <summary>
        /// Displays a form to gather information from the user in order to create an account.
        /// </summary>
        /// <returns>Returns a result</returns>
        [HttpGet]
        public ActionResult Register()
        {
            ActionResult response;

            //check if logged in
            if (Session["UserID"] == null)
            {
                //if not logged in, view the form
                response = View();
            }
            else
            {
                //if logged in, redirect to user details
                response = RedirectToAction("UserDetails", "Account", new { id = (int)Session["UserID"] });
            }

            return response;
        }

        /// <summary>
        /// Creates a user record in the database with information provided by the user.
        /// </summary>
        /// <param name="newUser">UserPO filled with information from the form</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult Register(UserPO newUser)
        {
            ActionResult response;

            //check if logged in
            if (Session["UserID"] == null)
            {
                //if not logged in, check model
                if (ModelState.IsValid)
                {
                    //if model is valid, access database
                    try
                    {
                        //pseudo mapper to set user with information from form and default role
                        UserDO userDO = new UserDO()
                        {
                            FirstName = newUser.FirstName,
                            LastName = newUser.LastName,
                            Username = newUser.Username,
                            Password = newUser.Password,
                            Email = newUser.Email,
                            RoleID = 1
                        };
                        //check for registration success
                        if (_UserDataAccess.RegisterUser(userDO))
                        {
                            //if successful, store user in session
                            TempData["Username"] = userDO.Username;
                            response = RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            //todo: separate email and username
                            //if not successful, return to form
                            TempData["EmailOrUsername"] = "Email address or Username is already connected to an account";
                            response = View(newUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                        TempData["RegisterError"] = "There was an issue in the registration process. Please check the information you entered and try again!";
                        response = View(newUser);
                    }
                    finally { }
                }
                else
                {
                    //if model is not valid, return to form
                    response = View(newUser);
                }
            }
            else
            {
                //if logged in, redirect to user details
                response = RedirectToAction("UserDetails", "Account", new { id = Session["UserID"] });
            }

            return response;
        }
        #endregion

        #region Read
        /// <summary>
        /// Displays a list of all users in the database.
        /// </summary>
        /// <returns>Returns a result based on status</returns>
        public ActionResult Index()
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if the user is an admin, access the database
                try
                {
                    //access database, instantiate list
                    List<UserDO> allUsers = _UserDataAccess.ViewUsers();

                    List<UserPO> mappedUsers = new List<UserPO>();

                    //map to presentation layer
                    foreach (UserDO user in allUsers)
                    {
                        RoleDO role = _RoleDataAccess.ViewRoleByID(user.RoleID);
                        UserPO mappedUser = _Mapper.MapDOtoPO(user);
                        mappedUser.RoleName = role.RoleName;
                        mappedUsers.Add(mappedUser);
                    }

                    response = View(mappedUsers);
                }
                catch (Exception ex)
                {
                    //log error
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                    response = RedirectToAction("Error", "Home");
                }
                finally { }
            }
            //check for user or mod
            else if (Session["RoleID"] != null && (int)Session["RoleID"] == 1 || Session["RoleID"] != null && (int)Session["RoleID"] == 4)
            {
                //if user or mod, redirect to home page
                response = RedirectToAction("Index", "Home");
            }
            //if not logged in
            else
            {
                //if not logged in, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Displays the information for one user.
        /// </summary>
        /// <param name="id">ID of user needing to be displayed</param>
        /// <returns>Returns a result based on status</returns>
        public ActionResult UserDetails(int id)
        {
            ActionResult response;

            ReviewVM viewModel = new ReviewVM();
            
            //check for permissions
            if (Session["UserID"] != null && (int)Session["UserID"] == id || Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                //if allowed, access the database
                try
                {
                    //access database, map to presentation, add to view model
                    //get user information
                    UserDO user = _UserDataAccess.ViewUserByID(id);
                    viewModel.User = _Mapper.MapDOtoPO(user);

                    //get role information for role name
                    RoleDO role = _RoleDataAccess.ViewRoleByID(user.RoleID);
                    viewModel.User.RoleName = role.RoleName;

                    //get user's most frequent category
                    viewModel.UserFavCategory = _reviewMapper.MapDOtoPO(_ReviewDataAccess.UserFavoriteCategory(viewModel.User.UserID));

                    //check id
                    if (viewModel.User.UserID > 0)
                    {
                        //if id is valid, access database
                        List<ReviewDO> gameReviews = _ReviewDataAccess.ViewReviews();

                        //instantiate new lists
                        List<ReviewPO> mappedReviews = new List<ReviewPO>();
                        viewModel.ReviewsList = new List<ReviewPO>();

                        //map to presenation, add to view model
                        foreach (ReviewDO reviewDO in gameReviews)
                        {
                            //map review information
                            ReviewPO reviewPO = _reviewMapper.MapDOtoPO(reviewDO);

                            //get game information for title
                            GameDO gameDO = _GameDataAccess.ViewGameByID(reviewPO.GameID);
                            reviewPO.GameTitle = gameDO.Title;

                            //get user information for username
                            UserDO userDO = _UserDataAccess.ViewUserByID(reviewPO.UserID);
                            reviewPO.Username = userDO.Username;

                            mappedReviews.Add(reviewPO);
                        }

                        //filter list by user, add to view model
                        viewModel.ReviewsList = mappedReviews.Where(n => n.UserID == viewModel.User.UserID).ToList();

                        response = View(viewModel);
                    }
                    else
                    {
                        //if model state is not valid, display error screen
                        response = RedirectToAction("Error", "Home");
                    }
                }
                catch (Exception ex)
                {
                    //log error
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);

                    //display error page
                    response = RedirectToAction("Error", "Home");
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

        #region Update
        /// <summary>
        /// Displays form to gather input from the user.
        /// </summary>
        /// <param name="id">ID of user needing to be updated</param>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult UpdateUser(int id)
        {
            ActionResult response;

            //check for permissions
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6 || Session["UserID"] != null && (int)Session["UserID"] == id)
            {
                //if allowed, access the database
                try
                {
                    //pull record from database, map to PO
                    UserDO user = _UserDataAccess.ViewUserByID(id);
                    UserPO mappedUser = _Mapper.MapDOtoPO(user);
                    mappedUser.ConfirmPassword = mappedUser.Password;

                    //check id
                    if (mappedUser.UserID > 0)
                    {
                        //if id is valid, fill drop down list, view form
                        ViewBag.roleList = RoleDropDown();

                        response = View(mappedUser);
                    }
                    else
                    {
                        //if id is not valid, display error page
                        response = RedirectToAction("Error", "Home");
                    }
                }
                catch (Exception ex)
                {
                    //log error
                    _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);

                    //display error page
                    response = RedirectToAction("Error", "Home");
                }
                finally { }
            }
            else
            {
                //if not allowed, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Updates a record in the database with information provided by the user.
        /// </summary>
        /// <param name="updatedUser">UserPO filled with information from the form</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult UpdateUser(UserPO updatedUser)
        {
            ActionResult response;

            //check permissions
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6 || Session["UserID"] != null && (int)Session["UserID"] == updatedUser.UserID)
            {
                //if allowed, check model
                if (ModelState.IsValid)
                {
                    //if model is valid, access the database
                    try
                    {
                        //map form, update database
                        UserDO userDO = _Mapper.MapPOtoDO(updatedUser);
                        _UserDataAccess.UpdateUser(userDO);

                        response = RedirectToAction("UserDetails", "Account", new { id = userDO.UserID });
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);

                        //set tempdata for error
                        TempData["UpdateUserError"] = "There was an issue updating this account, please check the information you entered and try again.";

                        //repopulate drop down list
                        ViewBag.roleList = RoleDropDown();

                        //return to form
                        response = View(updatedUser);
                    }
                    finally { }
                }
                else
                {
                    //if model is not valid, repopulate drop down list and return to form
                    ViewBag.roleList = RoleDropDown();
                    response = View(updatedUser);
                }
            }
            else
            {
                //if not allowed, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Displays a form that allows the user to change their password
        /// </summary>
        /// <param name="id">ID of user needing to change password</param>
        /// <returns>Returns a result based on status</returns>
        [HttpGet]
        public ActionResult ChangePassword(int id)
        {
            ActionResult response;

            //check for permissions
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6 || Session["UserID"] != null && (int)Session["UserID"] == id)
            {
                //if allowed, access the database
                try
                {
                    //pull record, map it
                    UserDO user = _UserDataAccess.ViewUserByID(id);
                    UserPO mappedUser = _Mapper.MapDOtoPO(user);

                    ChangePassword changedPassword = new ChangePassword();
                    changedPassword.UserID = mappedUser.UserID;

                    //check id
                    if (mappedUser.UserID > 0)
                    {
                        //if id is valid, check for admin, view form
                        if ((int)Session["RoleID"] == 6)
                        {
                            //if admin, set password for model
                            changedPassword.Password = user.Password;
                        }

                        response = View(changedPassword);
                    }
                    else
                    {
                        //if id is not valid, display error page
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
                //if not allowed, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }

        /// <summary>
        /// Update account in database with new password
        /// </summary>
        /// <param name="changedPassword">ChangePassword object with information provided by the user</param>
        /// <returns>Returns a result based on status</returns>
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword changedPassword)
        {
            ActionResult response;

            //check for permission
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6 || Session["UserID"] != null && (int)Session["UserID"] == changedPassword.UserID)
            {
                //if allowed, check the model
                if (ModelState.IsValid)
                {
                    //if model is valid, access the database
                    try
                    {
                        //map form
                        UserDO userDO = _UserDataAccess.ViewUserByID(changedPassword.UserID);
                        
                        //check password
                        if (userDO.Password.Equals(changedPassword.Password))
                        {
                            //if password is correct, update database
                            userDO.Password = changedPassword.NewPassword;

                            _UserDataAccess.UpdateUser(userDO);

                            response = RedirectToAction("UserDetails", "Account", new { id = userDO.UserID });
                        }
                        else
                        {
                            //if password is wrong, set tempdata and return to form
                            TempData["IncorrectPassword"] = "Incorrect password, please try again";
                            response = View(changedPassword);
                        }
                    }
                    catch (Exception ex)
                    {
                        //log error
                        _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);

                        //set tempdata and return to form
                        TempData["PasswordError"] = "You must enter a new password";
                        response = View(changedPassword);
                    }
                    finally { }
                }
                else
                {
                    //if model is not valid, return to form
                    response = View(changedPassword);
                }
            }
            else
            {
                //if not allowed, redirect to login page
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Delete
        //delete a user
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            ActionResult response;

            //check for admin
            if (Session["RoleID"] != null && (int)Session["RoleID"] == 6)
            {
                try
                {
                    //pull info from database, map it
                    UserDO user = _UserDataAccess.ViewUserByID(id);
                    UserPO mappedUser = _Mapper.MapDOtoPO(user);

                    //check id
                    if (mappedUser.UserID > 0)
                    {
                        _UserDataAccess.DeleteUser(mappedUser.UserID);

                    }

                    response = RedirectToAction("Index", "Account");
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
                response = RedirectToAction("Login", "Account");
            }

            return response;
        }
        #endregion

        #region Extras
        /// <summary>
        /// Populates the drop down list for roles in the update methods
        /// </summary>
        /// <returns>Returns a list of roles for drop down</returns>
        private static List<SelectListItem> RoleDropDown()
        {
            //instantiate list, add to it
            List<SelectListItem> dropDownItems = new List<SelectListItem>();
            dropDownItems.Add(new SelectListItem
            {
                Text = "User",
                Value = 1.ToString()
            }); dropDownItems.Add(new SelectListItem
            {
                Text = "Mod",
                Value = 4.ToString()
            }); dropDownItems.Add(new SelectListItem
            {
                Text = "Admin",
                Value = 6.ToString()
            });

            return dropDownItems;
        } 
        #endregion
    }
}