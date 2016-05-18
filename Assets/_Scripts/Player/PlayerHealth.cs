using UnityEngine;
using System.Collections;

public class PlayerHealth : Health
{
    public PlayerStateController m_stateController;
    
    public override void RegenerateHealth ()
    {
        m_stateController.m_playerHealth = Mathf.Clamp(m_stateController.m_playerHealth + (m_healthRegen * Time.deltaTime), 0.0f, 1.0f);
    }

    public override void TakeDamage (float damage)
    {
        Debug.Log("player taking damage!!!");

        m_stateController.m_playerHealth = Mathf.Clamp(m_stateController.m_playerHealth - (damage * m_damageMultiplier), 0.0f, 1.0f);
    }

    public override void TakeExplosionDamage(float damage)
    {
        if (m_takesExplosionDamage)
        {
            m_stateController.m_playerHealth = Mathf.Clamp(m_stateController.m_playerHealth - (damage * m_damageMultiplier), 0.0f, 1.0f);
        }
    }

    public override void GainHealth (float healthGain)
    {
        m_stateController.m_playerHealth = Mathf.Clamp(m_stateController.m_playerHealth + healthGain, 0.0f, 1.0f);
    }

    public override float GetHealth ()
    {
        return m_stateController.m_playerHealth;
    }

    public override void SetHealth (float health)
    {
        m_stateController.m_playerHealth = Mathf.Clamp(health, 0.0f, 1.0f);
    }
}
