using System.Collections.Generic;
using UnityEngine;

public class ArmorPlate : MonoBehaviour
{
    [SerializeField] private Health parentEntityHealth = null;
    [SerializeField] Collider selfColider;
    //[SerializeField] List<Collider> ignoredColliders = new List<Collider>();

    [SerializeField] Light light1 = null;
    [SerializeField] Light light2 = null;

    private void Start()
    {
        parentEntityHealth = gameObject.GetComponentInParent<Health>();
        //foreach (Collider col in ignoredColliders) { Physics.IgnoreCollision(col, selfColider); }

        if (parentEntityHealth.getTeam() == 1) { light1.color = Color.red; light2.color = Color.red; }
        else if (parentEntityHealth.getTeam() == 0) { light1.color = Color.blue; light2.color = Color.blue; }
        else { Debug.Log("Parent Entity does not have a team!"); };

        //Debug.Log(parentEntityHealth.gameObject.transform.parent + " " + parentEntityHealth.getTeam());
    }


    public void dealDamage(float damage)
    {
        parentEntityHealth.dealDamage(damage);
    }
}
