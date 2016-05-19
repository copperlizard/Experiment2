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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hello " + other.name);
    }
}
