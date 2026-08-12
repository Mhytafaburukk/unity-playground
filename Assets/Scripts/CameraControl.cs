using UnityEngine;
using Unity.Cinemachine;

public class KameraZoomKontrol : MonoBehaviour
{
    private CinemachineCamera camLeft;
    private CinemachineCamera camRight;
    
    public float normalBoyutt;

    public float helicopterBoyutt = 12f;
    public float uzakBoyut = 35f;
    public float gecisHizi = 2f;

    public bool canChange = false;
    private float hedefBoyutLeft;
    private float hedefBoyutRight;

    void Start()
    {
        normalBoyutt = 9f;
        GameObject objLeft = GameObject.Find("Cam_left");
        GameObject objRight = GameObject.Find("Cam_right");

        if (objLeft) camLeft = objLeft.GetComponent<CinemachineCamera>();
        if (objRight) camRight = objRight.GetComponent<CinemachineCamera>();

        hedefBoyutLeft = normalBoyutt;
        hedefBoyutRight = normalBoyutt;
    }
    public void isHelicopter(bool isP1)
    {
        canChange = true;
        if (isP1) hedefBoyutRight = helicopterBoyutt; 
        else hedefBoyutLeft = helicopterBoyutt;
    }

    void Update()
    {
        if (camLeft != null)
            camLeft.Lens.OrthographicSize = Mathf.Lerp(camLeft.Lens.OrthographicSize, hedefBoyutLeft, Time.deltaTime * gecisHizi);

        if (camRight != null)
            camRight.Lens.OrthographicSize = Mathf.Lerp(camRight.Lens.OrthographicSize, hedefBoyutRight, Time.deltaTime * gecisHizi);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1")) hedefBoyutRight = uzakBoyut;
        if (other.CompareTag("Player2")) hedefBoyutLeft = uzakBoyut;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player1")) hedefBoyutRight = normalBoyutt;
        if (other.CompareTag("Player2")) hedefBoyutLeft = normalBoyutt;
    }
    public void ZoomuKapat(bool isP1)
    {
    canChange = false;
    
    if (isP1)
    {
        hedefBoyutRight = normalBoyutt; 
    }
    else
    {
        hedefBoyutLeft = normalBoyutt; 
    }
    
    Debug.Log("Kamera Zoomu Kapatıldı: " + (isP1 ? "Player1" : "Player2"));
    }
}
