using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Pool;

public class EnemyM : MonoBehaviour
{
    private Barrier barrier;
    public float speed = 3f;
    private bool stopped = false;
    private Rigidbody2D rb;
    public float attackRate = 1f;
    private Coroutine attackCoroutine;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite attackSprite;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.left * speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        stopped = false;
        if (rb != null)
        rb.linearVelocity = Vector2.left * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            barrier = collision.GetComponent<Barrier>();
            stopped = true;
            rb.linearVelocity = Vector2.zero;
            if (attackCoroutine == null)
            { attackCoroutine = StartCoroutine(Bash());}
        }
    }
    private IEnumerator Bash()
    {
        while (stopped)
        {
            spriteRenderer.sprite = attackSprite; barrier.TakeDamage(10);
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.sprite = defaultSprite;
            yield return new WaitForSeconds(attackRate);
        }
    }
    private void OnDisable()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        stopped = false;
    }
}

