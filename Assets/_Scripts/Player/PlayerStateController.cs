using UnityEngine;
using System.Collections;

public class PlayerStateController : MonoBehaviour
{
    [HideInInspector]
    public Vector3 m_move;
    [HideInInspector]
    public float m_forwardAmount, m_sidewaysAmount, m_turnTarAng;
    [HideInInspector]
    public bool m_grounded, m_aiming, m_jump, m_crouch, m_slide, m_walk, m_sprint;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
