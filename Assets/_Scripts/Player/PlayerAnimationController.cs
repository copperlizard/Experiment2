﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimationController : MonoBehaviour
{
    public float m_animSpeedMultiplier = 1.0f, m_MoveSpeedMultiplier = 1.0f, m_runCycleLegOffset = 0.2f, m_stationaryTurnSpeed = 180.0f, m_movingTurnSpeed = 360.0f;

    private PlayerStateController m_playerStateController;

    private Animator m_animator;
    private Rigidbody m_rb;

	// Use this for initialization
	void Start ()
    {
        m_playerStateController = GetComponent<PlayerStateController>();

        m_animator = GetComponent<Animator>();
        m_rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {   
        UpdateAnimator();
	}

    void UpdateAnimator()
    {        
        float turn = m_playerStateController.m_turnTarAng - transform.rotation.eulerAngles.y;                
        turn /= 360.0f;

        RotatePlayer(turn);

        // update the animator parameters
        m_animator.SetFloat("Forward", m_playerStateController.m_forwardAmount, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Sideways", m_playerStateController.m_sidewaysAmount, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);
        m_animator.SetBool("Crouch", m_playerStateController.m_crouch);
        m_animator.SetBool("OnGround", m_playerStateController.m_grounded);
        if (!m_playerStateController.m_grounded)
        {
            m_animator.SetFloat("Jump", m_rb.velocity.y);
        }

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_runCycleLegOffset, 1);
        float jumpLeg = (runCycle < 0.5f ? 1 : -1) * m_playerStateController.m_forwardAmount;
        if (m_playerStateController.m_grounded)
        {
            m_animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_playerStateController.m_grounded && m_playerStateController.m_move.magnitude > 0)
        {
            m_animator.speed = m_animSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            m_animator.speed = 1;
        }
    }

    void OnAnimatorMove ()
    {
        if (m_playerStateController.m_grounded && Time.deltaTime > 0)
        {
            Vector3 v = (m_animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            //transform.rotation = transform.rotation * m_animator.deltaRotation;

            // we preserve the existing y part of the current velocity.
            v.y = m_rb.velocity.y;
            m_rb.velocity = v;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_playerStateController.m_forwardAmount);
        transform.Rotate(0, (m_playerStateController.m_turnTarAng - transform.rotation.eulerAngles.y) * turnSpeed * Time.deltaTime, 0);
    }

    void RotatePlayer(float ang) 
    {
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_playerStateController.m_forwardAmount);
        transform.Rotate(0, ang * turnSpeed * Time.deltaTime, 0);
    }
}
