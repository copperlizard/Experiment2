using UnityEngine;
using System.Collections;

public class AIOrbitCam : OrbitCam
{
    public GameObject m_enemy;
    public float m_firingRange;

    private RaycastHit m_sightHit;
    private Vector3 m_toTar;

    // Use this for initialization
    public override void Start ()
    {
        m_thisCam = GetComponent<Camera>();
        m_thisLayerMask = ~LayerMask.GetMask("AIEnemy", "Ignore Raycast");

        m_dist = m_startDist;
        transform.position = m_target.transform.position + new Vector3(0.0f, 0.0f, -m_dist);

        if (m_HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        AI();
	}

    void AI()
    {
        if (SeeTarget())
        {
            Debug.Log("can see enemy!");
        }
        else
        {
            Debug.Log("cannot see enemy!");
        }
    }

    bool SeeTarget()
    {
        m_toTar = m_enemy.transform.position - m_target.transform.position;

        //Target too far away
        if (m_toTar.magnitude > m_firingRange)
        {
            Debug.Log("enemy too far!");

            return false;
        }

        RaycastHit wtf;

        bool playerVisible = !Physics.Raycast(m_target.transform.position, m_toTar.normalized, out wtf, m_firingRange, ~LayerMask.GetMask("AIEnemy", "Player"));

        if(!playerVisible)
        {
            Debug.Log(wtf.collider.gameObject.name);
        }

#if UNITY_EDITOR
        Debug.DrawLine(m_target.transform.position, m_target.transform.position + m_toTar.normalized * m_firingRange);
#endif

        Debug.Log("playerVisible == " + playerVisible.ToString());

        //If in front of AI
        bool lineOfSight = (Vector3.Dot(transform.forward, m_toTar.normalized) > 0.5f) ? true : false;

        Debug.Log("lineOfSight == " + lineOfSight.ToString());

        return (playerVisible && lineOfSight);
    }
}
