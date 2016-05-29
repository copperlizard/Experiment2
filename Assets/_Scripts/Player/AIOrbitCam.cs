using UnityEngine;
using System.Collections;

public class AIOrbitCam : OrbitCam
{
    public GameObject m_enemy;
    public Vector3 m_enemyOffset;
    public float m_firingRange, m_rotInterRate;

    [HideInInspector]
    public bool m_aiming = false, m_fire = false;

    private RaycastHit m_sightHit;
    private Vector3 m_toTar;

    private float m_dv, m_dh;
    
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
            AimCam();
            m_aiming = true;
        }
        else
        {
            HomeCam();
            m_fire = false;
            m_aiming = false;
        }
    }

    bool SeeTarget()
    {
        m_toTar = (m_enemy.transform.position + m_enemyOffset) - m_target.transform.position;

        //Target too far away
        if (m_toTar.magnitude > m_firingRange)
        {
            //Debug.Log("enemy too far!");

            return false;
        }

        bool playerVisible = !Physics.Raycast(m_target.transform.position, m_toTar.normalized, m_toTar.magnitude, ~LayerMask.GetMask("AIEnemy", "Player"));
        
        /*
#if UNITY_EDITOR
        Debug.DrawLine(m_target.transform.position, m_enemy.transform.position);
#endif
        */

        bool lineOfSight = (Vector3.Dot(transform.forward, m_toTar.normalized) > 0.5f) ? true : false;

        return (playerVisible && lineOfSight);
    }

    void AimCam ()
    {
        float fCheck = Vector3.Dot(transform.forward, m_toTar.normalized);
        float uCheck = Vector3.Dot(transform.up, m_toTar.normalized);
        float rCheck = Vector3.Dot(transform.right, m_toTar.normalized);

        if (fCheck > 0.8f && m_toTar.magnitude <= m_firingRange)
        {
            m_fire = true;
        }
        else
        {
            m_fire = false;
        }
        
        if (uCheck > 0.0f)
        {
            m_dv = Mathf.Lerp(m_dv, -m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_v += m_dv;
        }
        else
        {
            m_dv = Mathf.Lerp(m_dv, m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_v += m_dv;
        }
        
        if (rCheck > 0.0f)
        {
            m_dh = Mathf.Lerp(m_dh, m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_h += m_dh;
        }
        else
        {
            m_dh = Mathf.Lerp(m_dh, -m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_h += m_dh;
        }
    }

    void HomeCam ()
    {
        float fCheck = Vector3.Dot(transform.forward, m_target.transform.forward);
        float uCheck = Vector3.Dot(transform.up, m_target.transform.forward);
        float rCheck = Vector3.Dot(transform.right, m_target.transform.forward);

        if (uCheck > 0.0f)
        {
            m_dv = Mathf.Lerp(m_dv, -m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_v += m_dv;
        }
        else
        {
            m_dv = Mathf.Lerp(m_dv, m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_v += m_dv;
        }

        if (rCheck > 0.0f)
        {
            m_dh = Mathf.Lerp(m_dh, m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_h += m_dh;
        }
        else
        {
            m_dh = Mathf.Lerp(m_dh, -m_rotSpeed * (1.0f - fCheck), m_rotInterRate);
            m_h += m_dh;
        }
    }
}
