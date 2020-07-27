using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkeletonAni : MonoBehaviour
{
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string STAND = "Stand";
    public const string RUN = "Run";
    public const string ATTACK = "Attack";
    public const string DAMAGE = "Damage";
    public const string KNOCK_BACK = "Knockback";
    public const string DEATH = "Death";
    public const string SKILL = "Skill";

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
