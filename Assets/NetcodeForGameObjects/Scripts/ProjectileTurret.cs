using Unity.Netcode;
using TMPro;
using UnityEngine;
using Cinemachine;

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


    private void Awake()
    {
        //Fill Magazine
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Start()
    {
        //Load Stuff that can't be set in the Prefab Designer
        //var temp = FindObjectOfType<CinemachineVirtualCamera>();
        myCamera = FindObjectOfType<Camera>();
        //var test = 2;
        //ammoDisplay = GetComponentInParent<TMP_Text>();
    }

    private void Update()
    {
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
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        //Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !IsReloading)
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
        RaycastHit hit;

        //Chek if bullet hits something in its Path;
        Vector3 targetPoint;
        if (Physics.Raycast(bulletPath, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = bulletPath.GetPoint(100); //Just pick a Point 100 Units away;
        }

        //Calculate Direction
        Vector3 directionOfBullet = targetPoint - attackPoint.position;

        //Calculate BulletSpread
        float x = Random.Range(-spread, +spread);
        float y = Random.Range(-spread, +spread);

        //Add Bulletspread
        Vector3 directionOfBulletWithSpread = directionOfBullet + new Vector3(x, y, 0);

        //Create Bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        //Rotate it properly
        currentBullet.transform.forward = directionOfBulletWithSpread.normalized;

        //Add Force to Bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionOfBulletWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Create MuzzleFlash if it exists
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        //Invoke ResetShot for a Cooldown between shots;
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShoooting);
            allowInvoke = false;
        }

        //If more than one bullet is fired per Tap repeat function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        IsReloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        IsReloading = false;
    }
}
