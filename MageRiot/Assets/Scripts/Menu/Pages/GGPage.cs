using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GGPage : UIMenuPage
{
    private MatchManager match;
    public Text heading;


    public void Awake()
    {
        match = Object.FindObjectOfType<MatchManager>();
        if (match == null) Debug.LogError("MatchManager missing.");
    }
    public void OnEnable()
    {
        //UIAudio.Instance.PlayPause();

        int result = match.GetWinningPlayer();
        if (result == 1)
        {
            heading.text = GameSettings.Instance.player_name[0] + " Wins";
        }
        else if (result == 2)
        {
            heading.text = GameSettings.Instance.player_name[1] + " Wins";
        }
        else
            heading.text = "Draw";
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
