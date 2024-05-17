using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_script : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector2 velocity;
    public float radius;
    public float mass;
    //public particle_script particle;
    GameObject script;
    main_script main;

    void Start()
    {
        script = GameObject.Find("main");
        main = script.GetComponent<main_script>();
        velocity = new Vector2(Random.Range(-8.0f, 8.0f), Random.Range(0.0f, 5.0f));
        radius = main.radius;
        Vector3 scale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
        this.transform.localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("accelerate", main.time_step);
    }

    void accelerate()
    {

        Vector3 pos = transform.position;
        pos.x += velocity.x * main.time_step;
        pos.y += velocity.y * main.time_step;
        transform.position = pos;

        /*

        //velocity.x += main.gravity;
        velocity.y += main.gravity;

        if (Mathf.Abs(pos.x + velocity.x * main.time_step) >= main.wall - radius)
        {
            velocity.x *= -1f * main.elastic;
            velocity.x *= (1f - (1f - main.elastic) * 0.1f);
        }

        if (Mathf.Abs(pos.y + velocity.y * main.time_step) >= main.wall - radius)
        {
            velocity.y *= -1f * main.elastic;
            velocity.x *= (1f - (1f - main.elastic) * 0.1f);
            //pos.y = 2f - radius;
            //pos.y += velocity.y * main.time_step;
        }*/
        /*
        if(radius + particle.radius > Mathf.Sqrt(Mathf.Pow(pos.x - particle.transform.position.x, 2) + Mathf.Pow(pos.y - particle.transform.position.y, 2))) {
            float theta = Mathf.Atan2(particle.transform.position.y - pos.y - velocity.y * main.time_step, particle.transform.position.x - pos.x - velocity.x * main.time_step);
            float alpha = -1f*(theta + Mathf.Atan2(velocity.y, velocity.x));

            //Vector2 v_diff = new Vector2(velocity.x - particle.velocity.x, velocity.y - particle.velocity.y);
            Vector2 v1 = new Vector2(-1f*particle.mass/(particle.mass + mass) * main.elastic * velocity.magnitude * Mathf.Cos(alpha), velocity.magnitude * Mathf.Sin(alpha));
            //Vector2 v1 = new Vector2(velocity.x, velocity.y);
            velocity.x = v1.magnitude * Mathf.Cos(Mathf.PI + theta - alpha);
            velocity.y = v1.magnitude * Mathf.Sin(Mathf.PI + theta - alpha);
            /*
            velocity.x -= main.elastic * (v1.x) + v2.x;
            particle.velocity.x += v2.magnitude / Mathf.Cos(theta);
            particle.velocity.y += v2.magnitude / Mathf.Sin(theta);
            
            
            Debug.Log("theta = " + theta + ", velocityRad = " + Mathf.Atan2(velocity.y, velocity.x) + ", alpha = " + alpha);
        }
        */


    }
}
