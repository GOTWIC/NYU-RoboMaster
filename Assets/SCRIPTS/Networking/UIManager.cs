using Mirror.Discovery;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject teamSelection = null;
    [SerializeField] private GameObject robotSelection = null;
    [SerializeField] private GameObject mapSelection = null;
    [SerializeField] private GameObject hostingJoining = null;
    [SerializeField] private GameObject playerName = null;
    [SerializeField] private GameObject background = null;
    [SerializeField] private GameObject logo = null;

    [SerializeField] public MyNetworkManager nwm;


    private static UIManager uiManagerInstance;

    private Scene sceneOnPrevFrame;


    private void Awake()
    {
        int numMusicPlayers = FindObjectsOfType<UIManager>().Length;
        //if (numMusicPlayers != 1) { Destroy(this.gameObject); }
        //else { DontDestroyOnLoad(gameObject); }
    }

    void Start()
    {
        MyNetworkDiscoveryHUD.enterRoom += goToRoom;
        MyNetworkDiscoveryHUD.enterServerFinder += goToServerFinder;
        goToHome();
    }

    private void Update()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (sceneOnPrevFrame == null) { sceneOnPrevFrame = scene; }

        if(scene.name != "Room")
        {
            goToMain();
        }

        // Check if scene changed, can change this to an event system later, linked to the room controls in the network manager
        if(scene.name == "Room" && (sceneOnPrevFrame.name == "Main" || sceneOnPrevFrame.name == "BobOmb"))
        {
            goToRoom();
        }

        sceneOnPrevFrame = scene;
    }

    public void goToRoom()
    {
        hostingJoining.SetActive(false);
        playerName.SetActive(false);
        teamSelection.SetActive(true);
        robotSelection.SetActive(true);
        background.SetActive(true);
        logo.SetActive(true);

        Debug.Log("1");

        if (nwm.nonNetworkedIsServer()) {
            Debug.Log("2");
           mapSelection.SetActive(true);
        }

    }

    public void goToServerFinder()
    {
        hostingJoining.SetActive(false);
        playerName.SetActive(false);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
        mapSelection.SetActive(false);
        background.SetActive(true);
        logo.SetActive(true);
    }

    
    public void goToHome()
    {
        hostingJoining.SetActive(true);
        playerName.SetActive(true);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
        mapSelection.SetActive(false);
        background.SetActive(true);
        logo.SetActive(true);
    }
    

    public void goToMain()
    {
        hostingJoining.SetActive(false);
        playerName.SetActive(false);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
        mapSelection.SetActive(false);
        background.SetActive(false);
        logo.SetActive(false);
    }
}
