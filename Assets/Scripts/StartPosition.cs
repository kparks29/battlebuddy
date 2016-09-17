using UnityEngine;
using System.Collections;

public class StartPosition : MonoBehaviour {

    public int index;
    public Transform buddyStart;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
