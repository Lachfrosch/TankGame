using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerPoints : NetworkBehaviour
{
    /*
    private int _points;
    private int _kills;
    private int _deaths;
    private TMP_Text _pointDisplay;

    // Start is called before the first frame update
    void Start()
    {
        _points = 0;
        var temp = GameObject.FindWithTag("PointDisplay");
        _pointDisplay = temp.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakePoints(int Points)
    {
        if (IsOwner)
        {
            _points += Points;
            _pointDisplay.text = _points.ToString();
        }
    }

    public void AddDeath()
    {
        if (IsOwner)
        {
            _deaths++;
        }
    }

    public void AddKills()
    {
        if (IsOwner)
        {
            _kills++;
        }
    }
    */
}
