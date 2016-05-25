using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshAgentGhost : MonoBehaviour
{
    //public GameObject m_target;

    private NavMeshAgent m_navMeshAgent;

	// Use this for initialization
	void Start ()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        m_navMeshAgent.updatePosition = false;
        m_navMeshAgent.updateRotation = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_navMeshAgent.Warp(transform.position);	    
	}
}
