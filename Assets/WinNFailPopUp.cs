using UnityEngine;

public class WinNFailPopUp : MonoBehaviour
{
    public static WinNFailPopUp Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance =this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int time;
    int currenttime;
    void Start()
    {
        
    }

    public void StarEnable()
    {
        GameMan.instance.Time = time;
        GameMan.instance.currentTime = currenttime;
        timeDivision();
    }

    void timeDivision()
    {
        int divsionStregth = time / 3;
        int firstD =time-(divsionStregth/2);
        int secondD =time-divsionStregth;
        if(currenttime<firstD)
        {
            print("3 stars");
        }
        else if(currenttime<secondD)
        {
            print("2 stars");
        }
        else if (currenttime<=time)
        {
            print("1 star");
        }
    }

}
