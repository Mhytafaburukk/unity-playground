using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] float hiz = 13f;
    [SerializeField] float hiz2 = 22f;
    [SerializeField] float cekilmeHizi = 12f;
    
    private float yon = 1f;
    private GameObject playerObject;
    public PlayerSpawn spawnManager;

    public PlayerController playerController;
    private KameraZoomKontrol cameraControl;
    private bool isMoving = true;
    private string originalTag;
    private bool isZipkin;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (spawnManager == null) spawnManager = Object.FindFirstObjectByType<PlayerSpawn>();
        if (cameraControl == null) cameraControl = Object.FindFirstObjectByType<KameraZoomKontrol>();
         if (playerController == null) playerController = Object.FindFirstObjectByType<PlayerController>();
        isZipkin = gameObject.CompareTag("Zipkin9");
        isMoving = true;
    }

    private void Update()
    {
        if (!isZipkin)
        {
            transform.Translate(Vector3.right * hiz * yon * Time.deltaTime);
        }
        else if (isZipkin && !isMoving)
        {
            playerObject.transform.position = Vector3.MoveTowards(playerObject.transform.position, transform.position, cekilmeHizi * Time.deltaTime);
            if (cameraControl != null) 
            {
                cameraControl.ZoomuKapat(playerObject.CompareTag("Player1"));
            }
            if (Vector3.Distance(playerObject.transform.position, transform.position) < 2f)
            {
                Debug.Log("Burdayim");
                playerObject.tag = originalTag;
                Destroy(gameObject);
                bool isP1 = playerObject.CompareTag("Player1");

                spawnManager.SwitchPlayer(playerController.isTrigerred1,playerController.isTrigerred2,playerController.isTrigerred3,playerController.SpawnLevel,false, false, transform.position, isP1, playerObject);
            }
        }
        else if (isZipkin && isMoving)
        {
            transform.Translate(Vector3.right * hiz2 * yon * Time.deltaTime);
        }
    }

    public void YonuAyarla(bool sagaBakiyor)
    {
        yon = sagaBakiyor ? 1f : -1f;

        Vector3 mermiScale = transform.localScale;
        mermiScale.x = Mathf.Abs(mermiScale.x); 
        transform.localScale = mermiScale;


        if (sagaBakiyor)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 180);

        yon = 1f; 
    }

    public void SelectedCharacter(GameObject player)
    {
        playerObject = player;
        Collider2D mermiCol = GetComponent<Collider2D>();

        // Ana collider'ı yoksay
        Collider2D playerCol = player.GetComponent<Collider2D>();
        if (mermiCol != null && playerCol != null)
        {
            Physics2D.IgnoreCollision(mermiCol, playerCol);
        }

        // Tüm child collider'ları da yoksay (Front, Top, Bottom, Right vb.)
        // Böylece helikopterin kendi mermisi kendi collider'larına çarpmaz
        Collider2D[] childColliders = player.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D childCol in childColliders)
        {
            if (mermiCol != null && childCol != null)
            {
                Physics2D.IgnoreCollision(mermiCol, childCol);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == playerObject) return;

        if (collision.gameObject.CompareTag("Helicopter"))
        {
            // Front collider'a çarptıysa (HealthProxy varsa) helikopteri patlat
            HealthProxy hp = collision.collider.GetComponent<HealthProxy>();
            if (hp != null)
            {
                TriggerManager tm = collision.gameObject.GetComponent<TriggerManager>();
                if (tm == null) tm = collision.gameObject.GetComponentInParent<TriggerManager>();
                if (tm != null)
                {
                    tm.DestroyHelicopter();
                }
                if (!isZipkin) { Destroy(gameObject); return; }
            }
            else
            {
                // Front değil, diğer collider'lara çarptı - normal hasar ver
                PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
                if (ph != null) ph.ChangeHealth(-6);

                // Helikopterdeki player'a da hasar ver
                PlayerHealth helicopterPH = collision.gameObject.GetComponent<PlayerHealth>();
                if (helicopterPH == null) helicopterPH = collision.gameObject.GetComponentInChildren<PlayerHealth>();
                if (helicopterPH != null)
                {
                    helicopterPH.ChangeHealth(-6);
                }

                if (!isZipkin) { Destroy(gameObject); return; }
            }
        }
        else if (collision.gameObject.CompareTag("Player1") || collision.gameObject.CompareTag("Player2") || collision.gameObject.GetComponent<HealthProxy>() != null)
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null) ph.ChangeHealth(isZipkin ? -1 : -6);
            if (!isZipkin) { Destroy(gameObject); return; }
        }
        else if (collision.gameObject.CompareTag("Stone"))
        {
            StoneEndurance se = collision.gameObject.GetComponent<StoneEndurance>();
            if (se != null) se.ChangeHealth(-1);
            if (!isZipkin) { Destroy(gameObject); return; }
        }

        if (isZipkin && isMoving)
        {
            isMoving = false;
            if (rb != null) rb.bodyType = RigidbodyType2D.Static;
            GetComponent<Collider2D>().isTrigger = true;

            if (playerObject != null)
            {
                originalTag = playerObject.tag;
                playerObject.tag = "Untagged";
                Rigidbody2D pRb = playerObject.GetComponent<Rigidbody2D>();
                if (pRb != null) 
                {
                    pRb.linearVelocity = Vector2.zero;
                    pRb.bodyType = RigidbodyType2D.Kinematic; 
                    pRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
        else if (!isZipkin)
        {
            Destroy(gameObject);
        }
    }
}

