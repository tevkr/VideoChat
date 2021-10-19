using System;
using System.Drawing;
using SharedLibrary.Data.Requests;
using SharedLibrary.Data.Responses;
using SharedLibrary.Data.UDPData;

namespace SharedLibrary.Data
{
   [Serializable]
    public class DataObject
    {
        private DataObject() { }
        public enum DataObjectTypes
        {
            // Requests
            changeUserNameRequest,
            createLobbyRequest,
            joinLobbyRequest,
            leaveLobbyRequest,
            getLobbiesRequest,
            // Responses
            successResponse,
            errorResponse,
            nameChangedResponse,
            lobbyInfoResponse,
            lobbiesInfoResponse,
            userJoinedToLobbyResponse,
            userLeavedFromLobbyResponse,
            // UDP Data
            newVideoFrame
        }
        public DataObjectTypes dataObjectType { get; private set; }
        public object dataObjectInfo { get; private set; }

        // Requests
        public static DataObject changeUserNameRequest(string userName)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.changeUserNameRequest;
            result.dataObjectInfo = new ChangeUserName(userName);
            return result;
        }
        public static DataObject createLobbyRequest(string lobbyName, int lobbyCapacity, string lobbyPassword)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.createLobbyRequest;
            result.dataObjectInfo = new CreateLobby(lobbyName, lobbyCapacity, lobbyPassword);
            return result;
        }
        public static DataObject joinLobbyRequest(string lobbyId, string lobbyPassword)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.joinLobbyRequest;
            result.dataObjectInfo = new JoinLobby(lobbyId, lobbyPassword);
            return result;
        }
        public static DataObject leaveLobbyRequest()
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.leaveLobbyRequest;
            result.dataObjectInfo = null;
            return result;
        }
        public static DataObject getLobbiesRequest()
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.getLobbiesRequest;
            result.dataObjectInfo = null;
            return result;
        }

        // Responses
        public static DataObject successResponse()
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.successResponse;
            result.dataObjectInfo = null;
            return result;
        }
        public static DataObject errorResponse()
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.errorResponse;
            result.dataObjectInfo = null;
            return result;
        }
        public static DataObject nameChangedResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.nameChangedResponse;
            result.dataObjectInfo = new NameChanged(userJson);
            return result;
        }
        public static DataObject lobbyInfoResponse(string lobbyJson)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.lobbyInfoResponse;
            result.dataObjectInfo = new LobbyInfo(lobbyJson);
            return result;
        }
        public static DataObject lobbiesInfoResponse(string lobbiesJson)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.lobbiesInfoResponse;
            result.dataObjectInfo = new LobbiesInfo(lobbiesJson);
            return result;
        }
        public static DataObject userJoinedToLobbyResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.userJoinedToLobbyResponse;
            result.dataObjectInfo = new UserJoinedToLobby(userJson);
            return result;
        }
        public static DataObject userLeavedFromLobbyResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.userLeavedFromLobbyResponse;
            result.dataObjectInfo = new UserLeavedFromLobby(userJson);
            return result;
        }

        // UDP Data
        public static DataObject newVideoFrame(Bitmap newFrame, string userId)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.newVideoFrame;
            result.dataObjectInfo = new NewVideoFrame(newFrame, userId);
            return result;
        }
        public static DataObject newVideoFrame(byte[] newFrame, string userId)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.newVideoFrame;
            result.dataObjectInfo = new NewVideoFrame(newFrame, userId);
            return result;
        }
    }
}
