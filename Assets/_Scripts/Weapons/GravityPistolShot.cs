using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class GravityPistolShot : MonoBehaviour
{
    public GameObject m_projectile, m_explosion, m_implosion;

    public AudioClip m_flightSound, m_explosionSound, m_implosionSound;

    public float m_explosionForce, m_implosionForce;

    private Rigidbody m_rb;
    private AudioSource m_audioSource;

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

    IEnumerator DeactivateTimer(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }
}
