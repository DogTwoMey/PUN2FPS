using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class FightManager : MonoBehaviour
{
    private void Awake(){
        GameApp.uiManager.CloseAllUI();
        GameApp.uiManager.ShowUI<FightUI>("FightUI");
        Transform pointTransform = GameObject.Find("Point").transform;
        Vector3   position       = pointTransform.GetChild(Random.Range(0, pointTransform.childCount)).position;
        PhotonNetwork.Instantiate("Player", position, Quaternion.identity);
    }
}
