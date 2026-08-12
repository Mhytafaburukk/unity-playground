using UnityEngine;
using System.Collections;

public class HealthProxy : MonoBehaviour
{
    
    public PlayerHealth mainHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Füze Front collider'a çarptığında helikopteri patlat
        if (collision.gameObject.CompareTag("Fuze"))
        {
            // Helikopterin ana objesini bul (parent)
            TriggerManager tm = GetComponentInParent<TriggerManager>();
            if (tm != null)
            {
                tm.DestroyHelicopter();
            }

            // Mermiyi yok et
            Destroy(collision.gameObject);
            return;
        }

        // Diğer çarpışmalar için eski davranış
        if (collision.gameObject.CompareTag("Zipkin"))
        {
            if (mainHealth != null)
            {
                mainHealth.ChangeHealth(-6);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fuze"))
        {
            TriggerManager tm = GetComponentInParent<TriggerManager>();
            if (tm != null) tm.DestroyHelicopter();
            Destroy(collision.gameObject);
        }
    }
}
