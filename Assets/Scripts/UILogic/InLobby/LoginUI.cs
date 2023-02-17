using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class LoginUI : MonoBehaviour,IConnectionCallbacks{
    private void Start(){
        transform.Find("startBtn").GetComponent<Button>().onClick.AddListener(onStartButtonClick);
        transform.Find("quitBtn").GetComponent<Button>().onClick.AddListener(onQuitButtonClick);
    }

    private void OnEnable(){
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable(){
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnConnected(){
        
    }

    public void OnConnectedToMaster(){
        GameApp.uiManager.CloseAllUI();
        // Debug.Log("连接成功");
        GameApp.uiManager.ShowUI<LobbyUI>("LobbyUI");
    }

    public void OnDisconnected(DisconnectCause cause){
        GameApp.uiManager.CloseUI("MaskUI");
    }

    public void OnRegionListReceived(RegionHandler regionHandler){
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data){
    }

    public void OnCustomAuthenticationFailed(string debugMessage){
        
    }
    public void onStartButtonClick(){
        GameApp.uiManager.ShowUI<MaskUI>("MaskUI").ShowMessage("正在连接服务器...");
        PhotonNetwork.ConnectUsingSettings();
        //Done to call OnConnectedToMaster
    }

    public void onQuitButtonClick(){
        Application.Quit();
    }
}
