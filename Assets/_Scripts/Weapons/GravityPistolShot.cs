using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class GravityPistolShot : MonoBehaviour
{
    public GameObject m_projectile, m_explosion, m_implosion;

    public AudioClip m_flightSound, m_explosionSound, m_implosionSound;

    public float m_explosionForce, m_explosionRadius, m_explosionDuration, m_implosionForce, m_implosionRadius, m_implosionDuration, 
        m_upMod, m_detonateTime, m_growthRate;

    private Rigidbody m_rb;
    private AudioSource m_audioSource;

    private bool m_detonated = false;

    void Awake()
    {
        m_rb = GetComponent<Rigidbody>();
        m_audioSource = GetComponent<AudioSource>();

        m_audioSource.loop = true;
        m_audioSource.clip = m_flightSound;
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnEnable ()
    {
        m_detonated = false;

        transform.localScale = Vector3.one * 0.1f;
        
        StartCoroutine(Grow());
        StartCoroutine(DetonateTimer(m_detonateTime));
        m_projectile.SetActive(true);

        m_rb.detectCollisions = true;
        m_rb.WakeUp();

        //Apply random rotation???
        m_rb.angularVelocity = transform.forward;

        m_audioSource.Play();
    }

    IEnumerator DeactivateTimer(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    IEnumerator Grow()
    {
        while (transform.localScale.x < 1.0f)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(transform.localScale.x, 1.0f, m_growthRate * Time.deltaTime);

            if (transform.localScale.x > 0.99f)
            {
                transform.localScale = Vector3.one;
            }

            yield return null;
        }
    }

    IEnumerator DetonateTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Detonate();
    }

    void OnCollisionEnter (Collision other)
    {
        Detonate();
    }

    void Detonate ()
    {
        if (!m_detonated)
        {
            m_detonated = true;

            m_projectile.SetActive(false);
            m_rb.detectCollisions = false;
            m_rb.Sleep();

            Explode();
        }      
    }

    void Explode ()
    {
        m_audioSource.Stop();

        m_explosion.SetActive(true);
        m_audioSource.PlayOneShot(m_explosionSound);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_explosionRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].attachedRigidbody != null)
            {
                hitColliders[i].attachedRigidbody.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius, m_upMod, ForceMode.Impulse);
            }
        }

        StartCoroutine(Implode(m_explosionDuration));
    }

    IEnumerator Implode (float waittime)
    {
        yield return new WaitForSeconds(waittime);

        m_explosion.SetActive(false);

        m_implosion.SetActive(true);
        m_audioSource.PlayOneShot(m_implosionSound);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_implosionRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].attachedRigidbody != null)
            {
                hitColliders[i].attachedRigidbody.AddExplosionForce(-m_implosionForce, transform.position, m_implosionRadius, m_upMod, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(m_implosionDuration);
        m_implosion.SetActive(false);
        gameObject.SetActive(false);
    }
}
