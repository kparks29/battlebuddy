﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float speed = 1000.0f;
    public float cd = 2.5f;

    public float damage = 10.0f;

    public Buddy maker;
    Rigidbody body;
    void Start()
    {
        body = GetComponent<Rigidbody>();
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
            if (maker.myPlayer.isLocalPlayer)
            {
                Buddy b = c.GetComponent<Buddy>();
                print("Hit player " + b.playerNumber);
                b.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
