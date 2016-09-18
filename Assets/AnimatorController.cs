using UnityEngine;
using System.Collections;

public class AnimatorController : MonoBehaviour {

    public Animator anim;
	
    public void SetAnimParams(Vector2 tuple)
    {
        anim.SetFloat("Forward", tuple.y);
        anim.SetFloat("Turn", tuple.x);
    }
}
