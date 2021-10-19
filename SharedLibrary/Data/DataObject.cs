using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SharedLibrary.Data.Models;
using SharedLibrary.Data.Requests;
using SharedLibrary.Data.Responses;
using SharedLibrary.Data.UDPData;

namespace SharedLibrary.Data
{
    class DataObject
    {
        private DataObject() { }
        public enum DataObjectTypes
        {
            // Requests
            ChangeUserNameRequest,
            CreateLobbyRequest,
            JoinLobbyRequest,
            LeaveLobbyRequest,
            GetLobbiesRequest,
            // Responses
            SuccessResponse,
            ErrorResponse,
            NameChangedResponse,
            LobbyInfoResponse,
            LobbiesInfoResponse,
            UserJoinedToLobbyResponse,
            UserLeavedFromLobbyResponse,
            // UDP Data
            NewVideoFrame
        }
        public DataObjectTypes DataObjectType { get; private set; }
        public object DataObjectInfo { get; private set; }

        // Requests
        public static DataObject ChangeUserNameRequest(string userName)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.ChangeUserNameRequest;
            result.DataObjectInfo = new ChangeUserName(userName);
            return result;
        }
        public static DataObject CreateLobbyRequest(string lobbyName, int lobbyCapacity, string lobbyPassword)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.CreateLobbyRequest;
            result.DataObjectInfo = new CreateLobby(lobbyName, lobbyCapacity, lobbyPassword);
            return result;
        }
        public static DataObject JoinLobbyRequest(string lobbyId, string lobbyPassword)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.JoinLobbyRequest;
            result.DataObjectInfo = new JoinLobby(lobbyId, lobbyPassword);
            return result;
        }
        public static DataObject LeaveLobbyRequest()
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.LeaveLobbyRequest;
            result.DataObjectInfo = null;
            return result;
        }
        public static DataObject GetLobbiesRequest()
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.GetLobbiesRequest;
            result.DataObjectInfo = null;
            return result;
        }

        // Responses
        public static DataObject SuccessResponse()
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.SuccessResponse;
            result.DataObjectInfo = null;
            return result;
        }
        public static DataObject ErrorResponse()
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.ErrorResponse;
            result.DataObjectInfo = null;
            return result;
        }
        public static DataObject NameChangedResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.NameChangedResponse;
            result.DataObjectInfo = new NameChanged(userJson);
            return result;
        }
        public static DataObject LobbyInfoResponse(string lobbyJson)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.LobbyInfoResponse;
            result.DataObjectInfo = new LobbyInfo(lobbyJson);
            return result;
        }
        public static DataObject LobbiesInfoResponse(string lobbiesJson)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.LobbiesInfoResponse;
            result.DataObjectInfo = new LobbiesInfo(lobbiesJson);
            return result;
        }
        public static DataObject UserJoinedToLobbyResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.UserJoinedToLobbyResponse;
            result.DataObjectInfo = new UserJoinedToLobby(userJson);
            return result;
        }
        public static DataObject UserLeavedFromLobbyResponse(string userJson)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.UserLeavedFromLobbyResponse;
            result.DataObjectInfo = new UserLeavedFromLobby(userJson);
            return result;
        }

        // UDP Data
        public static DataObject NewVideoFrame(Bitmap newFrame, string userId)
        {
            DataObject result = new DataObject();
            result.DataObjectType = DataObjectTypes.NewVideoFrame;
            result.DataObjectInfo = new NewVideoFrame(newFrame, userId);
            return result;
        }
    }
}
