using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using FishNet.Example.ColliderRollbacks;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine.InputSystem;
using System;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.InputSystem.DefaultInputActions;
using UnityEngine.InputSystem.EnhancedTouch;
using FishNet.Demo.AdditiveScenes;
using FishNet.Object.Synchronizing;

public class MPV_PlayerController : NetworkBehaviour
{
    [Header("Setting_Control")]
    [SerializeField]
    private bool Global = true;
    [SerializeField]
    private bool EnableMobileInput = true;

    [Header("Mobile Control")]
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private MPV_PlayerAction playerAction;
    private InputAction RunAction;
    private InputAction JumpAction;
    private InputAction CrouchAction;
    private InputAction CrawlAction;
    //private FixedJoystick ;

    [Header("Movement Value")]  
    private float NormalSpeed = 3f;
    [SerializeField]
    private float MoveSpeed = 3f;
    [SerializeField]
    private float RunSpeed = 4f;
    [SerializeField]
    private float SmoothRotationTime = 0.25f;

    private float CurentSpeed;   
    [SerializeField]
    private float CurentVeclocity;  
    [SerializeField]
    private float SpeedVeclocity;

    [SerializeField]
    private CapsuleCollider capsuleCollider;
    //private CharacterController capsuleCollider;


    [Header("Gravity")]
    [SerializeField]
    private float jumpForce;
    private Rigidbody rigidbody;
    private float grality = 10f;
    private float gralitymultiplier = 9.8f;
    private bool isJump = false;
    private bool isCrouch = false;
    private bool isCrawl = false;
    private bool isGround = true;


  

