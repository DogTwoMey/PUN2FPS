using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LossUI : MonoBehaviour{
    public Action onClickCallBack;

    private void Start(){
        transform.Find("resetBtn").GetComponent<Button>().onClick.AddListener(onResetButtonClick);
    }

    public void onResetButtonClick(){
        if (onClickCallBack != null){
            onClickCallBack();
        }

        GameApp.uiManager.CloseUI(gameObject.name);
    }
}