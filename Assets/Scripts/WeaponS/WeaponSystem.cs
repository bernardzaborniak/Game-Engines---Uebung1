﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField]
    public Weapon[] inventory; //will be set up in inspector
    Weapon currentSelectedWeapon;
    public Text weaponInfo; //shows weapon name and ammo

    public Transform weaponHolder;

    [Header("Ammo")]
    //ammo in pockets - not in magazines

    Dictionary<AmmoType, int> ammo; //TODO this better
    public int startRifleAmmo;
    public int startRocketAmmo;
    public int startLaserAmmo;
    public int startSMGAmmo;
    public int startShotgunAmmo;

    [Header("Animation")]
    public Animator animator;

    public static WeaponSystem Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        ammo = new Dictionary<AmmoType, int>();
    }

    void Start()
    {
        foreach (Weapon weapon in inventory)
        {
            if(weapon!=null)weapon.gameObject.SetActive(false);
        }
        ChangeWeapon(0);

        ammo[AmmoType.Rifle] = startRifleAmmo;
        ammo[AmmoType.Rocket] = startRocketAmmo;
        ammo[AmmoType.Laser] = startLaserAmmo;
        ammo[AmmoType.Smg] = startSMGAmmo;
        ammo[AmmoType.Shotgun] = startShotgunAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(2);
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftMouseButton();
        }

        if (Input.GetKeyDown(KeyCode.R)) StartReload();

        if (currentSelectedWeapon is ProjectileWeapon)
        {
            ProjectileWeapon selectedMissileWeapon = currentSelectedWeapon as ProjectileWeapon;
            switch (selectedMissileWeapon.ammoType)
            {
                case AmmoType.Rifle:
                    weaponInfo.text = selectedMissileWeapon.weaponName + "\n" + selectedMissileWeapon.GetCurrentMagazineAmmo() + "/" + ammo[AmmoType.Rifle];
                    break;
                case AmmoType.Rocket:
                    weaponInfo.text = selectedMissileWeapon.weaponName + "\n" + selectedMissileWeapon.GetCurrentMagazineAmmo() + "/" + ammo[AmmoType.Rocket];
                    break;
                case AmmoType.Laser:
                    weaponInfo.text = selectedMissileWeapon.weaponName + "\n" + selectedMissileWeapon.GetCurrentMagazineAmmo() + "/" + ammo[AmmoType.Laser];
                    break;
                case AmmoType.Smg:
                    weaponInfo.text = selectedMissileWeapon.weaponName + "\n" + selectedMissileWeapon.GetCurrentMagazineAmmo() + "/" + ammo[AmmoType.Smg];
                    break;
                case AmmoType.Shotgun:
                    weaponInfo.text = selectedMissileWeapon.weaponName + "\n" + selectedMissileWeapon.GetCurrentMagazineAmmo() + "/" + ammo[AmmoType.Shotgun];
                    break;
            }

        }
        else
        {
            weaponInfo.text = currentSelectedWeapon.weaponName;
        }

    }

    void ChangeWeapon(int inventorySlot)
    {
        animator.SetTrigger("changeWeapon");
        animator.SetBool("reloading", false);

        if (currentSelectedWeapon != null)
        {
            currentSelectedWeapon.gameObject.SetActive(false);
        }

        currentSelectedWeapon = inventory[inventorySlot];

        if (currentSelectedWeapon != null)
        {
            currentSelectedWeapon.gameObject.SetActive(true);
            if(currentSelectedWeapon is ProjectileWeapon)
            {
                animator.SetBool("rifleSelected", true);
                animator.SetBool("swordSelected", false);
                animator.SetBool("nothingSelected", false);
            }
            else if (currentSelectedWeapon is Sword)
            {
                animator.SetBool("rifleSelected", false);
                animator.SetBool("swordSelected", true);
                animator.SetBool("nothingSelected", false);
            }
        }
        else
        {
            animator.SetBool("rifleSelected", false);
            animator.SetBool("swordSelected", false);
            animator.SetBool("nothingSelected", true);
        }
    }

    public void HandleLeftMouseButton()
    {
        //wenn wir nur unsere Hände selected haben -> faustschlag
        if (currentSelectedWeapon == null)
        {
            animator.SetTrigger("fistAttack");
        }
        else if(currentSelectedWeapon is Sword)
        {
            Sword sword = (currentSelectedWeapon as Sword);
            if(sword.rightCut) animator.SetTrigger("swordAttackRight");
            else animator.SetTrigger("swordAttackLeft");
            sword.StartCutting();
        }

        /*
        else if (currentSelectedWeapon is MissileWeapon)
        {
            MissileWeapon currentSelectedRifle = currentSelectedWeapon as MissileWeapon;
            currentSelectedRifle.Shoot();
        }
        */
    }

    void StartReload()
    {
        if (currentSelectedWeapon is ProjectileWeapon)
        {
            //if((currentSelectedWeapon as MissileWeapon).ammoType
            animator.SetBool("reloading", true);
            StartCoroutine("ReloadingCoroutine");
        }
    }

    IEnumerator ReloadingCoroutine()
    {
        float reloadingTime = (currentSelectedWeapon as ProjectileWeapon).reloadTime;
        yield return new WaitForSeconds((reloadingTime));
        if (currentSelectedWeapon is ProjectileWeapon)
        {
            (currentSelectedWeapon as ProjectileWeapon).Reload();
        }
        animator.SetBool("reloading", false);
    }

    //called after gameOver to reset ammo
    public void Reset()
    {
        foreach(Weapon weapon in inventory)
        {
            if (weapon is ProjectileWeapon) (weapon as ProjectileWeapon).Reset();
        }
    }

    //returns currentWeapon
    public GameObject ChangeMissileWeapon(GameObject newWeapon)
    {
        GameObject currentWeapon = inventory[0].gameObject;
        inventory[0] = newWeapon.GetComponent<Weapon>();
        newWeapon.GetComponent<Weapon>().isEquipped = true;
        newWeapon.transform.parent = weaponHolder.transform;
        newWeapon.transform.position = weaponHolder.transform.position;
        newWeapon.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        if (currentSelectedWeapon == currentWeapon.GetComponent<Weapon>()) currentSelectedWeapon = inventory[0];
        if (currentSelectedWeapon != inventory[0]) inventory[0].gameObject.SetActive(false);

        return currentWeapon;
    }
    
    //gets called by the weapons
    public int GetAmmo(AmmoType ammoType)
    {
        return ammo[ammoType];
    }

    //gets called by the weapons
    public void SetAmmo(AmmoType ammoType, int value)
    {
        ammo[ammoType] = value;
    }

    public void AddAmmo(AmmoType ammoType, int value)
    {
        ammo[ammoType] += (int)Mathf.Round(value * currentSelectedWeapon.ammoMultiplier);
    }
}
