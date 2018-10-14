using UnityEngine;
using System.Collections;

public class Shooter : MonoBehaviour
{

    [SerializeField]float rateOfFire;
    [SerializeField]Projectile projectile;
    [SerializeField]Transform hand;

    private WeaponReloader Reloader;

    float nextFireAllowed;
    Transform muzzle;

    public bool canFire;

    public void Equip()
    {
		transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    void Awake()
    {
        muzzle = transform.Find("Model/Muzzle");
        Reloader = GetComponent<WeaponReloader>();
    }

    public void Reload()
    {
        if (Reloader == null)
            return;
        Reloader.Reload();
    }

    public virtual void Fire()
    {
        canFire = false;

        if (Time.time < nextFireAllowed)
            return;

        if (Reloader != null)
        {
            if (Reloader.IsReloading)
                return;

            if (Reloader.RoundsRemainingInClip == 0)
                return;

            Reloader.TakeFromClip(1);
        }

        nextFireAllowed = Time.time + rateOfFire;

        // instantiate the projectile
        Instantiate(projectile, muzzle.position, muzzle.rotation);

        canFire = true;
    }
}