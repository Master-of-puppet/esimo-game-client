using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using Electrotank.Electroserver5.Api;
using Electrotank.Electroserver5.Core;

public class DebugClientLog : MonoBehaviour
{

    ///// <summary>
    ///// Class xứ lý DEBUG_LOG
    ///// Key để tìm kiếm trong project, Find All project:
    ///// SAVE DEBUG_LOG
    ///// SAVE DEBUG_LOG REQUEST
    ///// REMOVE DEBUG_LOG
    ///// SAVE DEBUG_LOG TO FILE //for test, save ra file của PC khi ko muốn gửi lên server php
    ///// SEND DEBUG_LOG TO PHP //send file log lên server PHP
    ///// </summary>

    //private bool isSaveLogToFile = false;// biến đặt để save log vào file trên máy hay ko

    ///// <summary>
    ///// Kiểm tra nếu gặp lỗi UserNotJoinedToRoom, đã gửi log lên chưa 
    ///// Do lỗi UserNotJoinedToRoom bị lặp lại nhiều lần -> log bị gửi quá nhiều
    ///// </summary>
    //public bool isUserNotJoinedToRoom = false;


    //StringBuilder storeBuilder = new StringBuilder();
    //List<string> list = new List<string>(new string[] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", });
    //int currentIndex;//Chỉ số đang lưu hiện tại của list
    //float timeCountdown;
    //float time = 60;

    //void Start()
    //{
    //    timeCountdown = time;
    //    currentIndex = 0;
    //}

    //void Update()
    //{
    //    if (timeCountdown > 0)
    //    {
    //        timeCountdown -= Time.deltaTime;
    //    }
    //}

    //void SaveLogToStoreBuilder(string _value)
    //{
    //    storeBuilder.Append(_value);
    //    //Debug.LogWarning("Append to Store Builder: " + _value);    
    //    if (timeCountdown <= 0)
    //    {
    //        if (!String.IsNullOrEmpty(storeBuilder.ToString()))
    //        {
    //            //Debug.LogWarning("Store Builder: " + storeBuilder.ToString());               
    //            list[currentIndex] = storeBuilder.ToString();
    //        }
    //        storeBuilder.Remove(0, storeBuilder.Length);
    //        currentIndex++;
    //        if (currentIndex == list.Count) currentIndex = 0;
    //        timeCountdown = time;
    //    }
    //}

    //void SaveLogToCache()
    //{
    //    StoreGame.SaveString(StoreGame.EType.DEBUG_LOG, "");
    //    //Đưa Log còn lại ở StoreBuilder vào list
    //    currentIndex++;
    //    if (currentIndex == list.Count) currentIndex = 0;
    //    if (!String.IsNullOrEmpty(storeBuilder.ToString()))
    //        list[currentIndex] = storeBuilder.ToString();

    //    for (int i = currentIndex + 1; i < list.Count; i++)
    //    {
    //        StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, list[i].ToString());
    //        //Debug.LogWarning("List string push to cache: "+i+"_"+ list[i].ToString());
    //    }

    //    for (int i = 0; i < currentIndex + 1; i++)
    //    {
    //        StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, list[i].ToString());
    //        //Debug.LogWarning("List string push to cache: " + i + "_" + list[i].ToString());
    //    }
    //}

    //#region HoangVietDuc DEBUG_LOG Xử lý gửi debug log lên server PHP
    //public void SendDebugLog()
    //{
    //    string url = ServerWeb.URL_REQUEST_ERROR;
    //    string data = ConvertStringToJson(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));
    //    WWWForm form = new WWWForm();
    //    form.AddField("app_id", GameManager.GAME.ToString());
    //    form.AddField("game_version", GameSettings.CurrentVersion);
    //    form.AddField("user_id", GameManager.Instance.mInfo.id);
    //    form.AddField("username", GameManager.Instance.mInfo.username);
    //    form.AddField("scene", GameManager.CurrentScene.ToString());
    //    form.AddField("error", "");
    //    form.AddField("detail", "");
    //    form.AddField("environment", Common.GetDevice);
    //    form.AddField("debug_log", data);
    //    ServerWeb.StartThread(url, form, null);
    //    //if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG)==true)
    //    StoreGame.SaveString(StoreGame.EType.DEBUG_LOG, "");//Xóa hết log đi sau khi đã gửi
    //} 
    //public string ConvertStringToJson(string str)
    //{
    //    byte[] byteData = System.Text.Encoding.UTF8.GetBytes(str);
    //    string strBase64 = System.Convert.ToBase64String(byteData);
    //    System.Collections.Hashtable hash = new System.Collections.Hashtable();
    //    hash.Add("debug_log", strBase64);
    //    return JSON.JsonEncode(hash);
    //}
    //#endregion

