using GameGroove.Models;
using GameGrooveDAL.Models;

namespace GameGroove.Mapping
{
    public class GameMapper
    {
        public GamePO MapDOtoPO(GameDO gameDO)
        {
            GamePO gamePO = new GamePO();
            gamePO.GameID = gameDO.GameID;
            gamePO.Title = gameDO.Title;
            gamePO.ReleaseDate = gameDO.ReleaseDate;
            gamePO.Developer = gameDO.Developer;
            gamePO.Platform = gameDO.Platform;
            return gamePO;
        }

        public GameDO MapPOtoDO(GamePO gamePO)
        {
            GameDO gameDO = new GameDO();
            gameDO.GameID = gamePO.GameID;
            gameDO.Title = gamePO.Title;
            gameDO.ReleaseDate = gamePO.ReleaseDate;
            gameDO.Developer = gamePO.Developer;
            gameDO.Platform = gamePO.Platform;
            return gameDO;
        }
    }
}