using UnityEngine;
using System.Collections;

public class HoverTest2 : MonoBehaviour
{
    public Collider other;
    public float m_hoverForce;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        other.attachedRigidbody.AddForce(transform.up * m_hoverForce);
	}
}
