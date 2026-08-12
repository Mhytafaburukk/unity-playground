using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public PlayerSpawn spawnManager; 

    public TextMeshProUGUI healthText;

    public Animator healthTextAnim;

    public bool isHelicopter;

    public bool isP1;

    void Start()
    {
        isHelicopter = gameObject.CompareTag("Helicopter");
        string canvasName = gameObject.CompareTag("Player2") ? "Canvas_Left" : "Canvas_Right";
        GameObject canvas = GameObject.Find(canvasName);
        
        if (canvas != null)
        {
            healthText = canvas.GetComponentInChildren<TextMeshProUGUI>();
            if (healthText != null)
            {
                healthTextAnim = healthText.GetComponent<Animator>();
            }
            UpdateUI();
        }

        if (spawnManager == null) spawnManager = FindFirstObjectByType<PlayerSpawn>();
    }
    public void UpdateUI() 
    {
        if (healthText != null) healthText.text = currentHealth.ToString();
    }

public void ChangeHealth(int amount)
{
    Debug.Log("Kanit");
    if (gameObject.CompareTag("Untagged")) return;

    currentHealth += amount;

        if (gameObject.CompareTag("Player1"))
        {
            healthText.text = currentHealth+ " / " + maxHealth;
            healthTextAnim.Play("TextUpdate");
        }
        else if (gameObject.CompareTag("Player2"))
        {
            healthText.text = currentHealth+ " / " + maxHealth;
            healthTextAnim.Play("TextUpdate");
        }

    if (currentHealth <= 0)
    {
        if (isHelicopter)
        {
            // Particle efektiyle yok etmek için TriggerManager üzerinden yönet
            TriggerManager tm = GetComponent<TriggerManager>();
            if (tm != null)
            {
                tm.DestroyHelicopter();
            }
            else
            {
                // TriggerManager yoksa eski yöntemle respawn et
                Debug.Log("Burdaaa");
                StartCoroutine(HelicopterRespawnRoutine());
            }
        }
        else
        {
        bool isP1 = gameObject.CompareTag("Player1");
        PlayerController pc = gameObject.GetComponent<PlayerController>();

        if (spawnManager != null)
        {
            int asama = pc.SpawnLevel;
            spawnManager.RespawnPlayer(pc.isTrigerred1,pc.isTrigerred2,pc.isTrigerred3,asama,isP1, 2f);
        }


        Destroy(gameObject);
        }
    }
}
IEnumerator HelicopterRespawnRoutine()
{
    yield return new WaitForSeconds(5f);

    if (spawnManager != null)
    {
        spawnManager.RespawnPlayer2(5, isP1, 5f);
    }

    Destroy(gameObject);
}
}
