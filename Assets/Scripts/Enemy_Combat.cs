using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;
private void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2"))
    {
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ChangeHealth(-damage);
            Debug.Log("Hasar verildi! Yeni Can: " + health.currentHealth);
        }
    }
}
}
