using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;


public class SylvScript : MonoBehaviour,IPlayer {

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
    public GameObject darkSpell;
    public GameObject shinraTensei;

    public GameObject enemy;
    Vector3 normalizer; //serve para aumentar o y do raio que sera castado no inimigo

    float multiplyer = 1; //porcentagem de dano causado

    float timer; //timer do cooldown dos ataques
    float xtimer; //timer do cooldown da corrida
    float ultimatelenght; //duracao do ultimate
    float distance_to_ground;

    bool alive = true;

    bool grounded = true;
    bool doubleJump =false;

    int jumps = 2;
    float time;
    // Use this for initialization


    void Start () {

        //Destroy(prefabEffect);
        currentMana = 100;
        transformed = GetComponent<Transform>();
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();


    }
	
	// Update is called once per frame
	void Update () {
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
            anim.Play("Death [60]");
        }

        if (ultimate.activeSelf == true && ultimatelenght > 5f) //tempo de duracao do ultimate
        {
            ultimate.SetActive(false);
            ultimatelenght = 0;
        }

        if(ultimate.activeSelf == true)//dano aumentado enquanto estado de furia
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

        
        if (Input.GetKey(KeyCode.Semicolon))
        {
            Attack();
        }

        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        // Move the player around the scene.
        //Move(h, v);
        
        
        if (Input.GetKeyDown(KeyCode.L)) //pulo
        {
            playerRigidbody.AddForce(new Vector3(0, 3f, 0), ForceMode.Impulse);
            anim.Play("JumpStart [113]");
            xtimer = 0;

        }
        if (Input.GetKey(KeyCode.O))
        {
            ultimate.SetActive(true);
            currentMana += 1;
            ManaSlider.value += 1;
            //ManaIncrease(1);
        }
        
        /*if (Input.GetKeyUp(KeyCode.O)) //ativacao do ultimate
        {
            if (ultimate.activeSelf == false)
            {
                anim.Play("EmoteCheer [114]");
                ultimate.SetActive(true);
                no_time_for_games.time = 0.6f;
                no_time_for_games.Play();
                ManaIncrease(1);
                //ManaSlider.value -= 30;
            }
        }*/
        if (Input.GetKey(KeyCode.Z))
        {
            var rbEnemy = enemy.GetComponent<Rigidbody>();
            rbEnemy.isKinematic = true;

            anim.Play("Swim [37]");
            //transformed.position = Vector3.MoveTowards(transformed.position,new Vector3(2,3.6f,20),1f);   
            //transformed.position = Vector3.MoveTowards(transformed.position, new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z), 1f);
            time += Time.deltaTime/2.5f;
            transformed.position = Vector3.Lerp(transformed.position, new Vector3(enemy.transform.position.x-1, enemy.transform.position.y, enemy.transform.position.z), time);

            rbEnemy.isKinematic = false;
        }
        if (Input.GetKey(KeyCode.K))
        {
            if (timer > 1f)
            {
                Attack2();

                Vector3 spellPos = new Vector3(transform.position.x + 1, transform.position.y + 2, transform.position.z);
                GameObject spell = Instantiate(darkSpell, spellPos, transform.rotation);
            }
        }
        if (Input.GetKey(KeyCode.J))
        {
            anim.Play("SpinningKick [162]");
            xtimer = 0;
        }

