using UnityEngine;
using System.Collections;

public class OrbitCam : MonoBehaviour
{
    public GameObject m_target; //cam follow target
    public RaycastHit m_hit; //player target
    public float m_minDist = 0.0f, m_maxDist = 100.0f, m_startDist = 5.0f, m_minTilt, m_maxTilt, m_hidePlayerDist, m_rotSpeed, m_damp, m_fudge;
    public bool m_HideCursor = true;
    
    private SkinnedMeshRenderer[] m_meshRenderers;
    private RaycastHit m_interAt;
    private Quaternion m_rot;
    private Vector3 m_curVel = Vector3.zero;
    private float m_h, m_v, m_d, m_dist;
    private bool m_playerHidden = false;

	// Use this for initialization
	void Start()
    {            
        m_dist = m_startDist;
        transform.position = m_target.transform.position + new Vector3(0.0f, 0.0f, -m_dist);

        if(m_HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("no player found! (ensure player tagged player)");
        }

        m_meshRenderers = player.GetComponentsInChildren<SkinnedMeshRenderer>(true);        
	}
	
	// Update is called once per frame
	void Update()
    {
        GetInput();
	}

    // LateUpdate is called once per frame after Update
    void LateUpdate()
    {
        // Calculate m_rot
        //m_rot = Quaternion.Euler(m_v * m_rotSpeed, Mathf.Clamp(m_h * m_rotSpeed, m_minTilt, m_maxTilt), 0.0f);  
        //m_rot = Quaternion.Euler(m_v * m_rotSpeed, m_h * m_rotSpeed, 0.0f);
        
        float tilt;
        if (m_v * m_rotSpeed < 180.0f)
        {
            tilt = m_v * m_rotSpeed;
        }
        else
        {
            tilt = (m_v * m_rotSpeed) - 360.0f;
        }
        
        if (tilt >= m_maxTilt)
        {
            tilt = m_maxTilt;
            m_v = m_maxTilt / m_rotSpeed;
        }
        else if (tilt < m_minTilt)
        {
            tilt = m_minTilt;
            m_v = (m_minTilt + 360.0f) / m_rotSpeed;
        }

        if (tilt < 0.0f)
        {
            tilt += 360.0f;
        }

        m_rot = Quaternion.Euler(tilt, m_h * m_rotSpeed, 0.0f);
        
        // Find new cam position
        m_dist -= m_d;        
        m_dist = Mathf.Clamp(m_dist, m_minDist, m_maxDist);
        Vector3 tarPos = (m_rot * new Vector3(0.0f, 0.0f, -m_dist)) + m_target.transform.position;

        //Check for sight line intersection
        tarPos = IntersectCheck(tarPos);

        //Hide player and weapons if camera too close
        float distFromPlayer = (m_target.transform.position - tarPos).magnitude;
        
        if (distFromPlayer <= m_hidePlayerDist && !m_playerHidden)
        {
            //Debug.Log("Hiding player!");

            m_playerHidden = true;
            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //Debug.Log("Hiding mesh " + i.ToString() + " ; " + m_meshRenderers[i].name.ToString());

                m_meshRenderers[i].enabled = false;
            }
        }
        else if (distFromPlayer > m_hidePlayerDist && m_playerHidden)
        {
            //Debug.Log("Un Hiding player!");

            m_playerHidden = false;
            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                //Debug.Log("Un Hiding mesh " + i.ToString());

                m_meshRenderers[i].enabled = true;
            }
        }

        //Move camera
        transform.position = Vector3.SmoothDamp(transform.position, tarPos, ref m_curVel, m_damp);
        transform.rotation = m_rot;

        //Find "hit"
        if(!Physics.Raycast(transform.position, transform.forward, out m_hit, m_maxDist, LayerMask.NameToLayer("Player")))
        {
            m_hit.point = transform.position + transform.forward * m_maxDist;
            m_hit.normal = Vector3.up;
        }        
    }

    void GetInput()
    {
        bool aiming = Input.GetMouseButton(1);
        m_h += Input.GetAxis("Mouse X") * ((aiming) ? 0.5f : 1.0f);
        m_v -= Input.GetAxis("Mouse Y") * ((aiming) ? 0.5f : 1.0f);
        //m_d = Input.GetAxis("Mouse ScrollWheel");
    }

    Vector3 IntersectCheck(Vector3 target)
    {
        //If intersection (cast ray from camera to player)
        if (Physics.Raycast(m_target.transform.position, target - m_target.transform.position, out m_interAt, m_dist, ~LayerMask.GetMask("Player")))
        {
            //Debug.Log("INTERSECTION!!! " + m_interAt.collider.gameObject.tag.ToString() + " ; " + LayerMask.LayerToName(m_interAt.collider.gameObject.layer));
            
            if (m_interAt.collider.gameObject.tag == "Player")
            {
                return target;
            } 
               

#if UNITY_EDITOR
            Debug.DrawLine(m_target.transform.position, m_interAt.point, Color.yellow, 0.01f, true);
#endif      

            target = m_interAt.point;
        }

        return target;
    }

    public void SetCamDist(float dist)
    {
        m_dist = Mathf.Clamp(dist, m_minDist, m_maxDist);
    }

    public float GetCamDist()
    {
        return m_dist;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_hit.point, 0.05f);            
        }
    }
#endif

    float ClampAngle(float ang, float min, float max)
    {
        if (ang < -360.0f)
        {
            ang += 360.0f;
        }
        else if (ang > 360.0f)
        {
            ang -= 360.0f;
        }
        return Mathf.Clamp(ang, min, max);
    }    
}
