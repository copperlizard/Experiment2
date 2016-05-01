using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{
    //Interface class for weapons 
    public Transform m_leftHand, m_rightHand, m_leftElbow, m_rightElbow;

    [HideInInspector]
    public float m_leftHandWeight = 1.0f, m_rightHandWeight = 1.0f;

    private Animator m_wepAnimator;
    private bool m_aiming, m_firing, m_crouching, m_sprinting, m_sliding, m_idle;    

    void Start ()
    {
        m_wepAnimator = GetComponent<Animator>();
    }

    public virtual void Update ()
    {
        if(m_aiming && m_firing)
        {
            Fire();
        }
    }

    public virtual void Fire ()
    {

    }

    public virtual void Reload ()
    {

    }

    public virtual void UpdateWepAnimator (bool aiming, bool firing, bool crouching, bool sprinting, bool sliding, bool idle, bool grounded)
    {
        m_aiming = aiming;
        m_firing = firing;
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

    public void OnAnimatorIK ()
    {
        //aim gun
    }
}

[RequireComponent(typeof(Animator))]
public class PlayerWeaponController : MonoBehaviour
{   
    public GameObject m_weaponHolder, m_cam;
    public float m_maxPan = 60.0f;
    
    public List<GameObject> m_weapons = new List<GameObject>();

    private OrbitCam m_camController;
    private Weapon m_curWeapon;

    private Animator m_playerAnimator;
    private PlayerStateController m_stateController;

    private Transform m_leftHand, m_rightHand, m_leftElbow, m_rightElbow, m_spineTransform;

    private float m_rightElbowWeight;
    private int m_curWeaponNum = 0;

	// Use this for initialization
	void Awake ()
    {
        if (m_cam == null)
        {
            m_cam = Camera.main.gameObject;
        }
        m_camController = m_cam.GetComponent<OrbitCam>();

        if (m_camController == null)
        {
            Debug.Log("NO ORBIT CAM!!!");
        }

        m_playerAnimator = GetComponent<Animator>();
        m_spineTransform = m_playerAnimator.GetBoneTransform(HumanBodyBones.Spine); //maybe change to ribs... check model rig!!!

        m_stateController = GetComponent<PlayerStateController>();

        m_curWeapon = m_weapons[m_curWeaponNum].GetComponent<TestWeapon>();

        m_leftHand = m_curWeapon.m_leftHand;
        m_rightHand = m_curWeapon.m_rightHand;
        m_leftElbow = m_curWeapon.m_leftElbow;
        m_rightElbow = m_curWeapon.m_rightElbow;
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_curWeapon.UpdateWepAnimator(m_stateController.m_aiming, m_stateController.m_firing, m_stateController.m_crouch, m_stateController.m_sprint, 
            m_stateController.m_slide, (m_stateController.m_move.sqrMagnitude < 0.01f), m_stateController.m_grounded);
    }
        
    void OnAnimatorIK (int layer)
    {        
        if (layer == 1)
        {
            //Add animation layers with avatar masks for the left and right hand with a closed pose, and use layer weight to selectively override hand animations        
            //Aim/tilt weapon here or on weapon animator IK pass?

            //Aim body
            BodyAim();

            WeaponAim();           
        }

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
         
            //Debug.Log(pan.ToString());
            
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
        if (m_stateController.m_aiming)
        {
            Vector3 tarOffset = m_weapons[m_curWeaponNum].transform.position - m_weaponHolder.transform.position;

            Vector3 toTar = ((m_camController.m_hit.point + tarOffset) - m_weaponHolder.transform.position).normalized;

            float turnCheck = Vector3.Dot(toTar, transform.forward);


#if UNITY_EDITOR
            Debug.DrawLine(m_weaponHolder.transform.position, m_weaponHolder.transform.position + tarOffset);
            Debug.DrawLine(m_weaponHolder.transform.position, m_weaponHolder.transform.position + toTar, Color.red);
            Debug.DrawLine(m_weaponHolder.transform.position, m_weaponHolder.transform.position + m_weaponHolder.transform.forward, Color.blue);
#endif

            if (turnCheck > 0.5f)
            {
                Quaternion tarRot = Quaternion.LookRotation(toTar);

                m_weaponHolder.transform.rotation = tarRot;
            }
        }
    }

    public void ReloadWeapon ()
    {
        m_curWeapon.Reload();
    }

    public void HolsterWeapon ()
    {
        Debug.Log("HolsterWeapon()");
    }

    public void SelectWeapon (int num)
    {
        Debug.Log("SelectWeapon");
    }
}