        if (Input.GetKey(KeyCode.B))
        {
            
            var rbEnemy = enemy.GetComponent<Rigidbody>();
            if (  Mathf.Abs(transform.position.z - enemy.transform.position.z) < 5 && Mathf.Abs(transform.position.x - enemy.transform.position.x) < 5)
            {
                rbEnemy.velocity = Vector3.zero;
                rbEnemy.AddForce((transform.position.x - enemy.transform.position.x) * Time.deltaTime * -100, 0, (transform.position.z - enemy.transform.position.z) * Time.deltaTime * -100, ForceMode.Impulse);
            }
            shinraTensei.SetActive(true);
            ManaSlider.value -= 1;
            //rbEnemy.AddExplosionForce((transform.position.x - enemy.transform.position.x) * Time.deltaTime * -10000, 0, (transform.position.z - enemy.transform.position.z) * Time.deltaTime * -10000);
            //rbEnemy.AddExplosionForce(100f, new Vector3(rbEnemy.transform.position.x-10, rbEnemy.transform.position.y, rbEnemy.transform.position.z-10), 10f);
        }
        else
        {
            shinraTensei.SetActive(false);
        }
        
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (xtimer > 0.5f)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {

                Vector3 movement2 = new Vector3(v, 0.0f, -1*h);
                transform.rotation = Quaternion.LookRotation(movement2);
                
                anim.Play("Run [22]");
            }
        }
        
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Attack2() //punch
    {
        anim.Play("Attack2HLoosePierce [30]");
        RaycastHit hit = new RaycastHit();

        normalizer = transform.position;
        normalizer.y = normalizer.y + 1;


        /*if (Physics.Linecast(normalizer, enemy.transform.position, out hit))
        {
            //Debug.DrawLine(shootRay.origin, hit.point, Color.blue);

            Transform enemy = hit.transform.GetComponent<Transform>();
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30*multiplyer);
                
                //timer = 0f;
            }
        }*/
        timer = 0;
        xtimer = 0;
    }

    void Attack() //sided kick
    {

        //transform.rotation = Quaternion.LookRotation(new Vector3(enemy.transform.rotation.x, enemy.transform.rotation.y, enemy.transform.rotation.z));

        //transform.LookAt(new Vector3(enemy.transform.rotation.x, enemy.transform.rotation.y, enemy.transform.rotation.z));

        Vector3 vec;
        vec.x = transform.position.x - enemy.transform.position.x;
        vec.y = transform.position.y - enemy.transform.position.y -90;
        vec.z = transform.position.z - enemy.transform.position.z;
        //Vector3 vec = transform.position - enemy.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(vec);
        lookRot.x = 0; lookRot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Mathf.Clamp01(3.0f * Time.maximumDeltaTime));

        anim.Play("Kick [68]");
        anim.SetBool("kicking", true);
        anim.SetBool("running", false);



        RaycastHit hit = new RaycastHit() ;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 origin = transform.position;

        normalizer = transform.position;
        normalizer.y = normalizer.y + 1;
        

        Ray shootRay = new Ray();

        //shootRay.direction = transform.forward;
        Vector3 dir = new Vector3(enemy.transform.position.x,enemy.transform.position.y,enemy.transform.position.z);
        //dir.x = enemy.transform.position.x;
        dir.y = enemy.transform.position.y+2;
        //dir.z = enemy.transform.position.z;
        
        shootRay.direction = dir;
        shootRay.origin = normalizer;

        /*Ray shootRay2 = new Ray();
        shootRay2.direction = new Vector3(-enemy.transform.position.x, enemy.transform.position.y+2, -enemy.transform.position.z);
        shootRay2.origin = normalizer;

        Ray shootRay3 = new Ray();
        shootRay3.direction = new Vector3(enemy.transform.position.x, enemy.transform.position.y + 2, -enemy.transform.position.z);
        shootRay3.origin = normalizer;

        Ray shootRay4 = new Ray();
        shootRay4.direction = new Vector3(-enemy.transform.position.x, enemy.transform.position.y + 2, enemy.transform.position.z);
        shootRay4.origin = normalizer;
        */
        Debug.DrawLine(shootRay.origin, dir, Color.red);
        
        if(Physics.Linecast(normalizer,enemy.transform.position,out hit))
        {
            Debug.DrawLine(shootRay.origin, hit.point, Color.blue);

            Transform enemy = hit.transform.GetComponent<Transform>();
            //Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30*multiplyer);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }
        }
        /*if (Physics.Raycast(shootRay, out hit, 1f)) // Direcao 1 ORIGINAL
        {
            Debug.DrawLine(shootRay.origin, hit.point,Color.blue);
            
            Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if(enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI "+hit.transform.gameObject.name);                
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }

        }


        if (Physics.Raycast(shootRay2, out hit, 1f))// direcao 2
        {
            Debug.DrawLine(shootRay2.origin, hit.point, Color.blue);

            Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }

        }

        if (Physics.Raycast(shootRay3, out hit, 1f))// direcao 3
        {
            Debug.DrawLine(shootRay3.origin, hit.point, Color.blue);

            Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }

        }

        if (Physics.Raycast(shootRay4, out hit, 1f))// direcao 4
        {
            Debug.DrawLine(shootRay4.origin, hit.point, Color.blue);

            Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI " + hit.transform.gameObject.name);
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }

        }*/
        
        xtimer = 0f;
        

    }

    public void TakeDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        healthSlider.value = currentHealth;
        if(currentHealth <= 0)
        {
            alive = false;
        }
    }

    public void ManaIncrease(int amount)
    {
        currentMana = currentMana + amount;
        ManaSlider.value = currentMana;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Terrain")
        {
            grounded = true;
            jumps = 2;
            
        }
        if(collision.gameObject.tag == "Enemy")
        {
            if (Input.GetKey(KeyCode.C))
            {
                collision.transform.gameObject.SendMessage("TakeDamage", 30);
            }
        }
    }
    public void setAlive(bool trigger)
    {
        alive = trigger;
    }
}
