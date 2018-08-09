using GameGroove.Models;
using GameGrooveDAL.Models;

namespace GameGroove.Mapping
{
    public class RequestMapper
    {
        public RequestPO MapDOtoPO(RequestDO requestDO)
        {
            RequestPO requestPO = new RequestPO();
            requestPO.RequestID = requestDO.RequestID;
            requestPO.RequestText = requestDO.RequestText;
            requestPO.Username = requestDO.Username;
            requestPO.Date = requestDO.Date;
            return requestPO;
        }

        public RequestDO MapPOtoDO(RequestPO requestPO)
        {
            RequestDO requestDO = new RequestDO();
            requestDO.RequestID = requestPO.RequestID;
            requestDO.RequestText = requestPO.RequestText;
            requestDO.Username = requestPO.Username;
            requestDO.Date = requestPO.Date;
            return requestDO;
        }
    }
}