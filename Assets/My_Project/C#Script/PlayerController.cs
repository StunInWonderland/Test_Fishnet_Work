using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;

public class PlayerController : NetworkBehaviour
{
    // Start is called before the first frame update
    [Header("Parameter")]
    private float walkspeed=7.5f;
    private float runspeed=11.5f;
    private float jumpspeed=8;
    private float gravity = 20;
    private float lookspeed = 2;
    private float lookXlimit = 45;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    public float rotationX = 0;

    [HideInInspector]
    public bool canmove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playercamera;

    [Header("Animation")]
    private Animator animator;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playercamera = Camera.main;
            playercamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset,transform.position.z);
            playercamera.transform.SetParent(transform);
        }
        else 
        { 
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        bool isRuning = false;

        isRuning = Input.GetKey(KeyCode.LeftShift);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canmove? (isRuning? runspeed:walkspeed) * Input.GetAxis("Vertical"):0;
        float curSpeedY = canmove ? (isRuning ? runspeed : walkspeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canmove && characterController.isGrounded)
        {
            moveDirection.y = jumpspeed;
        }
        else 
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y-= gravity * Time.deltaTime;
        }
        characterController.Move(moveDirection * Time.deltaTime);

        if (canmove && playercamera != null) 
        {
            rotationX += Input.GetAxis("Mouse Y") * lookspeed;
            rotationX = Mathf.Clamp(rotationX,-lookXlimit, lookXlimit);
            playercamera.transform.localRotation = Quaternion.Euler(rotationX,0,0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X")*lookspeed, 0); ;
        }
    }
}
