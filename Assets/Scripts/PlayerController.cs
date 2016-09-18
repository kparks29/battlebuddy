﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    public Transform networkHead;
    public Transform networkLeftHand;
    public Transform networkRightHand;

    public Image image;

    [SyncVar]
    public int playerNumber = 0;

    [SyncVar]
    public int score = 0;


    public GameObject buddyPrefab;
    public Buddy myBuddy;
    public Transform buddyStart;

    public bool rightCasting = false;
    public bool leftCasting = false;
    PlatformController pc;

    Color rightHandColor;
    Color leftHandColor;
    bool rightTrigger = false;
    bool leftTrigger = false;
    bool readyCheck = false;

    Renderer rightRend;
    Renderer leftRend;

    Collider readyCollider;

    private SteamVR_Controller.Device leftHandDevice;
    private SteamVR_Controller.Device rightHandDevice;

    LineRenderer lineRend;

    Vector3 hitPos;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += LoadLevel;

        if (isLocalPlayer)
        {
            playerNumber = FindObjectsOfType<PlayerController>().Length;
            lineRend = GetComponent<LineRenderer>();
            pc = FindObjectOfType<PlatformController>();
            StartPosition[] startPos = FindObjectsOfType<StartPosition>();
            foreach (StartPosition s in startPos)
            {
                if (s.index == playerNumber)
                {
                    transform.forward = s.transform.forward;
                    pc.transform.position = s.transform.position;
                    pc.transform.rotation = s.transform.rotation;
                    buddyStart = s.buddyStart;
                }
            }

            CmdSetPlayerNumber(playerNumber);

            rightHandDevice = SteamVR_Controller.Input((int)pc.rightHand.GetComponent<SteamVR_TrackedObject>().index);
            leftHandDevice = SteamVR_Controller.Input((int)pc.leftHand.GetComponent<SteamVR_TrackedObject>().index);
        }
        else
        {
            image.enabled = false;
        }

        //print("Not Local Start");

        rightRend = networkRightHand.GetComponentInChildren<Renderer>();
        leftRend = networkLeftHand.GetComponentInChildren<Renderer>();
        rightHandColor = rightRend.material.color;
        leftHandColor = leftRend.material.color;
    }

    [Command]
    void CmdSetPlayerNumber(int num)
    {
        RpcSetPlayerNumber(num);
    }

    [ClientRpc]
    void RpcSetPlayerNumber(int num)
    {
        playerNumber = num;
    }

    void LoadLevel(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
        {
            print("Loaded Scene 1");
            if (isLocalPlayer)
            {
                print("And Local");
                //CmdCreateBuddy();
            }
        }
    }

    void LateUpdate()
    {
        if (isLocalPlayer)
        {
            networkHead.position = pc.head.position;
            networkLeftHand.position = pc.leftHand.position;
            networkRightHand.position = pc.rightHand.position;
            networkHead.rotation = pc.head.rotation;
            networkLeftHand.rotation = pc.leftHand.rotation;
            networkRightHand.rotation = pc.rightHand.rotation;

            if (myBuddy != null)
            {
                image.transform.localScale = new Vector3(myBuddy.health / 20, image.transform.localScale.y, image.transform.localScale.z);
            }

            if (leftCasting)
            {
                if (!lineRend.enabled)
                    lineRend.enabled = true;
                Ray ray = new Ray(networkLeftHand.position, networkLeftHand.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    lineRend.SetPosition(0, networkLeftHand.position);
                    lineRend.SetPosition(1, new Vector3(hit.point.x, hit.point.y, hit.point.z));
                    hitPos = hit.point;

                    if (hit.collider.tag == "ReadySign")
                    {
                        readyCheck = true;
                        readyCollider = hit.collider;
                    }
                    else
                    {
                        readyCheck = false;
                        readyCollider = null;
                    }

                }
            }
            else if (lineRend.enabled)
            {
                lineRend.enabled = false;
            }
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {

            if (leftHandDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                //print("Left CLick");
                //CmdLeftTriggerPull();
                leftCasting = true;
            }

            if (leftHandDevice.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                leftCasting = false;
                if (readyCollider != null)
                {
                    if (playerNumber == 1)
                    {
                        print("P1 click ready");
                        readyCollider.GetComponentInParent<LevelManager>().CmdP1Ready();
                    }
                    else if (playerNumber == 2)
                    {
                        print("P2 clicked ready");
                        readyCollider.GetComponentInParent<LevelManager>().CmdP2Ready();
                    }
                    else
                    {
                        Debug.LogWarning("No proper playerNumber");
                    }
                }
            }

            if (myBuddy != null)
            {
                if (rightHandDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    rightCasting = true;
                }
                if (rightHandDevice.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    //print(hitPos);
                    //lineRend.enabled = false;
                    rightCasting = false;
                    CmdRightTriggerPull(hitPos, myBuddy.transform.rotation);
                    //myBuddy.hitLocation.position = hitPos;
                    //myBuddy.Fire();
                }

                if (rightHandDevice.GetAxis() != Vector2.zero)
                {
                    //CmdRightMove(rightHandDevice.GetAxis());
                }

                if (leftHandDevice.GetAxis() != Vector2.zero)
                {
                    CmdLeftMove(leftHandDevice.GetAxis(), myBuddy.transform.position);
                }
            }
        }

        //if (isServer && Input.GetKeyDown(KeyCode.A))
        //{
        //    CmdCreateBuddy();
        //}
    }

    [Command]
    public void CmdCreateBuddy()
    {
        //foreach(PlayerController p in FindObjectsOfType<PlayerController>())
        //{

        //}
        //GameObject bud = (GameObject)Instantiate(buddyPrefab, buddyStart.position, buddyStart.rotation);
        //myBuddy = bud.GetComponent<Buddy>();
        RpcCreateBuddy();
    }

    [ClientRpc]
    void RpcCreateBuddy()
    {
        StartPosition[] startPos = FindObjectsOfType<StartPosition>();
        foreach (StartPosition s in startPos)
        {
            if (s.index == playerNumber)
            {
                buddyStart = s.buddyStart;
            }
        }
        
        GameObject bud = (GameObject)Instantiate(buddyPrefab, buddyStart.position, buddyStart.rotation);
        myBuddy = bud.GetComponent<Buddy>();
        myBuddy.myPlayer = this;
    }

    [Command]
    void CmdLeftMove(Vector2 movement, Vector3 pos)
    {
        if (myBuddy != null)
        {
            myBuddy.transform.position += transform.forward * movement.y * Time.deltaTime * 3;
            myBuddy.transform.position += transform.right * movement.x * Time.deltaTime * 3;
            RpcLeftMove(myBuddy.transform.position);
        }
        else
        {
            print("Buddy null");
        }
    }

    [ClientRpc]
    void RpcLeftMove(Vector3 pos)
    {
        myBuddy.transform.position = pos;
    }

    [Command]
    void CmdRightMove(Vector2 movement)
    {
        RpcRightMove(movement);
    }

    [ClientRpc]
    void RpcRightMove(Vector2 movement)
    {

        myBuddy.hitLocation.transform.position += myBuddy.transform.forward * movement.y * Time.deltaTime * 5;
        myBuddy.hitLocation.transform.position += myBuddy.transform.right * movement.x * Time.deltaTime * 5;
        
    }

    [Command]
    void CmdRightTriggerPull(Vector3 pos, Quaternion rot)
    {
        RpcRightTriggerPull(pos, rot);
    }

    [ClientRpc]
    void RpcRightTriggerPull(Vector3 pos, Quaternion rot)
    {
       
        myBuddy.transform.rotation = rot;
        myBuddy.Fire(pos);
    }

    [Command]
    void CmdLeftTriggerPull()
    {
        RpcLeftTriggerPull();
    }

    [ClientRpc]
    void RpcLeftTriggerPull()
    {
        leftTrigger = !leftTrigger;
        if (leftTrigger)
            leftRend.material.color = Color.yellow;
        else
            leftRend.material.color = leftHandColor;
    }

    [Command]
    public void CmdTakeDamage(float amount)
    {
        RpcTakeDamage(amount);
    }

    [ClientRpc]
    void RpcTakeDamage(float amount)
    {
        myBuddy.CalculateDamageTaken(amount);
        if(myBuddy.health <= 0.0f)
        {
            if (isLocalPlayer)
            {
                print("You lose");
            }
            else
            {
                print("You win!");
            }
            Destroy(myBuddy.gameObject);
        }
        if(isLocalPlayer && myBuddy.health <= 0.0f)
        {
            print("Dead");
        }

    }
}
