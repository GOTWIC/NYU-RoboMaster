using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject teamSelection = null;
    [SerializeField] private GameObject robotSelection = null;
    [SerializeField] private GameObject hostingJoining = null;
    [SerializeField] private GameObject playerName = null;


    private static UIManager uiManagerInstance;


    private void Awake()
    {
        int numMusicPlayers = FindObjectsOfType<UIManager>().Length;
        if (numMusicPlayers != 1) { Destroy(this.gameObject); }
        else { DontDestroyOnLoad(gameObject); } 
    }

    void Start()
    {
        HostControlHud.enterRoom += goToRoom;
        HostControlHud.exitRoom += goToHome;
        hostingJoining.SetActive(true);
        playerName.SetActive(true);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);

    }

    public void goToRoom()
    {
        hostingJoining.SetActive(false);
        playerName.SetActive(false);
        teamSelection.SetActive(true);
        robotSelection.SetActive(true);
    }

    public void goToHome()
    {
        hostingJoining.SetActive(true);
        playerName.SetActive(true);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
    }
}
