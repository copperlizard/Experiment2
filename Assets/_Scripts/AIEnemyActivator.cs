using UnityEngine;
using System.Collections;

public class AIEnemyActivator : MonoBehaviour
{
    public GameObject m_AIenemy, m_spawnEffect, m_bigScreen;
    public ObjectPool m_deadBodyPool;

    public float m_spawnEffectDuration;

    private AIControlInput m_AIcontrol;
    private PlayerHealth m_AIHealth;
    private Rigidbody m_AImainRB;

    private Animator m_AIanimator;

    /*
    private Transform m_hips, m_leftUpperLeg, m_rightUpperLeg, m_leftLowerLeg, m_rightLowerLeg, m_leftFoot, m_rightFoot,
        m_spine, m_chest, m_neck, m_head, m_leftShoulder, m_rightShoulder, m_leftUpperArm, m_rightUpperArm, m_leftLowerArm, m_rightLowerArm,
        m_leftHand, m_rightHand, m_leftToes, m_rightToes, m_leftEye, m_rightEye, m_jaw;*/

    private MeshRenderer[] m_AIweaponMeshes;
    private SkinnedMeshRenderer[] m_AIskinMeshes;
    private Collider[] m_AIColliders;

    private Vector3 m_startPos;
    private Quaternion m_startRot;

    private bool m_AIactive = false;

	// Use this for initialization
	void Start ()
    {
        m_AIcontrol = m_AIenemy.GetComponent<AIControlInput>();
        m_AImainRB = m_AIenemy.GetComponent<Rigidbody>();
        m_AIweaponMeshes = m_AIenemy.GetComponentsInChildren<MeshRenderer>(true);
        m_AIskinMeshes = m_AIenemy.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        m_AIColliders = m_AIenemy.GetComponentsInChildren<Collider>(true);

        m_AIHealth = m_AIenemy.GetComponent<PlayerHealth>();

        m_AIanimator = m_AIenemy.GetComponent<Animator>();
        //GetBones();

        m_startPos = m_AIenemy.transform.position;
        m_startRot = m_AIenemy.transform.rotation;

        SetAIState(false);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (m_AIHealth.GetHealth() <= 0.0f)
        {
            DropBody();

            m_AIenemy.transform.position = m_startPos;
            m_AIenemy.transform.rotation = m_startRot;

            m_AIHealth.SetHealth(1.0f);
            
            SetAIState(false);
        }
	}

    /*
    void GetBones ()
    {
        m_hips = m_AIanimator.GetBoneTransform(HumanBodyBones.Hips);
        m_leftUpperLeg = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        m_rightUpperLeg = m_AIanimator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        m_leftLowerLeg = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        m_rightLowerLeg = m_AIanimator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        m_leftFoot = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        m_rightFoot = m_AIanimator.GetBoneTransform(HumanBodyBones.RightFoot);
        m_spine = m_AIanimator.GetBoneTransform(HumanBodyBones.Spine);
        m_chest = m_AIanimator.GetBoneTransform(HumanBodyBones.Chest);
        m_neck = m_AIanimator.GetBoneTransform(HumanBodyBones.Neck);
        m_head = m_AIanimator.GetBoneTransform(HumanBodyBones.Head);
        m_leftShoulder = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        m_rightShoulder = m_AIanimator.GetBoneTransform(HumanBodyBones.RightShoulder);
        m_leftUpperArm = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        m_rightUpperArm = m_AIanimator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        m_leftLowerArm = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        m_rightLowerArm = m_AIanimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        m_leftHand = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftHand);
        m_rightHand = m_AIanimator.GetBoneTransform(HumanBodyBones.RightHand);
        m_leftToes = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftToes);
        m_rightToes = m_AIanimator.GetBoneTransform(HumanBodyBones.RightToes);
        m_leftEye = m_AIanimator.GetBoneTransform(HumanBodyBones.LeftEye);
        m_rightEye = m_AIanimator.GetBoneTransform(HumanBodyBones.RightEye);
        m_jaw = m_AIanimator.GetBoneTransform(HumanBodyBones.Jaw);
    }
    */

