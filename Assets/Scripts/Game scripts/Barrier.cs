using UnityEngine;
using System.Collections;

public class Barrier : MonoBehaviour
{
    public float health = 100f;
    private SpriteRenderer spriteRenderer;
    public Sprite highHealthSprite;
    public Sprite mediumHealthSprite;
    public Sprite lowHealthSprite;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateBarrierSprite();
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
        else
        {
            UpdateBarrierSprite();
        }

    }

    public void RepairBarrier(float repairAmount)
    {
        health = Mathf.Min(health + repairAmount, 100f);
        Debug.Log("Barrier Repaired! New Health: " + health);
        UpdateBarrierSprite();
    }

    IEnumerator pulse()
    {
        spriteRenderer.color = Color.grey;
        yield return new WaitForSeconds(.25f);
        spriteRenderer.color = Color.white;
    }
    private void UpdateBarrierSprite()
    {
        if (health < 25)
            spriteRenderer.sprite = lowHealthSprite;
        else if (health < 50)
            spriteRenderer.sprite = mediumHealthSprite;
        else if (health > 50)
            spriteRenderer.sprite = highHealthSprite;
    }
    void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}

