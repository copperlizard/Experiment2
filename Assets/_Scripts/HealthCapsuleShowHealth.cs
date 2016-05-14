using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class HealthCapsuleShowHealth : MonoBehaviour
{
    private Health m_health;

    private Renderer m_rend;

	// Use this for initialization
	void Start ()
    {
        m_health = gameObject.GetComponent<Health>();

        m_rend = gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_rend.material.color = MixColors();	
	}

    private Color MixColors()
    {
        Color red = Color.red * (1.0f - m_health.GetHealth());
        Color green = Color.green * m_health.GetHealth();

        return (red + green);
    }
}
