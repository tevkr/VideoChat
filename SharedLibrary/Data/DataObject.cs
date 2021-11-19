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
            signUpRequest,
            loginInRequest,
            createLobbyRequest,
            joinLobbyRequest,
            leaveLobbyRequest,
            getLobbiesRequest,
            // Responses
            successResponse,
            errorResponse,
            userInfoResponse,
            lobbyInfoResponse,
            lobbiesInfoResponse,
            userJoinedToLobbyResponse,
            userLeavedFromLobbyResponse,
            // UDP Data
            newVideoFrame,
            newAudioFrame
        }
        public DataObjectTypes dataObjectType { get; private set; }
        public object dataObjectInfo { get; private set; }

        // Requests
        public static DataObject signUpRequest(string userName, string password)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.signUpRequest;
            result.dataObjectInfo = new SignUp(userName, password);
            return result;
        }
        public static DataObject loginInRequest(string userName, string password)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.loginInRequest;
            result.dataObjectInfo = new LoginIn(userName, password);
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
        public static DataObject successResponse(string successString)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.successResponse;
            result.dataObjectInfo = successString;
            return result;
        }
        public static DataObject errorResponse(string errorString)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.errorResponse;
            result.dataObjectInfo = errorString;
            return result;
        }
        public static DataObject userInfoResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.userInfoResponse;
            result.dataObjectInfo = new UserInfo(userJson);
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
        public static DataObject newAudioFrame(byte[] newFrame, string userId)
        {
            DataObject result = new DataObject();
            result.dataObjectType = DataObjectTypes.newAudioFrame;
            result.dataObjectInfo = new NewAudioFrame(newFrame, userId);
            return result;
        }
    }
}
