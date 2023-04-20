using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [SerializeField] private Health entityHealth = null;
    [SerializeField] private Image healthBarImage = null;


    // Update is called once per frame
    void Update()
    {
        healthBarImage.fillAmount = entityHealth.getCurrentHealth() / entityHealth.getMaxHealth();
    }
}
