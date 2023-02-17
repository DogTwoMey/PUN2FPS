using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomUI : MonoBehaviour,IInRoomCallbacks{
    private Transform      startTransform;
    private Transform      contentTransform;
    private GameObject     roomPrefab;
    public  List<RoomItem> RoomList;
    void Awake(){
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(onCloseButtonClick);
        startTransform = transform.Find("bg/startBtn");
        startTransform.GetComponent<Button>().onClick.AddListener(onStartButtonClick);
        contentTransform                     = transform.Find("bg/Content");
        roomPrefab                           = transform.Find("bg/roomItem").gameObject;
        RoomList                             = new List<RoomItem>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start(){
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; ++i){
            Player player=PhotonNetwork.PlayerList[i];
            CreateRoomItem(player);
        }
    }

    void OnEnable(){
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable(){
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void onStartButtonClick(){
        PhotonNetwork.LoadLevel("game");
    }
    void onCloseButtonClick(){
        PhotonNetwork.Disconnect();
        GameApp.uiManager.CloseUI(gameObject.name);
        GameApp.uiManager.ShowUI<LoginUI>("LoginUI");
    }

    public void CreateRoomItem(Player player){
        GameObject _gameObject = Instantiate(roomPrefab, contentTransform);
        _gameObject.SetActive(true);
        RoomItem roomItem = _gameObject.AddComponent<RoomItem>();
        roomItem.OwnerID = player.ActorNumber;
        roomItem.IsReady = false;
        RoomList.Add(roomItem);

        bool isReady;
        if (player.CustomProperties.TryGetValue("IsReady", out isReady)){
            roomItem.IsReady = isReady;
        }
    }

    public void RemoveRoomItem(Player player){
        RoomItem roomItem = RoomList.Find((RoomItem item) => { return player.ActorNumber == item.OwnerID; });
        if (roomItem != null){
            Destroy(roomItem.gameObject);
            RoomList.Remove(roomItem);
        }
    }
    public void OnPlayerEnteredRoom(Player newPlayer){
        CreateRoomItem(newPlayer);
    }

    public void OnPlayerLeftRoom(Player otherPlayer){
        RemoveRoomItem(otherPlayer);
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged){
        
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){
        RoomItem item = RoomList.Find((roomItem) => {return roomItem.OwnerID == targetPlayer.ActorNumber; });
        if (item){
            item.IsReady = (bool)changedProps["IsReady"];
            item.ChangeReadyButtonText(targetPlayer.IsLocal);
        }
        if (PhotonNetwork.IsMasterClient){
            bool isAllReady = true;
            foreach (var roomItem in RoomList){
                isAllReady = isAllReady && roomItem.IsReady;
            }
            startTransform.gameObject.SetActive(isAllReady);
        }
        
    }

    public void OnMasterClientSwitched(Player newMasterClient){
    }
}
