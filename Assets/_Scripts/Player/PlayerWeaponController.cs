using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerWeaponController : MonoBehaviour
{
    public GameObject m_weaponHolder;

    //DEBUG
    public Transform leftHandHold, rightHandHold;

    private Animator m_playerAnimator;

	// Use this for initialization
	void Start ()
    {
        m_playerAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnAnimatorIK()
    {
        m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        m_playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandHold.position);
        m_playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandHold.rotation);

        m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        m_playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandHold.position);
        m_playerAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandHold.rotation);
    }
}
