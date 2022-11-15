using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{ 
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private Camera playerCamera = null;

    [SerializeField] private float fireRate = 1f;

    private float lastFireTime = 0f;


    private void Update()
    {
        if (!hasAuthority) { return; }

        //Debug.Log(projectileSpawnPoint.rotation);

        if (Input.GetMouseButtonDown(0))
        {
            tryToFire(playerCamera.transform.rotation);
        }
    }

    [Command]
    private void tryToFire(Quaternion rot)
    {
        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, rot);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;

        }
    }
}