    [Header("Look At")]
    [SerializeField] 
    MPV_CameraControl control;
    [SerializeField]
    private Transform LookAt;
    private void Awake()
    {
        RunAction = playerInput.actions["Run"];
        JumpAction = playerInput.actions["Jump"];
        CrouchAction = playerInput.actions["Crouch"];
        CrawlAction = playerInput.actions["Crawl"];
        rigidbody = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        RunAction.performed += RunActivate;
        JumpAction.performed += JumpActivate;
        CrouchAction.performed += CrouchActivate;
        CrawlAction.performed += CrawlActivate;
    }
    private void OnDisable()
    {
        RunAction.performed -= RunActivate;
        JumpAction.performed -= JumpActivate;
        CrouchAction.performed -= CrouchActivate;
        CrawlAction.performed -= CrawlActivate;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            control = MPV_CameraControl.Instance;
            control.SetTarget(LookAt);
        }
        else
        {
            gameObject.GetComponent<MPV_PlayerController>().enabled = false;
        }
    }
    private void Update()
    {
        Vector2 input = Vector3.zero;
        if (EnableMobileInput)
        { input = playerInput.actions["Move"].ReadValue<Vector2>(); }
        else
        { input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); }
        Vector2 inputDirection = input.normalized;
        Debug.LogError(inputDirection.normalized);
        //------------------------------------------------------------------------------------------
        if (inputDirection != Vector2.zero)
        { 
            float rotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + control.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation,ref CurentVeclocity, SmoothRotationTime);
        }
        //-----------------------------------------------------------------------------------------
        if (RunAction.IsPressed() && !isCrawl && !isCrouch )
        {
            Debug.LogError("Run");
            MoveSpeed = RunSpeed;
        }
        else
        {
            Debug.LogError("Walk");
            MoveSpeed = NormalSpeed;
        }

        float tagetspeed = MoveSpeed * inputDirection.magnitude;
        CurentSpeed = Mathf.SmoothDamp(CurentSpeed, tagetspeed, ref SpeedVeclocity, 0.1f);

        if (Global)
        transform.Translate(transform.forward * CurentSpeed * Time.deltaTime, Space.World);
        else
        transform.Translate(transform.forward * CurentSpeed * Time.deltaTime, Space.Self);

        playerAction.Ground(isGround);

        if (!isGround)
        {
            //rigidbody.AddForce(-grality * gralitymultiplier * Vector3.up,ForceMode.Acceleration);
        }
        else
        { 
             if (inputDirection.magnitude > 0)
            {
                playerAction.Move(CurentSpeed);
            }
            else 
            {
                playerAction.Idle();
            }
        }

   
    }

    /// <summary>
    /// รับคำสั่งเพื่อให้ผู้เล่นวิ่ง
    /// </summary>
    /// <param name="context"></param>
    private void RunActivate(InputAction.CallbackContext context) 
    {
        float Value = context.ReadValue<float>();
        Debug.LogError("RunActivate");
        //Run = !Run; 
    }
    /// <summary>
    /// รับคำสั่งเพื่อให้ผู้เล่นกระโดด
    /// </summary>
    /// <param name="context"></param>
    private void JumpActivate(InputAction.CallbackContext context)
    {
        Debug.LogError("JumpActivate");
        if (context.performed && isGround && !isCrawl && !isCrouch )
        { 
            isJump = true; StartCoroutine( JumpFunction()); 
        }
        else { }
    }
    /// <summary>
    /// จัดการให้ผู้เล่นกระโดดโดยหน่วงเวลาAnimation
    /// </summary>
    /// <param name="context"></param>
    private IEnumerator JumpFunction()
    {
        if (isGround && isJump)
        { 
            isGround = false;
            playerAction.Jump();
            yield return new WaitForSeconds(0.5f);
            rigidbody.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
             
        }  
    }
    /// <summary>
    /// รับคำสั่งเพื่อให้ผู้เล่นย่อง
    /// </summary>
    /// <param name="context"></param>
    private void CrouchActivate(InputAction.CallbackContext context)
    {
        Debug.LogError("CrouchActivate");
        if (context.performed && isGround == true)
        {
            isCrouch = !isCrouch;
            isCrawl = false;
            playerAction.Crouch(isCrouch);
            playerAction.Crawl(isCrawl);

            if (isCrouch)
                Server_ChangeCollisionSize(this.gameObject,0.4f,0.86f, 1);
            else
                Server_ChangeCollisionSize(this.gameObject, 0.65f, 1.4f, 1);
        }
        else 
        { 
        
        }
    }
    /// <summary>
    /// รับคำสั่งเพื่อให้ผู้เล่นคลาน
    /// </summary>
    /// <param name="context"></param>
    private void CrawlActivate(InputAction.CallbackContext context)
    {
        Debug.LogError("CrouchActivate");
        if (context.performed && isGround)
        {
            isCrawl = !isCrawl;
            isCrouch = false;
            playerAction.Crawl(isCrawl);
            playerAction.Crouch(isCrouch);

            if (isCrawl)
                Server_ChangeCollisionSize(this.gameObject, 0.2f,1.4f,2);
            else
                Server_ChangeCollisionSize(this.gameObject, 0.65f,1.4f, 1);

        }
        else { }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gound")
        {
            isGround = true;
            Debug.LogError("OnTriggerEnter");
        }
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
            Debug.LogError("OnCollisionEnter");
        }
    }

    public void SetCollision(float y_center,float height,int direction)
    {
        capsuleCollider.height = height;
        capsuleCollider.center = new Vector3(0, y_center,0);
        capsuleCollider.direction = direction;
    }
    [ServerRpc]
    public void Server_ChangeCollisionSize( GameObject player, float y_center, float height, int direction)
    {
        Observers_ChangeCollisionSize(player,y_center, height, direction);
        // mPV_Player.SetCollision(y_center, height, direction);
    }
    [ObserversRpc]
    public void Observers_ChangeCollisionSize(GameObject player, float y_center, float height, int direction)
    {
        player.GetComponent<MPV_PlayerController>().capsuleCollider.height = height;
        player.GetComponent<MPV_PlayerController>().capsuleCollider.center = new Vector3(0, y_center, 0);
        player.GetComponent<MPV_PlayerController>().capsuleCollider.direction = direction;
    }
}
