using TMPro;
using UnityEngine;
using System.Collections;
using System.Threading;

public class EnemyR : MonoBehaviour
{
    public float speed = 3f;
    public float stopTime = 2f;

    private Rigidbody2D rb;
    public GameObject projectilePrefab;
    private Transform bulletPoint;
    public float fireRate = 4.25f;
    private Coroutine shootingCoroutine;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite attackSprite;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletPoint = transform.GetChild(0);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);

        rb.linearVelocity = Vector2.left * speed;
        StartCoroutine(StopAndShoot());
    }

    private IEnumerator StopAndShoot()
    {
        yield return new WaitForSeconds(stopTime);

        rb.linearVelocity = Vector2.zero;

        shootingCoroutine = StartCoroutine(FireMode());
    }

    private IEnumerator FireMode()
    {
        while (true)
        {
            ShootRock(); spriteRenderer.sprite = attackSprite;
            yield return new WaitForSeconds (0.2f);
            spriteRenderer.sprite = defaultSprite;
            yield return new WaitForSeconds(fireRate);
           
        }
    }

    void ShootRock()
    {
        if (projectilePrefab != null && bulletPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, bulletPoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.left * 8f;
            }
        }
    }

    private void OnDisable()
    {
        if (shootingCoroutine != null)
            StopCoroutine(shootingCoroutine);
    }
}
