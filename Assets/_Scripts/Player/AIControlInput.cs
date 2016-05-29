using UnityEngine;
using System.Collections;

public class AIControlInput : MonoBehaviour
{
    public GameObject m_target;
    
    public AIOrbitCam m_AICam;

    public float m_minRange;

    private PlayerMovementController m_mover;
    private PlayerWeaponController m_weaponController;
    private Weapon m_curWeapon;

    private NavMeshAgent m_navAgent;
    private NavMeshPath m_curPath;
    private NavMeshHit m_navHit;

    private Vector3 m_toTar;
    private float m_v, m_h;
    private int m_wepNum, m_curCorner;
    private bool m_aiming, m_fire, m_fireCooldown, m_reload,
        m_jump, m_crouch, m_walk, m_sprint, m_holster, m_freshPath = false;

    // Use this for initialization
    void Start ()
    {
        m_mover = GetComponent<PlayerMovementController>();
        m_weaponController = GetComponent<PlayerWeaponController>();
        m_curWeapon = m_weaponController.m_weapons[m_weaponController.m_curWeaponNum].GetComponent<Weapon>();

        m_navAgent = GetComponentInChildren<NavMeshAgent>();
        m_curPath = new NavMeshPath();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!DirectPath(m_target.transform.position))
        {
            FindPath();
            //Debug.Log("no direct path!");
            AIMove(true);
        }
        else
        {
            m_freshPath = false;
            AIMove(false);
        }
        
        if (m_AICam.m_aiming)
        {
            m_aiming = true;
        }
        else
        {
            m_aiming = false;
        }
        
        if (m_AICam.m_fire && !m_fireCooldown)
        {
            //Debug.Log("firing weapon!");
            StartCoroutine(BurstFire(Random.Range(1.0f, 1.5f)));
        }
        else
        {
            m_fire = false;
        }
        
        if (m_curWeapon.m_thisMagazine == 0)
        {
            m_reload = true;
        }
        else
        {
            m_reload = false;
        }

#if UNITY_EDITOR
        if (m_freshPath)
        {
            //Debug.Log("drawing path!");

            for (int i = 0; i < m_curPath.corners.Length - 1; i++)
            {
                Debug.DrawLine(m_curPath.corners[i], m_curPath.corners[i + 1], Color.green);
            }
        }
#endif
    }

    void FixedUpdate ()
    {
        m_mover.Move(m_v, m_h, m_fire, m_aiming, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster, m_wepNum);
    }

    private void OnEnable ()
    {
        if (m_weaponController == null)
        {
            return;
        }

        if (!m_weaponController.enabled)
        {
            m_weaponController.enabled = true;

            m_mover.Move(0.0f, 0.0f, false, false, true, false, false, false, false, false, 0);
        }
    }

    private void OnDisable()
    {
        if (m_weaponController == null || m_mover == null)
        {
            return;
        }

        if (m_weaponController.enabled)
        {
            m_mover.Move(0.0f, 0.0f, false, false, true, false, false, false, false, true, 0);
            
            m_weaponController.enabled = false;
        }
    }

    bool DirectPath(Vector3 pos)
    {
        return !m_navAgent.Raycast(pos, out m_navHit);
    }

    void FindPath ()
    {
        if (!m_freshPath)
        {
            //Debug.Log("finding fresh path");

            m_freshPath = m_navAgent.CalculatePath(m_target.transform.position, m_curPath);
            m_curCorner = 0;
        }
    }

    void AIMove (bool path)
    {
        Vector3 toTar = m_target.transform.position - transform.position;

        toTar = Vector3.ProjectOnPlane(toTar, Vector3.up);

        toTar = transform.InverseTransformDirection(toTar);

        //Debug.DrawLine(transform.position, transform.position + toTar, Color.yellow);

        if (toTar.magnitude > m_minRange)
        {
            //Debug.Log("moving!!!");

            if (!path)
            {
                m_v = toTar.normalized.z;
                m_h = toTar.normalized.x;                
            }
            else
            {
                //Debug.Log("traversing path!");
                                
                TraversePath();
            }
        }
        else
        {
            //Debug.Log("enemy too close!");

            m_v = Mathf.Lerp(m_v, 0.0f, 0.5f);
            m_h = Mathf.Lerp(m_h, 0.0f, 0.5f);
        }
    }

    private void TraversePath()
    {
        if (m_freshPath)
        {
            Vector3 toTar = m_curPath.corners[m_curCorner] - transform.position;

            Debug.DrawLine(transform.position, transform.position + toTar, Color.yellow);

            if (toTar.magnitude < 2.0f)
            {
                m_curCorner++;

                if (m_curCorner >= m_curPath.corners.Length - 1)
                {
                    //Debug.Log("reached path end!");

                    m_freshPath = false;
                    TraversePath(); //start over
                    return;
                }                
            }
            else
            {
                //Debug.Log("approaching path corner!");

                if (!DirectPath(m_curPath.corners[m_curCorner]))
                {
                    //Debug.Log("path interupted!");

                    m_freshPath = false;
                    TraversePath();
                    return;
                }

                toTar = transform.InverseTransformDirection(toTar);                
                m_v = toTar.normalized.z;
                m_h = toTar.normalized.x;
            }
        }
        else
        {
            //Debug.Log("finding new path!");

            FindPath();  //not traversed until next call
        }
    }

    IEnumerator BurstFire(float time)
    {
        //Debug.Log("firing for " + time.ToString());

        m_fireCooldown = true;
        float startTime = Time.time;
        while (Time.time - startTime < time)
        {
            //Debug.Log("burst");
            m_fire = true;
            yield return null;
        }        
        m_fire = false;
        StartCoroutine(FireCooldown(Random.Range(0.25f, 0.75f)));
    }

    IEnumerator FireCooldown(float time)
    {
        //Debug.Log("cooldown for " + time.ToString());

        float startTime = Time.time;
        while (Time.time - startTime < time)
        {
            //Debug.Log("cooldown");
            yield return null;
        }
        m_fireCooldown = false;
    }
}
