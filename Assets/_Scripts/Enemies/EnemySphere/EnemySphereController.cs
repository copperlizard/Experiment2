using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemySphereController : MonoBehaviour
{
    public GameObject m_target;

    private NavMeshAgent m_navAgent;

	// Use this for initialization
	void Start ()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_navAgent.SetDestination(m_target.transform.position);
	}
}
