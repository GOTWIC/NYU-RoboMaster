using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{ 
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;

    [SerializeField] private float fireRate = 1f;

    private float lastFireTime = 0f;


    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            tryToFire();
        }
    }

    [Command]
    private void tryToFire()
    {
        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            /*
            Quaternion projectileRotation = Quaternion.LookRotation(
                target.getTargetPoint().position - projectileSpawnPoint.position);
            projectileRotation = transform.rotation;
            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);
            */

            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            //projectileInstance.GetComponent<Rigidbody>().AddForce(transform.forward * 100);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;

        }
    }
}
