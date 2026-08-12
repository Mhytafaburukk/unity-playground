using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float maxForce = 20f;
    public float maxLeanAngle = 35f;
    public float chargeSpeed = 1.5f;
    public int SkillDelay = 10;

    [Header("Referanslar")]
    public Transform visualTransform;
    public PlayerSpawn spawnManager;
    public SkillSpawner skillSpawner;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawn;

    private Rigidbody2D rb;
    private float currentCharge = 0f;
    private bool facingRight = true;
    private bool isGrounded = false;
    private bool isSwitching = false;
    private bool isSwitching2 = false;

    public bool isTrigerred1 = false;
    public bool isTrigerred2 = false;
    public bool isTrigerred3 = false;

    public int SpawnLevel = 0;

    public bool isBought = false;

    [HideInInspector] public bool canFire = false;
    [HideInInspector] public bool canFire2 = false;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode fireKey;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spawnManager == null) spawnManager = Object.FindFirstObjectByType<PlayerSpawn>();
        if (skillSpawner == null) skillSpawner = Object.FindFirstObjectByType<SkillSpawner>();
        
        if (visualTransform == null && transform.childCount > 0)
            visualTransform = transform.GetChild(0);

        SetupControls();
    }

    private void SetupControls()
    {
        if (gameObject.CompareTag("Player1"))
        {
            leftKey = KeyCode.Z;
            rightKey = KeyCode.X;
            fireKey = KeyCode.C;
        }
        else if (gameObject.CompareTag("Player2"))
        {
            leftKey = KeyCode.J;
            rightKey = KeyCode.K;
            fireKey = KeyCode.L;
        }
        else
        {
            leftKey = KeyCode.A;
            rightKey = KeyCode.S;
            fireKey = KeyCode.D;
        }
    }

    void Update()
    {
        if (gameObject.CompareTag("Untagged")) return;

        HandleInput();
        ApplyVisualLean();
    }

    private void HandleInput()
    {
        if (canFire && Input.GetKeyDown(fireKey))
        {
            isBought = false;
            Debug.Log("Fuze Ateşlendi!");
            MermiFirlatFNC();
            bool isP1 = gameObject.CompareTag("Player1");
            spawnManager.SwitchPlayer(isTrigerred1,isTrigerred2,isTrigerred3,SpawnLevel,false, false, transform.position, isP1, gameObject);
            return;
        }
        else if(canFire2 && Input.GetKeyDown(fireKey))
        {
            isBought = false;
            Debug.Log("Zıpkın Ateşlendi!");
            MermiFirlatFNC();
            canFire2 = false;
        }

        if (isGrounded && (Input.GetKey(leftKey) || Input.GetKey(rightKey)))
        {
            currentCharge = Mathf.MoveTowards(currentCharge, 1f, chargeSpeed * Time.deltaTime);
            if (Input.GetKey(rightKey) && !facingRight) Flip();
            else if (Input.GetKey(leftKey) && facingRight) Flip();
        }
        else
        {
            currentCharge = Mathf.MoveTowards(currentCharge, 0f, chargeSpeed * 3 * Time.deltaTime);
        }

        if (isGrounded)
        {
            if (Input.GetKeyUp(rightKey)) Jump(new Vector2(1, 1.5f));
            else if (Input.GetKeyUp(leftKey)) Jump(new Vector2(-1, 1.5f));
        }
    }

    void Jump(Vector2 direction)
    {
        isGrounded = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * maxForce * currentCharge, ForceMode2D.Impulse);
        currentCharge = 0f;
    }

    void ApplyVisualLean()
    {
        if (visualTransform == null) return;

        float targetAngle = (isGrounded && currentCharge > 0) ? -maxLeanAngle * currentCharge : 0f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        visualTransform.localRotation = Quaternion.Lerp(visualTransform.localRotation, targetRotation, Time.deltaTime * 20f);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void MermiFirlatFNC()
    {
        
        GameObject mermi = Instantiate(bulletPrefab, bulletSpawn.position, transform.rotation);
        BulletController bc = mermi.GetComponent<BulletController>();

        if (bc != null)
        {
            bc.YonuAyarla(facingRight);
            bc.SelectedCharacter(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground")|| collision.gameObject.CompareTag("Helicopter")) isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground")|| collision.gameObject.CompareTag("Helicopter")) isGrounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        bool isP1 = gameObject.CompareTag("Player1");
        SkillItem item = collider.GetComponent<SkillItem>();

        if (collider.CompareTag("Fuze1") && !isSwitching && !isBought) 
        {
            isBought = true;
            isSwitching = true;
            Debug.Log("Füze Alındı!");

            if (item != null && item.mySpawner != null) 
            {
                item.mySpawner.StartSpawnTimer(10f);
            }
            Destroy(collider.gameObject);
            
            canFire = true;
            spawnManager.SwitchPlayer(isTrigerred1,isTrigerred2,isTrigerred3,SpawnLevel,false, true, transform.position, isP1, gameObject);
        }
        else if(collider.CompareTag("Zipkin1") && !isSwitching2 && !isBought)
        {
            isBought = true;
            isSwitching2 = true;
            Debug.Log("Zıpkın Alındı!");
            if (item != null && item.mySpawner != null) 
            {
                item.mySpawner.StartSpawnTimer(10f);
            }
            Destroy(collider.gameObject);
            canFire2 = true;
            spawnManager.SwitchPlayer(isTrigerred1,isTrigerred2,isTrigerred3,SpawnLevel,true, false, transform.position, isP1, gameObject);
        }
        else if (collider.CompareTag("Spawn1") && !isTrigerred1)
        {
            Debug.Log("Degdi");
            isTrigerred1 = true;
            SpawnLevel++;
        }
        else if (collider.CompareTag("Spawn2") && !isTrigerred2)
        {
            isTrigerred2 = true;
            SpawnLevel++;
        }
        else if (collider.CompareTag("Spawn3") && !isTrigerred3)
        {
            isTrigerred3 = true;
            SpawnLevel++;
        }
    }
}
