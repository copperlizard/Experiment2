using UnityEngine;
using System.Collections;

public class GrenadeLauncher : Weapon
{

    private bool m_fireLock = false, m_reloadDone = true;

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

        if ((m_reloading | !m_reloadDone) && m_thisMagazine < m_magazineSize)
        {
            Reload();
        }
    }

    public override void Fire()
    {
        if (!m_fired && m_thisMagazine > 0)
        {
            m_fired = true;

            if (!m_reloadDone)
            {
                m_reloadDone = true;
            }

            m_wepAnimator.SetTrigger("Fire");
            StartCoroutine(Firing());
        }
        else if (m_thisMagazine <= 0 && !m_audioSource.isPlaying)
        {
            m_audioSource.PlayOneShot(m_emptyNoise);
        }
    }

    IEnumerator Reloading()
    {
        //Wait for animator transition
        while (!m_wepAnimator.GetCurrentAnimatorStateInfo(0).IsName("GunReload"))
        {
            //Debug.Log("Waiting for animator!!!");

            yield return null;
        }

        m_audioSource.PlayOneShot(m_reloadNoise);

        while (m_wepAnimator.GetCurrentAnimatorStateInfo(0).IsName("GunReload"))
        {
            //Debug.Log("Waiting for animator!!!");

            yield return null;
        }
        
        m_thisMagazine += 1;
        m_rounds -= 1;
        
        m_reloaded = false;

        if (m_thisMagazine == m_magazineSize)
        {
            m_reloadDone = true;
        }
        else if (!m_fired)
        {
            m_reloadDone = false;
        }
    }

    public override void Reload()
    {
        if (!m_reloaded && m_rounds > 0)
        {
            m_reloaded = true;
            m_wepAnimator.SetTrigger("Reload");
            StartCoroutine(Reloading());
        }
    }
}
