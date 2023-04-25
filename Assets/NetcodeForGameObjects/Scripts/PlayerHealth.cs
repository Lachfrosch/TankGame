using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    private int _health;
    private TMP_Text _healthDisplay;

    // Start is called before the first frame update
    void Start()
    {
        _health = 100;
        var temp = GameObject.FindWithTag("HealthDisplay");
        _healthDisplay = temp.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int Damage)
    {
        if (IsOwner)
        {
            var test = gameObject.transform;
            _health -= Damage;
            _healthDisplay.text = _health.ToString() + " / 100";
            if (_health < 0)
            {
                //Player should die
            }
        }
    }
}
