using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using TMPro;

public class UnitFiring : NetworkBehaviour
{ 
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private TMP_Text ammoCount = null;

    // Firing
    [SyncVar] bool firePressed = false;
    [SerializeField] private float fireRate = 1f;
    [SerializeField][SyncVar] public int ammo = 200;
    private float lastFireTime = 0f;
    private float ammoCountDisplay; // This is basically the same as ammo, but it updates with lag to create a cool effect
    [SerializeField] string fireMode = "";


    // Heat System
    [SerializeField] private float heatDissipationRate = 0.01f;
    [SerializeField][SyncVar] private float heatAccretion = 0;
    [SerializeField][SyncVar] public float heatThreshold = 100f;
    [SerializeField] public Image heatAccretionImage = null;

    // Misc
    [SerializeField] Health health = null;
    


    private void Start()
    {
        ammoCountDisplay = ammo;

        if (!hasAuthority) { disableUI(); }
    }

    private void disableUI()
    {
        // Disable the heat bar
        heatAccretionImage.transform.parent.gameObject.SetActive(false);

        // Disable the ammo count
        ammoCount.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!hasAuthority) { return; }
        
        if (firePressed && fireMode == "auto") { tryToFire(playerCamera.transform.rotation); }
        if (firePressed && fireMode == "semi") { tryToFire(playerCamera.transform.rotation); firePressed = false; }

        dissipateHeat();

        updateAmmoCount();

        heatAccretionImage.fillAmount = Math.Min(heatAccretion / heatThreshold, 1f);
    }

    private void updateAmmoCount()
    {
        // Change ammo count display gradually
        if (ammoCountDisplay != ammo)
        {
            if (ammoCountDisplay < ammo) { ammoCountDisplay += 0.8f; }
            else if (ammoCountDisplay - 0.8f >= ammo) { ammoCountDisplay -= 0.8f; } // To prevent glitches
            ammoCount.text = ((int)ammoCountDisplay).ToString(); // Intentionally cast to int
        }

    }

    [Command]
    private void dissipateHeat()
    {
        heatAccretion = Math.Max(0,heatAccretion-(heatDissipationRate * Time.deltaTime));
    }

    [Command]
    private void tryToFire(Quaternion rot)
    {
        // Cannot fire if the robot is dead
        if (health.inDeathState()) { return; }
        
        // Cannot fire if the robot is overheated or if shooting surpasses heat threshold
        if (heatAccretion + projectilePrefab.GetComponent<Bullet>().getHeatAccretion() > heatThreshold) { return; }

        // Cannot fire if the robot is out of ammo
        if (ammo <= 0) { return; }

        // Add Logic where robot has to go to the reload zone to unlock the gun after respawning

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            ammo--;
            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, rot);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            heatAccretion += projectilePrefab.GetComponent<Bullet>().getHeatAccretion();

            lastFireTime = Time.time;
        }
    }

    public void addAmmo(int refill) { ammo += refill; }


    [Command]
    public void resetFiringSystem()
    {
        ammo = 200;
        heatAccretion = 0;
    }

    [Command]
    public void SendFire()
    {
        firePressed = true;
    }
    [Command]
    public void CancelFire()
    {
        firePressed = false;
    }
}
