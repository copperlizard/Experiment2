using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimationController : MonoBehaviour
{
    public float m_jumpForce = 1.0f, m_animSpeedMultiplier = 1.0f, m_MoveSpeedMultiplier = 1.0f, m_crouchSpeedModifier = 1.0f, 
        m_sprintSpeedModifier = 1.0f, m_walkSpeedModifier = 1.0f, m_runCycleLegOffset = 0.2f, m_stationaryTurnSpeed = 180.0f, m_movingTurnSpeed = 360.0f;

    private PlayerStateController m_stateController;

    private Animator m_animator;
    private Rigidbody m_rb;

	// Use this for initialization
	void Start ()
    {
        m_stateController = GetComponent<PlayerStateController>();

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
        //float turnA = m_playerStateController.m_turnTarAng - transform.rotation.eulerAngles.y;
        //float turnB = (m_playerStateController.m_turnTarAng + 360.0f) - transform.rotation.eulerAngles.y;
        //float turn = Mathf.Min(Mathf.Abs(turnA), Mathf.Abs(turnB));

        float turn = m_stateController.m_turnTarAng - transform.rotation.eulerAngles.y;

        if (Mathf.Abs(turn) > 180.0f)
        {
            if (transform.rotation.eulerAngles.y < m_stateController.m_turnTarAng)
            {
                turn = m_stateController.m_turnTarAng - (transform.rotation.eulerAngles.y + 360.0f);
            }
            else
            {
                turn = (m_stateController.m_turnTarAng + 360.0f) - transform.rotation.eulerAngles.y;
            }
        }
        turn /= 180.0f;

        RotatePlayer(turn);

        JumpPlayer();

        // update the animator parameters
        m_animator.SetLayerWeight(1, (m_stateController.m_aiming) ? Mathf.Lerp(m_animator.GetLayerWeight(1), 1.0f, 0.1f) : Mathf.Lerp(m_animator.GetLayerWeight(1), 0.0f, 0.1f)); //set aiming layer weight
        m_animator.SetFloat("Forward", m_stateController.m_forwardAmount, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Sideways", m_stateController.m_sidewaysAmount, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", turn, 0.1f, Time.deltaTime);
        m_animator.SetBool("OnGround", m_stateController.m_grounded);
        m_animator.SetBool("Crouch", m_stateController.m_crouch);
        m_animator.SetBool("Slide", m_stateController.m_slide);

        if (!m_stateController.m_grounded)
        {
            m_animator.SetFloat("Jump", m_rb.velocity.y);
        }

        // check if sliding
        AnimatorStateInfo curAnimState = m_animator.GetCurrentAnimatorStateInfo(0);
        bool isSliding = curAnimState.IsName("Sliding");

        // calculate which leg is behind, so as to leave that leg trailing in the jump animation
        // (This code is reliant on the specific run cycle offset in our animations,
        // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
        float runCycle =
            Mathf.Repeat(
                m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_runCycleLegOffset, 1);
        float jumpLeg = (runCycle < 0.5f ? 1 : -1) * m_stateController.m_forwardAmount;
        if (m_stateController.m_grounded && !isSliding)
        {
            m_animator.SetFloat("JumpLeg", jumpLeg);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_stateController.m_grounded && m_stateController.m_move.magnitude > 0)
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
        if (m_stateController.m_grounded && Time.deltaTime > 0)
        {
            Vector3 v = (m_animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

            // Apply move speed modifier
            if (m_stateController.m_crouch)
            {
                v *= m_crouchSpeedModifier;
            }
            else if (m_stateController.m_sprint)
            {
                v *= m_sprintSpeedModifier;
            }
            else if (m_stateController.m_walk)
            {
                v *= m_walkSpeedModifier;
            }

            //transform.rotation = transform.rotation * m_animator.deltaRotation;

            // we preserve the existing y part of the current velocity.
            v.y = m_rb.velocity.y;
            m_rb.velocity = v;
        }
    }
    
    void RotatePlayer(float ang) 
    {
        float turnSpeed = Mathf.Lerp(m_stationaryTurnSpeed, m_movingTurnSpeed, m_stateController.m_forwardAmount);
        transform.Rotate(0, ang * turnSpeed * Time.deltaTime, 0);
    }

    void JumpPlayer()
    {
        if (m_stateController.m_jump && m_stateController.m_grounded)
        {
            m_stateController.m_grounded = false;
            m_stateController.m_jump = false;
            m_rb.AddForce(Vector3.up * m_jumpForce, ForceMode.VelocityChange);
        }
    }
}
