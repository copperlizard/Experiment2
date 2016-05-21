using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class GravityLauncher : MonoBehaviour
{
    public AudioClip m_launchSound;
    public float m_launcherForce;

    private AudioSource m_audioSource;

    // Use this for initialization
    void Start ()
    {
        m_audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.AddForce(transform.up * m_launcherForce);
            m_audioSource.Stop();
            m_audioSource.PlayOneShot(m_launchSound);
            m_audioSource.PlayDelayed(0.1f);
        }
    }
}
