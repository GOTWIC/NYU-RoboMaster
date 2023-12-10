using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayer : MonoBehaviour
{

    [SerializeField] GameObject robot;
    

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
        gameObject.transform.position = new Vector3 (robot.transform.position.x, 100, robot.transform.position.z);
        //gameObject.transform.localEulerAngles = new Vector3 (0, robot.transform.localEulerAngles.y, 0);
    }
}
