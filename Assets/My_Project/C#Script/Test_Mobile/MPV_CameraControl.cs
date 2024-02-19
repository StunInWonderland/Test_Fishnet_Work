using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class MPV_CameraControl : MonoBehaviour
{
    public static MPV_CameraControl Instance;

    [Header("Mobile Control")]
    [SerializeField]
    private PlayerInput playerInput;


    [SerializeField]
    private float Y_axis;
    [SerializeField]
    private float X_axis;
    [SerializeField]
    private float RotationSwnsitivity = 8f;

    [SerializeField]
    private float ZoomValue=5;
    [SerializeField]
    private Transform Target;

    [SerializeField]
    private float RotationMin = -40;
    [SerializeField]
    private float RotationMax = 80;
    [SerializeField]
    private float SmoothTime = 0.9f;

    Vector3 tagetRotation;
    Vector3 CurrentVelocity;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
    }
    private void OnEnable()
    {
        playerInput.enabled = true;
    }
    private void OnDisable()
    {
        playerInput.enabled = false;
    }
    public void SetTarget(Transform Target)
    {
        this.Target = Target; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null)
        {
            //#if UNITY_ANDROID
            Y_axis += playerInput.actions["Camera"].ReadValue<Vector2>().normalized.x * RotationSwnsitivity;
            //X_axis -= playerInput.actions["Camera"].ReadValue<Vector2>().normalized.y * RotationSwnsitivity;

            /*Y_axis += playerInput.actions["MouseX"].ReadValue<float>() * RotationSwnsitivity;
            X_axis -= playerInput.actions["MouseY"].ReadValue<float>() * RotationSwnsitivity;*/
//#else
//          Y_axis += Input.GetAxis("Mouse X") * RotationSwnsitivity;
//          X_axis -= Input.GetAxis("Mouse Y") * RotationSwnsitivity;
//#endif
            X_axis = Mathf.Clamp(X_axis, RotationMin, RotationMax);

            tagetRotation = Vector3.SmoothDamp(tagetRotation, new Vector3(X_axis, Y_axis), ref CurrentVelocity, SmoothTime);
            transform.eulerAngles = tagetRotation;

            transform.position = Target.position - (transform.forward * ZoomValue);
        }
     
    }

}
