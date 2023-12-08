using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;
using System.Collections.Generic;

public class RefereeSystem : NetworkBehaviour {
    [Header("Configuration")]
    [SerializeField] private float matchTimeMin = 2;
    [SerializeField] private float EoRDuration = 2;

    [Header("Red")]
    [SerializeField] private Health redBaseHealth = null;
    [SerializeField] private Image redBaseHealthBarImage = null;


    [SerializeField] private List<Image> redRobotHealthImgs = new List<Image>();
    [SerializeField] private List<Health> redRobotHealth = new List<Health>();

    [SerializeField] private TMP_Text redScoreText = null;
    [SerializeField] private GameObject redHealthGroup = null;
    [SerializeField] private GameObject RedRobotHealthUI = null;



    [Header("Blue")]
    [SerializeField] private Health blueBaseHealth = null;
    [SerializeField] private Image blueBaseHealthBarImage = null;

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

    [SyncVar] public bool transitioning = false;
    [SyncVar] public bool endOfGame = false;
    [SyncVar] public bool paused = false;
    private string buffer = "";

    [SyncVar] public int redScore = 0;
    [SyncVar] public int blueScore = 0;

    [SyncVar] int min = 0;
    [SyncVar] int sec = 0;

    private float matchStartTime = 0f;

    [SyncVar] bool resetbutton = false;

    private void Start() {
        roundEnd.SetActive(false);
        gameEnd.SetActive(false);
        pause.SetActive(false);
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<MyNetworkManager>();
        matchStartTime = Time.fixedTime;
    }


    // Update is called once per frame
    void Update() {
        if (resetbutton) {
            Debug.Log("RESETTING");
            resetbutton = false;
            nextGameSequence();
        }

        blueScoreText.text = blueScore.ToString();
        redScoreText.text = redScore.ToString();



        if (paused) {
            pause.SetActive(true);
        }
        else {
            pause.SetActive(false);
        }

        // TODO: Check if this should be here
        //if (transitioning) { return; }


        redBaseHealthBarImage.rectTransform.sizeDelta = new Vector2(840 * redBaseHealth.getCurrentHealth() / redBaseHealth.getMaxHealth(), 84);
        blueBaseHealthBarImage.rectTransform.sizeDelta = new Vector2(840 * blueBaseHealth.getCurrentHealth() / blueBaseHealth.getMaxHealth(), 84);

        for (int i = 0; i < redRobotHealthImgs.Count; i++) {
            redRobotHealthImgs[i].rectTransform.sizeDelta = new Vector3(125 * redRobotHealth[i].getCurrentHealth() / redRobotHealth[i].getMaxHealth(), 10);
        }

        for (int i = 0; i < blueRobotHealthImgs.Count; i++) {
            blueRobotHealthImgs[i].rectTransform.sizeDelta = new Vector3(125 * redRobotHealth[i].getCurrentHealth() / redRobotHealth[i].getMaxHealth(), 10);
        }

        // Server - Timer
        // Don't update timer is we are transitioning between rounds
        if (isServer && !transitioning) {
            min = (int)(matchTimeMin * 60 - (Time.fixedTime - matchStartTime) + 1) / 60;
            sec = (int)(matchTimeMin * 60 - (Time.fixedTime - matchStartTime) + 1) % 60;
        }

        // Client - Timer
        if (min >= 0 && sec >= 0) {
            buffer = (sec < 10) ? "0" : "";
            matchTimer.text = min.ToString() + ":" + buffer + sec.ToString();
        }

        // Everything after this is server only
        if (!isServer) { return; }


        // Server - Time Up
        if (min == 0 && sec == 0 && !transitioning) {

            List<string> winners = new List<string>();

            if (redBaseHealth.getCurrentHealth() < blueBaseHealth.getCurrentHealth()) {
                showResultText("BLUE");
                winners.Add("blue");
            }

            if (redBaseHealth.getCurrentHealth() > blueBaseHealth.getCurrentHealth()) {
                showResultText("RED");
                winners.Add("red");
            }

            if (redBaseHealth.getCurrentHealth() == blueBaseHealth.getCurrentHealth()) {
                showResultText("TIE");
            }

            endOfRound(winners);
        }


        // Server - Base Death

        if (redBaseHealth.getCurrentHealth() == 0 && !transitioning) {

            showResultText("BLUE");
            endOfRound(new List<string>() { "blue" });
        }

        if (blueBaseHealth.getCurrentHealth() == 0 && !transitioning) {
            showResultText("RED");
            endOfRound(new List<string>() { "red" });
        }
    }

