using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{
    //Interface class for weapons 
    public Transform m_leftHand, m_rightHand, m_rightElbow;

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

    public virtual void UpdateWepAnimator (bool aiming, bool firing, bool crouching, bool sprinting, bool sliding, bool idle)
    {
        m_aiming = aiming;
        m_firing = firing;
        m_crouching = crouching;
        m_sprinting = sprinting;
        m_sliding = sliding;
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
    //public GameObject m_weaponHolder;

    //DEBUG
    //public Transform leftHandHold, rightHandHold, rightElbow;    

    public List<GameObject> m_weapons = new List<GameObject>();
    private Weapon m_curWeapon;

    private Animator m_playerAnimator;
    private PlayerStateController m_stateController;

    private Transform m_leftHand, m_rightHand, m_rightElbow;

    private float m_rightElbowWeight;
    private int m_curWeaponNum = 0;

	// Use this for initialization
	void Awake ()
    {
        m_playerAnimator = GetComponent<Animator>();

        m_stateController = GetComponent<PlayerStateController>();

        m_curWeapon = m_weapons[m_curWeaponNum].GetComponent<TestWeapon>();

        m_leftHand = m_curWeapon.m_leftHand;
        m_rightHand = m_curWeapon.m_rightHand;
        m_rightElbow = m_curWeapon.m_rightElbow;
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_curWeapon.UpdateWepAnimator(m_stateController.m_aiming, m_stateController.m_firing, m_stateController.m_crouch, m_stateController.m_sprint, 
            m_stateController.m_slide, (m_stateController.m_move.sqrMagnitude < 0.01f));
    }

    void OnAnimatorIK ()
    {
        //Add animation layers with avatar masks for the left and right hand with a closed pose, and use hand weight to selectively override hand animations

        m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_curWeapon.m_leftHandWeight);
        m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_curWeapon.m_leftHandWeight);
        m_playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHand.position);
        m_playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, m_leftHand.rotation);

        m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_curWeapon.m_rightHandWeight);
        m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_curWeapon.m_rightHandWeight);
        m_playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, m_rightHand.position);
        m_playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, m_rightHand.rotation);

        m_playerAnimator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, Mathf.Lerp(m_rightElbowWeight, (m_stateController.m_aiming) ? 1.0f : 0.0f, 0.1f));
        m_playerAnimator.SetIKHintPosition(AvatarIKHint.RightElbow, m_rightElbow.position);
        
        
        //aim chest
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
