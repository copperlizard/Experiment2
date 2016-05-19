using UnityEngine;
using System.Collections;

public class EnemyActivator : MonoBehaviour
{
    public GameObject m_enemy, m_smoke;
    public float m_smokeTime;

    private Health m_enemyHealth;
    private Vector3 m_startPos;

	// Use this for initialization
	void Start ()
    {
        m_startPos = m_enemy.transform.position;

        m_enemyHealth = m_enemy.GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    IEnumerator DeactivateTimer(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    void OnCollisionEnter ()
    {
        if (!m_enemy.activeInHierarchy)
        {
            m_enemy.transform.position = m_startPos;
            m_enemyHealth.SetHealth(1.0f);

            m_smoke.SetActive(true);
            StartCoroutine(DeactivateTimer(m_smokeTime, m_smoke));
            m_enemy.SetActive(true);
        }
    }
}
