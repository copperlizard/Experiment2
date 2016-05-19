using UnityEngine;
using System.Collections;

public class EnemySphereWeaponController : MonoBehaviour
{
    public GameObject m_target, m_weapon, m_firePos, m_muzzleFlash;

    public AudioSource m_wepAudioSource;
    public AudioClip m_wepFireNoise;

    public Vector3 m_targetOffset = new Vector3(0.0f, 1.5f, 0.0f);
    public float m_fireRange = 20.0f, m_projectileSpeed = 60.0f;

    private Health m_health;
    private ObjectPool m_ammo;
    private RaycastHit m_hit;

    private bool m_fired = false;

	// Use this for initialization
	void Start ()
    {
        m_ammo = m_weapon.GetComponentInChildren<ObjectPool>();

        m_health = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        FaceTarget();

        AimWeapon();

        if (m_health.GetHealth() == 0.0f)
        {
            m_fired = false;
            gameObject.SetActive(false);
        }
    }

    private void FaceTarget ()
    {
        Vector3 toTar = m_target.transform.position - transform.position;

        toTar = Vector3.ProjectOnPlane(toTar, Vector3.up);

        Quaternion tarRot = Quaternion.LookRotation(toTar);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, tarRot, 10.0f);
    }

    private void AimWeapon ()
    {
        Vector3 toTar = ((m_target.transform.position + m_targetOffset) - m_weapon.transform.position);

        float dist = toTar.magnitude;

        toTar = toTar.normalized;

        bool tarObstructed = Physics.Raycast(m_weapon.transform.position, toTar, out m_hit, m_fireRange, ~LayerMask.GetMask("Player"));

        if (!tarObstructed && dist <= m_fireRange)
        {
            Quaternion tarRot = Quaternion.LookRotation(toTar);
            m_weapon.transform.rotation = Quaternion.Lerp(m_weapon.transform.rotation, tarRot, 0.1f);

            if (Vector3.Dot(m_weapon.transform.forward, toTar) > 0.95f)
            {
                FireWeapon();
            }
        }
        else
        {
            m_weapon.transform.rotation = Quaternion.Lerp(m_weapon.transform.rotation, transform.rotation, 0.1f);
        }
    }

    private void FireWeapon ()
    {
        if (!m_fired)
        {
            m_fired = true;
            StartCoroutine(Firing());
        }
    }

    IEnumerator Firing ()
    {
        m_muzzleFlash.SetActive(true);

        GameObject shot = m_ammo.GetObject();

        shot.transform.position = m_firePos.transform.position;
        shot.transform.rotation = m_weapon.transform.rotation;

        Rigidbody shotRB = shot.GetComponent<Rigidbody>();
        shot.SetActive(true);
        shotRB.velocity = m_firePos.transform.forward * m_projectileSpeed;

        m_wepAudioSource.PlayOneShot(m_wepFireNoise);

        yield return new WaitForSeconds(0.05f);

        m_muzzleFlash.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        m_fired = false;        
    }
}
