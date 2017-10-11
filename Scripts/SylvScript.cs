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
    public int startingHealth = 100;
    public int currentHealth;

    Vector3 normalizer; //serve para aumentar o y do raio que sera castado no inimigo


    float timer;
    // Use this for initialization


    void Start () {
        transformed = GetComponent<Transform>();
        currentHealth = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if (Input.GetKey(KeyCode.P))
        {
            Attack();
        }

    }

    void FixedUpdate()
    {
        playerRigidbody = GetComponent<Rigidbody>();

        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // Move the player around the scene.
        Move(h, v);

        if (Input.GetKey(KeyCode.Space))
        {
            playerRigidbody.AddForce(new Vector3(0, 0.5f, 0), ForceMode.Impulse);
            //playerRigidbody.AddForce(0, 10, 0);
            //rigidbody.velocity += jumpSpeed * Vector3.up;
        }
        /*if (Input.GetKey(KeyCode.Y))
        {
            playerRigidbody.MovePosition(new Vector3(2, 3.6f, 20));
        }*/

        if (Input.GetKey(KeyCode.Z))
        {
            transformed.position = Vector3.MoveTowards(transformed.position,new Vector3(2,3.6f,20),1f);   
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
        RaycastHit hit = new RaycastHit() ;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 origin = transform.position;

        normalizer = transform.position;
        normalizer.y = normalizer.y + 1;
        

        Ray shootRay = new Ray();
        //shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
        shootRay.origin = normalizer;
        

        
        if (Physics.Raycast(shootRay, out hit, 1f))
        {
            Debug.DrawLine(shootRay.origin, hit.point);
            Transform enemy = hit.collider.GetComponent<Transform>();
            //Debug.Log(hit.collider.transform.tag.ToString());
            if(enemy != null && hit.transform.gameObject.tag == "Enemy" && timer > 1f)
            {
                Debug.Log("ATAQUEI");                
                hit.transform.gameObject.SendMessage("TakeDamage", 30);

                //Destroy(hit.transform.gameObject);
                timer = 0f;
            }

        }

    }

    public void TakeDamage(int amount)
    {
        currentHealth = currentHealth - amount;
        healthSlider.value = currentHealth;
    }


}
