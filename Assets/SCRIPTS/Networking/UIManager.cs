using Mirror.Discovery;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject teamSelection = null;
    [SerializeField] private GameObject robotSelection = null;
    [SerializeField] private GameObject hostingJoining = null;
    [SerializeField] private GameObject playerName = null;
    [SerializeField] private GameObject background = null;


    private static UIManager uiManagerInstance;

    private Scene sceneOnPrevFrame;


    private void Awake()
    {
        int numMusicPlayers = FindObjectsOfType<UIManager>().Length;
        if (numMusicPlayers != 1) { Destroy(this.gameObject); }
        else { DontDestroyOnLoad(gameObject); }
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

        if(scene.name == "Main")
        {
            goToMain();
        }

        // Check if scene changed, can change this to an event system later, linked to the room controls in the network manager
        if(scene.name == "Room" && sceneOnPrevFrame.name == "Main")
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
    }

    public void goToServerFinder()
    {
        hostingJoining.SetActive(false);
        playerName.SetActive(false);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
        background.SetActive(true);
    }

    
    public void goToHome()
    {
        hostingJoining.SetActive(true);
        playerName.SetActive(true);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
        background.SetActive(true);
    }
    

    public void goToMain()
    {
        hostingJoining.SetActive(false);
        playerName.SetActive(false);
        teamSelection.SetActive(false);
        robotSelection.SetActive(false);
        background.SetActive(false);
    }
}
