using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using StarterAssets;

public class PlayerHealth : NetworkBehaviour
{
    private StarterAssetsInputs _input;

    public GameObject explosion;
    public Transform explosionPoint;

    private int _health;
    private TMP_Text _healthDisplay;
    private DateTime _lastHit;
    private bool _isKilled;

    // Start is called before the first frame update
    void Start()
    {
        _health = 100;
        _lastHit = DateTime.Now;
        var temp = GameObject.FindWithTag("HealthDisplay");
        _healthDisplay = temp.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //Input();
    }

    /*
    public void Input()
    {
        if(_input.kill)
        {
            _health = 0;
        }
        if (_health <= 0)
        {
            Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
            //PlayExplosionServerRpc(explosionPoint.position, Quaternion.identity);
            transform.position = spawnpoint.position;
            //_health = 100;
        }
    }
    */

    public void TakeDamage(int Damage)
    {
        if (IsOwner && (DateTime.Now - _lastHit).TotalSeconds > 0.1f)
        {
            _isKilled = false;
            _lastHit = DateTime.Now;
            _health -= Damage;
            _healthDisplay.text = _health.ToString() + " / 100";
            if (_health <= 0)
            {
                _isKilled = true;
                respawn();
                _health = 100;
            }
        }
    }

    private void respawn()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
        PlayExplosionServerRpc(explosionPoint.position, Quaternion.identity);
        transform.position = spawnpoint.position;
    }

   public bool CheckIfKilled()
   {
        if (_isKilled == true)
            return true;
        else
            return false;
   }

    
    [ServerRpc]
    private void PlayExplosionServerRpc(Vector3 position, Quaternion rotation)
    {
        try
        {
            GameObject currentExplosion = Instantiate(explosion, position, rotation);
            currentExplosion.transform.localScale = new Vector3(10, 10, 10);
            currentExplosion.GetComponent<NetworkObject>().Spawn();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }
}
