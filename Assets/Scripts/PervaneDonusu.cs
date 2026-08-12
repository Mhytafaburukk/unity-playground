using UnityEngine;

public class PervaneDonusu : MonoBehaviour
{
    [Header("Dönüş Ayarları")]
    public float donmeHizi = 500f; 

    public bool isTurning = false;

    void Update()
    {
        if(isTurning){
        transform.Rotate(0, donmeHizi * Time.deltaTime, 0);
        }
    }
}