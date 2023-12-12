using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlipDetector : MonoBehaviour
{
    public LayerMask layerMask;
    public Health playerHealth;
    public void OnCollisionStay(Collision collision)
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1, layerMask))
        {
            //playerHealth.dealDamage(1000);
        }
        Debug.DrawRay(transform.position, Vector3.down, Color.yellow);

    }//nouyuiyg
}
