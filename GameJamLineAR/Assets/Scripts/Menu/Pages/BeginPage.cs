using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BeginPage : UIMenuPage {

    private MatchManager match;
    //public Text heading;


    public void Awake()
    {
        match = Object.FindObjectOfType<MatchManager>();
        if (match == null) Debug.LogError("MatchManager missing.");
    }
    public void OnEnable()
    {
        //UIAudio.Instance.PlayPause();

        //heading.text = "FIGHT";
    }

    public void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if (Transition() == 1)
            {
                match.NewGame();
            }
        }
    }
}
