using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDLL
{
    [Serializable]
    public class ServerResponse
    {
        private ServerResponse() { }
        public enum Responses
        {
            Success,
            Error
        }
        private Responses _response;
        public Responses Response
        {
            get { return _response; }
            set { _response = value; }
        }
        public static ServerResponse CreateServerResponse(Responses response)
        {
            ServerResponse result = new ServerResponse();
            result.Response = response;
            return result;
        }
        public static ServerResponse SuccessResponse()
        {
            ServerResponse result = new ServerResponse();
            result.Response = Responses.Success;
            return result;
        }
        public static ServerResponse ErrorResponse()
        {
            ServerResponse result = new ServerResponse();
            result.Response = Responses.Error;
            return result;
        }
    }
}
