using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;

public class PlayerColorNetwork : NetworkBehaviour
{
    public GameObject Body;
    public Color endColor;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        { }
        else gameObject.GetComponent<PlayerColorNetwork>().enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeColorServer(gameObject,endColor);
        }
    }
    [ServerRpc]
    public void ChangeColorServer(GameObject player, Color color) 
    {
        ChangeColor(player, color);
    }

    [ObserversRpc]
    public void ChangeColor(GameObject player, Color color)
    {
        player.GetComponent<PlayerColorNetwork>().Body.GetComponent<Renderer>().material.color = color;
    }
}
