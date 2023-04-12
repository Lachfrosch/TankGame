using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    //Graphics
    public GameObject hitExplosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Create MuzzleFlash if it exists
        if (hitExplosion != null)
        {
            var currentMuzzleFlash = Instantiate(hitExplosion, collision.contacts[0].point, Quaternion.identity);
            currentMuzzleFlash.transform.localScale = new Vector3(3, 3, 3);
        }
        Destroy(gameObject);
    }
}
