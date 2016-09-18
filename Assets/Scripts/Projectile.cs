using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed = 10.0f;
    public float cd = 2.5f;

    public float damage = 10.0f;

    public Buddy maker;

    public GameObject explosion;
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
        if ((c.tag == "Buddy" || c.tag == "Wall") && c.GetComponent<Buddy>() != maker)
        {
            {
                Instantiate(explosion, transform.position, transform.rotation);
                Buddy b = c.GetComponent<Buddy>();
                if (b != null)
                {
                    b.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}
