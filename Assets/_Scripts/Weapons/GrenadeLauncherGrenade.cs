using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GrenadeLauncherGrenade : MonoBehaviour
{
    public GameObject m_projectile, m_explosion;

    public AudioClip m_timerSound, m_collisonSound, m_explosionSound;

    public float m_explosionForce, m_explosionRadius, m_upMod, m_explosionTime, m_detonateTime, m_agitatedTimer;

    private Rigidbody m_rb;
    private AudioSource m_audioSource;

    private bool m_detonated = false, m_shortFuse = false;

    void Awake ()
    {
        m_rb = GetComponent<Rigidbody>();
        m_audioSource = GetComponent<AudioSource>();

        m_audioSource.loop = true;
        m_audioSource.clip = m_timerSound;
    }

	// Use this for initialization
	void Start ()
    {
	
	}

    void OnEnable()
    {
        m_detonated = false;
        m_shortFuse = false;
                        
        StartCoroutine(DetonateTimer(m_detonateTime));
        m_projectile.SetActive(true);

        m_rb.detectCollisions = true;
        m_rb.WakeUp();

        //Timer sound
        m_audioSource.Play();
    }

    // Update is called once per frame
    void Update ()
    {
	
	}

    IEnumerator DeactivateTimer(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    IEnumerator DetonateTimer(float time)
    {
        yield return new WaitForSeconds(time);
        Detonate();
    }

    void Detonate ()
    {
        if (m_detonated)
        {
            return; //ensures only one explosion
        }

        m_detonated = true;

        m_projectile.SetActive(false);
        m_rb.detectCollisions = false;
        m_rb.Sleep();

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

        StartCoroutine(DeactivateTimer(m_explosionTime, m_explosion));
        StartCoroutine(DeactivateTimer(m_explosionTime + 0.1f, gameObject));
    }

    void OnCollisionEnter (Collision other)
    {
        m_audioSource.PlayOneShot(m_collisonSound);

        if (!m_shortFuse)
        {
            StartCoroutine(DetonateTimer(m_agitatedTimer));
            m_shortFuse = true;
        }        
    }
}
