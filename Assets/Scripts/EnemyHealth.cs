using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float health = 100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void GetHit(float damage)
    {
        health -= damage;
        Debug.Log("owie");
        if (health <= 0)
        {
            GetRekt();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GetRekt()
    {
        FindObjectOfType<SpawnSystem>().EnemyDestroyed(gameObject);
    }
}
