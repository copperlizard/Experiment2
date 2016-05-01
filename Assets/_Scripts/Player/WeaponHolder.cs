using UnityEngine;
using System.Collections;

public class WeaponHolder : MonoBehaviour
{
    public GameObject m_leftShoulder, m_rightShoulder; //Consider adding ability to switch shoulders...

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = m_rightShoulder.transform.position;	
	}
}
