using System.Collections;
using UnityEngine;

public class MainWeapon : Shooter {

	public override void Fire()
	{
		base.Fire();

		if(canFire)
		{
			// We can fire gun
		}
	}

	public void Update()
	{
		// if(GameManager.Instance.InputController.Reload)
		// {
		// 	Reload();
		// }
	}
}