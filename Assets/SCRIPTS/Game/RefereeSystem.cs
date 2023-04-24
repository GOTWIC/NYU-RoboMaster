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
    [SerializeField] private Image redRobot1HealthBarImage = null;
    [SerializeField] private Image redRobot2HealthBarImage = null;
    [SerializeField] private TMP_Text redScoreText = null;

    [SerializeField]
    private Health redRobot1Health = null;
    [SerializeField]
    private Health redRobot2Health = null;

    [Header("Blue")]
    [SerializeField] private Health blueBaseHealth = null;
    [SerializeField] private Image blueBaseHealthBarImage = null;
    [SerializeField] private Image blueRobot1HealthBarImage = null;
    [SerializeField] private Image blueRobot2HealthBarImage = null;
    [SerializeField] private TMP_Text blueScoreText = null;

    [SerializeField]
    private Health blueRobot1Health = null;
    [SerializeField]
    private Health blueRobot2Health = null;

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


        redBaseHealthBarImage.fillAmount = redBaseHealth.getCurrentHealth() / redBaseHealth.getMaxHealth();
        blueBaseHealthBarImage.fillAmount = blueBaseHealth.getCurrentHealth() / blueBaseHealth.getMaxHealth();
        if (redRobot1Health != null) { redRobot1HealthBarImage.fillAmount = redRobot1Health.getCurrentHealth() / redRobot1Health.getMaxHealth(); }
        if (redRobot2Health != null) { redRobot2HealthBarImage.fillAmount = redRobot2Health.getCurrentHealth() / redRobot2Health.getMaxHealth(); }
        if (blueRobot1Health != null) { blueRobot1HealthBarImage.fillAmount = blueRobot1Health.getCurrentHealth() / blueRobot1Health.getMaxHealth(); }
        if (blueRobot2Health != null) { blueRobot2HealthBarImage.fillAmount = blueRobot2Health.getCurrentHealth() / blueRobot2Health.getMaxHealth(); }

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

    public void togglePause()
    {
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

    public void setRobotHealthDisplayLink(int team, Health health)
    {
        Image healthBarImage = null;

        if (team == 1) {
            if (redRobot1Health == null) {
                redRobot1Health = health;
                healthBarImage = redRobot1HealthBarImage;
            }
            else {
                if (redRobot2Health == null) {
                    redRobot2Health = health;
                    healthBarImage = redRobot2HealthBarImage;
                }
            }
        }

        else if (team == 0)
        {
            if (blueRobot1Health == null)
            {
                blueRobot1Health = health;
                healthBarImage = blueRobot1HealthBarImage;
            }
            else
            {
                if (blueRobot2Health == null)
                {
                    blueRobot2Health = health;
                    healthBarImage = blueRobot2HealthBarImage;
                }
            }
        }

        updateRobotHealthDisplay(healthBarImage);
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
