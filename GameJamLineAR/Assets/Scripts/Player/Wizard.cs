using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Wizard : MonoBehaviour 
{
    // General
    public int player_number;
    public Color player_color;
    private CameraShake cam_shake;
    public MatchManager match;

    // Rendering
    public SpriteRenderer sprite;
    public FlashScreen flash;

    // Position and arena bounds
    private const int bounds_width = 36, bounds_height = 20;

    // Movement
    private const float max_speed = 7;

    // Stun
    private bool stunned = false;
    private float stun_time_left = 0;
    private float stun_duration;
    private const float default_stun_duration = 1f;
    
    // Lives
    private bool alive = true;
    private int lives = 2;
    private float time_as_ghost = 6;

    private string wizard_layer_name = "Wizard";
    private string ghost_layer_name = "Ghost";
    private int wizard_layer, ghost_layer;


    // Lightning ability
    public Lightning2 lightning_prefab;

    // Input
    private Vector2 input_direction = Vector2.zero;

    // Events
    public event EventHandler<EventArgs<float>> event_stunned;


    // PUBLIC MODIFIERS

    public void Awake()
    {   
        // set colors
        player_color = GameSettings.Instance.GetPlayerColor(player_number);
        //sprite.color = player_color;

        // pool lightning objects
        ObjectPool.Instance.RequestObjects(lightning_prefab, 20, true);
    }
    public void Start()
    {
        // get references
        cam_shake = Camera.main.GetComponent<CameraShake>();
        if (!cam_shake) Debug.LogError("main camera has no CameraShake component");

        wizard_layer = LayerMask.NameToLayer(wizard_layer_name);
        ghost_layer = LayerMask.NameToLayer(ghost_layer_name);


        Reset();
    }
    public void Update()
    {
        if (OutOfLives()) return;

        if (!Stunned()) UpdateMovement();

        UpdateSprite();
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
    public void Kill()
    {
        lives--;
        alive = false;

        if (lives > 0)
            BecomeGhost();
        else
            DieForGood();

        cam_shake.Shake(new CamShakeInstance(1f, 1f));
    }

    // racquet control
    public void SetMoveDirection(Vector2 direction)
    {
        input_direction = direction;
    }
    public void FireLightning()
    {
        if (!alive || match.IsGameOver()) return;

        Vector2 v = GetComponent<Rigidbody2D>().velocity;
        if (v.magnitude == 0) return;

        Lightning2 lightning = ObjectPool.Instance.GetObject(lightning_prefab, false);
        lightning.Initialize(this);

        lightning.Fire(transform.position, v);

        // effects
        cam_shake.Shake(new CamShakeInstance(0.2f, 0.1f));
        flash.Flash(Color.Lerp(player_color, Color.white, 0.35f));
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
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity / (1 + 1.5f * Time.deltaTime);
        }
        else
        {
            GetComponent<Rigidbody2D>().AddForce(input_direction * 4000f * Time.deltaTime);
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
    private void UpdateSprite()
    {
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        Vector2 direction = rigid.velocity.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x);
        sprite.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + 90);
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

    private void DieForGood()
    {
        sprite.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        match.GameOver();
    }
    private void BecomeGhost()
    {
        gameObject.layer = ghost_layer;

        // visual
        SpriteRenderer rend = sprite.GetComponent<SpriteRenderer>();
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 0.3f);

        StartCoroutine("UpdateGhost");
    }
    private void Respawn()
    {
        alive = true;
        gameObject.layer = wizard_layer;

        SpriteRenderer rend = sprite.GetComponent<SpriteRenderer>();
        rend.color = new Color(rend.color.r, rend.color.g, rend.color.b, 1f);
    }
    private IEnumerator UpdateGhost()
    {
        yield return new WaitForSeconds(time_as_ghost);
        
        // respawn
        Respawn();
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
    public bool OutOfLives()
    {
        return lives <= 0;
    }
}
