using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed = 8.5f;
    private Transform weapon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        weapon = transform.GetChild(0);
    }
    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        rb.linearVelocity = new Vector2 (moveHorizontal, moveVertical) * speed;
        RotateWeapon();
    }
    private void Shoot()
    {

    }
    private void RotateWeapon()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint
                 (Input.mousePosition) - weapon.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        weapon.rotation = rotation;
    }
}
