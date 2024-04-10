using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerUnitFire : NetworkBehaviour
{
    UnitFiring unitFiring;
    void Start()
    {
        unitFiring = GetComponent<UnitFiring>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetMouseButtonDown(0)) { unitFiring.SendFire(); }
        if (!Input.GetMouseButton(0)) { unitFiring.CancelFire(); }
    }
}
