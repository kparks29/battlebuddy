using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour {
    public Image im;
    bool clicked = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clicked()
    {
        clicked = !clicked;
        if (clicked)
        {
            im.color = Color.red;
        }
        else
        {
            im.color = Color.black;
        }
    }
}
