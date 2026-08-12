using System;
using System.Collections;
using UnityEngine;

public class SpawnSosisManager : MonoBehaviour
{
    public GameObject objePrefab;
    public float spawnAraliği = 3f;
    public float destroyTime = 2f;

    void Start()
    {
        StartCoroutine(SpawnRouitine());
    }
    IEnumerator SpawnRouitine(){
        while(true){
            SpawnObje();
            yield return new WaitForSeconds(spawnAraliği);
        }
    }
    void SpawnObje(){
        GameObject sosisEngeli = Instantiate(objePrefab,transform.position,Quaternion.identity);
        sosisEngeli.tag = "ground";
        Destroy(sosisEngeli,destroyTime);
    }

}
