using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    //Interface class for weapons
    public string m_wepName = "Default";
         
    public Transform m_leftHand, m_rightHand, m_leftElbow, m_rightElbow, m_firePos;

    public GameObject m_weaponModel, m_weaponHUD;       

    public float m_projectileSpeed = 3.0f, m_leftHandWeight = 1.0f, m_rightHandWeight = 1.0f;
    public int m_magazineSize = 100, m_rounds = 1000;

    [HideInInspector]
    public AudioSource m_audioSource;

    public AudioClip m_fireNoise, m_emptyNoise, m_reloadNoise;

    [HideInInspector]
    public Animator m_wepAnimator;

    [HideInInspector]
    public int m_thisMagazine;

    [HideInInspector]
    public bool m_aiming, m_firing, m_fired, m_reloading, m_reloaded, m_crouching, m_sprinting, m_sliding, m_idle;

    private ObjectPool m_ammo;

    void Awake ()
    {
        m_wepAnimator = GetComponent<Animator>();

        m_audioSource = GetComponent<AudioSource>();

        m_ammo = GetComponentInChildren<ObjectPool>();

        m_thisMagazine = m_magazineSize;
    }

    void OnEnable ()
    {
        //Debug.Log("rebinding animator!");

        //m_wepAnimator.Rebind();
        //m_wepAnimator.StartPlayback();
    }

    public virtual void Update ()
    {
        if (m_aiming)
        {
            if (!m_weaponHUD.activeInHierarchy)
            {
                m_weaponHUD.SetActive(true);
            }
            
            if (m_firing)
            {
                Fire();
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

    public virtual IEnumerator Firing ()
    {
        //Wait for animator transition
        while (!m_wepAnimator.GetCurrentAnimatorStateInfo(0).IsName("GunFire"))
        {
            //Debug.Log("Waiting for animator!!!");

            yield return null;
        }

        //Shoot projectile
        Debug.DrawLine(m_firePos.transform.position, m_firePos.transform.position + m_firePos.transform.forward);

        GameObject shot = m_ammo.GetObject();
        shot.transform.position = m_firePos.transform.position;
        shot.transform.rotation = Quaternion.LookRotation(m_firePos.transform.forward);

        Rigidbody shotRB = shot.GetComponent<Rigidbody>();
        shot.SetActive(true);
        shotRB.velocity = m_firePos.transform.forward * m_projectileSpeed;

        m_thisMagazine--;

        m_audioSource.PlayOneShot(m_fireNoise);

        //Wait for animator transition
        while (m_wepAnimator.GetCurrentAnimatorStateInfo(0).IsName("GunFire"))
        {
            //Debug.Log("Waiting for animator!!!");

            yield return null;
        }

        //Reset
        m_fired = false;

        //Debug.Log("Fired!!!");
    }

    public virtual void Fire ()
    {
        if (!m_fired && m_thisMagazine > 0)
        {
            m_fired = true;
            m_wepAnimator.SetTrigger("Fire");
            StartCoroutine(Firing());
        } 
        else if (m_thisMagazine <= 0 && !m_audioSource.isPlaying)
        {               
            m_audioSource.PlayOneShot(m_emptyNoise);
        }      
    }

    public virtual IEnumerator Reloading ()
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

        int dif = m_magazineSize - m_thisMagazine;
        dif = Mathf.Clamp(dif, 0, m_rounds);
        m_thisMagazine += dif;
        m_rounds -= dif;        
        m_reloaded = false;
    }
    
    public virtual void Reload ()
    {
        if (!m_reloaded && m_rounds > 0)
        {
            m_reloaded = true;
            m_wepAnimator.SetTrigger("Reload");
            StartCoroutine(Reloading());
        }
    }

    public virtual void UpdateWepAnimator (bool aiming, bool firing, bool reloading, bool crouching, bool sprinting, bool sliding, bool idle, bool grounded)
    {
        m_aiming = aiming;
        m_firing = firing;
        m_reloading = reloading;
        m_crouching = crouching && grounded;
        m_sprinting = sprinting && grounded;
        m_sliding = sliding && grounded;
        m_idle = idle;       
        m_wepAnimator.SetBool("Aiming", aiming);
        m_wepAnimator.SetBool("Firing", firing);
        m_wepAnimator.SetBool("Crouching", crouching);
        m_wepAnimator.SetBool("Sprinting", sprinting);
        m_wepAnimator.SetBool("Sliding", sliding);
        m_wepAnimator.SetBool("Idle", idle);        
    }
}

[RequireComponent(typeof(Animator))]
public class PlayerWeaponController : MonoBehaviour
{   
    public GameObject m_weaponHolder, m_cam;
    public float m_maxPan = 60.0f, m_weaponAimSpeed = 0.1f, m_camAimDist = 1.0f;
    public Vector3 m_camTarAimPosOffset = new Vector3(0.2f, 0.0f, 0.0f), m_camTarCrouchAimPosOffset;

    [System.Serializable]
    public class PlayerHUD
    {
        public Text m_wepAndAmmoText;

        public Slider m_healthSlider, m_ammoSlider;
        
        public  Image m_wepImage1, m_wepImage2, m_wepImage3;
    }
    public PlayerHUD m_HUD;

    public List<GameObject> m_weapons = new List<GameObject>();

    private OrbitCam m_camController;
    private Weapon m_curWeapon;

    private Animator m_playerAnimator;
    private PlayerStateController m_stateController;

    private Transform m_leftHand, m_rightHand, m_leftElbow, m_rightElbow, m_spineTransform;

    private Vector3 m_camTarStartLocalPos;

    private float m_rightElbowWeight, m_lastCamDist;
    private int m_curWeaponNum = 0;

    private bool m_holstered = false, m_holsterLock = false;

	// Use this for initialization
	void Awake ()
    {
        if (m_cam == null)
        {
            m_cam = Camera.main.gameObject;
        }
        m_camController = m_cam.GetComponent<OrbitCam>();

        m_camTarStartLocalPos = m_camController.m_target.transform.localPosition;        

        if (m_camController == null)
        {
            Debug.Log("NO ORBIT CAM!!!");
        }

        m_playerAnimator = GetComponent<Animator>();
        m_spineTransform = m_playerAnimator.GetBoneTransform(HumanBodyBones.Spine); //maybe change to ribs... check model rig!!!

        m_stateController = GetComponent<PlayerStateController>();

        m_curWeapon = m_weapons[m_curWeaponNum].GetComponent<Weapon>();

        m_leftHand = m_curWeapon.m_leftHand;
        m_rightHand = m_curWeapon.m_rightHand;
        m_leftElbow = m_curWeapon.m_leftElbow;
        m_rightElbow = m_curWeapon.m_rightElbow;
    }

    void Start ()
    {
        m_lastCamDist = m_camController.GetCamDist();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_stateController.m_holster && !m_holstered && !m_holsterLock)
        {
            HolsterWeapon();
            m_holstered = true;
            m_holsterLock = true;
        }
        else if (m_stateController.m_holster && m_holstered && !m_holsterLock) 
        {
            SelectWeapon(m_stateController.m_wepNum);
            m_holstered = false;
            m_holsterLock = true;
        }
        else if (!m_stateController.m_holster && m_holsterLock)
        {
            m_holsterLock = false;
        }

        if (m_curWeaponNum != m_stateController.m_wepNum)
        {
            SelectWeapon(m_stateController.m_wepNum);
        }

        if (!m_holstered)
        {
            m_curWeapon.UpdateWepAnimator(m_stateController.m_aiming, m_stateController.m_firing, m_stateController.m_reloading, m_stateController.m_crouch, m_stateController.m_sprint,
            m_stateController.m_slide, (m_stateController.m_move.sqrMagnitude < 0.01f), m_stateController.m_grounded);
        }        

        UpdateHUD();
    }

    void UpdateHUD ()
    {
        string txt = m_curWeapon.m_wepName + " | " + m_curWeapon.m_thisMagazine.ToString() + "/" + m_curWeapon.m_magazineSize.ToString() + "/" + m_curWeapon.m_rounds.ToString();

        m_HUD.m_wepAndAmmoText.text = txt;

        m_HUD.m_healthSlider.value = m_stateController.m_playerHealth;

        if (!m_holstered)
        {
            m_HUD.m_ammoSlider.value = (float)m_curWeapon.m_thisMagazine / (float)m_curWeapon.m_magazineSize;
        }
        else
        {
            m_HUD.m_ammoSlider.value = 0.0f;
        }        

        switch(m_curWeaponNum)
        {            
            case 0:
                m_HUD.m_wepImage1.color = Color.white;
                m_HUD.m_wepImage2.color = Color.grey;
                m_HUD.m_wepImage3.color = Color.grey;
                break;
            case 1:
                m_HUD.m_wepImage1.color = Color.grey;
                m_HUD.m_wepImage2.color = Color.white;
                m_HUD.m_wepImage3.color = Color.grey;
                break;
            case 2:
                m_HUD.m_wepImage1.color = Color.grey;
                m_HUD.m_wepImage2.color = Color.grey;
                m_HUD.m_wepImage3.color = Color.white;
                break;
        }
    }
        
    void OnAnimatorIK (int layer)
    {    
        if (!m_holstered)
        {   
            //Aiming layer    
            if (layer == 1)
            {
                BodyAim();
                WeaponAim();
            }

            //Aim Camera
            CamAim();

            //Hold Weapon
            m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_curWeapon.m_leftHandWeight);
            m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_curWeapon.m_leftHandWeight);
            m_playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHand.position);
            m_playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, m_leftHand.rotation);

            m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_curWeapon.m_rightHandWeight);
            m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_curWeapon.m_rightHandWeight);
            m_playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, m_rightHand.position);
            m_playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, m_rightHand.rotation);

            m_rightElbowWeight = Mathf.Lerp(m_rightElbowWeight, (m_stateController.m_aiming) ? 1.0f : 0.0f, 0.1f);
            m_playerAnimator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, m_rightElbowWeight);
            m_playerAnimator.SetIKHintPosition(AvatarIKHint.RightElbow, m_rightElbow.position);

            m_playerAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, m_rightElbowWeight);
            m_playerAnimator.SetIKHintPosition(AvatarIKHint.LeftElbow, m_leftElbow.position);
        }
        else
        {
            m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.0f);
            m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0.0f);

            m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.0f);
            m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.0f);

            m_playerAnimator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0.0f);

            m_playerAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0.0f);
        }        
    }

    void BodyAim ()
    {
        if (m_stateController.m_aiming)
        {
            Vector3 toTar = (m_camController.m_hit.point - m_spineTransform.position).normalized;
            float turnCheck = Vector3.Dot(transform.forward, toTar);

/*
#if UNITY_EDITOR
            Debug.DrawLine(m_spineTransform.position, m_spineTransform.position + toTar, Color.red);
            Debug.DrawLine(m_spineTransform.position, m_spineTransform.position + m_spineTransform.up, Color.blue);
#endif       
*/
            
            Quaternion deltaRot = Quaternion.FromToRotation(m_spineTransform.up, toTar);
            float pan = deltaRot.eulerAngles.y - m_spineTransform.rotation.eulerAngles.x;

            if (Mathf.Abs(pan) > 180.0f)
            {
                if (m_spineTransform.rotation.eulerAngles.x < deltaRot.eulerAngles.y)
                {
                    pan = deltaRot.eulerAngles.y - (m_spineTransform.rotation.eulerAngles.x + 360.0f);
                }
                else
                {
                    pan = (deltaRot.eulerAngles.y + 360.0f) - m_spineTransform.rotation.eulerAngles.x;
                }
            }

            if (pan > m_maxPan)
            {
                pan = m_maxPan;
            }
            else if (pan < -m_maxPan)
            {
                pan = -m_maxPan;
            }
            
            //Head
            m_playerAnimator.SetLookAtWeight(Mathf.Lerp(0.4f, 1.0f, turnCheck));
            m_playerAnimator.SetLookAtPosition(m_camController.m_hit.point);

            Quaternion tarRot = Quaternion.Euler(m_spineTransform.localRotation.eulerAngles.x - pan, m_spineTransform.localRotation.eulerAngles.y, m_spineTransform.localRotation.eulerAngles.z);
            m_spineTransform.localRotation = tarRot;
            m_playerAnimator.SetBoneLocalRotation(HumanBodyBones.Spine, tarRot);            
        }
    }
    
    void WeaponAim ()
    {
        if (m_stateController.m_aiming && !m_holstered)
        {
            Vector3 wepPosOffset = m_weapons[m_curWeaponNum].transform.position - m_weaponHolder.transform.position;

            //Debug.Log(m_weapons[m_curWeaponNum].transform.position.ToString());

#if UNITY_EDITOR
            Debug.DrawLine(m_weaponHolder.transform.position, m_weaponHolder.transform.position + wepPosOffset);
#endif

            Vector3 toTar = ((m_camController.m_hit.point + wepPosOffset) - m_weaponHolder.transform.position).normalized;

            float turnCheck = Vector3.Dot(toTar, transform.forward);
            if (turnCheck >= 0.0f)            
            {   
                Quaternion tarRot = Quaternion.LookRotation(toTar);
                Quaternion deltaRot = tarRot * Quaternion.Inverse(m_weaponHolder.transform.rotation);

                //Debug.Log("tarRot == " + tarRot.eulerAngles.ToString() + " ; deltaRot == " + deltaRot.eulerAngles.ToString());
                
                Vector3 rottedOffset = deltaRot * wepPosOffset;

                m_weapons[m_curWeaponNum].transform.position = m_weaponHolder.transform.position + rottedOffset;

                //Debug.Log(m_weapons[m_curWeaponNum].transform.position.ToString());

                Vector3 wepToTar = (m_camController.m_hit.point - m_weapons[m_curWeaponNum].transform.position).normalized;
                Quaternion wepTarRot = Quaternion.LookRotation(wepToTar);
                
                m_weapons[m_curWeaponNum].transform.rotation = wepTarRot;

#if UNITY_EDITOR
                Debug.DrawLine(m_weapons[m_curWeaponNum].transform.position, m_weapons[m_curWeaponNum].transform.position + wepToTar, Color.red);
#endif
            }
            else
            {
                Quaternion tarRot = Quaternion.LookRotation(toTar);
                Quaternion deltaRot = tarRot * Quaternion.Inverse(m_weaponHolder.transform.rotation);

                float pan, tilt;
                if (deltaRot.eulerAngles.y <= 180.0f)
                {
                    //Debug.Log("aiming right!");
                    pan = deltaRot.eulerAngles.y;
                    pan = Mathf.Clamp(pan, -45.0f, 45.0f);
                }
                else
                {
                    //Debug.Log("aiming left!");
                    pan = deltaRot.eulerAngles.y - 360.0f;
                    pan = Mathf.Clamp(pan, -45.0f, 45.0f);
                }

                if (deltaRot.eulerAngles.x <= 180.0f)
                {
                    //Debug.Log("aiming down!");
                    tilt = deltaRot.eulerAngles.x;
                    tilt = Mathf.Clamp(tilt, -45.0f, 45.0f);
                }
                else
                {
                    //Debug.Log("aiming up!");
                    tilt = deltaRot.eulerAngles.x - 360.0f;
                    tilt = Mathf.Clamp(tilt, -45.0f, 45.0f);
                }

                deltaRot = Quaternion.Euler(tilt, pan, deltaRot.eulerAngles.z);
                
                Vector3 rottedOffset = deltaRot * wepPosOffset;

                m_weapons[m_curWeaponNum].transform.position = m_weaponHolder.transform.position + rottedOffset;
                
                Vector3 wepToTar = (m_camController.m_hit.point - m_weapons[m_curWeaponNum].transform.position).normalized;
                
                Quaternion wepTarRot = Quaternion.LookRotation(wepToTar);

                m_weapons[m_curWeaponNum].transform.rotation = wepTarRot;
               
#if UNITY_EDITOR
                Debug.DrawLine(m_weapons[m_curWeaponNum].transform.position, m_weapons[m_curWeaponNum].transform.position + wepToTar, Color.red);
#endif
            }
        }
    }

    void CamAim()
    {
        if (m_stateController.m_aiming)
        {
            m_camController.m_target.transform.localPosition = m_camTarStartLocalPos + m_camTarAimPosOffset;

            if (m_stateController.m_crouch)
            {
                m_camController.m_target.transform.localPosition += m_camTarCrouchAimPosOffset;
            }
            
            if (m_camController.GetCamDist() != m_camAimDist)
            {
                m_lastCamDist = m_camController.GetCamDist();
                m_camController.SetCamDist(m_camAimDist);
            }
        }
        else
        {
            m_camController.m_target.transform.localPosition = m_camTarStartLocalPos;
            
            if (m_camController.GetCamDist() == m_camAimDist)
            {
                m_camController.SetCamDist(m_lastCamDist);
            }
            else
            {
                m_lastCamDist = m_camController.GetCamDist();
            }
        }
    }

    public void ReloadWeapon ()
    {
        if (!m_holstered)
        {
            m_curWeapon.Reload();
        }
    }

    public void HolsterWeapon ()
    {        
        m_curWeapon.m_weaponModel.SetActive(false);
        m_curWeapon.enabled = false;
    }

    public void SelectWeapon (int num)
    {
        //Out of selection range
        if (num < 0 && num > m_weapons.Count - 1)
        {
            return;
        }

        //Switch weapon
        if (num != m_curWeaponNum)
        {
            HolsterWeapon();
            m_curWeaponNum = num;
            m_weapons[m_curWeaponNum].SetActive(true);
            m_curWeapon = m_weapons[m_curWeaponNum].GetComponent<Weapon>();

            if (!m_holstered)
            {
                m_curWeapon.m_weaponModel.SetActive(true);
                m_curWeapon.enabled = true;
            }
            
            m_leftHand = m_curWeapon.m_leftHand;
            m_rightHand = m_curWeapon.m_rightHand;
            m_leftElbow = m_curWeapon.m_leftElbow;
            m_rightElbow = m_curWeapon.m_rightElbow;
        }
        //Draw weapon
        else if (!m_curWeapon.m_weaponModel.activeInHierarchy)
        {
            m_curWeapon.m_weaponModel.SetActive(true);
            m_curWeapon.enabled = true;            
        }
    }
}
