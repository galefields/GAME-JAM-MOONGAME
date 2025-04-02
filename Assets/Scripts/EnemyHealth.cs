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
    void OnEnable()
    {
        health = 100;
        spriteRenderer.color = Color.white;
    }
    public void GetHit(float damage)
    {
        health -= damage;
        Debug.Log("owie");
        StartCoroutine(DamageFx());
        if (health <= 0)
        {
            GetRekt();
        }
    }
    IEnumerator DamageFx()
    {
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.3f);
        spriteRenderer.color = Color.white;
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
