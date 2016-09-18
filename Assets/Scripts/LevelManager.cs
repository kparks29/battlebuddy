using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkBehaviour {

    public NetworkManager manager;

    [SyncVar]
    public bool p1Ready = false;

    [SyncVar]
    public bool p2Ready = false;

    bool go = true;

    public GameObject p1ReadySign;
    public GameObject p2ReadySign;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += LoadedLevel;
	}
	

    void LoadedLevel(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 1)
        {
            p1ReadySign.SetActive(false);
            p2ReadySign.SetActive(false);
        }
        else
        {
            p1ReadySign.SetActive(true);
            p2ReadySign.SetActive(true);
        }
    }

    [Command]
    public void CmdP1Ready(bool truth)
    {
        RpcP1Ready(truth);
    }

    [ClientRpc]
    void RpcP1Ready(bool truth)
    {
        p1ReadySign.GetComponent<Renderer>().material.color = Color.green;
        p1Ready = truth;
    }

    [Command]
    public void CmdP2Ready(bool truth)
    {
        RpcP2Ready(truth);
    }

    [ClientRpc]
    void RpcP2Ready(bool truth)
    {
        p2ReadySign.GetComponent<Renderer>().material.color = Color.green;
        p2Ready = truth;
    }

    [Command]
    void CmdStartBattle()
    {
        RpcStartBattle();
    }

    [ClientRpc]
    void RpcStartBattle()
    {
        p1ReadySign.GetComponent<Renderer>().material.color = Color.red;
        p2ReadySign.GetComponent<Renderer>().material.color = Color.red;
    }

    [Command]
    public void CmdEndBattle()
    {
        RpcEndBattle();
    }

    [ClientRpc]
    void RpcEndBattle()
    {
        p1ReadySign.GetComponent<Renderer>().material.color = Color.grey;
        p2ReadySign.GetComponent<Renderer>().material.color = Color.grey;
        if (isServer)
        {
            CmdP1Ready(false);
            CmdP2Ready(false);

            StartCoroutine(WaitToEnableGo());
        }
    }

    IEnumerator WaitToEnableGo()
    {
        yield return new WaitForSeconds(1.5f);
        go = true;
    }

    // Update is called once per frame
    void Update () {
	    if(p1Ready && p2Ready && go)
        {
            if (isServer)
            {
                go = false;
                StartCoroutine(LoadBattle());
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad0) && isServer)
        {
            //StartCoroutine(LoadBattle());
        }
	}

    IEnumerator LoadBattle()
    {
        yield return new WaitForSeconds(1.25f);
        foreach(PlayerController p in FindObjectsOfType<PlayerController>())
        {
            p.CmdCreateBuddy();
        }

        CmdStartBattle();
        //manager.ServerChangeScene("Arena");
    }

    IEnumerator LoadLobby()
    {
        yield return new WaitForSeconds(1.25f);
        //manager.ServerChangeScene("Lobby");
    }
}
