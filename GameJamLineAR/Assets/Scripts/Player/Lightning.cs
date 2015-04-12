using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Lightning : MonoBehaviour 
{
    // Visual
    private const float redraw_rate = 15; // redraws per second
    private float redraw_timer = 0;

    private const int max_num_positions = 20;
    private const float length = 10f;
    private const float zigzag_intensity = 0.8f;
    public LineRenderer line;

    private List<Vector2> fixed_points;
    private Vector2 direction, velocity;

    // movement and collision
    private const float speed = 5;
    public Rigidbody2D rig2D;


    // stun
    //private const float stun_duration = 0.75f;
    //private int power = 1; // stun_duration is multiplied by power
    //private float stun_duration;

    // audio
    //public WorldSound shock_sound;

    // References
    private Wizard wizard;
    //private CameraShake cam_shake;
    //public LayerMask racquets_layer;



    // PUBLIC MODIFIERS


    // PUBLIC MODIFIERS

    public void Initialize(Wizard wizard)
    {
        this.wizard = wizard;
        line.SetColors(wizard.player_color, Color.Lerp(wizard.player_color, Color.white, 0.9f));

        //cam_shake = Camera.main.GetComponent<CameraShake>();
        //if (!cam_shake) Debug.LogError("no CameraShake found");
    }
    public void Fire(Transform cast_point, Vector2 direction)
    {
        rig2D.velocity = direction.normalized * speed;

        // positions
        rig2D.transform.position = (Vector2)cast_point.position + direction.normalized * length;

        fixed_points = new List<Vector2>();
        fixed_points.Add(cast_point.position);
        fixed_points.Add(rig2D.transform.position);

        
        // visual
        //cam_shake.Shake(new CamShakeInstance(0.3f * stun_duration, 0.1f));
        line.enabled = true;
        
        // audio
        //shock_sound.Play();

        // draw and collision (stun players)
        //StartCoroutine(ReCreateBolt());
        redraw_timer = 0;
    }

    public void Update()
    {
        if (!line.enabled) return;

        MoveLightningAlong();

        // update redraw and re-collide
        redraw_timer -= Time.deltaTime;
        if (redraw_timer <= 0)
        {
            // redraw and collide
            ReDrawLine();
            //HandleCollision();

            redraw_timer = 1f / redraw_rate;
        }
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

    private void MoveLightningAlong()
    {
        // move last fixed point along
        Vector2 last_segment_dir = (fixed_points[1] - fixed_points[0]).normalized;
        fixed_points[0] += last_segment_dir * speed;

        // if last segment is small enough, remove the last fixed point
        float last_segment_dist = (fixed_points[1] - fixed_points[0]).magnitude;
        if (last_segment_dist <= 0.5f)
        {
            Debug.Log("remove");
            fixed_points.RemoveAt(0);
        }

        // move first fixed point
        //Vector2 first_segment_dir = (fixed_points[fixed_points.Count - 1] - fixed_points[fixed_points.Count - 2]).normalized;
        fixed_points[fixed_points.Count - 1] = rig2D.transform.position;
    }
    private IEnumerator ReCreateBolt()
    {
        while (true)
        {
            // redraw and collide
            ReDrawLine();
            HandleCollision();

            yield return new WaitForSeconds(1f/redraw_rate);
        }

        // finish bolt
        //line.enabled = false;
    }
    private void ReDrawLine()
    {
        // get number of positions per unit distance
        //float total_dist = 0;
        for (int i = 1; i < fixed_points.Count; ++i)
        {
            //float dist = (fixed_points[i] - fixed_points[i-1]).magnitude;
            //total_dist += dist;
            Debug.DrawLine(fixed_points[i - 1], fixed_points[i], Color.red, 0.5f);
        }
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
            for (int j = 0; j < num_positions+1; ++j)
            {
                if (segment_first_i + j >= max_num_positions) break;

                Vector2 pos = Vector2.Lerp(fixedpoint1, fixedpoint2, j / (float)num_positions);
                pos += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * intensity;
                intensity /= 1.08f;

                line.SetPosition(segment_first_i + j, pos);
            }

            segment_first_i += num_positions;
            
        }



        // clear the unused positions
        //for (int j = num_positions; j <= max_num_positions; ++i)
        //{
        //    line.SetPosition(i, fixedpoint2);
        //}

    }

    private void HandleCollision()
    {
        /*
        Vector2 pos1 = fixed_points[fixed_points.Count - 1];
        Vector2 pos2 = fixed_points[fixed_points.Count - 2];
        Vector2 dir = (pos1 - pos2).normalized;

        RaycastHit2D[] hits = Physics2D.LinecastAll(pos1, pos2);
        foreach (RaycastHit2D hit in hits)
        {
            // vs obstacle
            if (hit.collider.CompareTag("Obstacle") && last_hit != hit.collider)
            {
                // reflect the lightning

                // set the first (front) fixed point to the collision point
                Vector2 inter_pos = hit.point;
                fixed_points[fixed_points.Count - 1] = inter_pos;

                // add a new point (the new front of the bolt)
                Vector2 v_out = dir - (2 * Vector2.Dot(dir, hit.normal)) * hit.normal;

                fixed_points.Add(inter_pos + v_out * 0.1f);


                // save collider
                last_hit = hit.collider;
            }
        }



        /*
        Vector2 pos1 = bolt_start.position;
        Vector2 pos2 = bolt_end.position;

        // see if the player is hit (and hit them)
        RaycastHit2D[] hits = Physics2D.LinecastAll(pos1, pos2, racquets_layer.value);

        foreach (RaycastHit2D hit in hits)
        {
            // stun opponent racquet
            //Racquet r = hit.collider.GetComponent<Racquet>();
            //if (r != null && r.player_number != racquet.player_number) // affect only the opponent
            //{
            //    r.Stun(stun_duration);
            //}
        }
         * */
        
    }
}
