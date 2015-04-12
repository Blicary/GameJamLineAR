using UnityEngine;
using System.Collections;

public class MatchManager : MonoBehaviour 
{
    public Wizard wiz1, wiz2;
    public GGPage page_gg;


    public void NewGame()
    {
        Application.LoadLevel("test_scene");
    }
    public void GameOver()
    {
        page_gg.TransitionIn();
    }

    public int GetWinningPlayer()
    {
        return wiz1.OutOfLives() ? 2 : wiz2.OutOfLives() ? 1 : 0;
    }
}
