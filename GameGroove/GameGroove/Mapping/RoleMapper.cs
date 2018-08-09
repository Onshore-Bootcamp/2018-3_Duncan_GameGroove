using GameGroove.Models;
using GameGrooveDAL.Models;

namespace GameGroove.Mapping
{
    public class RoleMapper
    {
        public RolePO MapDOtoPO(RoleDO roleDO)
        {
            RolePO rolePO = new RolePO();
            rolePO.RoleID = roleDO.RoleID;
            rolePO.RoleName = roleDO.RoleName;
            return rolePO;
        }
    }
}