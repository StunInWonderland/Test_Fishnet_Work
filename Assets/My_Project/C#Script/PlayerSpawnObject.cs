using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using JetBrains.Annotations;

public class PlayerSpawnObject : NetworkBehaviour
{
    public GameObject ObjectToSpawn;
    [HideInInspector]public GameObject spawnObject;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        { 
             
        }
        else GetComponent<PlayerSpawnObject>().enabled = false;
    }

    private void Update()
    {
        if (spawnObject == null && Input.GetKeyDown(KeyCode.Alpha1) ) 
        {
            SpawnObject(ObjectToSpawn, transform,this);
        }
        if (spawnObject != null && Input.GetKeyDown(KeyCode.Alpha2))
        {
            DespawnObject(spawnObject);
        }
    }
    [ServerRpc]
    public void SpawnObject(GameObject obj,Transform player,PlayerSpawnObject script)
    {
        GameObject spawned = Instantiate(obj, player.position + player.forward,Quaternion.identity);
        ServerManager.Spawn(spawned);
        SetSpawnObject(spawned, script);
    }

    [ObserversRpc]
    public void SetSpawnObject(GameObject spawned, PlayerSpawnObject script) 
    {
        script.spawnObject = spawned;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnObject(GameObject spawned)
    {
        ServerManager.Despawn(spawned);
    }

}
