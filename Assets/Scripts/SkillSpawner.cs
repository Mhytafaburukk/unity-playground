using UnityEngine;
using System;
using System.Collections;

public class SkillSpawner : MonoBehaviour
{
    public GameObject[] objePrefabs;
    private bool isTimerRunning = false;


    private void Start()
    {
        int index = UnityEngine.Random.Range(0, objePrefabs.Length);
        GameObject firstObj = Instantiate(objePrefabs[index], transform.position, Quaternion.identity);
        SkillItem item = firstObj.GetComponent<SkillItem>();
        if (item != null)
        {
            item.mySpawner = this;
        }
    }

    public void StartSpawnTimer(float delay)
    {
        if (!isTimerRunning) 
        {
            StartCoroutine(SpawnTimerRoutine(delay));
        }
    }

    private IEnumerator SpawnTimerRoutine(float delay)
    {
        isTimerRunning = true; 
        yield return new WaitForSeconds(delay);
        
        SpawnSkill();
        isTimerRunning = false; 
    }

    public void SpawnSkill()
    {
        int index = UnityEngine.Random.Range(0, objePrefabs.Length);
        GameObject spawnedObj = Instantiate(objePrefabs[index], transform.position, Quaternion.identity);
        SkillItem item = spawnedObj.GetComponent<SkillItem>();
        if(item != null) item.mySpawner = this;
    }
}