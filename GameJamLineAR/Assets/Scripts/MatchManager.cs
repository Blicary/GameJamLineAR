using UnityEngine;
using System.Collections;

public class MatchManager : MonoBehaviour 
{
    public Wizard wiz1, wiz2;
    public GGPage page_gg;

    private bool game_over;


    public void NewGame()
    {
        Application.LoadLevel("test_scene");
    }
    public void GameOver()
    {
        Debug.Log("here");
        game_over = true;
        page_gg.TransitionIn();
    }

    public int GetWinningPlayer()
    {
        return wiz1.OutOfLives() ? 2 : wiz2.OutOfLives() ? 1 : 0;
    }
    public bool IsGameOver()
    {
        return game_over;
    }
}
