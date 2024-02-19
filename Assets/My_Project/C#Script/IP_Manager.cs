using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting.Tugboat;
using TMPro;

public class IP_Manager : MonoBehaviour
{
    public Tugboat _tugboat;
    public TMP_InputField _inputField;
    
    // Start is called before the first frame update
    void Start()
    {
       //_tugboat.SetClientAddress("");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetClientIP() 
    {
        _tugboat?.SetClientAddress(_inputField.text);
    }

}
