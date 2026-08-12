using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorMovement : MonoBehaviour
{
    public Transform hedefDown, hedefUp;
    public float moveSpeed; 
    public bool isMoving;

    void Start()
    {
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        while (true)
        {
            isMoving = true;
            yield return StartCoroutine(MoveToTarget(hedefUp.position));

            isMoving = false;
            yield return new WaitForSeconds(2f); 

            isMoving = true;
            yield return StartCoroutine(MoveToTarget(hedefDown.position));

            isMoving = false;
            yield return new WaitForSeconds(2f); 
        }
    }

    IEnumerator MoveToTarget(Vector3 hedef)
    {
        while (Vector3.Distance(transform.position, hedef) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, hedef, moveSpeed * Time.deltaTime);
            
            yield return null; 
        }

        transform.position = hedef;
    }
}
