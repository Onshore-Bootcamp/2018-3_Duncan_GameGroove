using GameGroove.Mapping;
using GameGrooveDAL;
using System.Collections.Generic;
using System.Configuration;

namespace GameGroove.Models
{
    public class ReviewVM
    {
        public GamePO Game { get; set; }
        public ReviewPO Review { get; set; }
        public UserPO User { get; set; }

        public List<ReviewPO> ReviewsList { get; set; }
        public ReviewPO UserFavCategory { get; set; }
    }
}