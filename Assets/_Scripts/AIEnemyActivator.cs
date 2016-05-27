using UnityEngine;
using System.Collections;

public class AIEnemyActivator : MonoBehaviour
{
    public GameObject m_AIenemy;

    private AIControlInput m_AIcontrol;
    private PlayerHealth m_AIHealth;
    private Rigidbody m_AImainRB;

    private MeshRenderer[] m_AImeshes;
    private Collider[] m_AIColliders;

    private Vector3 m_startPos;
    private Quaternion m_startRot;

    private bool m_AIactive = false;

	// Use this for initialization
	void Start ()
    {
        m_AIcontrol = m_AIenemy.GetComponent<AIControlInput>();
        m_AImainRB = m_AIenemy.GetComponent<Rigidbody>();
        m_AImeshes = m_AIenemy.GetComponentsInChildren<MeshRenderer>(true);
        m_AIColliders = m_AIenemy.GetComponentsInChildren<Collider>(true);

        m_AIHealth = m_AIenemy.GetComponent<PlayerHealth>();

        m_startPos = m_AIenemy.transform.position;
        m_startRot = m_AIenemy.transform.rotation;

        SetAIState(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (m_AIHealth.GetHealth() <= 0.0f)
        {
            m_AIenemy.transform.position = m_startPos;
            m_AIenemy.transform.rotation = m_startRot;

            SetAIState(false);
        }
	}

    void OnCollisionEnter (Collision other)
    {
        if (!m_AIactive)
        {
            SetAIState(true);
        }
    }

    void SetAIState (bool state)
    {
        m_AIactive = state;

        m_AIcontrol.enabled = state;
                
        m_AImainRB.useGravity = state;

        if (state)
        {
            m_AImainRB.WakeUp();
        }
        else
        {
            m_AImainRB.Sleep();
        }        

        for (int i = 0; i < m_AImeshes.Length; i++)
        {
            m_AImeshes[i].enabled = state;
        }

        for (int i = 0; i < m_AIColliders.Length; i++)
        {
            m_AIColliders[i].enabled = state;
        }
    }
}
