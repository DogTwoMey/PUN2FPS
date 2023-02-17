using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun, IPunObservable{
#region UnityComponent

    private Animator  animator;
    private Rigidbody rigidbody;
    private Transform cameraTransform;

#endregion

#region NumericValue

    public int   CurrentHP;
    public int   MaxHP;
    public float MoveSpeed;

#endregion

#region KeyboardMoveInput

    public float   H;
    public float   V;
    public Vector3 MoveDirection;

#endregion

#region MouseMoveInput

    public float MouseX;
    public float MouseY;
    public float Scroll;
    public float AngleX;
    public float AngleY;

#endregion

#region ComponentNumericValue

    public Vector3    CameraPlayerOffset;
    public Quaternion CameraRotation;

#endregion

#region WeaponData

    public Gun gun;

#endregion

#region AudioClip

    public AudioClip ReloadAudioClip;
    public AudioClip ShootAudioClip;

#endregion

#region State

    public bool       IsDead;
    public Vector3    currentPosition;
    public Quaternion currentQuaternion;

#endregion

    private void Start(){
        animator           = GetComponent<Animator>();
        rigidbody          = GetComponent<Rigidbody>();
        cameraTransform    = Camera.main.transform;
        CurrentHP          = 100;
        MaxHP              = 100;
        MoveSpeed          = 35f;
        CameraPlayerOffset = new Vector3(2f, 2.5f, -2f);
        AngleX             = transform.eulerAngles.x;
        AngleY             = transform.eulerAngles.y;
        Cursor.lockState   = CursorLockMode.Locked;
        gun                = GetComponentInChildren<Gun>();
        ReloadAudioClip    = Resources.Load<AudioClip>("assault_rifle_02_reload_ammo_left");
        ShootAudioClip     = Resources.Load<AudioClip>("shoot");
        IsDead             = false;
        currentPosition    = transform.position;
        currentQuaternion  = transform.rotation;
        if (photonView.IsMine){
            GameApp.uiManager.GetUI<FightUI>().UpdateHP(CurrentHP, MaxHP);
        }
    }

    private void Update(){
        if (photonView.IsMine){
            UpdatePosition();
            UpdateRotation();
            InputControl();
        }
        else{
            UpdateOtherPlayer();
        }
    }

    private void LateUpdate(){
        animator.SetFloat("Horizontal", H);
        animator.SetFloat("Vertical",   V);
        animator.SetBool("isDie", IsDead);
    }

    private void UpdatePosition(){
        H             = Input.GetAxisRaw("Horizontal");
        V             = Input.GetAxisRaw("Vertical");
        MoveDirection = (cameraTransform.forward * V + cameraTransform.right * H).normalized;
        rigidbody.MovePosition(transform.position    + MoveDirection         * Time.deltaTime * MoveSpeed);
    }

    private void UpdateRotation(){
        MouseX                   =  Input.GetAxis("Mouse X");
        MouseY                   =  Input.GetAxis("Mouse Y");
        Scroll                   =  Input.GetAxis("Mouse ScrollWheel");
        AngleX                   -= MouseY;
        AngleY                   += MouseX;
        AngleX                   =  ClampAngle(AngleX, -60,  60);
        AngleY                   =  ClampAngle(AngleY, -360, 360);
        CameraRotation           =  Quaternion.Euler(AngleX, AngleY, 0);
        CameraPlayerOffset.z     += Scroll;
        CameraPlayerOffset.z     =  Mathf.Clamp(CameraPlayerOffset.z, -4, 1);
        cameraTransform.rotation =  CameraRotation;
        cameraTransform.position =  transform.position + cameraTransform.rotation * CameraPlayerOffset;
        transform.eulerAngles    =  new Vector3(0, cameraTransform.eulerAngles.y, 0);
    }

    private float ClampAngle(float val, float min, float max){
        while (val > 360){
            val -= 360;
        }

        while (val < -360){
            val += 360;
        }

        return Mathf.Clamp(val, min, max);
    }

    private void OnAnimatorIK(){
        if (animator){
            Vector3 angle = animator.GetBoneTransform(HumanBodyBones.Chest).localEulerAngles;
            angle.x = AngleX;
            animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Euler(angle));
        }
    }

    public void InputControl(){
        if (Input.GetMouseButtonDown(0)){
            if (animator.GetCurrentAnimatorStateInfo(1).IsName("Reload")){
                return;
            }

            if (gun.BulletCount > 0){
                gun.BulletCount--;
                GameApp.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);
                animator.Play("Fire", 1, 0);
                StopAllCoroutines();
                StartCoroutine(AttackCoroutine());
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && gun.BulletCount < gun.BulletMaxCount){
            AudioSource.PlayClipAtPoint(ReloadAudioClip, transform.position);
            animator.Play("Reload");
            gun.BulletCount = gun.BulletMaxCount;
            GameApp.uiManager.GetUI<FightUI>("FightUI").UpdateBulletCount(gun.BulletCount);
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return)){
            GetHit();
        }
#endif
    }

    public IEnumerator AttackCoroutine(){
        yield return new WaitForSeconds(0.05f);
        AudioSource.PlayClipAtPoint(ShootAudioClip, transform.position);
        Ray        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("Player"))){
            Debug.Log("命中");
            hit.transform?.GetComponent<PlayerController>()?.GetHit();
        }

        Debug.DrawLine(transform.position, hit.point, Color.red);
        photonView.RPC("AttackRPC", RpcTarget.All);
    }


    public void GetHit(){
        if (IsDead){
            return;
        }

        photonView.RPC("GetHitRPC", RpcTarget.All);
    }

    public void Reset(){
        Cursor.lockState = CursorLockMode.Locked;
        photonView.RPC("ResetPRC", RpcTarget.All);
    }

    public void GameOver(){
        GameApp.uiManager.ShowUI<LossUI>("LossUI");
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if (stream.IsWriting){
            stream.SendNext(H);
            stream.SendNext(V);
            stream.SendNext(AngleX);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }

        if (stream.IsReading){
            H                 = (float)stream.ReceiveNext();
            V                 = (float)stream.ReceiveNext();
            AngleX            = (float)stream.ReceiveNext();
            currentPosition   = (Vector3)stream.ReceiveNext();
            currentQuaternion = (Quaternion)stream.ReceiveNext();
        }
    }

    public void UpdateOtherPlayer(){
        transform.position = Vector3.Lerp(transform.position, currentPosition, Time.deltaTime       * MoveSpeed * 10);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentQuaternion, Time.deltaTime * 500);
    }

    [PunRPC]
    public void AttackRPC(){
        gun.Attack();
    }

    [PunRPC]
    public void GetHitRPC(){
        CurrentHP -= gun.AttackPower;
        if (CurrentHP <= 0){
            CurrentHP = 0;
            IsDead    = true;
        }

        if (photonView.IsMine){
            GameApp.uiManager.GetUI<FightUI>("FightUI").UpdateHP(CurrentHP, MaxHP);
            GameApp.uiManager.GetUI<FightUI>("FightUI").UpdateBlood();
            if (CurrentHP <= 0){
                Invoke("GameOver", 3);
            }
        }
    }

    [PunRPC]
    public void ResetPRC(){
        IsDead    = false;
        CurrentHP = MaxHP;
        if (photonView.IsMine){
            GameApp.uiManager.GetUI<FightUI>().UpdateHP(CurrentHP, MaxHP);
        }
    }
}