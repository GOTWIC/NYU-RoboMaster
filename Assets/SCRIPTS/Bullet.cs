using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class Bullet : NetworkBehaviour
{
    [SerializeField] float bulletSpeed = 100f;
    [SerializeField] private float destroyAfterSeconds = 20f;
    [SerializeField] private int damage = 5;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(destroySelf), destroyAfterSeconds);
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed * 100);
    }

    // Update is called once per frame
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit!");

        // IMPORTANT:  DEPENDING ON NESTED LEVEL OF ARMOR PLATES, THIS MIGHT HAVE TO CHANGE
        NetworkIdentity otherNetID = other.GetComponentInParent<NetworkIdentity>();

        
        // If object we hit is owned by us, return
        if (otherNetID != null)
        {
            if (otherNetID.connectionToClient == connectionToClient) { return; }
        }

        Health otherHealth = other.GetComponentInParent<Health>();

        // If object has a health, deal damage
        if (otherHealth != null)
        {
            //GameObject explosion = Instantiate(bulletImpact, transform.position, new Quaternion(0, 0, 1, 0) * transform.rotation);
            //NetworkServer.Spawn(explosion);
            otherHealth.dealDamage(damage);
            destroySelf();
        }

        // Object has no health
        destroySelf();
    }

    [Command]
    private void destroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
