using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lightning2 : MonoBehaviour 
{
    // movement and collision
    private const float speed = 20;
    public Rigidbody2D rig2D;

    // general
    private Wizard wizard;

    // visual
    public LineRenderer line;
    private const float zigzag_intensity = 0.5f;
    private const float redraw_rate = 30; // redraws per second
    private float redraw_timer = 0;
    
    private const int max_num_positions = 10;
    private bool[] point_active = new bool[max_num_positions];
    private bool[] point_fixed = new bool[max_num_positions];
    private Vector2[] points = new Vector2[max_num_positions];
    private int start_point_i = 0;

    
    // PUBLIC MODIFIERS

    public void Initialize(Wizard wizard)
    {
        this.wizard = wizard;
        line.SetColors(Color.Lerp(wizard.player_color, Color.white, 0.8f),
            Color.Lerp(wizard.player_color, Color.white, 0.4f));

        //cam_shake = Camera.main.GetComponent<CameraShake>();
        //if (!cam_shake) Debug.LogError("no CameraShake found");
    }
    public void Fire(Vector2 pos, Vector2 direction)
    {
        direction.Normalize();
        rig2D.velocity = direction * speed;
        rig2D.transform.position = pos + direction;

        for (int i = 0; i < max_num_positions; ++i)
        {
            point_fixed[i] = false;
            point_active[i] = false;
            points[i] = new Vector2();
        }
        start_point_i = -1;
        redraw_timer = 0f;
    }
    public void Update()
    {
        if (!line.enabled) return;

        UpdateLifeTime();

        // update redraw and re-collide
        redraw_timer -= Time.deltaTime;
        if (redraw_timer <= 0)
        {
            ActivateNextPoint(false);

            // redraw
            ReDrawLine();
            redraw_timer = 1f / redraw_rate;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            // add fixed point            
            ActivateNextPoint(true);
        }
        if (collision.collider.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            Wizard wiz = collision.collider.GetComponent<Wizard>();
            wiz.Kill();
        }
        if (collision.collider.CompareTag("Civilian"))
        {
            //gameObject.SetActive(false);
            Civilian civ = collision.collider.GetComponent<Civilian>();
            civ.Kill();
        }
    }



    // PRIVATE MODIFIERS

    private void UpdateLifeTime()
    {
        if (rig2D.velocity.magnitude <= speed / 2f)
        {
            gameObject.SetActive(false);
        }
    }
    private void ActivateNextPoint(bool fix_point)
    {
        // bring next point to front of line
        start_point_i = (start_point_i + 1) % max_num_positions;

        point_active[start_point_i] = true;
        point_fixed[start_point_i] = fix_point;
        points[start_point_i] = rig2D.transform.position;
    }
    private void ReDrawLine()
    {
        float intensity = zigzag_intensity;

        for (int i = 0; i < max_num_positions; ++i)
        {
            int point_i = mod(start_point_i - i, max_num_positions);

            if (!point_active[i])
            {
                line.SetPosition(point_i, points[0]);
                continue;
            }
                

            Vector2 pos = points[i];

            if (!point_fixed[i])
               pos += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * intensity;
            intensity /= 1.08f;

            line.SetPosition(point_i, pos);
        }
    }


    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

}
