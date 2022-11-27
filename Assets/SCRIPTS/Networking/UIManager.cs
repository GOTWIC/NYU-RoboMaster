using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject teamSelection = null;
    [SerializeField] private GameObject robotSelection = null;
    [SerializeField] private GameObject hosting_joining = null;

    // Start is called before the first frame update
    void Start()
    {
        HostControlHud.enterRoom += goToRoom;
        HostControlHud.exitRoom += goToHome;
        hosting_joining.SetActive(true);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToRoom()
    {
        hosting_joining.SetActive(false);
        teamSelection.SetActive(true);
        robotSelection.SetActive(true);
    }

    public void goToHome()
    {
        hosting_joining.SetActive(true);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
    }
}
