using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Wizard : MonoBehaviour 
{
    // General
    public int player_number;
    public Color player_color;

    // Rendering
    public SpriteRenderer sprite;

    // Position and arena bounds
    private const int bounds_width = 33, bounds_height = 17;

    // Movement
    private const float max_speed = 5;

    // Stun
    private bool stunned = false;
    private float stun_time_left = 0;
    private float stun_duration;
    private const float default_stun_duration = 1f;
    
    // Lightning ability
    public Lightning lightning_prefab;

    // Input
    private Vector2 input_direction = Vector2.zero;

    // Events
    public event EventHandler<EventArgs<float>> event_stunned;


    // PUBLIC MODIFIERS

    public void Awake()
    {   
        // set colors
        player_color = GameSettings.Instance.GetPlayerColor(player_number);
        sprite.color = player_color;

        // pool lightning objects
        ObjectPool.Instance.RequestObjects(lightning_prefab, 5, true);
    }
    public void Start()
    {
        // get references
        //cam_shake = Camera.main.GetComponent<CameraShake>();
        //if (!cam_shake) Debug.LogError("main camera has no CameraShake component");

        Reset();
    }
    public void Update()
    {
        if (!Stunned()) UpdateMovement();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
    }
    
    public void Stun(float duration)
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;

        if (duration > stun_time_left)
        {
            if (event_stunned != null) event_stunned(this, new EventArgs<float>(duration - stun_time_left));

            stun_duration = duration;
            stun_time_left = duration;

            
        }

        if (!stunned)
        {
            stunned = true;

            StartCoroutine("UpdateStun");
        }
    }
    public void Stun()
    {
        Stun(default_stun_duration);
    }

    // racquet control
    public void SetMoveDirection(Vector2 direction)
    {
        input_direction = direction;
    }
    public void FireLightning()
    {
        Lightning lightning = ObjectPool.Instance.GetObject(lightning_prefab, false);
        lightning.Initialize(this);

        lightning.Fire(transform, GetComponent<Rigidbody2D>().velocity);
    }
    
    public void Reset()
    {
    }


    // PRIVATE MODIFIERS

    private void UpdateMovement()
    {
        if (stunned) return;

        // slow down when no input to move
        if (input_direction.magnitude <= 0f)
        {
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / (1 + 2f * Time.deltaTime);
        }
        else
        {
            GetComponent<Rigidbody2D>().AddForce(input_direction * 5000f * Time.deltaTime);
            GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(GetComponent<Rigidbody2D>().velocity, max_speed);
        }

        // restrict movement to court
        if (Mathf.Abs(transform.position.x) > bounds_width / 2f)
        {
            //float x = transform.position.x / bounds_width / 2f;
            //rigidbody2D.AddForce(new Vector2(-x * Time.deltaTime * 60000f, 0));
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
            transform.position = new Vector2(Mathf.Sign(transform.position.x) * bounds_width / 2f, transform.position.y);
        }
        if (Mathf.Abs(transform.position.y) > bounds_height / 2f)
        {
            //float y = transform.position.y / bounds_height / 2f;
            //rigidbody2D.AddForce(new Vector2(0, -y * Time.deltaTime * 60000f));
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
            transform.position = new Vector2(transform.position.x, Mathf.Sign(transform.position.y) * bounds_height / 2f);
        }
    }
    private IEnumerator UpdateStun()
    {
        while (stun_time_left > 0)
        {
            stun_time_left -= Time.deltaTime;
            yield return null;
        }

        // on stun end
        stun_time_left = 0;
        stunned = false;
        GetComponent<Rigidbody2D>().isKinematic = false;
    }

    // PUBLIC ACCESSORS 

    public bool Stunned()
    {
        return stunned;
    }
    public Collider2D PhysicalCollider()
    {
        return GetComponent<Collider2D>();
    }
}
