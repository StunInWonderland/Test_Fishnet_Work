using FishNet.Component.Animating;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MPV_PlayerAction : MonoBehaviour
{
    private Animator animator;
    private NetworkAnimator netanimator;
    const int countDamageAnimate = 3;
    int lastDamageAnimate = 3;
    // Start is called before the first frame update
    void Start()
    {
        netanimator = GetComponent<NetworkAnimator>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Idle()
    {
        //animator.SetBool("Aiming",false);
        animator.SetFloat("Speed", 0);
    }
    public void Move(float value)
    {
        //animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", value);
    }
    public void Jump()
    {
        //animator.SetBool("Aiming", false);
        netanimator.SetTrigger("Jump");
    }
    public void Crouch(bool crouch)
    {
        //animator.SetBool("Aiming", false);
        animator.SetBool("Crouch", crouch);
    }
    public void Crawl(bool crawl)
    {
        //animator.SetBool("Aiming", false);
        animator.SetBool("Crawl", crawl);
    }
    public void Ground(bool gound)
    {
        animator.SetBool("Ground", gound);
    }
    public void Attack()
    {
        animator.SetBool("Aiming", false);
        animator.SetFloat("Speed", 1f);
    }
    public void Death()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        animator.Play("Idle", 0);
        else
        animator.SetTrigger("Death");
    }
}
