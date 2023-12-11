using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BridgeVisibility : MonoBehaviour
{
    // Create list of GameObjects
    [SerializeField] private List<Transform> bridges = new List<Transform>();

    List<GameObject> currentCollisions = new List<GameObject>();

    [SerializeField] public PlayerMovement playerMovement;

    [SerializeField] public float topSpeedMultiplier = 1.3f;
    [SerializeField] public float accelMultiplier = 1.5f;



    private void Start()
    {
        GameObject bridgeParent = GameObject.Find("Bridges");
        Debug.Log(bridgeParent);
        foreach (Transform bridge in bridgeParent.transform) {
            Debug.Log(bridge);
            if (bridge.tag == "Bridge"){
                bridges.Add(bridge);
            }
        }
    }

    // Update is called once per frame
    void Update() {

        bool onBridgeFlag = false;

        foreach (Transform bridge in bridges) {
            if (onBridge(bridge)) {
                foreach(Transform child in bridge) {
                    child.gameObject.GetComponent<MeshRenderer>().enabled = true;
                }

                onBridgeFlag = true;
            }

            else {
                foreach (Transform child in bridge) {
                    child.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }

        if (onBridgeFlag) {
            playerMovement.setTopSpeedMulti(topSpeedMultiplier);
            playerMovement.setAccelMulti(accelMultiplier);
        }

        else
        {
            playerMovement.setTopSpeedMulti(1f);
            playerMovement.setAccelMulti(1f);
        }
    }

    private bool onBridge(Transform bridge)
    {
        foreach (Transform child in bridge)
        {
            if (currentCollisions.Contains(child.gameObject)) {  return true; }
        }

        return false;
    }

    void OnCollisionEnter(Collision col)
    {
        currentCollisions.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        currentCollisions.Remove(col.gameObject);
    }
}
