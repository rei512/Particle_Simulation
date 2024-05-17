using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main_script : MonoBehaviour
{

    public Vector2 gravity;
    public float time_step;
    public float elastic;
    public float wall;

    public int N;
    public float radius;

    public GameObject particle;
    public AudioSource audioSource;
    private List<GameObject> particles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        
        for (int i = 0; i < Mathf.Sqrt(N); i++)
            for(int j = 0; j < Mathf.Sqrt(N); j++)
                particles.Add(Instantiate(particle, new Vector3(-4.0f + 2.0f * radius + i * (8.0f - 3.0f * radius) / Mathf.Sqrt(N), -4.0f + 2.0f * radius + j * (8.0f - 3.0f * radius) / Mathf.Sqrt(N)), Quaternion.identity));
        
        
        //particles.Add(Instantiate(particle, new Vector3(-2f, 0f, 0.0f), Quaternion.identity));
        //particles.Add(Instantiate(particle, new Vector3(2f, 0f, 0.0f), Quaternion.identity));
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("func", time_step);
    }

    void func()
    {
        for (int i = 0; i < N; i++)
        {
            particle_script this_particle = particles[i].GetComponent<particle_script>();
            Vector3 this_pos = this_particle.transform.position;

            //velocity.x += main.gravity;
            this_particle.velocity.x += gravity.x * time_step;
            this_particle.velocity.y += gravity.y * time_step;

            if (Mathf.Abs(this_pos.x + this_particle.velocity.x * time_step) >= wall - this_particle.radius)
            {
                this_particle.velocity.x *= -1f * elastic;
                this_particle.velocity.y *= (1f - (1f - elastic) * 0.1f);
                if(Mathf.Abs(this_particle.velocity.x) > 0.1f)
                    audioSource.PlayOneShot(audioSource.clip, Mathf.Abs(this_particle.velocity.x)>1f?1f: Mathf.Abs(this_particle.velocity.x));
            }

            if (Mathf.Abs(this_pos.y + this_particle.velocity.y * time_step) >= wall - this_particle.radius)
            {
                this_particle.velocity.y *= -1f * elastic;
                this_particle.velocity.x *= (1f - (1f - elastic) * 0.1f);
                if (Mathf.Abs(this_particle.velocity.y) > 0.1f)
                    audioSource.PlayOneShot(audioSource.clip, Mathf.Abs(this_particle.velocity.y) > 1f ? 1f : Mathf.Abs(this_particle.velocity.y));
            }

            for (int j = i + 1; j < N; j++)
            {
                particle_script another_particle = particles[j].GetComponent<particle_script>();
                Vector3 another_pos = another_particle.transform.position;

                Vector2 r = new Vector2(another_pos.x + another_particle.velocity.x * time_step - this_pos.x - this_particle.velocity.x * time_step, another_pos.y + another_particle.velocity.y * time_step - this_pos.y - this_particle.velocity.y * time_step);    //距離ベクトルr
                //Debug.Log(r.magnitude);
                if (r.sqrMagnitude <= Mathf.Pow(this_particle.radius + another_particle.radius, 2f))
                {
                    Vector2 v = new Vector2(this_particle.velocity.x - another_particle.velocity.x, this_particle.velocity.y - another_particle.velocity.y);    //相対速度v

                    float theta = Mathf.Atan2(r.y, r.x);
                    float alpha = Mathf.Atan2(v.y, v.x)-theta;

                    Vector2 v1 = new Vector2(v.magnitude * Mathf.Cos(alpha), v.magnitude * Mathf.Sin(alpha));

                    Vector2 vi = new Vector2((1 + elastic) * another_particle.mass / (this_particle.mass + another_particle.mass) * v1.x, 0f);
                    Vector2 vj = new Vector2((1 + elastic) * this_particle.mass / (this_particle.mass + another_particle.mass) * v1.x, 0f);

                    this_particle.velocity.x -= vi.x * Mathf.Cos(theta);
                    this_particle.velocity.y -= vi.x * Mathf.Sin(theta);

                    another_particle.velocity.x += vj.x * Mathf.Cos(theta);
                    another_particle.velocity.y += vj.x * Mathf.Sin(theta);
                }
            }
        }
    }
}
