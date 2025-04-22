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

    bool gyroMove;

    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       
        gyroMove = false;
      //  initialPosition = transform.position;
        cageRenderer = cage.GetComponent<SpriteRenderer>();
        Input.gyro.enabled = false;
      //  rb.bodyType = RigidbodyType2D.Kinematic;
        //  startstage();
    }

    public void InitialStage()
    {
        gyroMove = false;
        Input.gyro.enabled = false;
    }
    void FixedUpdate()
    {
        if (gyroMove==true)
        {
            Vector3 gravity = Input.gyro.gravity;
            Vector2 movement = new Vector2(gravity.x, gravity.y) * moveSpeed;
            rb.linearVelocity = movement;
        }
    }


    //public void startstage()
    //{
    //    Input.gyro.enabled = false;

    //    transform.position = initialPosition;
    //    rb.linearVelocity = Vector2.zero;
    //    rb.angularVelocity = 0f;
    //    rb.bodyType = RigidbodyType2D.Kinematic;
    //    cage.SetActive(true);

    //    // Reset alpha to full if reused
    //    SetCageAlpha(1f);
    //}
    //public void startstage2(Vector3 spawnPosition)
    //{
    //    Input.gyro.enabled = false;

    //    transform.position = spawnPosition;
    //    rb.linearVelocity = Vector2.zero;
    //    rb.angularVelocity = 0f;
    //    rb.bodyType = RigidbodyType2D.Kinematic;

    //    cage.SetActive(true);
    //    SetCageAlpha(1f);
    //}


    private void OnMouseDown()
    {
        if (CompareTag("Ball"))
        {
            print("ball");
           
            // LeanTween fade and destroy
            LeanTween.alpha(cage, 0f, 0.5f).setOnComplete(() =>
            {
                cage.SetActive(false); // or Destroy(cage);
            });
          //  Input.gyro.enabled = true;
            gyroMove=true;
            Input.gyro.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            GameMan.instance.TimeStart();
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

    //public void ResetVelocity()
    //{
    //    rb.linearVelocity = Vector2.zero;
    //    rb.angularVelocity = 0f;
    //    rb.bodyType = RigidbodyType2D.Kinematic;
    //}


    #region win
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Win"))
        {
            GameMan.instance.winMethod();
            GameMan.instance.StopTimer();
            Input.gyro.enabled = false;
          //  rb.linearVelocity = Vector2.zero;
          //  rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            if(rb.bodyType == RigidbodyType2D.Kinematic)
            {
                print("rb is kinematic");
            }

            gyroMove=false;
            cage.SetActive(true);
            Input.gyro.enabled = false;
        }
    }
    #endregion
}
