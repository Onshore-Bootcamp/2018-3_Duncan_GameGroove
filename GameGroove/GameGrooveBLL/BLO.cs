using GameGrooveBLL.Mapping;
using GameGrooveBLL.Models;
using GameGrooveDAL;
using GameGrooveDAL.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace GameGrooveBLL
{
    public class BLO
    {
        #region Build
        //mapper and constructor variables
        private readonly ReviewMapper _Mapper = new ReviewMapper();
        private readonly Logger _Logger;

        /// <summary>
        /// Business Logic Object. Requires a file path for an errorlog. Holds methods that perform requested calculations.
        /// </summary>
        /// <param name="logPath">File path for ErrorLog.txt found in WebConfig</param>
        public BLO(string logPath)
        {
            _Logger = new Logger(logPath);
        } 
        #endregion
        
        /// <summary>
        /// Filters a list of Reviews to find the most frequent game. Displays on the home page.
        /// </summary>
        /// <param name="allReviews">List of all Review records in the Reviews table in the GAMEGROOVE database</param>
        /// <returns>Returns the ID of the most frequent game in all reviews</returns>
        public int TopGame(List<ReviewDO> allReviews)
        {
            ReviewBO topGame = new ReviewBO();
            int topGameID;

            try
            {
                //sort and filter list of reviews to find most frequent game ID
                var popGame = allReviews.GroupBy(r => r.GameID).OrderByDescending(grp => grp.Count());
                
                //send ID of first group in list
                topGameID = popGame.FirstOrDefault().Key;
            }
            catch (Exception ex)
            {
                //log error
                _Logger.ErrorLog(MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name, ex);
                throw ex;
            }
            finally { }

            return topGameID;
        }
    }
}
