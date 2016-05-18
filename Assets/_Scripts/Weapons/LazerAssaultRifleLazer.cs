using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class LazerAssaultRifleLazer : MonoBehaviour
{
    public GameObject m_projectile, m_hitEffect;

    public AudioClip m_flightSound, m_hitSound;

    public float m_projectileForce = 3.0f, m_damage = 0.1f;

    private Rigidbody m_rb;
    private AudioSource m_audioSource;

    void Awake ()
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

    void OnEnable ()
    {
        StartCoroutine(DeactivateTimer(10.0f, gameObject));
        m_projectile.SetActive(true);
        m_rb.detectCollisions = true;
        m_rb.WakeUp();
                
        m_audioSource.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (!m_projectile.activeInHierarchy && !m_hitEffect.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
	}

    IEnumerator DeactivateTimer(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    void OnCollisionEnter (Collision other)
    {      
        m_projectile.SetActive(false);
        m_rb.detectCollisions = false;
        m_rb.Sleep();

        m_hitEffect.SetActive(true);

        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_hitSound);

        if (other.rigidbody != null)
        {
            other.rigidbody.AddForceAtPosition(transform.forward * m_projectileForce, other.contacts[0].point);
        }

        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Player") //add player tag check when you add player health!!!
        {
            Debug.Log(other.gameObject.name);

            if (other.gameObject.tag == "Player")
            {
                Debug.Log("hit player!");
            }

            Health enemyHealth = other.gameObject.GetComponent<Health>();

            if (enemyHealth != null)
            {
                Debug.Log("damage health!!!");

                enemyHealth.TakeDamage(m_damage);
            }
        }

        StartCoroutine(DeactivateTimer(0.05f, m_hitEffect));
        StartCoroutine(DeactivateTimer(0.06f, gameObject));        
    }
}
