using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private int _health;
    private TMP_Text _healthDisplay;
    private DateTime _lastHit;

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

    }

    public void TakeDamage(int Damage)
    {
        if (IsOwner && (DateTime.Now - _lastHit).TotalSeconds > 0.1f)
        {
            _lastHit = DateTime.Now;
            _health -= Damage;
            _healthDisplay.text = _health.ToString() + " / 100";
            if (_health <= 0)
            {
                Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
                transform.position = spawnpoint.position;
                _health = 100;
            }
        }
    }
}
