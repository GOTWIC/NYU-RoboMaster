using UnityEngine;
using Mirror;

public class RoomPlayerScript : NetworkRoomPlayer
{
    //[SerializeField] private int teamSelection = 0;

    [SerializeField] private GameObject teamSelectionObject = null;

    //private Switch teamSelectionSwitch = null;


    void Start()
    {
        //teamSelectionObject = GameObject.Find("/UI/TeamSelection");
        //teamSelectionSwitch = teamSelectionObject.GetComponent(typeof(Switch)) as Switch;
    }

    private void Update()
    {
        //teamSelection = teamSelectionSwitch.getIndex();
    }
}
