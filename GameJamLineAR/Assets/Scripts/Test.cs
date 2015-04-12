using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

    public Lightning2 lightning;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        lightning.OnCollisionEnter2D(collision);
    }
}
