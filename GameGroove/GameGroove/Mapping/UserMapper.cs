using GameGroove.Models;
using GameGrooveDAL.Models;

namespace GameGroove.Mapping
{
    public class UserMapper
    {
        public UserPO MapDOtoPO(UserDO userDO)
        {
            UserPO userPO = new UserPO();
            userPO.UserID = userDO.UserID;
            userPO.FirstName = userDO.FirstName;
            userPO.LastName = userDO.LastName;
            userPO.Username = userDO.Username;
            userPO.Password = userDO.Password;
            userPO.Email = userDO.Email;
            userPO.RoleID = userDO.RoleID;

            return userPO;
        }

        public UserDO MapPOtoDO(UserPO userPO)
        {
            UserDO userDO = new UserDO();
            userDO.UserID = userPO.UserID;
            userDO.FirstName = userPO.FirstName;
            userDO.LastName = userPO.LastName;
            userDO.Username = userPO.Username;
            userDO.Password = userPO.Password;
            userDO.Email = userPO.Email;
            userDO.RoleID = userPO.RoleID;

            return userDO;
        }
    }
}