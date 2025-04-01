using UnityEngine;
using System.Collections;

public class Barrier : MonoBehaviour
{
    public float health = 100f;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Barrier Health: " + health);
        StartCoroutine(pulse());

        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Barrier Destroyed!");
            GameOver();
        }
    }
    IEnumerator pulse()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(.25f);
        spriteRenderer.color = Color.white;
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}

