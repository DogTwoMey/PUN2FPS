using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MaskUI : MonoBehaviour
{
    public void ShowMessage(string message){
        transform.Find("msg/bg/Text").GetComponent<Text>().text = message;
    }
}
