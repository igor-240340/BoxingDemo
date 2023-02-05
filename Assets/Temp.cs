using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (anim != null)
        {
            /*// Attack
            if (Input.GetKeyDown(KeyCode.Q))
            {
                anim.Play("hp_straight_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                anim.Play("hp_straight_right_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                anim.Play("bp_upper_left_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                anim.Play("bp_hook_right_A", 0);
            }*/
            
            // Defence
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                anim.Play("knockdown_A", 0);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.Play("win_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                anim.Play("hb_front_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                anim.Play("hb_front_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                anim.Play("bb_front_A", 0);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                anim.Play("bb_front_A", 0);
            }
        }
    }
}