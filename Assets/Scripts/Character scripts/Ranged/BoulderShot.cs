using UnityEngine;
using System.Collections;

public class BoulderShot : MonoBehaviour
{
    public float speed = 8f;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.left * speed; // Move left
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            Debug.Log("BoulderS hit a barrier!");
            Barrier barrier = collision.GetComponent<Barrier>();
            barrier.TakeDamage(5);
            Destroy(gameObject);
        }
    }
}