    void DropBody()
    {
        /*
        Transform hips, leftUpperLeg, rightUpperLeg, leftLowerLeg, rightLowerLeg, leftFoot, rightFoot,
        spine, chest, neck, head, leftShoulder, rightShoulder, leftUpperArm, rightUpperArm, leftLowerArm, rightLowerArm,
        leftHand, rightHand /*leftToes, rightToes, leftEye, rightEye, jaw*///;        

        GameObject deadBody = m_deadBodyPool.GetObject();

        deadBody.transform.position = m_AIenemy.transform.position;
        deadBody.transform.rotation = m_AIenemy.transform.rotation;

        deadBody.SetActive(true);

        Rigidbody[] rbs = deadBody.GetComponentsInChildren<Rigidbody>();

        Vector3 vel = m_AImainRB.velocity;

        for (int i = 0; i < rbs.Length; i++)
        {
            rbs[i].velocity = vel;
        }

        /*
        Animator deadBodyAnimator = deadBody.GetComponent<Animator>();

        
        hips = deadBodyAnimator.GetBoneTransform(HumanBodyBones.Hips);
        leftUpperLeg = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        rightUpperLeg = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        leftLowerLeg = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        rightLowerLeg = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        leftFoot = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
        spine = deadBodyAnimator.GetBoneTransform(HumanBodyBones.Spine);
        chest = deadBodyAnimator.GetBoneTransform(HumanBodyBones.Chest);
        neck = deadBodyAnimator.GetBoneTransform(HumanBodyBones.Neck);
        head = deadBodyAnimator.GetBoneTransform(HumanBodyBones.Head);
        leftShoulder = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder);
        rightShoulder = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightShoulder);
        leftUpperArm = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        rightUpperArm = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        leftLowerArm = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        rightLowerArm = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        leftHand = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
        rightHand = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        //leftToes = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftToes);
        //rightToes = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightToes);
        //leftEye = deadBodyAnimator.GetBoneTransform(HumanBodyBones.LeftEye);
        //rightEye = deadBodyAnimator.GetBoneTransform(HumanBodyBones.RightEye);
        //jaw = deadBodyAnimator.GetBoneTransform(HumanBodyBones.Jaw);

        
        hips.position = m_hips.position;
        hips.rotation = m_hips.rotation;
        
        leftUpperLeg.position = m_leftUpperLeg.position;
        leftUpperLeg.rotation = m_leftUpperLeg.rotation;

        rightUpperLeg.position = m_rightUpperLeg.position;
        rightUpperLeg.rotation = m_rightUpperLeg.rotation;

        leftLowerLeg.position = m_leftLowerLeg.position;
        leftLowerLeg.rotation = m_leftLowerLeg.rotation;

        rightLowerLeg.position = m_rightLowerLeg.position;
        rightLowerLeg.rotation = m_rightLowerLeg.rotation;

        leftFoot.position = m_leftFoot.position;
        leftFoot.rotation = m_leftFoot.rotation;

        rightFoot.position = m_rightFoot.position;
        rightFoot.rotation = m_rightFoot.rotation;

        spine.position = m_spine.position;
        spine.rotation = m_spine.rotation;

        chest.position = m_chest.position;
        chest.rotation = m_chest.rotation;

        neck.position = m_neck.position;
        neck.rotation = m_neck.rotation;

        head.position = m_head.position;
        head.rotation = m_head.rotation;

        leftShoulder.position = m_leftShoulder.position;
        leftShoulder.rotation = m_leftShoulder.rotation;

        rightShoulder.position = m_rightShoulder.position;
        rightShoulder.rotation = m_rightShoulder.rotation;

        leftUpperArm.position = m_leftUpperArm.position;
        leftUpperArm.rotation = m_leftUpperArm.rotation;

        rightUpperArm.position = m_rightUpperArm.position;
        rightUpperArm.rotation = m_rightUpperArm.rotation;

        leftLowerArm.position = m_leftLowerArm.position;
        leftLowerArm.rotation = m_leftLowerArm.rotation;

        rightLowerArm.position = m_rightLowerArm.position;
        rightLowerArm.rotation = m_rightLowerArm.rotation;

        leftHand.position = m_leftHand.position;
        leftHand.rotation = m_leftHand.rotation;

        rightHand.position = m_rightHand.position;
        rightHand.rotation = m_rightHand.rotation;
        */

        /*
        leftToes.position = m_leftToes.position;
        leftToes.rotation = m_leftToes.rotation;

        rightToes.position = m_rightToes.position;
        rightToes.rotation = m_rightToes.rotation;

        leftEye.position = m_leftEye.position;
        leftEye.rotation = m_leftEye.rotation;

        rightEye.position = m_rightEye.position;
        rightEye.rotation = m_rightEye.rotation;

        jaw.position = m_jaw.position;
        jaw.rotation = m_jaw.rotation;
        */
    }

    IEnumerator SpawnEffect ()
    {
        m_spawnEffect.SetActive(true);
        yield return new WaitForSeconds(m_spawnEffectDuration);
        m_spawnEffect.SetActive(false);
    }

    void OnCollisionEnter (Collision other)
    {
        if (!m_AIactive)
        {
            SetAIState(true);

            StartCoroutine(SpawnEffect());
        }
    }

    void SetAIState (bool state)
    {
        m_AIactive = state;

        m_bigScreen.SetActive(state);

        //Debug.Log("setting ai state to " + state.ToString());

        m_AIcontrol.enabled = state;
                
        m_AImainRB.useGravity = state;

        if (state)
        {
            m_AImainRB.WakeUp();
        }
        else
        {
            m_AImainRB.Sleep();
        }        

        for (int i = 0; i < m_AIweaponMeshes.Length; i++)
        {
            m_AIweaponMeshes[i].enabled = state;

            //Debug.Log("setting mesh " + m_AIweaponMeshes[i].name + " to " + state.ToString());
        }
        
        
        for (int i = 0; i < m_AIskinMeshes.Length; i++)
        {
            m_AIskinMeshes[i].enabled = state;

            //Debug.Log("setting skin mesh " + m_AIskinMeshes[i].name + " to " + state.ToString());
        }
        

        for (int i = 0; i < m_AIColliders.Length; i++)
        {
            m_AIColliders[i].enabled = state;
        }
    }
}
