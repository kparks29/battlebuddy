using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Buddy : NetworkBehaviour {

    public Transform fireLocation;
    public Transform hitLocation;
    public GameObject myBullet;
    public Transform body;
    public PlayerController myPlayer;

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

    // Use this for initialization
    void Start () {
        lineRend = GetComponent<LineRenderer>();
        hitLocation.transform.parent = null;
        startHeight = fireLocation.position.y;
    }

    public void Fire(Vector3 pos)
    {
        if (cd <= 0.0f)
        {
            Vector3 forw = pos - transform.position;
            var lookPos = hitPos - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
            bullet = (GameObject)Instantiate(myBullet, fireLocation.position, fireLocation.rotation);
            Projectile proj = bullet.GetComponent<Projectile>();
            proj.maker = this;
            cd = proj.cd;
        }
    }

    void LateUpdate()
    {
        if (myPlayer.isLocalPlayer && myPlayer.casting)
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

            var lookPos = hitPos - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;

        }
        else if (lineRend.enabled)
        {
            lineRend.enabled = false;
        }

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
        //if (myPlayer.isLocalPlayer)
        {
            myPlayer.CmdTakeDamage(amount);
        }
    }
    
    public float CalculateDamageTaken(float amount)
    {
        health -= amount;
        print("Took " + amount + " damage, " + health + " health remaining");
        return health;
    }
}
