using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
	[SerializeField]float weaponSwitchTime;

    Shooter[] weapons;
    Shooter activeWeapon;

    int currentWeaponIndex;
	bool canFire;
	Transform weaponHolster;

    public Shooter ActiveWeapon
    {
        get
        {
            return activeWeapon;
        }
    }

    void Awake()
    {
		canFire = true;
		weaponHolster = transform.Find("Weapons");
        weapons = weaponHolster.GetComponentsInChildren<Shooter>();
        if (weapons.Length > 0)
            Equip(0);
    }

	void DeactivateWeapons()
	{
		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].gameObject.SetActive(false);
			weapons[i].transform.SetParent(weaponHolster);
		}
	}

    void SwitchWeapon(int index)
    {   
		canFire = false;
        // El index es el número de array del arma
        // Al momento hay solo 2 armas. 0 y 1
		GameManager.Instance.Timer.Add(() => {
			Equip(index);
		}, weaponSwitchTime);
    }

    void Equip(int index)
    {
		DeactivateWeapons();
		canFire = true;
		activeWeapon = weapons[index];
		activeWeapon.Equip();
		weapons[index].gameObject.SetActive(true);
    }

    void Update()
    {	
        // Al momento hay solo 2 armas. 0 y 1
		if(GameManager.Instance.InputController.Key1) 
        {
			SwitchWeapon(0);
        }
		
		if(GameManager.Instance.InputController.Key2)
        {
			SwitchWeapon(1);
        }

		if(!canFire)
        {
			return;
        }

        // Can shoot while not sprinting
        if (GameManager.Instance.InputController.Fire1 && GameManager.Instance.InputController.IsSprinting == false)
        {
            activeWeapon.Fire();
        }
    }
}
