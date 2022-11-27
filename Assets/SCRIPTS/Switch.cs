using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{

    [SerializeField] private Image left;
    [SerializeField] private Image right;

    [SerializeField] private int index;


    void Start()
    {
        LEFT();
    }

    public void LEFT()
    {
        index = 0;
        left.gameObject.SetActive(true);
        right.gameObject.SetActive(false);
    }

    public void RIGHT()
    {
        index = 1;
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(true);
    }

    public int getIndex()
    {
        return index;
    }
}