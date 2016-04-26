using UnityEngine;
using System.Collections;

public class TestWeapon : Weapon
{       
    override public void Fire ()
    {
        Debug.Log("TestWeaponFire!");
    }

    override public void Reload ()
    {
        Debug.Log("TestWeaponReload!");
    }	
	
    /*
	override public void UpdateWepAnimator ()
    {
	
	}
    */
}
