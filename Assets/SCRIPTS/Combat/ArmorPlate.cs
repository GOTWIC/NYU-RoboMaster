using System.Collections.Generic;
using UnityEngine;

public class ArmorPlate : MonoBehaviour
{
    [SerializeField] private Health parentEntityHealth = null;
    [SerializeField] Collider selfColider;
    [SerializeField] List<Collider> ignoredColliders = new List<Collider>();

    private void Start()
    {
        parentEntityHealth = gameObject.GetComponentInParent<Health>();
        foreach (Collider col in ignoredColliders) { Physics.IgnoreCollision(col, selfColider); }
    }


    public void dealDamage(float damage)
    {
        parentEntityHealth.dealDamage(damage);
    }
}