    ////#region HoangVietDuc DEBUG_LOG Lưu lại các Request Client gửi lên server
    ////public void SaveRequest(EsMessage request)
    ////{
    ////    string store = "\n\n";
    ////    if (request is PluginRequest)
    ////    {
    ////        store += DateTime.Now.ToString() + " ";
    ////        //Debug.LogWarning("PluginRequest Server time : " + System.DateTime.Now.ToString() + " - " + ((PluginRequest)request).PluginName + " - param :" + ((PluginRequest)request).Parameters);
    ////        store += "[REQUEST PLUGIN]" + " [" + ((PluginRequest)request).PluginName + "]";
    ////        store += "\n RequestId: " + ((PluginRequest)request).RequestId +
    ////            "  __ServerId: " + ((PluginRequest)request).ServerId +
    ////            "  __SessionKey: " + ((PluginRequest)request).SessionKey +
    ////            "  __ZoneId: " + ((PluginRequest)request).ZoneId +
    ////            "  __RoomId: " + ((PluginRequest)request).RoomId +
    ////            "  __GetType: " + ((PluginRequest)request).GetType().Name;
    ////        store += "\n Parameters: " + ((PluginRequest)request).Parameters;
    ////    }
    ////    else
    ////    {
    ////        store += DateTime.Now.ToString() + " ";
    ////        store += "[REQUEST]" + " [" + request.GetType().Name + "]";
    ////        store += "\n RequestId: " + request.RequestId +
    ////           "  __ServerId: " + request.ServerId +
    ////           "  __MessageNumber: " + request.MessageNumber;
    ////    }

    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region Lưu các response mà server gửi về cho client

    ////#region LoginResponse
    ////public void SaveLogLoginResponse(LoginResponse es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[LoginResponse]";
    ////    store += "\n Successful: " + es.Successful + "  __Error: " + es.Error;
    ////    store += "\n EsObject: " + es.EsObject;
    ////    store += "\n UserVariables: " + es.UserVariables;
    ////    SaveLogToStoreBuilder(store);

    ////}
    ////#endregion

    ////#region ConnectionClosedEvent
    ////public void SaveLogConnectionClosedEvent(ConnectionClosedEvent es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[ConnectionClosedEvent]";
    ////    store += "\n ConnectionId: " + es.ConnectionId;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region ConnectionResponse
    ////public void SaveLogConnectionResponse(ConnectionResponse es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[ConnectionResponse]";
    ////    store += "\n Successful: " + es.Successful +
    ////        "  __Error: " + es.Error +
    ////        "  __ServerVersion: " + es.ServerVersion +
    ////        "  __MessageType: " + es.MessageType;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region ServerKickUserEvent
    ////public void SaveLogServerKickUserEvent(ServerKickUserEvent es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[ServerKickUserEvent]";
    ////    store += "\n Error: " + es.Error +
    ////        "  __MessageType: " + es.MessageType;
    ////    store += "\n EsObject: " + es.EsObject;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region GatewayKickUserRequest
    ////public void SaveLogGatewayKickUserRequest(GatewayKickUserRequest es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[GatewayKickUserRequest]";
    ////    store += "\n Error: " + es.Error +
    ////        "  __MessageType: " + es.MessageType +
    ////        "  __ClientId: " + es.ClientId;
    ////    store += "\n EsObject: " + es.EsObject;
    ////}
    ////#endregion

