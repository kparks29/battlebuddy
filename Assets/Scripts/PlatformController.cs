using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(WaitToColor());
    }

    IEnumerator WaitToColor()
    {
        yield return new WaitForSeconds(2.5f);

        foreach (Renderer rend in leftHand.GetComponentsInChildren<Renderer>())
        {
            rend.material.color = Color.blue;
        }

        foreach (Renderer rend in rightHand.GetComponentsInChildren<Renderer>())
        {
            rend.material.color = Color.yellow;
        }
    }
}
