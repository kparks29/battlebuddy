using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
