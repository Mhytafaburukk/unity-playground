using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class TriggerManager : MonoBehaviour
{
    [Header("Kontrol Ayarları")]
    public float kontrolHizi = 7f; 
    private bool isControlled = false; 
    private Rigidbody2D rb; 

    [HideInInspector] public bool canEscape = false; 

    [Header("Story Mod (Sinematik) Ayarları")]
    public Transform[] storyHedefler;
    public float storyHiz = 5f;
    private bool isStoryModeActive = false;
    private int currentStoryIndex = 0;
    private GameObject storyModeObj;

    [Header("Bileşenler")]
    public BoxCollider2D obstacle;
    private GameObject internalColliderObj; 
    public CinemachineCamera vcam1;
    public CinemachineCamera vcam2;
    public KameraZoomKontrol cameraControl;
    public SkillSpawner skillSpawner;
    public PervaneDonusu pervaneDonusu;
    public PlayerHealth playerHealth;
    public GameObject player;

    [Header("Silah Sistemi")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;
    public float atisAraligi = 5f;
    private bool isFiring = false; 

    [Header("Patlama Ayarları")]
    [SerializeField] private ParticleSystem patlamaParticle; 
    [SerializeField] private float patlamaSuresi = 2f;

    private bool isZehirli;
    private bool isP1;
    private bool canChange;
    private bool canChange2;
    private bool isDestroyed = false;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.gravityScale = 0; 
            rb.freezeRotation = true; 
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (playerHealth == null) playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        if (skillSpawner == null) skillSpawner = Object.FindFirstObjectByType<SkillSpawner>();
        if (cameraControl == null) cameraControl = Object.FindFirstObjectByType<KameraZoomKontrol>();
        if (pervaneDonusu == null) pervaneDonusu = Object.FindFirstObjectByType<PervaneDonusu>();
        if (patlamaParticle == null) patlamaParticle = GetComponentInChildren<ParticleSystem>();

        internalColliderObj = transform.Find("GameObject")?.gameObject;
        
        // Sahnede StoryMode tag'li objeyi bul ve referans olarak tut
        storyModeObj = GameObject.FindGameObjectWithTag("StoryMode");

        GameObject objLeft = GameObject.Find("Cam_left");
        GameObject objRight = GameObject.Find("Cam_right");
        if (objRight) vcam1 = objRight.GetComponent<CinemachineCamera>();
        if (objLeft) vcam2 = objLeft.GetComponent<CinemachineCamera>();

        isZehirli = gameObject.CompareTag("Zehirli");
        
        if (obstacle != null) obstacle.enabled = false;

        // Başlangıçta GameObject tamamen kapalı olsun
        if (internalColliderObj != null)
        {
            internalColliderObj.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDestroyed) return;
        
        if (canChange) HandleCameraChange();
        if (canChange2) HandleCameraReturnToPlayer();

        // Story Mode hareketi Update üzerinden yapılıyor
        if (isStoryModeActive && storyHedefler != null && storyHedefler.Length > 0)
        {
            HandleStoryModeMovement();
        }
    }

    private void HandleStoryModeMovement()
    {
        if (currentStoryIndex >= storyHedefler.Length) return; // Zaten son hedefe varıldı

        Transform hedef = storyHedefler[currentStoryIndex];
        transform.position = Vector3.MoveTowards(transform.position, hedef.position, storyHiz * Time.deltaTime);

        // Hedefe yaklaştıysak
        if (Vector2.Distance(transform.position, hedef.position) < 0.1f)
        {
            currentStoryIndex++;
            
            // Eğer son hedefe vardıysa oyuncuyu indir
            if (currentStoryIndex >= storyHedefler.Length)
            {
                FinishStoryMode();
            }
        }
    }

    private void FinishStoryMode()
    {
        isStoryModeActive = false;
        gameObject.tag = "Untagged";
        if (pervaneDonusu != null) pervaneDonusu.isTurning = false;
        
        // Ateş etmeyi garantili durdur
        StopAllCoroutines();
        isFiring = false;

        // Bütün colliderları kapat ki oyuncu aşağı düşsün/insin
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach(Collider2D c in allColliders)
        {
            if (c != null) c.enabled = false;
        }

        // Helikopterin kendisindeki collider'ı da kapat
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null) myCol.enabled = false;
        
        if (cameraControl != null) cameraControl.ZoomuKapat(isP1);

        Debug.Log("Story Mode Bitti, oyuncu helikopterden indi.");

        // Scriptin çalışmasını tamamen durdur
        this.enabled = false;
    }

    private void FixedUpdate()
    {
        if (isDestroyed || (!isControlled && !isStoryModeActive)) 
        {
            if (rb != null && !isDestroyed && !isStoryModeActive) rb.linearVelocity = Vector2.zero;
            return;
        }

        // Eğer story moddaysa sadece oyuncu girdilerini alma (MoveTowards zaten Update'de hareket ettiriyor)
        if (isStoryModeActive)
        {
            rb.linearVelocity = Vector2.zero;
            // Helikopteri yavaşça düze çıkar
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.fixedDeltaTime * 2f);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(h, v).normalized;
        rb.linearVelocity = input * kontrolHizi;

        float tilt = h * -12f;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, tilt), Time.fixedDeltaTime * 5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed) return;

        if (collision.gameObject.CompareTag("Fuze"))
        {
            DestroyHelicopter();
            Destroy(collision.gameObject);
            return;
        }

        bool p1 = collision.gameObject.CompareTag("Player1");
        bool p2 = collision.gameObject.CompareTag("Player2");

        // Zehirli nesne oyuncuya değdiğinde oyuncuyu yok et
        if ((p1 || p2) && isZehirli)
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.ChangeHealth(-6);
            }
            Destroy(collision.gameObject);
            return;
        }

        if ((p1 || p2) && !isZehirli)
        {
            Debug.Log("Girdim - Pervane başlatılıyor");
            isP1 = p1;
            player = collision.gameObject;
            if (playerHealth != null) playerHealth.isP1 = isP1;

            isControlled = true;
            if (pervaneDonusu != null) {
                Debug.Log("PervaneDonusu atandı, isTurning = true");
                pervaneDonusu.isTurning = true;
            } else {
                Debug.LogError("PervaneDonusu NULL!");
            }
            
            // OBSTACLE VE COLLIDER AYARLARI
            if (obstacle != null) obstacle.enabled = true;
            if (internalColliderObj != null)
            {
                internalColliderObj.SetActive(true);
                internalColliderObj.tag = "Untagged";

                Rigidbody2D internalRb = internalColliderObj.GetComponent<Rigidbody2D>();
                if (internalRb != null)
                {
                    internalRb.bodyType = RigidbodyType2D.Kinematic;
                    internalRb.linearVelocity = Vector2.zero;
                }

                // Girdiği anda zemin collider'ını hemen aç ki düşmesin!
                Collider2D internalCol = internalColliderObj.GetComponent<Collider2D>();
                if (internalCol != null) internalCol.enabled = true;
            }

            // ATEŞLEMEYİ GARANTİYE AL
            StopAllCoroutines();
            isFiring = false;
            StartCoroutine(AtesEtmeRoutine());

            canChange = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fuze"))
        {
            DestroyHelicopter();
            Destroy(collision.gameObject);
        }

        // StoryMode trigger'ına çarpınca story mod başlasın
        if (collision.gameObject.CompareTag("StoryMode") && isControlled && !isStoryModeActive)
        {
            Debug.Log("StoryMode başladı!");
            isControlled = false; // Oyuncunun kontrolünü al
            isStoryModeActive = true;
            currentStoryIndex = 0;
            
            if (rb != null)
            {
                // Fizikten kopar, MoveTowards rahat çalışsın
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private IEnumerator AtesEtmeRoutine()
    {
        isFiring = true;
        
        if (bulletSpawn == null) bulletSpawn = transform.Find("BulletPoint");

        if (bulletPrefab == null || bulletSpawn == null)
        {
            Debug.LogWarning("bulletPrefab veya bulletSpawn atanmamış!");
            isFiring = false;
            yield break;
        }

        while (!isDestroyed && isControlled)
        {
            // Mermiyi BulletPoint pozisyonunda oluştur
            GameObject mermi = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
            
            BulletController bc = mermi.GetComponent<BulletController>();
            if (bc != null)
            {
                bc.SelectedCharacter(gameObject);
                float angle = bulletSpawn.localEulerAngles.z;
                bool sagaBakiyor = (angle < 90f || angle > 270f);
                bc.YonuAyarla(sagaBakiyor);
            }
            
            yield return new WaitForSeconds(atisAraligi);
        }
        isFiring = false;
    }

    private void HandleCameraChange()
    {
        if (isP1)
        {
            if (vcam1 != null) vcam1.Target.TrackingTarget = gameObject.transform;
            if (cameraControl != null) cameraControl.isHelicopter(true);
        }
        else
        {
            if (cameraControl != null) cameraControl.isHelicopter(false);
            if (vcam2 != null) vcam2.Target.TrackingTarget = gameObject.transform;
        }
        canChange = false;
    }

    private void HandleCameraReturnToPlayer()
    {
        if (player == null) return;
        if (isP1 && vcam1 != null) vcam1.Target.TrackingTarget = player.transform;
        else if (vcam2 != null) vcam2.Target.TrackingTarget = player.transform;
    }

    public void DestroyHelicopter()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        isControlled = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in renderers) if (sr != null) sr.enabled = false;

        if (patlamaParticle != null)
        {
            patlamaParticle.transform.SetParent(null);
            patlamaParticle.Play();
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in colliders) if (c != null) c.enabled = false;

        if (pervaneDonusu != null) pervaneDonusu.isTurning = false;
        if (cameraControl != null) cameraControl.ZoomuKapat(isP1);

        StartCoroutine(PatlamaVeRespawnRoutine());
    }

    private IEnumerator PatlamaVeRespawnRoutine()
    {
        yield return new WaitForSeconds(patlamaSuresi);
        PlayerSpawn spawnManager = Object.FindFirstObjectByType<PlayerSpawn>();
        if (spawnManager != null && player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                spawnManager.RespawnPlayer(pc.isTrigerred1, pc.isTrigerred2, pc.isTrigerred3, pc.SpawnLevel, isP1, 0.5f);
            }
            Destroy(player);
        }
        Destroy(gameObject);
    }
}