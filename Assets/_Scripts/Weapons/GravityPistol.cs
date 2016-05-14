using UnityEngine;
using System.Collections;

public class GravityPistol : Weapon
{
    private bool m_fireLock = false;

    public override void Update() //Single shot mod
    {
        if (m_aiming)
        {
            if (!m_weaponHUD.activeInHierarchy)
            {
                m_weaponHUD.SetActive(true);
            }

            if (m_firing && !m_fireLock)
            {
                Fire();
                m_fireLock = true;
            }
            else if (!m_firing && m_fireLock)
            {
                m_fireLock = false;
            }
        }
        else
        {
            if (m_weaponHUD.activeInHierarchy)
            {
                m_weaponHUD.SetActive(false);
            }
        }

        if (m_reloading && m_thisMagazine < m_magazineSize)
        {
            Reload();
        }
    }
}
