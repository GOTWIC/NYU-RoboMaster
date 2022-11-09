using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    [SerializeField] float bulletSpeed = 100f;
    [SerializeField] private float destroyAfterSeconds = 20f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(destroySelf), destroyAfterSeconds);
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed * 100);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    [Command]
    private void destroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
