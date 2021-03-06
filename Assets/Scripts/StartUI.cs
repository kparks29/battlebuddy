﻿using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;

public class StartUI : NetworkBehaviour {

    public Text code;
    Button currentButton;
    Image currentImage;
    PlatformController pc;
    bool leftCasting = false;
    LineRenderer lineRend;
    bool buttonSet = false;
    public int player;

    public NetworkManager manager;
    public GameObject canvas;

    public User user;

    public List<Item> weaponList;
    public List<Item> armorList;
    public List<Item> speedList;

    void Start()
    {
        //user = new User();
        leftCasting = true;
        lineRend = GetComponent<LineRenderer>();
        pc = GetComponent<PlatformController>();
        code.text = "";
    }

	void InputCode(int num)
    {
        if(code.text.Length < 3)
        {
            code.text += num;
        }
    }

    void EraseCode()
    {
        code.text = "";
    }

    void EnterCode()
    {

        StartCoroutine(GetComponent<WebRequestManager>().getUser(code.text, (newUser) =>
        {
            user = newUser;
        }));

        StartCoroutine(GetComponent<WebRequestManager>().getItems("weapon", (itemSet) =>
        {
            weaponList = itemSet;
        }));

        StartCoroutine(GetComponent<WebRequestManager>().getItems("armor", (itemSet) =>
        {
            armorList = itemSet;
        }));

        StartCoroutine(GetComponent<WebRequestManager>().getItems("speed", (itemSet) =>
        {
            speedList = itemSet;
        }));

        if (currentButton != null || currentImage != null)
        {
            currentImage.color = currentButton.colors.normalColor;
            currentImage = null;
            currentButton = null;
            buttonSet = false;
        }
        leftCasting = false;
        lineRend.enabled = false;
        canvas.SetActive(false);
        //Code entered here
        if (player == 1)
        {
            //manager.StartServer();
            manager.StartHost();
        }
        else
        {
            manager.StartClient();
        }
    }

    void Update()
    {
        if (SteamVR_Controller.Input((int)pc.leftHand.GetComponent<SteamVR_TrackedObject>().index).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (currentButton != null)
            {
                currentImage.color = currentButton.colors.pressedColor;
            }
        }

        if (SteamVR_Controller.Input((int)pc.leftHand.GetComponent<SteamVR_TrackedObject>().index).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (currentButton != null)
            {
                if (currentButton.transform.name == "Enter")
                {
                    EnterCode();
                }
                else if (currentButton.transform.name == "Delete")
                {
                    EraseCode();
                }
                else
                    InputCode(Convert.ToInt32(currentButton.transform.name));

                if(currentImage != null)
                    currentImage.color = currentButton.colors.normalColor;
            }
        }

        if (leftCasting)
        {
            if (!lineRend.enabled)
                lineRend.enabled = true;
            Ray ray = new Ray(pc.leftHand.position, pc.leftHand.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.tag == "UIButton")
                {
                    if (!buttonSet)
                    {
                        buttonSet = true;
                        currentButton = hit.collider.GetComponent<Button>();
                        currentImage = hit.collider.GetComponent<Image>();
                        currentImage.color = currentButton.colors.highlightedColor;
                    }
                }
                else
                {
                    if(currentButton != null || currentImage != null)
                    {
                        currentImage.color = currentButton.colors.normalColor;
                        currentImage = null;
                        currentButton = null;
                        buttonSet = false;
                    }
                }

                lineRend.SetPosition(0, pc.leftHand.position);
                lineRend.SetPosition(1, new Vector3(hit.point.x, hit.point.y, hit.point.z));
            }
        }
        else
        {
            if (lineRend.enabled)
                lineRend.enabled = false;
        }
    }
}
