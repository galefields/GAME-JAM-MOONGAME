using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AmmoBar : MonoBehaviour
{
    
    [SerializeField] private TMP_Text ammo;
    public GameObject Bar;
    public Image reloadBar;
    
    public void UpdateAmmo(int ammoInClip, float totalAmmo)
    {
        ammo.text = $"{ammoInClip} / {(totalAmmo == Mathf.Infinity ? "∞" : totalAmmo.ToString())}";
    }
    public void StartReload(float reloadTime)
    {
        Bar.SetActive(true);
        StartCoroutine(ReloadProgress(reloadTime));
    }
    private IEnumerator ReloadProgress(float reloadTime)
    {
        float time = 0f;

        while (time < reloadTime)
        {
            ammo.text = "Reloading...";
            reloadBar.fillAmount = 1 - (time / reloadTime);
            time += Time.deltaTime;
            yield return null;
        }
        reloadBar.fillAmount = 0f;
        Bar.SetActive(false);
    }
}
