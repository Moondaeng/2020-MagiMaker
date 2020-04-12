using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerAni : MonoBehaviour
{
    public const int ANI_IDLE = 0;
    public const int ANI_RUN = 1;
    public const int ANI_ATTACK = 2;
    public const int ANI_ATKIDLE = 3;
    public const int ANI_WARP = 4;
    public const int ANI_SKILL = 5;
    public const int ANI_DEATH = 10;

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ChangeAni(int aniNumber)
    {
        anim.SetInteger("aniName", aniNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
