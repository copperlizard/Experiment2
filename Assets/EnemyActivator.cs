using UnityEngine;
using System.Collections;

public class EnemyActivator : MonoBehaviour
{
    public GameObject m_enemy;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter ()
    {
        if (!m_enemy.activeInHierarchy)
        {
            m_enemy.SetActive(true);
        }
    }
}
