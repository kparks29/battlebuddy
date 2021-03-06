﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ItemSet : NetworkBehaviour {

    public GameObject[] weapons;
    public GameObject[] projectiles;

    public GameObject[] armor;

    public GameObject[] speed;

    public Transform activeWeapon;
    public Transform activeArmor;
    public Transform activeSpeed;

    Buddy myBuddy;

    void Start()
    {
        myBuddy = GetComponent<Buddy>();
    }

    public void setupWeapon(string itemName)
    {
      int i = 0;
      foreach(GameObject g in weapons)
      {
            if(g.name == itemName)
            {
                SetupWeap(i);
                ////GetComponent<Buddy>().myBullet = projectiles[i];
                //if (GetComponent<Buddy>().myPlayer.isLocalPlayer)
                //{
                //    CmdSetupWeapon(i);
                //}
            }
            else
            {
                g.SetActive(false);
            }
            i++;
        }
    }

    public void SetupWeap(int i)
    {
        if (GetComponent<Buddy>().myPlayer.isLocalPlayer)
        {
            CmdSetupWeapon(i);
        }
    }

    [Command]
    void CmdSetupWeapon(int i)
    {
        RpcSetupWeapon(i);
    }

    [ClientRpc]
    void RpcSetupWeapon(int i)
    {
        //if(myBuddy != null)
        {
            GetComponent<Buddy>().myBullet = projectiles[i];
        }
    }


    public void setupArmor(string itemName)
    {
        foreach (GameObject g in armor)
        {
            if (g.name == itemName)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
    }

    public void setupSpeed(string itemName)
    {
        foreach (GameObject g in speed)
        {
            if (g.name == itemName)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
    }
}
