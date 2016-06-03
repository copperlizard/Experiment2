using UnityEngine;
using System.Collections;

public class AIOrbitCam : OrbitCam
{
    public GameObject m_enemy, m_targetTarget;
    public PlayerWeaponController m_weaponController;
        
    public Vector3 m_targetTargetOffset;
    public float m_firingRange, m_rotInterRate;

    [HideInInspector]
    public int m_nextWeapon = 0;

    [HideInInspector]
    public bool m_aiming = false, m_fire = false;

    private Rigidbody m_enemyRB;
    private Weapon m_curWeapon;

    private RaycastHit m_sightHit;
    private Vector3 m_toTar;

    private float m_dv, m_dh;
    private int m_lastWeapon = -1;

    private bool m_selectWeapon = true;
    
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

        m_lastWeapon = m_weaponController.m_curWeaponNum;
        m_curWeapon = m_weaponController.m_weapons[m_lastWeapon].GetComponent<Weapon>();

        m_enemyRB = m_enemy.GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        AI();
    }

    IEnumerator SelectWeapon()
    {
        m_selectWeapon = false;
        
        while (m_curWeapon.m_thisMagazine < m_curWeapon.m_magazineSize)
        {
            //Debug.Log("waiting for reload");
            yield return null;
        }

        m_nextWeapon = (Random.Range(0.0f, 1.0f) > 0.5f) ? 0 : 1;
        
        float wepDuration = Random.Range(3.0f, 30.0f);
        float startTime = Time.time;

        while (Time.time - startTime < wepDuration)
        {            
            yield return null;
        }

        m_selectWeapon = true;
    }

    void AI()
    {
        /*
        if (m_selectWeapon)
        {
            StartCoroutine(SelectWeapon());
        }
        */
        

        if (SeeTarget())
        {   
            AimCam();
            m_aiming = true;
        }
        else
        {
            //Debug.Log("can't see player!");

            HomeCam();
            m_fire = false;
        }
    }

    bool SeeTarget()
    {
        if (m_enemyRB.velocity.magnitude > 0.1f)
        {
            PredictTargetPosition();
        }
        else
        {
            m_toTar = (m_targetTarget.transform.position + m_targetTargetOffset) - m_target.transform.position;
        }

        //Target too far away
        if (m_toTar.magnitude > m_firingRange)
        {
            //Debug.Log("enemy too far!");
            return false;
        }

        //Debug.DrawLine(m_target.transform.position, m_target.transform.position + m_toTar, Color.red);        

        bool playerVisible = !Physics.Raycast(m_target.transform.position, m_toTar.normalized, m_toTar.magnitude, ~LayerMask.GetMask("AIEnemy", "Player", "Ignore Raycast"));
        
        bool lineOfSight = (Vector3.Dot(transform.forward, m_toTar.normalized) > 0.0f) ? true : false;

        //Debug.Log("playerVisible == " + playerVisible.ToString() + " ; lineOfSight == " + lineOfSight.ToString());

        return (playerVisible && lineOfSight);
    }

    void PredictTargetPosition ()
    {
        if (m_lastWeapon != m_weaponController.m_curWeaponNum)
        {
            Debug.Log("Fetching weapon stats!");            
            m_lastWeapon = m_weaponController.m_curWeaponNum;
            m_curWeapon = m_weaponController.m_weapons[m_lastWeapon].GetComponent<Weapon>();
        }

        Vector3 playerStartPos = m_targetTarget.transform.position + m_targetTargetOffset;        
        Vector3 bulletStartPos = m_curWeapon.m_firePos.position;

        Vector3 playerFuturePos = playerStartPos + (m_enemyRB.velocity * Time.fixedDeltaTime * 30.0f);
        
        Vector3 toPlayerFuturePos = playerFuturePos - m_target.transform.position;
        Vector3 bulletFuturePos = m_target.transform.position + (toPlayerFuturePos.normalized * m_curWeapon.m_projectileSpeed * Time.fixedDeltaTime * 30.0f);
                
        Vector3 predictedPlayerPath = playerFuturePos - playerStartPos; //P
        Vector3 predictedBulletPoss = bulletFuturePos - bulletStartPos; //Q
        
        //Debug.DrawLine(playerStartPos, playerStartPos + predictedPlayerPath, Color.blue);
        //Debug.DrawLine(bulletStartPos, bulletStartPos + predictedBulletPoss, Color.magenta);

        //Find closest point on P to Q
        Vector3 w = playerStartPos - bulletStartPos;
        float a = Vector3.Dot(predictedPlayerPath, predictedPlayerPath);
        float b = Vector3.Dot(predictedPlayerPath, predictedBulletPoss);
        float c = Vector3.Dot(predictedBulletPoss, predictedBulletPoss);
        float d = Vector3.Dot(predictedPlayerPath, w); 
        float e = Vector3.Dot(predictedBulletPoss, w);

        float sc = (b*e - c*d) / (a*c - b*b);

        Vector3 predictedTarPos = (m_targetTarget.transform.position + m_targetTargetOffset) + (predictedPlayerPath.normalized * sc);

        //Debug.DrawLine(m_target.transform.position, predictedTarPos, Color.cyan);

        m_toTar = predictedTarPos - m_target.transform.position;
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

        if (fCheck > 0.95f)
        {
            m_aiming = false;
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
}
