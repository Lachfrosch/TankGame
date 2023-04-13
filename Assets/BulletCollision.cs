using Unity.Netcode;
using UnityEngine;

public class BulletCollision : NetworkBehaviour
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
        CreateExplosionAndDespawnServerRpc(collision.contacts[0].point);
    }

    [ServerRpc]
    private void CreateExplosionAndDespawnServerRpc(Vector3 location)
    {
        //Create MuzzleFlash if it exists
        if (hitExplosion != null)
        {
            GameObject currentMuzzleFlash = Instantiate(hitExplosion, location, Quaternion.identity);
            currentMuzzleFlash.transform.localScale = new Vector3(3, 3, 3);
            currentMuzzleFlash.GetComponent<NetworkObject>().Spawn();
        }
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
