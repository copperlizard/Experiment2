using UnityEngine;
using System.Collections;

public class AIControlInput : MonoBehaviour
{
    public GameObject m_target, m_camera;

    public float m_firingRange;

    private PlayerMovementController m_mover;

    private NavMeshAgent m_navAgent;
    private NavMeshPath m_curPath;
    private NavMeshHit m_navHit;

    private RaycastHit m_sightHit;

    private Vector3 m_toTar;
    private float m_v, m_h;
    private bool m_fire1, m_fire2, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster;
    private int m_wepNum, m_curCorner;

    // Use this for initialization
    void Start ()
    {
        m_mover = GetComponent<PlayerMovementController>();

        m_navAgent = GetComponentInChildren<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (SeeTarget())
        {
            AimCamera(true);
        }
        else
        {
            AimCamera(false);
        }
        

        if (!DirectPath())
        {
            FindPath();
        }        
    }

    void FixedUpdate ()
    {
        m_mover.Move(m_v, m_h, m_fire1, m_fire2, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster, m_wepNum);
    }
    
    bool DirectPath()
    {
        return !m_navAgent.Raycast(m_target.transform.position, out m_navHit);
    }

    void FindPath ()
    {
        m_navAgent.CalculatePath(m_target.transform.position, m_curPath);
    }

    bool SeeTarget()
    {
        m_toTar = m_target.transform.position - transform.position;

        //Target too far away
        if (m_toTar.magnitude > m_firingRange)
        {
            return false;
        }

        //False if target obstructed
        bool playerVisible = !Physics.Raycast(transform.position, m_toTar.normalized, out m_sightHit, m_firingRange, ~LayerMask.GetMask("Player", "AIEnemy"));
        
        //If in front of player
        bool lineOfSight = (Vector3.Dot(transform.forward, m_toTar.normalized) > 0.5f) ? true : false;
        
        return (playerVisible && lineOfSight);
    }

    void AimCamera(bool atTar)
    {
        Quaternion tarRot = Quaternion.LookRotation(m_toTar.normalized);
        if (atTar)
        {
            Debug.Log("aiming camera at target");

            m_camera.transform.rotation = Quaternion.Lerp(m_camera.transform.rotation, tarRot, 0.1f);
            m_camera.transform.position = transform.position - (m_toTar.normalized * 5.0f);
        }
        else
        {
            Debug.Log("homing camera");

            m_camera.transform.rotation = Quaternion.Lerp(m_camera.transform.rotation, transform.rotation, 0.1f);
            m_camera.transform.position = transform.position - m_camera.transform.forward * 5.0f;
        }
    }

    void Attack ()
    {
        
    }
    
    
}
