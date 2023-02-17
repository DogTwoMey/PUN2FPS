using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviourPunCallbacks{
    private TypedLobby lobbyType;
    private Transform  contentTransform;
    private GameObject roomPrefab;
    void Start(){
        contentTransform = transform.Find("content/Scroll View/Viewport/Content");
        roomPrefab       = transform.Find("content/Scroll View/Viewport/item").gameObject;
        transform.Find("content/title/closeBtn").GetComponent<Button>()?.onClick.AddListener(onCloseButtonClick);
        transform.Find("content/createBtn").GetComponent<Button>().onClick.AddListener(onCreateButtonClick);
        transform.Find("content/updateBtn").GetComponent<Button>().onClick.AddListener(onUpdateButtonClick);
        lobbyType = new TypedLobby("FPSLobby",LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(lobbyType);
    }

    public override void OnJoinedLobby(){
        base.OnJoinedLobby();
        Debug.Log("进入大厅");
    }

    public void onCloseButtonClick(){
        PhotonNetwork.Disconnect();
        GameApp.uiManager.CloseUI(gameObject.name);
        GameApp.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    public void onCreateButtonClick(){
        GameApp.uiManager.ShowUI<CreateRoomUI>("CreateRoomUI");
    }

    public void onUpdateButtonClick(){
        GameApp.uiManager.ShowUI<MaskUI>("MaskUI").ShowMessage("正在刷新...");
        PhotonNetwork.GetCustomRoomList(lobbyType, "1");
    }

    private void ClearRoomList(){
        while (contentTransform.childCount != 0){
            DestroyImmediate(contentTransform.GetChild(0).gameObject);
        }
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        Debug.Log("RoomListCount:"+roomList.Count);
        GameApp.uiManager.CloseUI("MaskUI");
        Debug.Log("房间刷新完毕");
        ClearRoomList();
        for (int i = 0; i < roomList.Count; ++i){
            GameObject _gameObject = Instantiate(roomPrefab, contentTransform);
            _gameObject.SetActive(true);
            string roomName = roomList[i].Name;
            _gameObject.transform.Find("roomName").GetComponent<Text>().text = roomName;
            _gameObject.transform.Find("joinBtn").GetComponent<Button>().onClick.AddListener(delegate(){
                Debug.Log(roomName);
                GameApp.uiManager.ShowUI<MaskUI>("MaskUI").ShowMessage("正在加入房间...");
                PhotonNetwork.JoinRoom(roomName);
            });
        }
    }

    public override void OnJoinedRoom(){
        GameApp.uiManager.CloseAllUI();
        GameApp.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    public override void OnJoinRoomFailed(short returnCode, string message){
        GameApp.uiManager.CloseUI("MaskUI");
    }
}
