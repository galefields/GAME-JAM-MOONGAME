using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 8.5f;
    private Transform mc;
    private Transform bulletPoint;
    private float bulletSpeed = 100;
    private float maxDistance = 25;

    public int scraps = 10;
    public int scraps2win = 100;

    public bool pistolMode;
    public bool shotgunMode;
    public bool railgunMode;

    public float pistolClipSize = 24;
    public float pistolAmmoInClip = 24;
    public float pistolTotalAmmo = float.PositiveInfinity;

    public float shotgunClipSize = 6;
    public float shotgunAmmoInClip = 6;
    public float shotgunTotalAmmo = 18;

    public float railgunClipSize = 3;
    public float railgunAmmoInClip = 3;
    public float railgunTotalAmmo = 3;

    public bool Reloading = false;
    private float reloadTime;
    private AmmoBar ammoBar;

    private Barrier currentBarrier;
    private bool nearBarrier = false;
    private bool repairing = false;
    private float repairTime = 2.5f;
    [SerializeField] private GameObject repairPromptUI;

    [SerializeField] private TrailRenderer pistolTrail;
    [SerializeField] private TrailRenderer railgunTrail;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask barrierLayer;
    [SerializeField] private LayerMask BulletHitLayer;
    void Start()
    {
        Pistol();
        rb = GetComponent<Rigidbody2D>();
        mc = GetComponent<Transform>();
        ammoBar = FindObjectOfType<AmmoBar>();
        bulletPoint = transform.GetChild(0);
        UpdateAmmoUI();
    }
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2(moveHorizontal, moveVertical) * speed;
        RotateGuy();

        if (Input.GetMouseButtonDown(0) && !Reloading)
        {
            if (!HasAmmo()) StartCoroutine(Reload());
            else Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && !Reloading)
        {
            StartCoroutine(Reload());
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            CycleWeapon(1);
        }
        else if (scroll < 0f)
        {
            CycleWeapon(-1);
        }
        
        if (nearBarrier && currentBarrier.health < 50)
    {
        if (Input.GetKey(KeyCode.E) && !repairing)
        {
            StartCoroutine(RepairBarrier());
        }
    }

    // If player moves away, cancel repair
    if (repairing && !nearBarrier)
    {
        StopCoroutine(RepairBarrier());
        repairing = false;
        HideRepairUI();
    }
    }
    void Shoot()
    {
        if (!HasAmmo())
        {
            Debug.Log("No ammo!");
            return;
        }

        Vector3 startPoint = bulletPoint.position;

        if (shotgunMode)
        {
            ShootWithSpread(startPoint, 5, 30f, 35f);
        }
        else if (pistolMode)
        {
            ShootSingle(startPoint, 35f);
        }
        else if (railgunMode)
        {
            ShootSingle(startPoint, 100f);
        }

        ReduceAmmo();
    }

    void ShootSingle(Vector3 startPoint, float damage)
    {
        Vector3 direction = bulletPoint.up;
        Vector3 endPoint = startPoint + direction * maxDistance;

        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, maxDistance, barrierLayer | enemyLayer | BulletHitLayer);

        while (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit");
                EnemyHealth enemyH = hit.collider.GetComponent<EnemyHealth>();
                enemyH.GetHit(damage);
                endPoint = hit.point;
                break;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Barrier"))
            {
                startPoint = (Vector3)hit.point + direction * 0.1f;
                hit = Physics2D.Raycast(startPoint, direction, maxDistance, enemyLayer);
                continue;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BulletHitLayer"))
            {
                Debug.Log("Bullet hit an object in BulletHitLayer.");
                endPoint = hit.point;
                break;
            }
            break;
        }

        StartCoroutine(BulletTrail(startPoint, endPoint));
    }
    void ShootWithSpread(Vector3 startPoint, int pelletCount, float spreadAngle, float damage)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            float angleOffset = -spreadAngle / 2 + (spreadAngle / (pelletCount - 1)) * i;
            Vector3 direction = Quaternion.Euler(0, 0, angleOffset) * bulletPoint.up;

            Vector3 endPoint = startPoint + direction * maxDistance;

            RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, maxDistance, barrierLayer | enemyLayer | BulletHitLayer);

            while (hit.collider != null)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy hit");
                    EnemyHealth enemyH = hit.collider.GetComponent<EnemyHealth>();
                    enemyH.GetHit(damage);
                    endPoint = hit.point;
                    break;
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Barrier"))
                {
                    startPoint = (Vector3)hit.point + direction * 0.1f;
                    hit = Physics2D.Raycast(startPoint, direction, maxDistance, enemyLayer);
                    continue;
                }
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("BulletHitLayer"))
                {
                    Debug.Log("Bullet hit an object in BulletHitLayer.");
                    endPoint = hit.point;
                    break;
                }
                break;
            }

            StartCoroutine(BulletTrail(startPoint, endPoint));
        }
    }

    bool HasAmmo()
    {
        if (pistolMode && pistolAmmoInClip > 0) return true;
        if (shotgunMode && shotgunAmmoInClip > 0) return true;
        if (railgunMode && railgunAmmoInClip > 0) return true;
        return false;
    }
    void ReduceAmmo()
    {
        if (pistolMode && pistolAmmoInClip > 0)
            pistolAmmoInClip--;
        else if (shotgunMode && shotgunAmmoInClip > 0)
            shotgunAmmoInClip--;
        else if (railgunMode && railgunAmmoInClip > 0)
            railgunAmmoInClip--;

        UpdateAmmoUI();
    }
    IEnumerator Reload()
    {
        Reloading = true;
        ammoBar.StartReload(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        if (pistolMode)
        {
            float neededAmmo = pistolClipSize - pistolAmmoInClip;
            float ammoToLoad = Mathf.Min(neededAmmo, pistolTotalAmmo);
            pistolAmmoInClip += ammoToLoad;
            pistolTotalAmmo -= ammoToLoad;
        }
        else if (shotgunMode)
        {
            float neededAmmo = shotgunClipSize - shotgunAmmoInClip;
            float ammoToLoad = Mathf.Min(neededAmmo, shotgunTotalAmmo);
            shotgunAmmoInClip += ammoToLoad;
            shotgunTotalAmmo -= ammoToLoad;
        }
        else if (railgunMode)
        {
            float neededAmmo = railgunClipSize - railgunAmmoInClip;
            float ammoToLoad = Mathf.Min(neededAmmo, railgunTotalAmmo);
            railgunAmmoInClip += ammoToLoad;
            railgunTotalAmmo -= ammoToLoad;
        }

        Reloading = false;
        UpdateAmmoUI();
    }
    void UpdateAmmoUI()
    {
        if (ammoBar != null)
        {
            int currentAmmo = pistolMode ? (int)pistolAmmoInClip : shotgunMode ? (int)shotgunAmmoInClip : (int)railgunAmmoInClip;
            float totalAmmo = pistolMode ? pistolTotalAmmo : shotgunMode ? shotgunTotalAmmo : railgunTotalAmmo;

            ammoBar.UpdateAmmo(currentAmmo, totalAmmo);
        }
    }
    void Pistol()
    {
        pistolMode = true;
        shotgunMode = false;
        railgunMode = false;
        reloadTime = 0.5f;
        UpdateAmmoUI();
    }
    void Shotgun()
    {
        shotgunMode = true;
        pistolMode = false;
        railgunMode = false;
        reloadTime = 1f;
        UpdateAmmoUI();
    }
    void Railgun()
    {
        railgunMode = true;
        pistolMode = false;
        shotgunMode = false;
        reloadTime = 1f;
        UpdateAmmoUI();
    }

    void CycleWeapon(int direction)
    {
        if (pistolMode)
        {
            if (direction > 0) Shotgun();
            else Railgun();
        }
        else if (shotgunMode)
        {
            if (direction > 0) Railgun();
            else Pistol();
        }
        else if (railgunMode)
        {
            if (direction > 0) Pistol();
            else Shotgun();
        }

        Debug.Log("Switched to " + (pistolMode ? "Pistol" : shotgunMode ? "Shotgun" : "Railgun"));
    }

    private IEnumerator BulletTrail(Vector3 start, Vector3 end)
    {
        TrailRenderer trailPrefab = railgunMode ? railgunTrail : pistolTrail;
        TrailRenderer trail = Instantiate(trailPrefab, start, Quaternion.identity);

        float time = 0;
        float duration = Vector3.Distance(start, end) / bulletSpeed;

        while (time < duration)
        {
            trail.transform.position = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = end;
        Destroy(trail.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            currentBarrier = collision.GetComponent<Barrier>();

            if (currentBarrier.health < 50)
            {
                ShowRepairUI();
                nearBarrier = true;
                ammoBar.reloadBar.fillAmount = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            HideRepairUI();
            nearBarrier = false;
            currentBarrier = null;
        }
    }

    private IEnumerator RepairBarrier()
    {
        repairing = true;
        float elapsed = 0;
        scraps -= 5;
        while (elapsed < repairTime)
        {
            if (!nearBarrier) break; // Stop if player moves away
            ammoBar.Bar.SetActive(true);
            elapsed += Time.deltaTime;
            ammoBar.reloadBar.fillAmount = elapsed / repairTime; // Update UI Fill
            yield return null;
        }

        if (nearBarrier) // If still near after full repair time
        {
            ammoBar.Bar.SetActive(false);
            currentBarrier.RepairBarrier(50);
        }

        repairing = false;
        HideRepairUI();
    }

    void ShowRepairUI()
    {
        repairPromptUI.SetActive(true);
        ammoBar.reloadBar.fillAmount = 0;
    }

    void HideRepairUI()
    {
        repairPromptUI.SetActive(false);
    }
    void RotateGuy()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint
        (Input.mousePosition) - mc.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        mc.rotation = rotation;
    }
}