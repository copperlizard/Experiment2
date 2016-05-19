using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemySphereController : MonoBehaviour
{
    public GameObject m_target;

    //private Health m_health;
    private NavMeshAgent m_navAgent;
    private NavMeshPath m_curPath;
    private NavMeshHit m_hit;
    private int m_curCorner;
    private bool m_havePath = false;

	// Use this for initialization
	void Start ()
    {
        m_navAgent = GetComponent<NavMeshAgent>();

        //m_health = GetComponent<Health>();
	}
	
	// Update is called once per frame
    void Update ()
    {
        
    }

	void FixedUpdate ()
    {
        if (SeePlayer())
        {
#if UNITY_EDITOR
            //Draw sightline
            Debug.DrawLine(transform.position, m_target.transform.position);
#endif

            //Dump path
            if (m_havePath)
            {
                m_curPath.ClearCorners();
                m_havePath = false;
                m_curCorner = 0;
            }

            //Calc dir
            Vector3 dir = m_target.transform.position - transform.position;

            if (dir.magnitude > 8.0f)
            {
                Move(dir.normalized);
            }
        }
        else
        {
            if (!m_havePath)
            {
                m_curPath = FindPath();
                m_havePath = true;
                m_curCorner = 0;
            }
            else if (m_curCorner < m_curPath.corners.Length - 1)
            {
#if UNITY_EDITOR
                //Draw path
                for (int i = 0; i < m_curPath.corners.Length - 1; i++)
                {
                    Debug.DrawLine(m_curPath.corners[i], m_curPath.corners[i + 1], Color.green);
                }
#endif
                //Traverse path
                Vector3 vel = (m_curPath.corners[m_curCorner] - transform.position);

                if (vel.magnitude <= 0.5f)
                {
                    //Next corner in path
                    m_curCorner++;

                    vel = (m_curPath.corners[m_curCorner] - transform.position);
                }

                Move(vel.normalized);
            }
            else
            {               
                m_havePath = false;
            }
        }
	}

    private bool SeePlayer ()
    {
        return !m_navAgent.Raycast(m_target.transform.position, out m_hit);
    }

    private NavMeshPath FindPath ()
    {
        NavMeshPath path = new NavMeshPath();

        if (m_navAgent.CalculatePath(m_target.transform.position, path))
        {
            return path;
        }
        else
        {
            //Path for when player lost... patrol waypoints maybe...

            return path;
        }
    }

    private void Move (Vector3 dir)
    {
        m_navAgent.velocity = dir * 3.0f;
    }

    void OnCollisionEnter (Collision other)
    {
        if (m_havePath)
        {
            m_havePath = false;
        }
    }
}