    public void clientResetButton() {
        Debug.Log("RESET BUTTON PRESSED");
        resetbutton = true;
    }

    [ClientRpc]
    public void showResultText(string text) {

        if (text == "TIE") { text = "ROUND TIED"; }

        if (text == "RED") {
            if (redScore == 2) { return; }
            else { text = "ROUND WON: RED TEAM"; }
        }

        if (text == "BLUE") {
            if (blueScore == 2) { return; }
            else { text = "ROUND WON: BLUE TEAM"; }
        }

        resultText.text = text;
        
        roundEnd.SetActive(true);
    }

    [Server]
    public void togglePause() {
        Debug.Log("toggled");
        paused = !paused;
    }

    [Server]
    public void endOfRound(List<string> winners) {
        // Stops time related tasks, such as the stopping timer and preventing from this method from being called multiple times
        transitioning = true;

        // Change the score. While this change happens on the server, the syncvar propogates it to the clients
        for (int i = 0; i < winners.Count; i++) {
            if (winners[i] == "red") { redScore += 1; }
            if (winners[i] == "blue") { blueScore += 1; }
        }

        // Check if there are more rounds to be played
        if (redScore < 2 && blueScore < 2) {
            Invoke(nameof(nextRoundSequence), EoRDuration / 2);
        }

        else {
            // End of Game logic
            gameEnd.SetActive(true);
            endOfGame = true;
        }

        Time.timeScale = 0.5f;
    }

    [Server]
    public void nextRoundSequence() {
        networkManager.resetGame();
        resetBaseHealths();
        transitioning = false; // updates for all clients
        matchStartTime = Time.fixedTime;
        clientResumeGame();
    }

    [Server]
    public void nextGameSequence() {
        networkManager.resetGame();
        resetScores();
        resetBaseHealths();
        transitioning = false; // updates for all clients
        endOfGame = false;
        paused = false;
        clientResumeGame();
    }

    [Server]
    public void resetBaseHealths() {
        blueBaseHealth.resetHealth();
        redBaseHealth.resetHealth();
    }

    [Server]
    public void resetScores() {
        redScore = 0;
        blueScore = 0;
    }

    [ClientRpc]
    public void clientResumeGame() {
        roundEnd.SetActive(false);
        gameEnd.SetActive(false);
    }

    public void addRobotHealthDisplayLink(int team, Health health) {
        Image healthBarImage = null;

        if (team == 1) {

            GameObject redRobotHealthInst = Instantiate<GameObject>(RedRobotHealthUI, redHealthGroup.transform);
            redRobotHealthImgs.Add(redRobotHealthInst.transform.GetChild(3).GetComponent<Image>());                 //Add the bar image from the new UI element to the list of images
            redRobotHealth.Add(health);                                                                             //Link the robot's health to the UI
        }

        else if (team == 0) {
            GameObject blueRobotHealthInst = Instantiate<GameObject>(BlueRobotHealthUI, blueHealthGroup.transform);
            redRobotHealthImgs.Add(blueRobotHealthInst.transform.GetChild(3).GetComponent<Image>());                 //Add the bar image from the new UI element to the list of images
            redRobotHealth.Add(health);                                                                             //Link the robot's health to the UI
        }

        //updateRobotHealthDisplay(healthBarImage);
    }


    void updateRobotHealthDisplay(Image healthBarImage) {
        healthBarImage.transform.parent.gameObject.SetActive(true);
    }

    public bool isPaused() {
        return paused;
    }

    public bool isTransitioning() {
        return transitioning;
    }

    public bool isEndOfGame() {
        return endOfGame;
    }

    public bool isInPlay() {
        return !(isPaused() || isTransitioning() || isEndOfGame());
    }
}