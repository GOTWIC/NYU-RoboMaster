using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RobotCustomization : NetworkBehaviour
{
    [SerializeField] Robot robot = null;
    [SerializeField] List<Light> lights = new List<Light>();
    [SerializeField] GameObject Shield = null;
    [SerializeField] Material redShield = null;
    [SerializeField] Material blueShield = null;

    private float team = -1;


    private void Start()
    {
        team = robot.getTeam();

        if (team == 1) {
            for (int i = 0; i < lights.Count; i++) { lights[i].color = Color.red; }
            Shield.GetComponent<Renderer>().material = redShield;
        }
        else if (team == 0) {
            for (int i = 0; i < lights.Count; i++) { lights[i].color = Color.blue; }
            Shield.GetComponent<Renderer>().material = blueShield;
        }
    }
}
