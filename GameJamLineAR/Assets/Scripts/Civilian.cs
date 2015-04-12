using UnityEngine;
using System.Collections;

public class Civilian : MonoBehaviour 
{
    private string[] faces = { ":(", ":O", ":S" };
    private string dead_face = "X(";
    private string ghost_layer_name = "Ghost";
    private int ghost_layer;

    public TextMesh face;
    private Rigidbody2D rigid;

    private Vector2 direction;
    private float direction_timer;
    private float speed, max_speed = 2, min_speed = 0.4f;


    public void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        face.text = faces[Random.Range(0, faces.Length)];
        speed = Random.Range(min_speed, max_speed);

        ghost_layer = LayerMask.NameToLayer(ghost_layer_name);
        
        RandomizeDirection();
        direction_timer = Random.Range(1, 5);
    }

    public void Update()
    {
        direction_timer -= Time.deltaTime;
        if (direction_timer <= 0)
        {
            RandomizeDirection();
            direction_timer = Random.Range(1, 5);
        }

        rigid.AddForce(direction * speed * 2f);
    }
	public void Kill()
    {
        //GameObject.Destroy(gameObject);
        face.text = dead_face;
        face.color = Color.red;
        rigid.isKinematic = true;
        gameObject.layer = ghost_layer;
    }

    private void RandomizeDirection()
    {
        direction = new Vector2(GeneralHelpers.RandomSign(), GeneralHelpers.RandomSign());
        Debug.Log(direction);

    }
}
