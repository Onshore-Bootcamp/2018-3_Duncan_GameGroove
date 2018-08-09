namespace GameGrooveDAL.Models
{
    public class ReviewDO
    {
        public int ReviewID { get; set; }

        public string ReviewText { get; set; }

        public string DatePosted { get; set; }

        public string Category { get; set; }

        public int UserID { get; set; }

        public int GameID { get; set; }
    }
}
