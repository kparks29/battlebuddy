﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Buddy : NetworkBehaviour {

    public Transform fireLocation;
    public Transform hitLocation;
    public GameObject myBullet;
    public Transform body;
    public PlayerController myPlayer;
    public Renderer rend;
    public Image image;


    [SyncVar]
    public int playerNumber;

    [SyncVar]
    public float health = 100;

    GameObject bullet;

    Vector3 firePos;

    [SyncVar]
    Vector3 hitPos;
   
    bool bulletFired;
    float startTime;
    float travelTime = .75f;
    float cd = 1.0f;
    float startHeight;

    LineRenderer lineRend;

    Color myOGColor;

    // Use this for initialization
    void Start () {
        myOGColor = rend.material.color;
        lineRend = GetComponent<LineRenderer>();
        hitLocation.transform.parent = null;
        startHeight = fireLocation.position.y;
        if (myPlayer.isLocalPlayer)
        {
            image.enabled = false;
            playerNumber = myPlayer.playerNumber;
        }
    }

    public void Fire(Vector3 pos)
    {
        if (cd <= 0.0f)
        {
            bullet = (GameObject)Instantiate(myBullet, fireLocation.position, fireLocation.rotation);
            Projectile proj = bullet.GetComponent<Projectile>();
            proj.maker = this;
            cd = proj.cd;
        }
    }

    void LateUpdate()
    {
        if (myPlayer != null && myPlayer.isLocalPlayer && myPlayer.rightCasting)
        {
            if (!lineRend.enabled)
                lineRend.enabled = true;
            Ray ray = new Ray(myPlayer.networkRightHand.position, myPlayer.networkRightHand.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                lineRend.SetPosition(0, fireLocation.position);
                lineRend.SetPosition(1, new Vector3(hit.point.x, fireLocation.position.y, hit.point.z));
                hitPos = hit.point;
            }

            var lookPos = hitPos - body.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            body.rotation = rotation;

        }
        else if (lineRend.enabled)
        {
            lineRend.enabled = false;
        }

        if (image.enabled)
        {
            image.transform.localScale = new Vector3(health / 20, image.transform.localScale.y, image.transform.localScale.z);
        }

        var look = Camera.main.transform.position - transform.position;
        look.y = 0;
        var rot = Quaternion.LookRotation(look);
        image.transform.rotation = rot;


        //var lookPos = hitPos - transform.position;
        //lookPos.y = 0;
        //var rotation = Quaternion.LookRotation(lookPos);
        //transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update () {
        if(cd >= 0.0f)
        {
            cd -= Time.deltaTime;
        }
    }

    IEnumerator KillBullet()
    {
        yield return new WaitForSeconds(.5f);
        if(bullet != null)
            Destroy(bullet);
        bulletFired = false;
    }


    public void TakeDamage(float amount)
    {
        if (myPlayer.isLocalPlayer)
        {
            print("Hit");
            myPlayer.CmdTakeDamage(amount / myPlayer.defense);
        }
    }
    
    public float CalculateDamageTaken(float amount)
    {
        health -= amount;
        print("Took " + amount + " damage, " + health + " health remaining");
        rend.material.color = Color.yellow;
        StartCoroutine(ChangeColorBack());
        return health;
    }

    IEnumerator ChangeColorBack()
    {
        yield return new WaitForSeconds(.15f);
        rend.material.color = myOGColor;
    }
}
