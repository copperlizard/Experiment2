﻿using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public float m_healthRegen, m_damageMultiplier = 1.0f;

    public bool m_regenHealth = false, m_takesExplosionDamage = false;

    private float m_health = 1.0f;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_regenHealth)
        {
            RegenerateHealth();
        }
	}

    public virtual void RegenerateHealth ()
    {
        m_health = Mathf.Clamp(m_health + (m_healthRegen * Time.deltaTime), 0.0f, 1.0f);
    }

    public virtual void TakeDamage (float damage)
    {
        m_health = Mathf.Clamp(m_health - (damage * m_damageMultiplier), 0.0f, 1.0f);
    }

    public virtual void TakeExplosionDamage(float damage)
    {
        if (m_takesExplosionDamage)
        {
            m_health = Mathf.Clamp(m_health - (damage * m_damageMultiplier), 0.0f, 1.0f);
        }
    }

    public virtual void GainHealth (float healthGain)
    {
        m_health = Mathf.Clamp(m_health + healthGain, 0.0f, 1.0f);
    }

    public virtual float GetHealth ()
    {
        return m_health;
    }

    public virtual void SetHealth (float health)
    {
        m_health = Mathf.Clamp(health, 0.0f, 1.0f);
    }
}
