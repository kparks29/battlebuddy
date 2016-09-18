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
    public void CmdP1Ready()
    {
        RpcP1Ready();
    }

    [ClientRpc]
    void RpcP1Ready()
    {
        p1Ready = true;
    }

    [Command]
    public void CmdP2Ready()
    {
        RpcP2Ready();
    }

    [ClientRpc]
    void RpcP2Ready()
    {
        p2Ready = true;
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
        //manager.ServerChangeScene("Arena");
    }

    IEnumerator LoadLobby()
    {
        yield return new WaitForSeconds(1.25f);
        //manager.ServerChangeScene("Lobby");
    }
}
