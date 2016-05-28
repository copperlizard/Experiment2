using UnityEngine;
using System.Collections;

public class BoxSounds : MonoBehaviour
{
    private AudioSource m_audioSource;

    private Rigidbody m_rb;

	// Use this for initialization
	void Start ()
    {
        m_audioSource = GetComponent<AudioSource>();

        m_rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter (Collision other)
    {
        if (!m_audioSource.isPlaying)
        {
            m_audioSource.pitch = Random.Range(0.8f, 1.0f);

            float volume;

            if (other.rigidbody != null)
            {
                volume = (m_rb.velocity.magnitude + other.rigidbody.velocity.magnitude) / 10.0f;
            }
            else
            {
                volume = m_rb.velocity.magnitude / 5.0f;
            }

            m_audioSource.volume = Mathf.SmoothStep(0.5f, 1.0f, volume);

            m_audioSource.Play();
        }
    }
}
