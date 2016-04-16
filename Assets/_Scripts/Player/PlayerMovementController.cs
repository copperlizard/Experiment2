using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerAnimationController))]
[RequireComponent(typeof(PlayerStateController))]
public class PlayerMovementController : MonoBehaviour
{
    public GameObject m_cam;
    public float m_moveSpeed = 5.0f, m_crouchSpeedModifier = 0.5f, m_walkSpeedModifier = 0.5f, m_sprintSpeedModifier = 1.5f, m_maxTurnSpeed = 5.0f, 
        m_groundCheckDist = 0.1f, m_headCheckGroundOffset = 0.1f, m_headCheckDist;
    
    private PlayerStateController m_stateController;
    private Vector3 m_move, m_groundNormal;        

	// Use this for initialization
	void Start ()
    {   
        m_stateController = GetComponent<PlayerStateController>();

        if (m_cam == null)
        {
            m_cam = Camera.main.gameObject;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position + transform.forward * 3.0f, Color.red);
        Debug.DrawLine(transform.position, transform.position + m_move, Color.blue);
#endif
    }

    void FixedUpdate ()
    {        
        
    }
    
    public void Move (float v, float h, bool fire1, bool fire2, bool jump, bool crouch, bool walk, bool sprint)
    {
        // update player state info        
        m_stateController.m_grounded = CheckGround();
        m_stateController.m_aiming = fire2;
        m_stateController.m_jump = jump && !crouch;
        m_stateController.m_crouch = crouch;
        m_stateController.m_walk = walk && !crouch && !sprint; //crouching and or sprint cancels walk
        m_stateController.m_sprint = sprint && !fire2 && !crouch; //aiming and/or crouching cancels sprint

        //Check for head room
        HeadCheck();

        if (fire2)
        {
            AimMove(v, h);
        }
        else
        {
            StandardMove(v, h);
        }

        m_stateController.m_move = m_move;
    }

    void HeadCheck ()
    {
        if(!m_stateController.m_crouch)
        {
            Vector3 startPos = transform.position;
            startPos.y += m_headCheckGroundOffset;

#if UNITY_EDITOR
            Debug.DrawLine(startPos, startPos + Vector3.up * m_headCheckDist, Color.green);
#endif
            if(Physics.Raycast(startPos, Vector3.up, m_headCheckDist))
            {
                m_stateController.m_crouch = true;
                m_stateController.m_walk = false;
                m_stateController.m_sprint = false;
                m_stateController.m_jump = false;
            }
        }
    }

    void StandardMove (float v, float h)
    {
        // Airborne move
        if (!m_stateController.m_grounded)
        {
            m_move = Vector3.zero;
            return;
        }

        // Not aiming
        m_move = new Vector3(h, 0.0f, v);
        m_move.Normalize();

        if (m_move.magnitude > 0.0f)
        {
            // Rotate input to match camera
            m_move = Quaternion.Euler(0.0f, m_cam.transform.eulerAngles.y, 0.0f) * m_move;

            // Look "forward"
            Quaternion lookRot = Quaternion.LookRotation(m_move);            
            m_stateController.m_turnTarAng = lookRot.eulerAngles.y;

            // Apply move speed modifier
            if (m_stateController.m_crouch)
            {
                m_move *= m_crouchSpeedModifier;
            }
            else if (m_stateController.m_sprint)
            {
                m_move *= m_sprintSpeedModifier;
            }
            else if (m_stateController.m_walk)
            {
                m_move *= m_walkSpeedModifier;
            }

            // Set move speed
            m_move *= m_moveSpeed;

            // Account for "hills"
            m_move = Vector3.ProjectOnPlane(m_move, m_groundNormal);
        }

        m_stateController.m_forwardAmount = m_move.magnitude;
        m_stateController.m_sidewaysAmount = 0.0f;
    }

    void AimMove (float v, float h)
    {
        // Airborne move
        if (!m_stateController.m_grounded)
        {
            m_move = Vector3.zero;
            return;
        }

        // Not aiming
        m_move = new Vector3(h, 0.0f, v);
        m_move.Normalize();

        if (m_stateController.m_sprint)
        {
            m_move *= 2.0f;
        }
        else if (m_stateController.m_walk)
        {
            m_move *= 0.5f;
        }

        // Rotate input to match camera
        m_move = Quaternion.Euler(0.0f, m_cam.transform.eulerAngles.y, 0.0f) * m_move;

        // Rotate player to match input        
        m_stateController.m_turnTarAng = m_cam.transform.eulerAngles.y;

        // Apply move speed modifier
        if (m_stateController.m_crouch)
        {
            m_move *= m_crouchSpeedModifier;
        }        
        else if (m_stateController.m_walk)
        {
            m_move *= m_walkSpeedModifier;
        }

        // Set move speed
        m_move *= m_moveSpeed;

        // Account for "hills"
        m_move = Vector3.ProjectOnPlane(m_move, m_groundNormal);

        
        Vector3 localMove = transform.InverseTransformVector(m_move);
        m_stateController.m_forwardAmount = localMove.z;
        m_stateController.m_sidewaysAmount = localMove.x;
    }

    bool CheckGround ()
    {   

#if UNITY_EDITOR
        Debug.DrawLine(transform.position + (Vector3.up * m_groundCheckDist * 0.5f), (transform.position + (Vector3.up * m_groundCheckDist * 0.5f)) + Vector3.down * m_groundCheckDist);
#endif

        RaycastHit hit;
        if(Physics.Raycast(transform.position + (Vector3.up * m_groundCheckDist * 0.5f), Vector3.down, out hit, m_groundCheckDist))
        {
            m_groundNormal = hit.normal;
            return true;
        }

        return false;
    }
}
