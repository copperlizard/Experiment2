using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResetSwitch : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(5.0f * Time.deltaTime, 15.0f * Time.deltaTime, 30.0f * Time.deltaTime);
	}

    void OnCollisionEnter(Collision other)
    {
        SceneManager.LoadScene(0);
    }
}
