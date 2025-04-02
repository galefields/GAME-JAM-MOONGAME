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
        int reward = Random.Range(0, 3); // 0 = Shotgun Ammo, 1 = Railgun Ammo, 2 = Scraps
        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null)
        {
            switch (reward)
            {
                case 0:
                    player.shotgunTotalAmmo += 12;
                    
                    break;
                case 1:
                    player.railgunTotalAmmo += 9;
                   
                    break;
                case 2:
                    player.scraps += Random.Range(2,6);
                    
                    break;
            }
        }

        FindObjectOfType<SpawnSystem>().EnemyDestroyed(gameObject);
    }
}
