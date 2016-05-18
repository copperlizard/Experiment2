using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovementController))]
public class UserControlInput : MonoBehaviour
{
    private PlayerMovementController m_mover;


    private float m_v, m_h;
    private bool m_fire1, m_fire2, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster;
    private int m_wepNum;

	// Use this for initialization
	void Start ()
    {
        m_mover = GetComponent<PlayerMovementController>();        
    }
	
	// Update is called once per frame
	void Update ()
    {
        m_v = Input.GetAxis("Vertical");
        m_h = Input.GetAxis("Horizontal");
        m_fire1 = Input.GetMouseButton(0);
        m_fire2 = Input.GetMouseButton(1);
        m_reload = Input.GetKey(KeyCode.R);
        m_jump = Input.GetKey(KeyCode.Space);
        m_crouch = Input.GetKey(KeyCode.C);
        m_walk = Input.GetKey(KeyCode.LeftAlt);
        m_sprint = Input.GetKey(KeyCode.LeftShift);
        m_holster = Input.GetKey(KeyCode.H);

        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_wepNum = 0;
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            m_wepNum = 1;
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            m_wepNum = 2;
        }
	}

    void FixedUpdate ()
    {
        m_mover.Move(m_v, m_h, m_fire1, m_fire2, m_reload, m_jump, m_crouch, m_walk, m_sprint, m_holster, m_wepNum);
    }
}
