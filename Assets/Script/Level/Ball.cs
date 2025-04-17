using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public static Ball Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private Rigidbody2D rb;
    public float moveSpeed = 5f;

    [SerializeField] private GameObject cage;
    private SpriteRenderer cageRenderer;

    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       

        initialPosition = transform.position;
        cageRenderer = cage.GetComponent<SpriteRenderer>();

        startstage();
    }

    void FixedUpdate()
    {
        Vector3 gravity = Input.gyro.gravity;
        Vector2 movement = new Vector2(gravity.x, gravity.y) * moveSpeed;
        rb.linearVelocity = movement;
    }

   
    public void startstage()
    {
        Input.gyro.enabled = false;

        transform.position = initialPosition;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        cage.SetActive(true);

        // Reset alpha to full if reused
        SetCageAlpha(1f);
    }
   

    private void OnMouseDown()
    {
        if (CompareTag("Ball"))
        {
            print("ball");
            rb.bodyType = RigidbodyType2D.Dynamic;
            GameMan.instance.TimeStart();
            // LeanTween fade and destroy
            LeanTween.alpha(cage, 0f, 0.5f).setOnComplete(() =>
            {
                cage.SetActive(false); // or Destroy(cage);
            });
            Input.gyro.enabled = true;
        }
    }

    private void SetCageAlpha(float alpha)
    {
        if (cageRenderer != null)
        {
            Color color = cageRenderer.color;
            color.a = alpha;
            cageRenderer.color = color;
        }
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }


    #region win
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Win"))
        {
            GameMan.instance.winMethod();
            GameMan.instance.StopTimer();
            Input.gyro.enabled = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if(rb.bodyType == RigidbodyType2D.Kinematic)
            {
                print("rb is kinematic");
            }

            


        }
    }
    #endregion
}
