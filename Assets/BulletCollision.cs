using System;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class BulletCollision : NetworkBehaviour
{
    PlayerPoints playerPoints;
    private NetworkVariable<FixedString64Bytes> _Owner = new NetworkVariable<FixedString64Bytes>();
    private LobbyController _lobbyController;

    //Graphics
    public GameObject hitExplosion;

    // Start is called before the first frame update
    void Start()
    {
        _lobbyController = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOwner(string playerID)
    {
        _Owner.Value = playerID;
        Debug.Log(_Owner);
    }

    public string GetOwner()
    {
        return _Owner.Value.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.gameObject.SetActive(false);
        CreateExplosionAndDespawn(collision.contacts[0].point);
    }

    private void CreateExplosionAndDespawn(Vector3 location)
    {
        //Create MuzzleFlash if it exists
        if (hitExplosion != null)
        {
            GameObject currentMuzzleFlash = Instantiate(hitExplosion, location, Quaternion.identity);
            currentMuzzleFlash.transform.localScale = new Vector3(3, 3, 3);
            currentMuzzleFlash.GetComponent<NetworkObject>().Spawn();
        }

        Invoke(nameof(DeleteObject), 1f);
    }

    private void DeleteObject()
    {
        if (IsServer)
        {
            gameObject.GetComponent<NetworkObject>().Despawn();
            GameObject.Destroy(this.gameObject);
        }
    }
}
