using UnityEngine;
using TMPro;
using System.Collections;

public class StoneEndurance : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    public TriggerManager triggerManager;


    void Start()
    {
        if (triggerManager == null) triggerManager = Object.FindFirstObjectByType<TriggerManager>();
        currentHealth = maxHealth;

        maxHealth = 15;

    }

public void ChangeHealth(int amount)
{
    if (gameObject.CompareTag("Untagged")) return;

    currentHealth += amount;
    if(currentHealth == 1)
        {
            triggerManager.canEscape = true;
        }

    if (currentHealth <= 0)
    {
        Destroy(gameObject);
    }
    }
}
