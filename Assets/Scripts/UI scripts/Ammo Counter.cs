using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    private TMP_Text ammoText;
    [SerializeField] private PlayerController mc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ammoText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmmoUI();
    }
    void UpdateAmmoUI()
    {
        if (mc.pistolMode)
            ammoText.text = $"{mc.pistolAmmoInClip} / {mc.pistolTotalAmmo}";
        else if (mc.shotgunMode)
            ammoText.text = $"{mc.shotgunAmmoInClip} / {mc.shotgunTotalAmmo}";
        else if (mc.railgunMode)
            ammoText.text = $"{mc.railgunAmmoInClip} / {mc.railgunTotalAmmo}";
    }
}