    ////#region GenericErrorResponse
    ////public void SaveLogGenericErrorResponse(GenericErrorResponse es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[GenericErrorResponse]";
    ////    store += "\n RequestMessageType: " + es.RequestMessageType + "  __ErrorType: " + es.ErrorType;
    ////    store += "\n ExtraData: " + es.ExtraData;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region PluginMessageEvent
    ////public void SaveLogPluginMessageEvent(PluginMessageEvent es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[PluginMessageEvent]";
    ////    store += "\n MessageType: " + es.MessageType;
    ////    store += "\n OriginZoneId: " + es.OriginZoneId + "  __OriginRoomId: " + es.OriginRoomId +
    ////        "__DestinationZoneId: " + es.DestinationZoneId + "  __DestinationRoomId: " + es.DestinationRoomId;
    ////    store += "\n PluginName: " + es.PluginName +
    ////        "  __RoomLevelPlugin: " + es.RoomLevelPlugin +
    ////        "  __SentToRoom: " + es.SentToRoom;
    ////    store += "\n Parameters: " + es.Parameters;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region JoinRoomEvent
    ////public void SaveLogJoinRoomEvent(JoinRoomEvent es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[JoinRoomEvent]";
    ////    store += "\n Capacity: " + es.Capacity +
    ////        "  __HasPassword: " + es.HasPassword +
    ////        "  __Hidden: " + es.Hidden;
    ////    store += "\n RoomId: " + es.RoomId + "  __RoomName: " + es.RoomName + "  __RoomDescription: " + es.RoomDescription +
    ////        "  __ZoneId: " + es.ZoneId;
    ////    store += "\n RoomVariables: " + es.RoomVariables;
    ////    store += "\n Users: " + es.Users;
    ////    store += "\n PluginHandles: " + es.PluginHandles;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region LeaveRoomEvent
    ////public void SaveLogLeaveRoomEvent(LeaveRoomEvent es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[LeaveRoomEvent]";
    ////    store += "\n MessageType: " + es.MessageType;
    ////    store += "\n RoomId: " + es.RoomId + "  __ZoneId: " + es.ZoneId;
    ////    store += "\n RequestId: " + es.RequestId;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region PublicMessageEvent
    ////public void SaveLogPublicMessageEvent(PublicMessageEvent es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[PublicMessageEvent]";
    ////    store += "\n MessageType: " + es.MessageType;
    ////    store += "\n RoomId: " + es.RoomId + "  __ZoneId: " + es.ZoneId;
    ////    store += "\n RequestId: " + es.RequestId;
    ////    store += "\n UserName: " + es.UserName;
    ////    store += "\n EsObject: " + es.EsObject;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region CreateOrJoinGameResponse
    ////public void SaveLogCreateOrJoinGameResponse(CreateOrJoinGameResponse es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[CreateOrJoinGameResponse]";
    ////    store += "\n Successful: " + es.Successful + "  __Error: " + es.Error;
    ////    store += "\n MessageType: " + es.MessageType;
    ////    store += "\n RoomId: " + es.RoomId + "  __ZoneId: " + es.ZoneId;
    ////    store += "\n RequestId: " + es.RequestId;
    ////    store += "\n GameId: " + es.GameId;
    ////    store += "\n GameDetails: " + es.GameDetails;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region AddBuddiesResponse
    ////public void SaveLogAddBuddiesResponse(AddBuddiesResponse es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[AddBuddiesResponse]";
    ////    store += "\n BuddiesAdded: " + es.BuddiesAdded + "  __BuddiesNotAdded: " + es.BuddiesNotAdded;
    ////    store += "\n MessageType: " + es.MessageType;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#region RemoveBuddiesResponse
    ////public void SaveLogRemoveBuddiesResponse(RemoveBuddiesResponse es)
    ////{
    ////    string store = "\n\n"; store += DateTime.Now.ToString() + " ";
    ////    store += "[RemoveBuddiesResponse]";
    ////    store += "\n BuddiesRemoved: " + es.BuddiesRemoved + "  __BuddiesNotRemoved: " + es.BuddiesNotRemoved;
    ////    store += "\n MessageType: " + es.MessageType;
    ////    SaveLogToStoreBuilder(store);
    ////}
    ////#endregion

    ////#endregion

    //public void checkEsConnection(ElectroServer es)
    //{
    //    if (GameManager.CurrentScene != ESceneName.LoginScreen)
    //    {
    //        if (!es.Engine.Connected)
    //        {
    //            StoreGame.SaveLog(StoreGame.EType.DEBUG_LOG, "=======================[Not Connected to ElectroServer]=======================");
    //            HandleDebugLog(false);//DEBUG_LOG 

    //        }
    //    }
    //}

    ///// <summary>
    ///// Xử lý kiểm tra khi nào save log vào file hoặc gửi lên php
    ///// </summary>
    ///// <param name="isLogin">Kiểm tra có phải thực hiện login, vì nếu lỗi xảy ra khi mất net, sẽ phải gửi log vào lần đăng nhập sau</param>
    //public void HandleDebugLog(bool isLogin)
    //{
    //    SaveLogToCache();//Mỗi lần trước khi push log sẽ xóa cache trước, nên ko cần xóa sau khi gửi nữa
    //    if (!Common.IsRelease && isSaveLogToFile==true)
    //    {
    //        if (isLogin == false)
    //        {
    //            if(StoreGame.Contains(StoreGame.EType.DEBUG_LOG))
    //                SaveAndLoadFile.Save(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));//SAVE DEBUG_LOG TO FILE
    //        }
    //        else
    //        {
    //            if (StoreGame.LoadString(StoreGame.EType.BOOL_SEND_LOG_TO_SERVER) == "true")
    //            {
    //                if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG))  
    //                    SaveAndLoadFile.Save(StoreGame.LoadString(StoreGame.EType.DEBUG_LOG));//SAVE DEBUG_LOG TO FILE
    //            }
    //        }
    //    }
    //    else
    //    {
    //        if (isLogin == false)
    //            if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG))
    //                SendDebugLog();//SEND DEBUG_LOG TO PHP
    //        else
    //        {
    //            if (StoreGame.LoadString(StoreGame.EType.BOOL_SEND_LOG_TO_SERVER) == "true")
    //            {
    //                if (StoreGame.Contains(StoreGame.EType.DEBUG_LOG))
    //                    SendDebugLog();
    //            }
    //        }


    //    }
    //}
}
