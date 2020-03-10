using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAni : MonoBehaviour
{
    public const string IDLE = "idle";
    public const string ATTACK1 = "attack1";
    public const string ATTACK2 = "attack2";
    public const string DEATH1 = "death1";
    public const string DEATH2 = "death2";
    public const string HIT1 = "hit1";
    public const string HIT2 = "hit2";
    public const string CAST = "cast";
    public const string IDLE_battle = "idle_battle";
    public const string POWERATTACK = "pow_attack";
    public const string RUN = "run";
    public const string WALK = "walk";

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
