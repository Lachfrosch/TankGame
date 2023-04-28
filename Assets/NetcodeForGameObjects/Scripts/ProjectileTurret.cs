using Unity.Netcode;
using TMPro;
using UnityEngine;
using System;
using StarterAssets;

public class ProjectileTurret : NetworkBehaviour
{
    
    //Bullet
    public GameObject bullet;

    //Bullet Force
    public float shootForce;

    //Gun
    public float timeBetweenShoooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    private int bulletsLeft, bulletsShot;
    private bool isShooting, readyToShoot, IsReloading;

    //For Debugging
    public bool allowInvoke = true;

    //References
    public Camera myCamera;
    public Transform attackPoint;

    //Graphics
    public GameObject muzzleFlash;
    public TMP_Text ammoDisplay;

    public LayerMask mask;

    private StarterAssetsInputs _input;

    // recoil
    private float _recoil = 10000.0f;

    // rigidbody
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //Fill Magazine
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        //Load Stuff that can't be set in the Prefab Designer
        myCamera = FindObjectOfType<Camera>();
        var temp = GameObject.FindWithTag("AmmoDisplay");
        ammoDisplay = temp.GetComponent<TMP_Text>();
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        MyInput();

        //Set AmmoDisplay, if it exists
        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    private void MyInput()
    {
        //Check if hold to fire is allowed
        if (allowButtonHold)
        {
            isShooting = _input.shooting;
        }
        else
        {
            isShooting = _input.shooting;
        }

        //Reloading
        if (_input.reaload && bulletsLeft < magazineSize && !IsReloading)
        {
            Reload();
        }
        if (readyToShoot && isShooting && !IsReloading && bulletsLeft <= 0)
        {
            Reload();
        }

        //Check if ready to fire
        if (readyToShoot && isShooting && !IsReloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }

    }

    private void Shoot()
    {
        readyToShoot = false;
        Ray bulletPath = myCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        //Chek if bullet hits something in its Path;
        Vector3 targetPoint;
        if (Physics.Raycast(bulletPath, out RaycastHit hit, mask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = bulletPath.GetPoint(100); //Just pick a Point 100 Units away;
        }

        SpawnBulletServerRpc(targetPoint, attackPoint.position, Quaternion.identity);

        // recoil
        Vector3 force = attackPoint.TransformDirection(Vector3.back);
        rb.AddForceAtPosition(force * _recoil, attackPoint.position, ForceMode.Impulse);

        bulletsLeft--;
        bulletsShot++;

        //Invoke ResetShot for a Cooldown between shots;
        if (allowInvoke)
        {
            Invoke(nameof(ResetShot), timeBetweenShoooting);
            allowInvoke = false;
        }

        //If more than one bullet is fired per Tap repeat function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke(nameof(Shoot), timeBetweenShots);
        }

        /*
        if (checkKill == true)
        {
            PlayerPoints.makePoints(50);
        }
        */
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        IsReloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        IsReloading = false;
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 targetPoint, Vector3 position, Quaternion rotation)
    {
        try
        {
            //Calculate Direction
            Vector3 directionOfBullet = targetPoint - position;

            //Calculate BulletSpread
            float x = UnityEngine.Random.Range(-spread, +spread);
            float y = UnityEngine.Random.Range(-spread, +spread);

            //Add Bulletspread
            Vector3 directionOfBulletWithSpread = directionOfBullet + new Vector3(x, y, 0);

            //Create Bullet
            GameObject currentBullet = Instantiate(bullet, position, rotation);
            currentBullet.GetComponent<NetworkObject>().Spawn();

            //Rotate it properly
            currentBullet.transform.forward = directionOfBulletWithSpread.normalized;

            //Add Force to Bullet
            currentBullet.GetComponent<Rigidbody>().AddForce(directionOfBulletWithSpread.normalized * shootForce, ForceMode.Impulse);

            //Create MuzzleFlash if it exists
            if (muzzleFlash != null)
            {
                GameObject currentMuzzleFlash = Instantiate(muzzleFlash, position, rotation);
                currentMuzzleFlash.transform.localScale = new Vector3(3, 3, 3);
                currentMuzzleFlash.GetComponent<NetworkObject>().Spawn();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /*
    bool checkKill()
    {
        if (isKilled == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    */

}
