using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
public GameObject[] prefabs;

public CinemachineCamera vcam1;
public CinemachineCamera vcam2;

public Transform[] hedefler;
public Transform helicopterStart;

public void Start()
    {
        GameObject objLeft = GameObject.Find("Cam_left");
        GameObject objRight = GameObject.Find("Cam_right");

        if (objRight) vcam1 = objRight.GetComponent<CinemachineCamera>();
        if (objLeft) vcam2 = objLeft.GetComponent<CinemachineCamera>();
    }
public void RespawnPlayer(bool isTrigerred1,bool isTrigerred2,bool isTrigerred3,int asama,bool isPlayer1, float delay)
{
Debug.Log("Calistim");
StartCoroutine(SpawnRoutine(isTrigerred1,isTrigerred2,isTrigerred3,asama,isPlayer1, delay));
}
public void RespawnPlayer2(int asama,bool isPlayer1,float delay)
{
Debug.Log("Calistim");
StartCoroutine(SpawnRoutine3(asama,isPlayer1,delay));
}
public void SwitchPlayer(bool isTrigerred1,bool isTrigerred2,bool isTrigerred3,int asama ,bool isZipkinli ,bool isFuzeli,Vector3 playerPos , bool isPlayer1,GameObject oldPlayer)
{
StartCoroutine(SpawnRoutine2(isTrigerred1,isTrigerred2,isTrigerred3,asama,isZipkinli,isFuzeli,playerPos , isPlayer1,oldPlayer));
}


private IEnumerator SpawnRoutine2(bool isTrigerred1,bool isTrigerred2,bool isTrigerred3,int asama,bool isZipkinli,bool isFuzeli, Vector3 playerPos, bool isPlayer1, GameObject oldPlayer)
{
    int index = isZipkinli ? (isPlayer1 ? 5:4) :isFuzeli ? (isPlayer1 ? 3 : 2) : (isPlayer1 ? 1 : 0);
    GameObject newPlayer = Instantiate(prefabs[index], playerPos, Quaternion.identity);
    
    PlayerController pc = newPlayer.GetComponent<PlayerController>();
    PlayerHealth ph = newPlayer.GetComponent<PlayerHealth>();
    PlayerHealth phold = oldPlayer.GetComponent<PlayerHealth>();
    if (pc != null)
    {
        pc.isTrigerred1 = isTrigerred1;
        pc.isTrigerred2 = isTrigerred2;
        pc.isTrigerred3 = isTrigerred3;
        pc.SpawnLevel = asama;
        pc.isBought = (isZipkinli || isFuzeli);
        pc.canFire = isFuzeli;
        pc.canFire2 = isZipkinli;
        pc.spawnManager = this;
    }
    if(ph != null && phold != null)
        {
            ph.currentHealth = phold.currentHealth;
            ph.maxHealth = phold.maxHealth;
        }

    if (isPlayer1)
    {
        if (vcam1 != null) vcam1.Target.TrackingTarget = newPlayer.transform;
    }
    else
    {
        if (vcam2 != null) vcam2.Target.TrackingTarget = newPlayer.transform;
    }


    if (oldPlayer != null)
    {
        oldPlayer.tag = "Untagged"; 
        Destroy(oldPlayer);        
    }
    
    yield break; 
}
private IEnumerator SpawnRoutine(bool isTrigerred1,bool isTrigerred2,bool isTrigerred3,int asama,bool isPlayer1, float delay)
{
yield return new WaitForSeconds(delay);


int index = isPlayer1 ? 1 : 0;

GameObject newPlayer = Instantiate(prefabs[index], hedefler[asama].position, Quaternion.identity);
PlayerController pc = newPlayer.GetComponent<PlayerController>();
    if(pc!= null)
        {
            pc.isTrigerred1 = isTrigerred1;
            pc.isTrigerred2 = isTrigerred2;
            pc.isTrigerred3 = isTrigerred3;
            pc.SpawnLevel = asama;
        }
if (isPlayer1)
{
if (vcam1 != null) vcam1.Target.TrackingTarget = newPlayer.transform;
}
else
{
if (vcam2 != null) vcam2.Target.TrackingTarget = newPlayer.transform;
}
}
    private IEnumerator SpawnRoutine3(int asama, bool isPlayer1, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Index geçersizse 0'a düşür
        if (asama < 0 || asama >= hedefler.Length)
        {
            Debug.LogWarning($"[PlayerSpawn] Helicopter spawn index {asama} geçersiz! hedefler.Length={hedefler.Length}. Index 0 kullanılıyor.");
            asama = 0;
        }

        if (hedefler.Length == 0)
        {
            Debug.LogError("[PlayerSpawn] hedefler dizisi boş! Helikopter spawn edilemiyor.");
            yield break;
        }

        GameObject newHelicopter = Instantiate(prefabs[6], hedefler[asama].position, Quaternion.identity);
        if (isPlayer1)
        {
            if (vcam1 != null) vcam1.Target.TrackingTarget = newHelicopter.transform;
        }
        else
        {
            if (vcam2 != null) vcam2.Target.TrackingTarget = newHelicopter.transform;
        }
    }

    } 
