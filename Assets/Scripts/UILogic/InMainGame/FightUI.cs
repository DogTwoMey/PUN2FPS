using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : MonoBehaviour{
    private Image bloodImage;

    private void Start(){
        bloodImage = transform.Find("blood").GetComponent<Image>();
    }

    public void UpdateBulletCount(int count){
        transform.Find("bullet/Text").GetComponent<Text>().text = count.ToString();
    }

    public void UpdateHP(float currentValue, float maxValue){
        transform.Find("hp/fill").GetComponent<Image>().fillAmount = currentValue / maxValue;
        transform.Find("hp/Text").GetComponent<Text>().text        = currentValue + "/" + maxValue;
    }

    public void UpdateBlood(){
        StopAllCoroutines();
        StartCoroutine(UpdateBloodCoroutine());
    }

    public IEnumerator UpdateBloodCoroutine(){
        bloodImage.color=Color.white;
        Color color = bloodImage.color;
        float timer = 0.25f;
        while (timer > 0){
            timer            -= Time.deltaTime;
            color.a          =  Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup));
            
            yield return null;
        }

        color.a          = 0;
        bloodImage.color = color;
    }
}