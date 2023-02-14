
using UnityEngine;

public class PlayerCardSlot : MonoBehaviour
{
    [SerializeField] private bool isFilled = false;

    public void changeState()
    {
        isFilled = !isFilled;
    }

    public bool getState()
    {
        return isFilled;
    }
}
