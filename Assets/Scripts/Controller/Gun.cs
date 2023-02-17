using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour{
#region NumericValue

    public readonly int   BulletMaxCount = 30;
    public          int   BulletCount    = 30;
    public          float BulletSpeed    = 100f;
    public          int   AttackPower    = 4;

#endregion

#region Prefabs

    public GameObject BulletPrefab;
    public GameObject CastingPrefab;

#endregion

#region Transform

    public Transform BulletTransform;
    public Transform CastingTransform;

#endregion

    private void Start(){
        BulletPrefab     = Resources.Load<GameObject>("Bullet_Prefab");
        CastingPrefab    = Resources.Load<GameObject>("Big_Casing_Prefab");
        BulletTransform  = transform.Find("Bullet_Spawnpoint");
        CastingTransform = transform.Find("Casing_Spawnpoint");
    }

    public void Attack(Transform targetTransform){
        GameObject bulletObject = Instantiate(BulletPrefab, BulletTransform.position, Quaternion.identity);
        bulletObject.GetComponent<Rigidbody>().AddForce(targetTransform?(targetTransform.position - BulletTransform.position):transform.forward * BulletSpeed, ForceMode.Impulse);
        GameObject castingObject = Instantiate(CastingPrefab, CastingTransform.position, Quaternion.identity);
    }

    public void Attack(){
        GameObject bulletObject = Instantiate(BulletPrefab, BulletTransform.position, Quaternion.identity);
        bulletObject.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed, ForceMode.Impulse);
        GameObject castingObject = Instantiate(CastingPrefab, CastingTransform.position, Quaternion.identity);
    }
}