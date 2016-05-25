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
    private bool m_aiming, m_fire, m_fireCooldown, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster;
    private int m_wepNum, m_curCorner;

    // Use this for initialization
    void Start ()
    {
        m_mover = GetComponent<PlayerMovementController>();
        m_weaponController = GetComponent<PlayerWeaponController>();
        m_curWeapon = m_weaponController.m_weapons[m_weaponController.m_curWeaponNum].GetComponent<Weapon>();

        m_navAgent = GetComponentInChildren<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!DirectPath())
        {
            FindPath();
            Debug.Log("no direct path!");
            AIMove(true);
        }
        AIMove(false);
        
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
            m_fire = true;
            StartCoroutine(BurstFire(Random.Range(0.1f, 0.3f)));
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
    }

    void FixedUpdate ()
    {
        m_mover.Move(m_v, m_h, m_fire, m_aiming, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster, m_wepNum);
    }
    
    bool DirectPath()
    {
        return !m_navAgent.Raycast(m_target.transform.position, out m_navHit);
    }

    void FindPath ()
    {
        m_navAgent.CalculatePath(m_target.transform.position, m_curPath);
        m_curCorner = 0;
    }

    void AIMove (bool path)
    {
        Vector3 toTar = m_target.transform.position - transform.position;

        if (toTar.magnitude > m_minRange)
        {
            Debug.Log("moving!!!");

            if (!path)
            {
                if (!m_aiming)
                {   
                    Debug.Log("direct move!");
                }
                else
                {
                    m_v = 1.0f;

                    Debug.Log("aim move!");
                }
            }
            else
            {                
                TraversePath();
            }
        }
        else
        {
            Debug.Log("enemy too close!");

            m_v = 0.0f;
            m_h = 0.0f;
        }
    }

    private void TraversePath()
    {
        Vector3 toTar = m_curPath.corners[m_curCorner] - transform.position;

        if (toTar.magnitude < 0.1f)
        {
            m_curCorner++;

            if (m_curCorner >= m_curPath.corners.Length)
            {
                FindPath();
                TraversePath();
            }
            else
            {
                toTar = m_curPath.corners[m_curCorner] - transform.position;
                m_v = toTar.normalized.z;
                m_h = toTar.normalized.x;
            }
        }
        else
        {
            m_v = toTar.normalized.z;
            m_h = toTar.normalized.x;
        }

        
    }

    IEnumerator BurstFire(float time)
    {
        m_fireCooldown = true;
        float startTime = Time.time;
        while (Time.time - startTime < time && m_fire)
        {
            yield return null;
        }        
        m_fire = false;
        StartCoroutine(FireCooldown(Random.Range(time, 0.3f)));
    }

    IEnumerator FireCooldown(float time)
    {   
        float startTime = Time.time;
        while (Time.time - startTime < time && m_fire)
        {
            yield return null;
        }
        m_fireCooldown = false;
    }
}
