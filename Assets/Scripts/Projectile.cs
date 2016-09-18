using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed = 10.0f;
    public float cd = 2.5f;

    public float damage = 10.0f;

    public Buddy maker;
    Rigidbody body;
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        if(body != null)
        {
            body.AddForce(transform.forward * speed);
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Buddy" && c.GetComponent<Buddy>() != maker)
        {
            //if (maker.myPlayer.isLocalPlayer)
            {
                Buddy b = c.GetComponent<Buddy>();
                print("Player "+maker.myPlayer.playerNumber+ " Hit player " + b.playerNumber);
                b.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
