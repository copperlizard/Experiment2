using UnityEngine;
using System.Collections;

public class GravityLauncher : MonoBehaviour
{
    public float m_launcherForce;

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerStay(Collider other)
    {
        Debug.Log("hello " + other.name);

        if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.AddForce(transform.up * m_launcherForce);
        }
    }
}
