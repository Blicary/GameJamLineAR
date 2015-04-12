using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lightning2 : MonoBehaviour 
{
    // movement and collision
    private const float speed = 5;
    public Rigidbody2D rig2D;

    // general
    private Wizard wizard;

    // visual
    public LineRenderer line;
    private const float length = 10f;
    private const float zigzag_intensity = 0.8f;
    private const float redraw_rate = 15; // redraws per second
    private float redraw_timer = 0;
    private const int max_num_positions = 3;
    private List<Vector2> fixed_points;


    
    // PUBLIC MODIFIERS

    public void Initialize(Wizard wizard)
    {
        this.wizard = wizard;
        line.SetColors(wizard.player_color, Color.Lerp(wizard.player_color, Color.white, 0.9f));

        //cam_shake = Camera.main.GetComponent<CameraShake>();
        //if (!cam_shake) Debug.LogError("no CameraShake found");
    }
    public void Fire(Vector2 pos, Vector2 direction)
    {
        direction.Normalize();
        rig2D.velocity = direction * speed;
        rig2D.transform.position = pos + direction;

        fixed_points = new List<Vector2>();
        fixed_points.Add(pos);
        fixed_points.Add(pos + direction);

        redraw_timer = 0;
    }
    public void Update()
    {
        if (!line.enabled) return;

        UpdateFixedPositions();

        // update redraw and re-collide
        redraw_timer -= Time.deltaTime;
        if (redraw_timer <= 0)
        {
            // redraw and collide
            ReDrawLine();
            redraw_timer = 1f / redraw_rate;
        }
    }
    public void UpdateFixedPositions()
    {
        // shrink trailing segment of line
        Vector2 segment_dir = (fixed_points[1] - fixed_points[0]).normalized;
        //fixed_points[0] += segment_dir * speed;

        // update tip of line
        fixed_points[fixed_points.Count - 1] = rig2D.transform.position;
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            // add fixed point            
            fixed_points.Add(collision.collider.transform.position);
        }
    }



    // PRIVATE MODIFIERS

    private void ReDrawLine()
    {
        float positions_per_dist = max_num_positions / length;

        int segment_first_i = 0;
        float intensity = zigzag_intensity;

        // update positions for each segment (between fixed points)
        for (int i = 1; i < fixed_points.Count; ++i)
        {
            Vector2 fixedpoint1 = fixed_points[i - 1];
            Vector2 fixedpoint2 = fixed_points[i];

            float dist = (fixedpoint2 - fixedpoint1).magnitude;
            int num_positions = Mathf.FloorToInt(positions_per_dist * dist);

            // update positions along this segment
            for (int j = 0; j < num_positions + 1; ++j)
            {
                if (segment_first_i + j >= max_num_positions) break;

                Vector2 pos = Vector2.Lerp(fixedpoint1, fixedpoint2, j / (float)num_positions);
                //pos += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * intensity;
                intensity /= 1.08f;

                line.SetPosition(segment_first_i + j, pos);
            }

            segment_first_i += num_positions;

        }
    }

}
