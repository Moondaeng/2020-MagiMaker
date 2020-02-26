using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAni : MonoBehaviour
{
    public const string IDLE = "Stand";
    public const string RUN = "Run";
    public const string WALK = "Walk";
    public const string ATTACK = "Attack";
    public const string DAMAGE = "Damage";
    //    public const string DAMAGE = "DAMAGE";
    //    public const string KNOCK_BACK = "KNOCK BACK";
    //    public const string DIE = "";

    Animation anim;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animation>();
    }
    // Animation.CrossFade(보여줄 모션, 몇초동안); 
    public void ChangeAni(string aniName)
    {
        anim.CrossFade(aniName, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
