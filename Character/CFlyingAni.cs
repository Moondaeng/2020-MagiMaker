using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFlyingAni : MonoBehaviour
{
    public const string IDLE1 = "idle1";
    public const string IDLE2 = "idle2";
    public const string ATTACK1 = "atack1";
    public const string ATTACK2 = "atack2";
    public const string GRAB = "grab";
    public const string FLYING = "flying";
    public const string ROAR = "roar";
    public const string HIT = "gethit";
    public const string DEATH = "death";

    Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animation>();
    }
    // Animation.CrossFade(보여줄 모션, 몇초동안); 
    public void ChangeAni(string aniName)
    {
        anim.CrossFade(aniName);
    }

    // Update is called once per frame
    void Update()
    {
    }
}