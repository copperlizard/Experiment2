using UnityEngine;
using System.Collections;

public class AIControlInput : MonoBehaviour
{
    public GameObject m_target;

    public AIOrbitCam m_AICam;

    private PlayerMovementController m_mover;

    private NavMeshAgent m_navAgent;
    private NavMeshPath m_curPath;
    private NavMeshHit m_navHit;

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
}
