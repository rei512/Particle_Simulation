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
    private float L;

    private int _grid;
    private particle_script[,] field = new particle_script[0, 0];
    public int grid {
        get => _grid;
        set { 
            field = new particle_script[value+2, value+2];
            _grid = value;
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        L = radius * Mathf.Sqrt(2);
        grid = (int)(wall/L);

        Debug.Log(_grid);
        /*
        L = wall / _grid;
        radius = L / 1.42f;
        */
        int k = 0;
        for (int i = 0; i < Mathf.Sqrt(N); i++)
            for (int j = 0; j < Mathf.Sqrt(N); j++)
            {
                //particles.Add(Instantiate(particle, new Vector3(-4.0f + 2.0f * radius + i * (8.0f - 3.0f * radius) / Mathf.Sqrt(N), -4.0f + 2.0f * radius + j * (8.0f - 3.0f * radius) / Mathf.Sqrt(N)), Quaternion.identity));
                particles.Add(Instantiate(particle, new Vector3(-2.0f + 4.0f * i, -2.0f + 4.0f * j, -0.1f), Quaternion.identity));

                particle_script this_particle = particles[k++].GetComponent<particle_script>();
                Vector3 pos = this_particle.transform.position;
                //this_particle.velocity = new Vector2(Random.Range(-8.0f, 8.0f), Random.Range(0.0f, 5.0f));
                this_particle.velocity = new Vector2(0f, 0f);
                this_particle.radius = radius;
                Vector3 scale = new Vector3(radius * 2f, radius * 2f, radius * 2f);
                this_particle.transform.localScale = scale;
            }

        //particles.Add(Instantiate(particle, new Vector3(-2f, 0f, 0.0f), Quaternion.identity));
        //particles.Add(Instantiate(particle, new Vector3(2f, 0f, 0.0f), Quaternion.identity));
    }

    // Update is called once per frame
    void Update()
    {
        //Invoke("search", time_step);
        search();
    }

    void search()
    {
        for (int i = 0; i < grid+2; i++)
            for (int j = 0; j < grid+2; j++)
                field[i, j] = null;


        for (int i = 0; i < N; i++)
        {
            particle_script this_particle = particles[i].GetComponent<particle_script>();
            Vector3 this_pos = this_particle.transform.position;
            if ((int)((this_pos.x + wall / 2f) / L) > grid-1 || (int)((this_pos.x + wall / 2f) / L) < 0)
                this_pos.x = 0;
            if ((int)((this_pos.y + wall / 2f) / L) > grid-1 || (int)((this_pos.y + wall / 2f) / L) < 0)
                this_pos.y = 0;
            field[(int)((this_pos.x + wall / 2f) / L), (int)((this_pos.y + wall/2f)/L)] = particles[i].GetComponent<particle_script>();
        }

        for (int i = 0; i < grid; i++)
            for (int j = 0; j < grid; j++)
            {
                if(field[i,j] != null)
                {
                    particle_script this_particle = field[i, j];
                    Vector3 this_pos = this_particle.transform.position;

                    this_particle.velocity.x += gravity.x * time_step;
                    this_particle.velocity.y += gravity.y * time_step;


                    if (Mathf.Abs(this_pos.x + this_particle.velocity.x * time_step) >= wall / 2f - this_particle.radius)
                    {
                        this_particle.velocity.x *= -1f * elastic;
                        this_particle.velocity.y *= (1f - (1f - elastic) * 0.1f);
                        if (Mathf.Abs(this_particle.velocity.x) > 0.1f)
                            audioSource.PlayOneShot(audioSource.clip, Mathf.Abs(this_particle.velocity.x) > 1f ? 1f : Mathf.Abs(this_particle.velocity.x));
                    }

                    if (Mathf.Abs(this_pos.y + this_particle.velocity.y * time_step) >= wall / 2f - this_particle.radius)
                    {
                        this_particle.velocity.y *= -1f * elastic;
                        this_particle.velocity.x *= (1f - (1f - elastic) * 0.1f);
                        if (Mathf.Abs(this_particle.velocity.y) > 0.1f)
                            audioSource.PlayOneShot(audioSource.clip, Mathf.Abs(this_particle.velocity.y) > 1f ? 1f : Mathf.Abs(this_particle.velocity.y));
                    }

                    for (int k = -2; k <= 2; k++)
                        for (int u = -2; u < 2; u++)
                        {
                            if (u > 0 || (u==0 && k>=0))
                                break;

                            if (i+ k >= 0 && j+ u >= 0 && field[i + k, j + u] != null)
                            {
                                particle_script another_particle = field[i+k, j+u];
                                Vector3 another_pos = another_particle.transform.position;

                                Vector2 r = new Vector2(another_pos.x + another_particle.velocity.x * time_step - this_pos.x - this_particle.velocity.x * time_step, another_pos.y + another_particle.velocity.y * time_step - this_pos.y - this_particle.velocity.y * time_step);    //距離ベクトルr
                                //Debug.Log(r.magnitude);
                                if (r.sqrMagnitude <= Mathf.Pow(this_particle.radius + another_particle.radius, 2f))
                                {
                                    Vector2 v = new Vector2(this_particle.velocity.x - another_particle.velocity.x, this_particle.velocity.y - another_particle.velocity.y);    //相対速度v

                                    float theta = Mathf.Atan2(r.y, r.x);
                                    float alpha = Mathf.Atan2(v.y, v.x) - theta;

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


        for (int i = 0; i < N; i++)
        {
            particle_script this_particle = particles[i].GetComponent<particle_script>();
            Vector3 pos = this_particle.transform.position;

            pos.x += this_particle.velocity.x * time_step;
            pos.y += this_particle.velocity.y * time_step;
            this_particle. transform.position = pos;
        }




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

            if (Mathf.Abs(this_pos.x + this_particle.velocity.x * time_step) >= wall/2f - this_particle.radius)
            {
                this_particle.velocity.x *= -1f * elastic;
                this_particle.velocity.y *= (1f - (1f - elastic) * 0.1f);
                if(Mathf.Abs(this_particle.velocity.x) > 0.1f)
                    audioSource.PlayOneShot(audioSource.clip, Mathf.Abs(this_particle.velocity.x)>1f?1f: Mathf.Abs(this_particle.velocity.x));
            }

            if (Mathf.Abs(this_pos.y + this_particle.velocity.y * time_step) >= wall/2f - this_particle.radius)
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
