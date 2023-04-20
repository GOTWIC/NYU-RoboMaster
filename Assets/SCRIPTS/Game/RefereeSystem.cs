using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System.Text.RegularExpressions;

public class RefereeSystem : NetworkBehaviour
{

    [Header("Configuration")]
    [SerializeField] private int matchTimeMin = 5;

    [Header("Red")]
    [SerializeField] private Health redBaseHealth = null;
    [SerializeField] private Image redBaseHealthBarImage = null;
    [SerializeField] private Image redRobot1HealthBarImage = null;
    [SerializeField] private Image redRobot2HealthBarImage = null;
    [SerializeField] private TMP_Text redScore = null;

    [SerializeField]
    private Health redRobot1Health = null;
    [SerializeField]
    private Health redRobot2Health = null;

    [Header("Blue")]
    [SerializeField] private Health blueBaseHealth = null;
    [SerializeField] private Image blueBaseHealthBarImage = null;
    [SerializeField] private Image blueRobot1HealthBarImage = null;
    [SerializeField] private Image blueRobot2HealthBarImage = null;
    [SerializeField] private TMP_Text blueScore = null;

    [SerializeField]
    private Health blueRobot1Health = null;
    [SerializeField]
    private Health blueRobot2Health = null;

    [Header("Misc")]
    [SerializeField] private GameObject roundEnd = null;
    [SerializeField] private TMP_Text resultText = null;
    [SerializeField] private TMP_Text matchTimer = null;
   

    private void Start()
    {
        roundEnd.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        redBaseHealthBarImage.fillAmount = redBaseHealth.getCurrentHealth() / redBaseHealth.getMaxHealth();
        blueBaseHealthBarImage.fillAmount = blueBaseHealth.getCurrentHealth() / blueBaseHealth.getMaxHealth();
        if (redRobot1Health != null) { redRobot1HealthBarImage.fillAmount = redRobot1Health.getCurrentHealth() / redRobot1Health.getMaxHealth(); }
        if (redRobot2Health != null) { redRobot2HealthBarImage.fillAmount = redRobot2Health.getCurrentHealth() / redRobot2Health.getMaxHealth(); }
        if (blueRobot1Health != null) { blueRobot1HealthBarImage.fillAmount = blueRobot1Health.getCurrentHealth() / blueRobot1Health.getMaxHealth(); }
        if (blueRobot2Health != null) { blueRobot2HealthBarImage.fillAmount = blueRobot2Health.getCurrentHealth() / blueRobot2Health.getMaxHealth(); }

        int min = (int)(matchTimeMin*60 - Time.fixedTime)/60;
        int sec = (int)(matchTimeMin*60 - Time.fixedTime) % 60;


        if(min == 0 && sec == 0) {
            if(redBaseHealth.getCurrentHealth() < blueBaseHealth.getCurrentHealth()){
                resultText.text = "BLUE TEAM WINS";
                roundEnd.SetActive(true);
            }

            if (redBaseHealth.getCurrentHealth() > blueBaseHealth.getCurrentHealth())
            {
                resultText.text = "RED TEAM WINS";
                roundEnd.SetActive(true);
            }

            if (redBaseHealth.getCurrentHealth() == blueBaseHealth.getCurrentHealth())
            {
                resultText.text = "ROUND TIED";
                roundEnd.SetActive(true);
            }
        }
        
        if(min >= 0 && sec >= 0)
        {
            matchTimer.text = min.ToString() + ":" + sec.ToString();
        }

        if(redBaseHealth.getCurrentHealth() == 0) {
            resultText.text = "BLUE TEAM WINS";
            roundEnd.SetActive(true);

        }

        if (blueBaseHealth.getCurrentHealth() == 0) {
            resultText.text = "RED TEAM WINS";
            roundEnd.SetActive(true);
        }
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
}
