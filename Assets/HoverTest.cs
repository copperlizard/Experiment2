using UnityEngine;
using System.Collections;

public class HoverTest : MonoBehaviour
{
    public float m_hoverForce;

    private Rigidbody m_rb;

    void Start ()
    {
        m_rb = GetComponent<Rigidbody>();
    }

	// Update is called once per frame
	void Update ()
    {
        m_rb.AddForce(transform.up * m_hoverForce);
	}
}
