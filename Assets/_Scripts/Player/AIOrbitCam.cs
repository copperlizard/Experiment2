using UnityEngine;
using System.Collections;

public class AIOrbitCam : OrbitCam
{
    public GameObject m_AIEnemy;

	// Use this for initialization
	public override void Start ()
    {
        m_dist = m_startDist;
        transform.position = m_target.transform.position + new Vector3(0.0f, 0.0f, -m_dist);

        if (m_HideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }        

        m_meshRenderers = m_AIEnemy.GetComponentsInChildren<SkinnedMeshRenderer>(true);
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        GetAIInput();
	}

    void GetAIInput()
    {
        //Do nothing for now
    }
}
