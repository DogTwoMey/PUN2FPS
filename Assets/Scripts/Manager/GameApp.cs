using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameApp : MonoBehaviour{
    //TODO:Update to singleton
    public static UIManager uiManager;
    public static bool      isLoaded;

    private void Awake(){
        if (isLoaded){
            Destroy(gameObject);
        }
        else{
            isLoaded = true;
            DontDestroyOnLoad(gameObject);
            uiManager = new UIManager();
            uiManager.Init();
            PhotonNetwork.SendRate          = 50;
            PhotonNetwork.SerializationRate = 50;
        }

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start(){
        uiManager.ShowUI<LoginUI>("LoginUI");
    }
}