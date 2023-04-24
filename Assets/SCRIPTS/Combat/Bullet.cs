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

    [SerializeField] GameObject Robot1;
    [SerializeField] GameObject Robot2;

    [SerializeField] List<Collider> ignoredColliders = new List<Collider>();

    [SerializeField] private float heatAccretion = 10;

    // Start is called before the first frame update

    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed * 100);
        if (!hasAuthority) { return; }
        Initialization();
    }

    [Command]
    void Initialization()
    {
        foreach (Collider col in ignoredColliders) { Physics.IgnoreCollision(col, GetComponent<Collider>()); }  
        Invoke(nameof(destroySelf), destroyAfterSeconds);
    }


    [ServerCallback]
    void OnCollisionEnter(Collision other)
    {
        // Check if bullet collided with an armor plate
        ArmorPlate armorPlate = other.collider.GetComponentInParent<ArmorPlate>();

        // Return if armor plate is null (didn't hit armor plate)
        if (!armorPlate) { return; }

        // Deal Damage
        armorPlate.dealDamage(damage);

        return;

    }

    [Command]
    private void destroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    public float getHeatAccretion()
    {
        return heatAccretion;
    }


    // Deprecated
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {        
        /*
        
        // IMPORTANT:  DEPENDING ON NESTED LEVEL OF ARMOR PLATES, THIS MIGHT HAVE TO CHANGE
        // For now, the hierarchy is:
        // Empty Gameobject Parent (ie base/robot object, has network identity) -->
        // Armor Plate Prefab (also an empty gameobject, has armor plate script) -->
        // Plate (has box collider)


        // Need to change how we check for valid damage updates, shifting from netid (ownership) to team-based system

        //NetworkIdentity otherNetID = other.GetComponentInParent<NetworkIdentity>();
        NetworkIdentity otherNetID = other.transform.parent.GetComponentInParent<NetworkIdentity>();

        // If other object doesn't have a network identity, return
        if (otherNetID == null) { return; }

        // If object we hit is owned by us, return
        if (otherNetID != null)
        {
            if (otherNetID.connectionToClient == connectionToClient) { return; }
        }

        Debug.Log(other.gameObject);

        //ArmorPlate armorPlate = other.GetComponentInParent<ArmorPlate>();

        // If object has a health, deal damage
        if (armorPlate != null)
        {
            //GameObject explosion = Instantiate(bulletImpact, transform.position, new Quaternion(0, 0, 1, 0) * transform.rotation);
            //NetworkServer.Spawn(explosion);
            Debug.Log("Hit Armor Plate Confirmed");
            armorPlate.dealDamage(damage);
            destroySelf();
        }
        else
        {
            Debug.Log("Hit something but unable to get armor plate");
        }


        // Object has no health
        destroySelf();

        */
    }
}
