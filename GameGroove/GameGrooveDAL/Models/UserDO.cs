namespace GameGrooveDAL.Models
{
    public class UserDO
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public int RoleID { get; set; }
    }
}
