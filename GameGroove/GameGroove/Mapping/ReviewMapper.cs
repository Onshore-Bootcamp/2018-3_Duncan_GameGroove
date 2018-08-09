using GameGroove.Models;
using GameGrooveBLL.Models;
using GameGrooveDAL.Models;

namespace GameGroove.Mapping
{
    public class ReviewMapper
    {
        public ReviewPO MapDOtoPO(ReviewDO ReviewDO)
        {
            ReviewPO reviewPO = new ReviewPO();
            reviewPO.ReviewID = ReviewDO.ReviewID;
            reviewPO.ReviewText = ReviewDO.ReviewText;
            reviewPO.DatePosted = ReviewDO.DatePosted;
            reviewPO.Category = ReviewDO.Category;
            reviewPO.UserID = ReviewDO.UserID;
            reviewPO.GameID = ReviewDO.GameID;
            return reviewPO;
        }

        public ReviewDO MapPOtoDO(ReviewPO reviewPO)
        {
            ReviewDO reviewDO = new ReviewDO();
            reviewDO.ReviewID = reviewPO.ReviewID;
            reviewDO.ReviewText = reviewPO.ReviewText;
            reviewDO.DatePosted = reviewPO.DatePosted;
            reviewDO.Category = reviewPO.Category;
            reviewDO.UserID = reviewPO.UserID;
            reviewDO.GameID = reviewPO.GameID;
            return reviewDO;
        }

        public ReviewPO MapBOtoPO(ReviewBO reviewBO)
        {
            ReviewPO reviewPO = new ReviewPO();
            reviewPO.ReviewID = reviewBO.ReviewID;
            reviewPO.ReviewText = reviewBO.ReviewText;
            reviewPO.DatePosted = reviewBO.DatePosted;
            reviewPO.Category = reviewBO.Category;
            reviewPO.UserID = reviewBO.UserID;
            reviewPO.GameID = reviewBO.GameID;
            return reviewPO;
        }
    }
}