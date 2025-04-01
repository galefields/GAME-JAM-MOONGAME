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

    [SerializeField] private TrailRenderer pistolTrail;
    [SerializeField] private TrailRenderer railgunTrail;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask barrierLayer;
    [SerializeField] private LayerMask BulletHitLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Pistol();
        rb = GetComponent<Rigidbody2D>();
        mc = GetComponent<Transform>();
        bulletPoint = transform.GetChild(0);
    }
    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2 (moveHorizontal, moveVertical) * speed;
        RotateGuy();
       
        if (Input.GetMouseButtonDown(0) && !Reloading)
        {
            Shoot();
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
    }
    void Shoot()
    {
        if (!HasAmmo())
        {
            Debug.Log("No ammo!");
            return;
        }

        // Start at the bullet point
        Vector3 startPoint = bulletPoint.position;

        if (shotgunMode)
        {
            // Shotgun has multiple pellets with spread
            ShootWithSpread(startPoint, 5, 30f, 35f); // 5 pellets, 30° spread, 35 damage
        }
        else if (pistolMode)
        {
            // Pistol is a single shot with direct damage
            ShootSingle(startPoint, 35f); // 35 damage for pistol
        }
        else if (railgunMode)
        {
            // Railgun is a single shot with high damage
            ShootSingle(startPoint, 100f); // 100 damage for railgun
        }

        ReduceAmmo();
    }

    void ShootSingle(Vector3 startPoint, float damage)
    {
        // Fire a single shot in the current direction
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
            pistolAmmoInClip = Mathf.Max(0, pistolAmmoInClip - 1);
        if (shotgunMode && shotgunAmmoInClip > 0)
            shotgunAmmoInClip = Mathf.Max(0, shotgunAmmoInClip - 1);
        if (railgunMode && railgunAmmoInClip > 0)
            railgunAmmoInClip = Mathf.Max(0, railgunAmmoInClip - 1);
    }
    IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        Reloading = true;
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
        Debug.Log("Reloaded!");
    }
    void Pistol()
    {
        pistolMode = true;
        shotgunMode = false;
        railgunMode = false;
        reloadTime = 1.5f;
    }
    void Shotgun()
    {
        shotgunMode = true;
        pistolMode = false;
        railgunMode = false;
        reloadTime = 2.5f;
    }
    void Railgun()
    {
        railgunMode = true;
        pistolMode = false;
        shotgunMode = false;
        reloadTime = 3.5f;
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
    void RotateGuy()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint
        (Input.mousePosition) - mc.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        mc.rotation = rotation;
    }
}
