using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour {

    public Transform networkHead;
    public Transform networkLeftHand;
    public Transform networkRightHand;

    [SyncVar]
    public int playerNumber = 0;

    [SyncVar]
    public int score = 0;


    public GameObject buddyPrefab;
    public Buddy myBuddy;
    public Transform buddyStart;

    public bool casting = false;

    PlatformController pc;
    
    Color rightHandColor;
    Color leftHandColor;
    bool rightTrigger = false;
    bool leftTrigger = false;

    Renderer rightRend;
    Renderer leftRend;


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
            
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (myBuddy != null)
            {
                if (rightHandDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    casting = true;
                }
                if (rightHandDevice.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    //print(hitPos);
                    //lineRend.enabled = false;
                    casting = false;
                    CmdRightTriggerPull(hitPos);
                    //myBuddy.hitLocation.position = hitPos;
                    //myBuddy.Fire();
                }
                if (leftHandDevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    CmdLeftTriggerPull();
                }

                if (rightHandDevice.GetAxis() != Vector2.zero)
                {
                    //CmdRightMove(rightHandDevice.GetAxis());
                }

                if (leftHandDevice.GetAxis() != Vector2.zero)
                {
                    CmdLeftMove(leftHandDevice.GetAxis());
                }
            }
        }

        if (isServer && Input.GetKeyDown(KeyCode.A))
        {
            CmdCreateBuddy();
        }
    }

    [Command]
    public void CmdCreateBuddy()
    {
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
    void CmdLeftMove(Vector2 movement)
    {
        RpcLeftMove(movement);
    }

    [ClientRpc]
    void RpcLeftMove(Vector2 movement)
    {
        myBuddy.transform.position += transform.forward * movement.y * Time.deltaTime * 3;
        myBuddy.transform.position += transform.right * movement.x * Time.deltaTime * 3;
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
    void CmdRightTriggerPull(Vector3 pos)
    {
        RpcRightTriggerPull(pos);
    }

    [ClientRpc]
    void RpcRightTriggerPull(Vector3 pos)
    {
        var lookPos = pos - myBuddy.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        myBuddy.transform.rotation = rotation;
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
