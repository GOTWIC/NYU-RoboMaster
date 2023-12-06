using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using System.Collections.Generic;

public class RefereeSystem : NetworkBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float matchTimeMin = 2;
    [SerializeField] private float EoRDuration = 2;

    [Header("Red")]
    [SerializeField] private Health redBaseHealth = null;
    [SerializeField] private Image redBaseHealthBarImage = null;

    //[SerializeField] private Image redRobot1HealthBarImage = null;
    //[SerializeField] private Image redRobot2HealthBarImage = null;
    //[SerializeField] private Health redRobot1Health = null;
    //[SerializeField] private Health redRobot2Health = null;


    [SerializeField] private List<Image> redRobotHealthImgs = new List<Image>();
    [SerializeField] private List<Health> redRobotHealth = new List<Health>();

    [SerializeField] private TMP_Text redScoreText = null;
    [SerializeField] private GameObject redHealthGroup = null;
    [SerializeField] private GameObject RedRobotHealthUI = null;



    [Header("Blue")]
    [SerializeField] private Health blueBaseHealth = null;
    [SerializeField] private Image blueBaseHealthBarImage = null;

    //[SerializeField] private Image blueRobot1HealthBarImage = null;
    //[SerializeField] private Image blueRobot2HealthBarImage = null;
    //[SerializeField] private Health blueRobot1Health = null;
    //[SerializeField] private Health blueRobot2Health = null;

    [SerializeField] private List<Image> blueRobotHealthImgs = new List<Image>();
    [SerializeField] private List<Health> blueRobotHealth = new List<Health>();

    [SerializeField] private TMP_Text blueScoreText = null;
    [SerializeField] private GameObject blueHealthGroup = null;
    [SerializeField] private GameObject BlueRobotHealthUI = null;

    [Header("Misc")]
    [SerializeField] private GameObject roundEnd = null;
    [SerializeField] private GameObject gameEnd = null;
    [SerializeField] private GameObject pause = null;
    [SerializeField] private TMP_Text resultText = null;
    [SerializeField] private TMP_Text matchTimer = null;

    private MyNetworkManager networkManager = null;

    [SyncVar] private bool transitioning = false;
    [SyncVar] public bool paused = false;
    private string buffer = "";

    [SyncVar] public int redScore = 0;
    [SyncVar] public int blueScore = 0;

    private float matchStartTime = 0f;

    private void Start()
    {
        roundEnd.SetActive(false);  
        gameEnd.SetActive(false);
        pause.SetActive(false);
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        matchStartTime = Time.fixedTime;
    }


    // Update is called once per frame
    void Update()
    {
        blueScoreText.text = blueScore.ToString();
        redScoreText.text = redScore.ToString();

        

        if (paused) { 
            Time.timeScale = 0;
            pause.SetActive(true);
        }
        else { 
            Time.timeScale = 1;
            pause.SetActive(false);
        }


        if (transitioning) { return; }


        redBaseHealthBarImage.rectTransform.sizeDelta = new Vector2(840 * redBaseHealth.getCurrentHealth() / redBaseHealth.getMaxHealth(), 84);
        blueBaseHealthBarImage.rectTransform.sizeDelta = new Vector2(840 * blueBaseHealth.getCurrentHealth() / blueBaseHealth.getMaxHealth(), 84);
        
        //if (redRobot1Health != null) redRobot1HealthBarImage.rectTransform.sizeDelta = new Vector3(840 * redRobot1Health.getCurrentHealth() / redRobot1Health.getMaxHealth(), 84);
        //if (redRobot2Health != null) redRobot2HealthBarImage.fillAmount = redRobot2Health.getCurrentHealth() / redRobot2Health.getMaxHealth();

        for(int i = 0; i < redRobotHealthImgs.Count; i++)
        {
            redRobotHealthImgs[i].rectTransform.sizeDelta = new Vector3(125 * redRobotHealth[i].getCurrentHealth() / redRobotHealth[i].getMaxHealth(), 10);
        }

        for (int i = 0; i < blueRobotHealthImgs.Count; i++)
        {
            blueRobotHealthImgs[i].rectTransform.sizeDelta = new Vector3(125 * redRobotHealth[i].getCurrentHealth() / redRobotHealth[i].getMaxHealth(), 10);
        }

        //if (blueRobot1Health != null) blueRobot1HealthBarImage.fillAmount = blueRobot1Health.getCurrentHealth() / blueRobot1Health.getMaxHealth();
        //if (blueRobot2Health != null) blueRobot2HealthBarImage.fillAmount = blueRobot2Health.getCurrentHealth() / blueRobot2Health.getMaxHealth();

        int min = (int)(matchTimeMin*60 - (Time.fixedTime - matchStartTime)+1) /60;
        int sec = (int)(matchTimeMin*60 - (Time.fixedTime - matchStartTime)+1) % 60;


        if(min == 0 && sec == 0) {

            List<string> winners = new List<string>();
            
            if(redBaseHealth.getCurrentHealth() < blueBaseHealth.getCurrentHealth()){
                resultText.text = "BLUE TEAM WINS";
                winners.Add("blue");
            }

            if (redBaseHealth.getCurrentHealth() > blueBaseHealth.getCurrentHealth())
            {
                resultText.text = "RED TEAM WINS";
                winners.Add("red");
            }

            if (redBaseHealth.getCurrentHealth() == blueBaseHealth.getCurrentHealth())
            {
                resultText.text = "ROUND TIED";
            }

            endOfRound(winners);
        }
        
        if(min >= 0 && sec >= 0)
        {
            buffer = (sec < 10) ? "0" : "";            
            matchTimer.text = min.ToString() + ":" + buffer + sec.ToString();
        }

        if(redBaseHealth.getCurrentHealth() == 0) {
            resultText.text = "BLUE TEAM WINS";
            endOfRound(new List<string>() { "blue" });

        }

        if (blueBaseHealth.getCurrentHealth() == 0) {
            resultText.text = "RED TEAM WINS";
            endOfRound(new List<string>() { "red" });
        }
    }

    [Server]
    public void togglePause() {
        Debug.Log("toggled");
        paused = !paused;
    }

    public void endOfRound(List<string> winners)
    {
        if (isServer) {
            for (int i = 0; i < winners.Count; i++) {
                if (winners[i] == "red") { redScore += 1; }
                if (winners[i] == "blue") { blueScore += 1; }
            }
        }

        transitioning = true;

        if (redScore < 2 && blueScore < 2)
        {
            roundEnd.SetActive(true);
            if (isServer)
            {
                Invoke(nameof(nextRoundSequence), EoRDuration);
            }
        }

        else
        {
            // End of Game logic
            gameEnd.SetActive(true);
        }

        Time.timeScale = 0.5f;
    }

    [Server]
    public void nextRoundSequence()
    {
        networkManager.resetGame();
        resetBaseHealths();
        clientResumeGame(); 
    }

    [Server]
    public void resetBaseHealths()
    {
        blueBaseHealth.resetHealth();
        redBaseHealth.resetHealth();
    }

    [ClientRpc]
    public void clientResumeGame()
    {
        Time.timeScale = 1f;
        roundEnd.SetActive(false);
        transitioning = false;
        matchStartTime = Time.fixedTime;
    }

    public void addRobotHealthDisplayLink(int team, Health health)
    {
        Image healthBarImage = null;

        if (team == 1) {

            GameObject redRobotHealthInst = Instantiate<GameObject>(RedRobotHealthUI, redHealthGroup.transform);
            redRobotHealthImgs.Add(redRobotHealthInst.transform.GetChild(3).GetComponent<Image>());                 //Add the bar image from the new UI element to the list of images
            redRobotHealth.Add(health);                                                                             //Link the robot's health to the UI
        }

        else if (team == 0)
        {
            GameObject blueRobotHealthInst = Instantiate<GameObject>(BlueRobotHealthUI, blueHealthGroup.transform);
            redRobotHealthImgs.Add(blueRobotHealthInst.transform.GetChild(3).GetComponent<Image>());                 //Add the bar image from the new UI element to the list of images
            redRobotHealth.Add(health);                                                                             //Link the robot's health to the UI
        }

        //updateRobotHealthDisplay(healthBarImage);
    }


    void updateRobotHealthDisplay(Image healthBarImage){
        healthBarImage.transform.parent.gameObject.SetActive(true);
    }

    public bool isPaused()
    {
        return paused;
    }

    public bool isGameEndActive()
    {
        return gameEnd.activeSelf;
    }
}
