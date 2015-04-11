using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public Wizard wizard;

    public void Instantiate(Wizard wizard)
    {
        this.wizard = wizard;
    }
    public void Update()
    {
        int pn = wizard.player_number;


        // move
        wizard.SetMoveDirection(new Vector2(InputManager.Horizontal(pn),
            InputManager.Vertical(pn)));

        // lightning
        if (InputManager.ActionButton(pn)) wizard.FireLightning();
    }
	
}
