using GameGrooveBLL.Models;
using GameGrooveDAL.Models;

namespace GameGrooveBLL.Mapping
{
    public class ReviewMapper
    {
        public ReviewBO MapDOtoBO(ReviewDO reviewDO)
        {
            ReviewBO reviewBO = new ReviewBO();
            reviewBO.ReviewID = reviewDO.ReviewID;
            reviewBO.ReviewText = reviewDO.ReviewText;
            reviewBO.DatePosted = reviewDO.DatePosted;
            reviewBO.Category = reviewDO.Category;
            reviewBO.UserID = reviewDO.UserID;
            reviewBO.GameID = reviewDO.GameID;
            return reviewBO;
        }
    }
}
