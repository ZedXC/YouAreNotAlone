using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    Animator anim;
    
    IEnumerator Start()
    {
        anim = GetComponent<Animator>();

        while(true){
            yield return new WaitForSeconds(5);
            anim.SetInteger("AnimationIndex", Random.Range(0,2));
            anim.SetTrigger("Activate");
        }
    }
}
