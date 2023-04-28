using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    PlayerPoints playerPoints;

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
        this.gameObject.SetActive(false);
        var hitTarget = collision.gameObject;
        if (hitTarget != null)
        {
            var playerHealth = hitTarget.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.gameObject == hitTarget)
            {
                playerHealth.TakeDamage(25);
                if (playerHealth.CheckIfKilled() == true)
                {
                    //var playerPoints = .GetComponent<PlayerPoints>();
                    playerPoints.MakePoints(50);
                }
            }
        }
        CreateExplosionAndDespawn(collision.contacts[0].point);
    }

    private void CreateExplosionAndDespawn(Vector3 location)
    {
        //Create MuzzleFlash if it exists
        if (hitExplosion != null)
        {
            GameObject currentMuzzleFlash = Instantiate(hitExplosion, location, Quaternion.identity);
            currentMuzzleFlash.transform.localScale = new Vector3(3, 3, 3);
        }

        Invoke(nameof(DeleteObject), 1f);
    }

    private void DeleteObject()
    {
        Destroy(gameObject);
    }
}
