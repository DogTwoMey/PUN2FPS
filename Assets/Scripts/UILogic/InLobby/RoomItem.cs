using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class RoomItem : MonoBehaviour{
    public int  OwnerID;
    public bool IsReady;
    [SerializeField]
    private Transform ownerButtonTransform;
    private void Start(){
        IsReady              = false;
        ownerButtonTransform = transform.Find("Button");
        if (OwnerID == PhotonNetwork.LocalPlayer.ActorNumber){
            ownerButtonTransform.GetComponent<Button>().onClick.AddListener(OnReadyButtonClick);
        } 
        else{
            ownerButtonTransform.GetComponent<Image>().color=Color.black;
        }
    }

    public void OnReadyButtonClick(){
        IsReady = !IsReady;
        ExitGames.Client.Photon.Hashtable hashTable = new ExitGames.Client.Photon.Hashtable();
        hashTable.Add("IsReady",IsReady);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashTable);
    }

    public void ChangeReadyButtonText(bool isLocal){
        if (isLocal){
            ChangeLocalReadyButtonText();
        }
        else{
            ChangeOtherReadyButtonText();
        }
    }

    public void ChangeLocalReadyButtonText(){
        ownerButtonTransform.Find("Text").GetComponent<Text>().text = IsReady ? "取消准备" : "准备";
    }

    public void ChangeOtherReadyButtonText(){
        ownerButtonTransform.Find("Text").GetComponent<Text>().text = IsReady ? "已准备" : "未准备";
    }
}
