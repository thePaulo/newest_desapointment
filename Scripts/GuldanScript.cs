using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Threading;


public class GuldanScript : MonoBehaviour, IPlayer
{

    public float speed = 6f;            // The speed that the player will move at.

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    Transform transformed;

    public Slider healthSlider;
    public Slider ManaSlider;
    public int startingHealth = 100;
    public int currentHealth;
    public int currentMana;
    public Animator anim;
    public GameObject ultimate;
    public AudioSource no_time_for_games;
    public GameObject fireSpell;
    public GameObject freezeSpell;

    public GameObject enemy;
    Vector3 normalizer; //serve para aumentar o y do raio que sera castado no inimigo
    

    float multiplyer = 1; //porcentagem de dano causado

    float timer; //timer do cooldown dos ataques
    float xtimer; //timer do cooldown da corrida
    float ultimatelenght; //duracao do ultimate
    float distance_to_ground;

    bool alive = true;

    bool grounded = true;
    bool doubleJump = false;

    int jumps = 2;
    float time;
    // Use this for initialization


    void Start()
    {


        transformed = GetComponent<Transform>();
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        xtimer += Time.deltaTime;
        if (ultimate.activeSelf)
        {
            ultimatelenght += Time.deltaTime;
        }

        if (alive == true)
        {
            Move();
        }
        else
        {
            anim.Play("Death [7]");
        }

        if (ultimate.activeSelf == true && ultimatelenght > 5f) //tempo de duracao do ultimate
        {
            ultimate.SetActive(false);
            ultimatelenght = 0;
        }

        if (ultimate.activeSelf == true)
        {
            multiplyer = 1.5f;
        }
        else
        {
            multiplyer = 1;
        }
    }

    void FixedUpdate()
    {


        if (Input.GetKey(KeyCode.C))
        {
            Attack();
        }

        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        // Move the player around the scene.
        //Move(h, v);


        if (Input.GetKeyDown(KeyCode.Space)) //pulo
        {
            playerRigidbody.AddForce(new Vector3(0, 3f, 0), ForceMode.Impulse);
            anim.Play("JumpStart [45]");
            xtimer = 0;

        }
        if (Input.GetKey(KeyCode.F))
        {
            anim.Play("EmoteCheer [55]");
            ultimate.SetActive(true);
            ManaSlider.value += 1;
            currentMana += 1;
        }
        /*if (Input.GetKeyUp(KeyCode.F)) //ativacao do ultimate
        {
            anim.Play("EmoteCheer [55]");
            ultimate.SetActive(true);
            //no_time_for_games.time = 0.6f;
            //no_time_for_games.Play();
            ManaSlider.value -= 30;
        }*/
        if (Input.GetKey(KeyCode.X))
        {
            var rbEnemy = enemy.GetComponent<Rigidbody>();
            rbEnemy.isKinematic = true;

            anim.Play("Swim [31]");
            //transformed.position = Vector3.MoveTowards(transformed.position,new Vector3(2,3.6f,20),1f);   
            //transformed.position = Vector3.MoveTowards(transformed.position, new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z), 1f);
            time += Time.deltaTime / 2.5f;
            transformed.position = Vector3.Lerp(transformed.position, new Vector3(enemy.transform.position.x - 1, enemy.transform.position.y, enemy.transform.position.z), time);

            rbEnemy.isKinematic = false;
        }
        if (Input.GetKey(KeyCode.V))
        {
            if (timer > 1)
            {
                Attack2();
                Vector3 spellPos = new Vector3(transform.position.x + 1, transform.position.y + 4, transform.position.z);
                GameObject spell = Instantiate(fireSpell, spellPos, transform.rotation);
                timer = 0;
            }
        }
        if (Input.GetKey(KeyCode.N))
        {
            enemy.SendMessage("setAlive", false);
            var epos = enemy.transform.position;
            var mypos = transform.position;
            /*
            Thread thread = new Thread(() => freezingCast(mypos,epos));
            thread.Start();*/


            //freezeSpell.transform.position = enemy.transform.position;
            //freezeSpell.SetActive(true);
        }
        else{
            freezeSpell.SetActive(false);
            enemy.SendMessage("setAlive", true);
        }
        

    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal2");
        float v = Input.GetAxis("Vertical2");

        if (xtimer > 0.5f)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {

                Vector3 movement2 = new Vector3(-v, 0.0f,  -h);
                transformed.rotation = Quaternion.LookRotation(movement2);

                anim.Play("Run [2]");
            }
        }

        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, -v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Attack2() //punch
    {
        anim.Play("Slam [163]");
        RaycastHit hit = new RaycastHit();

        normalizer = transform.position;
        normalizer.y = normalizer.y + 1;


        if (Physics.Linecast(normalizer, enemy.transform.position, out hit))
        {
            //Debug.DrawLine(shootRay.origin, hit.point, Color.blue);

            Transform enemy = hit.transform.GetComponent<Transform>();
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30 * multiplyer);

                timer = 0f;
            }
        }

        xtimer = 0;
    }

    void Attack() //sided kick
    {


        anim.Play("Attack1H [10]");



        RaycastHit hit = new RaycastHit();
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 origin = transform.position;

        normalizer = transform.position;
        normalizer.y = normalizer.y + 1;


        Ray shootRay = new Ray();

        //shootRay.direction = transform.forward;
        Vector3 dir = new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z);
        //dir.x = enemy.transform.position.x;
        dir.y = enemy.transform.position.y + 2;
        //dir.z = enemy.transform.position.z;

        shootRay.direction = dir;
        shootRay.origin = normalizer;

        Debug.DrawLine(shootRay.origin, dir, Color.red);

        if (Physics.Linecast(normalizer, enemy.transform.position, out hit))
        {
            Debug.DrawLine(shootRay.origin, hit.point, Color.blue);

            Transform enemy = hit.transform.GetComponent<Transform>();
            //Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if (enemy != null && hit.transform.gameObject.tag == "Player" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30 * multiplyer);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }
        }


        xtimer = 0f;


    }

    public void TakeDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        healthSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            alive = false;
        }
    }
    

}
