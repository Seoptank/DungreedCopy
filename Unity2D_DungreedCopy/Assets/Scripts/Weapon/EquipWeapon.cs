using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EquipWeapon : MonoBehaviour
{
    [Header("무기 종류")]
    private KeyCode weapon1 = KeyCode.Alpha1;
    private KeyCode weapon2 = KeyCode.Alpha2;
    public GameObject[] Weapons;
    public int equipWeapon = 15;

    [SerializeField]
    private InventorySO inventory;
    [SerializeField]
    private Image equipWeaponImage;
    private int currentWeapon1code = 0;
    private int currentWeapon2code;
    [SerializeField]
    private GameObject equipUI1;
    [SerializeField]
    private GameObject equipUI2;

    [SerializeField]
    private Transform uiPos1;

    [SerializeField]
    private Transform uiPos2;

    void Update()
    {
        ItemSO EquipItem1 = inventory.inventoryItems[15].item;
        ItemSO EquipItem2 = inventory.inventoryItems[16].item;

        if (Input.GetKeyDown(weapon1))
        {
            PlayerController.instance.canAttack = true;
            SwitchingWeapon1(EquipItem1, EquipItem2);
        }
        if (Input.GetKeyDown(weapon2))
        {
            PlayerController.instance.canAttack = true;
            SwitchingWeapon2(EquipItem1, EquipItem2);
        }

        CheckWeapon(EquipItem1, EquipItem2);
    }

    private void CheckWeapon(ItemSO EquipItem1, ItemSO EquipItem2)
    {
        if (equipWeapon == 15 && EquipItem1 == null)
        {
            Weapons[currentWeapon1code].SetActive(false);
            equipWeapon = -1;
        }
        else if (equipWeapon == 15 && currentWeapon1code != EquipItem1.Code)
        {
            Weapons[currentWeapon1code].SetActive(false);
            currentWeapon1code = EquipItem1.Code;
            Weapons[EquipItem1.Code].SetActive(true);
            PlayerStats.instance.WP_MINATK = EquipItem1.MinDamage;
            PlayerStats.instance.WP_MAXATK = EquipItem1.MaxDamage;
            PlayerStats.instance.WP_ATS = EquipItem1.AttckSpeed;
            PlayerController.instance.canAttack = true;
        }

        if (equipWeapon == 16 && EquipItem2 == null)
        {
            Weapons[currentWeapon2code].SetActive(false);
            equipWeapon = -1;
        }
        else if (equipWeapon == 16 && currentWeapon2code != EquipItem2.Code)
        {
            Weapons[currentWeapon2code].SetActive(false);
            currentWeapon2code = EquipItem2.Code;
            Weapons[EquipItem2.Code].SetActive(true);
            PlayerStats.instance.WP_MINATK = EquipItem2.MinDamage;
            PlayerStats.instance.WP_MAXATK = EquipItem2.MaxDamage;
            PlayerStats.instance.WP_ATS = EquipItem2.AttckSpeed;
            PlayerController.instance.canAttack = true;
        }
    }

    private void SwitchingWeapon1(ItemSO EquipItem1, ItemSO EquipItem2)
    {
        if (EquipItem1 != null && equipWeapon != 15)
        {
            if(equipWeapon == 16)
            {
                Weapons[EquipItem2.Code].SetActive(false);
            }
            Weapons[EquipItem1.Code].SetActive(true);
            currentWeapon1code = EquipItem1.Code;
            equipWeapon = 15;
            PlayerStats.instance.WP_MINATK = EquipItem1.MinDamage;
            PlayerStats.instance.WP_MAXATK = EquipItem1.MaxDamage;
            PlayerStats.instance.WP_ATS = EquipItem1.AttckSpeed;
            equipUI1.transform.position = uiPos1.position;
            equipUI2.transform.position = uiPos2.position;
            equipUI2.GetComponent<RectTransform>().SetAsFirstSibling();
            equipUI1.GetComponent<RectTransform>().SetAsLastSibling();
        }
    }
    private void SwitchingWeapon2(ItemSO EquipItem1, ItemSO EquipItem2)
    {
        if (EquipItem2 != null && equipWeapon != 16)
        {
            if (equipWeapon == 15)
            {
                Weapons[EquipItem1.Code].SetActive(false);
            }
            Weapons[EquipItem2.Code].SetActive(true);
            currentWeapon2code = EquipItem2.Code;
            equipWeapon = 16;
            PlayerStats.instance.WP_MINATK = EquipItem2.MinDamage;
            PlayerStats.instance.WP_MAXATK = EquipItem2.MaxDamage;
            PlayerStats.instance.WP_ATS = EquipItem2.AttckSpeed;
            equipUI2.transform.position = uiPos1.position;
            equipUI1.transform.position = uiPos2.position;
            equipUI1.GetComponent<RectTransform>().SetAsFirstSibling();
            equipUI2.GetComponent<RectTransform>().SetAsLastSibling();

        }
    }
}
