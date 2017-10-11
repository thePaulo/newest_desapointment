using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GirlScript : MonoBehaviour
{

    public float speed = 6f;            // The speed that the player will move at.

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    Transform transformed;

    public Slider healthSlider;
    public int startingHealth = 100;
    public int currentHealth;
    GameObject player;


    Animator anim;
    float timer;
    Vector3 normalizer;
    // Use this for initialization


    void Start()
    {
        transformed = GetComponent<Transform>();
        currentHealth = startingHealth;

        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;


    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Attack();
        }

        playerRigidbody = GetComponent<Rigidbody>();

        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        float h = Input.GetAxis("Horizontal2");
        float v = Input.GetAxis("Vertical2");
        // Move the player around the scene.
        Move(h, v);

        if (Input.GetKey(KeyCode.N))
        {
            playerRigidbody.AddForce(new Vector3(0, 0.5f, 0), ForceMode.Impulse);
            //playerRigidbody.AddForce(0, 10, 0);
            //rigidbody.velocity += jumpSpeed * Vector3.up;
        }
        if (Input.GetKey(KeyCode.Y))
        {
            playerRigidbody.MovePosition(new Vector3(2, 3.6f, 20));
        }

        if (Input.GetKey(KeyCode.Z))
        {
            transformed.position = Vector3.MoveTowards(transformed.position, new Vector3(2, 3.6f, 20), 2f);
        }


    }

    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        movement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        movement = movement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Attack()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 origin = transform.position;

        normalizer = transform.position;
        normalizer.y = normalizer.y + 1;

        anim.Play("Spinkick");
        //int shootableSomething = LayerMask.GetMask("Default");

        Ray shootRay = new Ray();
        //shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        shootRay.origin = normalizer;

        if (Physics.Raycast(transform.position, Vector3.back, out hit, 5))
        {
            Debug.DrawLine(shootRay.origin, hit.point);
            //print(hit.transform.gameObject.name);
            if (hit.transform.gameObject.name == "Player" && timer > 1f)
            {
                print("Kermit atacou");
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                timer = 0f;


            }
            //print("Found an object - distance: " + hit.distance);
        }
        /*if (Physics.Raycast(shootRay, out hit, 10))
        {

            Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if (enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI");
                //TakeDamage(30);
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                timer = 0f;
            }

        }*/

    }

    void TakeDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        healthSlider.value = currentHealth;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            Debug.Log("toquei aki ó " + other.name);
        }
        if (Input.GetKey(KeyCode.K))
        {
            if (other.tag == "Player")
            {
                other.GetComponent<IPlayer>().TakeDamage(20);
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (other.gameObject.name == "Player")
            {
                other.GetComponent<IPlayer>().TakeDamage(10);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<IPlayer>() != null)
        {
            if (Input.GetKey(KeyCode.B))
            {
                Debug.Log("ihhhhhhh");
                collision.gameObject.GetComponent<IPlayer>().TakeDamage(10);
            }
        }
    }

}
