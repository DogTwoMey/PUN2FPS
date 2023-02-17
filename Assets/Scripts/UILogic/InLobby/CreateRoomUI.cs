using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviourPunCallbacks{

    private InputField roomNameInput;

    void Start(){
        transform.Find("bg/title/closeBtn").GetComponent<Button>().onClick.AddListener(onCloseButtonClick);
        transform.Find("bg/okBtn").GetComponent<Button>().onClick.AddListener(onCreateButtonClick);
        roomNameInput      = transform.Find("bg/InputField").GetComponent<InputField>();
        roomNameInput.text = "room_" + Random.Range(1, 10000-1);
    }

    public void onCreateButtonClick(){
        GameApp.uiManager.ShowUI<MaskUI>("MaskUI").ShowMessage("创建中...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(roomNameInput.text,roomOptions);
    }
    public void onCloseButtonClick(){
        GameApp.uiManager.CloseUI(gameObject.name);
    }

    public override void OnCreatedRoom(){
        Debug.Log("创建成功");
        GameApp.uiManager.CloseAllUI();
        GameApp.uiManager.ShowUI<RoomUI>("RoomUI");
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        GameApp.uiManager.CloseUI("MaskUI");
    }
}
