using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public GameObject explosion;
    public Transform explosionPoint;
    public string Owner;

    private int _health;
    private TMP_Text _healthDisplay;
    private DateTime _lastHit;

    private LobbyController _lobbyController;

    // Start is called before the first frame update
    void Start()
    {
        _health = 100;
        _lastHit = DateTime.Now;
        var temp = GameObject.FindWithTag("HealthDisplay");
        _healthDisplay = temp.GetComponent<TMP_Text>();

        _lobbyController = GameObject.FindGameObjectWithTag("LobbyController").GetComponent<LobbyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetOwner(string owner)
    {
        Owner = owner;
    }

    public string getOwner()
    {
        return Owner;
    }


    public bool TakeDamage(int Damage)
    {
        if (IsOwner && (DateTime.Now - _lastHit).TotalSeconds > 0.1f)
        {
            _lastHit = DateTime.Now;
            _health -= Damage;
            UpdateHealthHud();
            if (_health <= 0)
            {
                Respawn();
                return true;
            }
        }
        return false;
    }

    private void UpdateHealthHud()
    {
        _healthDisplay.text = _health.ToString() + " / 100";
    }

    private void Respawn()
    {
        CallClientRpcServerRpc(AuthenticationService.Instance.PlayerId);
        _health = 100;
        UpdateHealthHud();
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        PlayExplosion(explosionPoint.position, Quaternion.identity);
        transform.position = spawnpoint.position;
        transform.rotation = spawnpoint.rotation;
    }

    [ServerRpc]
    private void CallClientRpcServerRpc(string Owner)
    {
        AddPlayerDeathClientRpc(Owner);
    }

    [ClientRpc]
    private void AddPlayerDeathClientRpc(string target)
    {
        _lobbyController.AddPlayerDeath(target);
    }

    private void PlayExplosion(Vector3 position, Quaternion rotation)
    {
        GameObject currentExplosion = Instantiate(explosion, position, rotation);
        currentExplosion.transform.localScale = new Vector3(10, 10, 10);
        currentExplosion.GetComponent<NetworkObject>().Spawn();
    }
    
}
