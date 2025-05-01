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

    [SerializeField] private GameObject Hand;


    private SpriteRenderer cageRenderer;

    public bool gyroMove;

   

    private Vector3 initialPosition;

    void Start()
    {
        Hand.SetActive(true);
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
       
            if (gyroMove == true)
            {
                Vector3 gravity = Input.gyro.gravity;
                Vector2 movement = new Vector2(gravity.x, gravity.y) * moveSpeed;
                rb.linearVelocity = movement;
            }
        
    }


  

    private void OnMouseDown()
    {
        if (CompareTag("Ball"))
        {
            SoundManager.Instance.PlayDestroy();

            Hand.SetActive(false);

            print("ball");
           
            // LeanTween fade and destroy
            LeanTween.alpha(cage, 0f, 0.5f).setOnComplete(() =>
            {
                cage.SetActive(false); // or Destroy(cage);
              //  gyroMove = true;
                Input.gyro.enabled = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                GameMan.instance.TimeStart();
            });
          //  Input.gyro.enabled = true;
           
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

   

    #region win
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Win"))
        {
            SoundManager.Instance.PlayHomeCollider();
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
        if (collision.gameObject.CompareTag("Gem"))
        {
            SoundManager.Instance.PlayGem();
            GameMan.instance.gemTempAdd();
            collision.gameObject.SetActive(false);
           
            
        }
        if (collision.gameObject.CompareTag("Coin"))
        {
            SoundManager.Instance.PlayCoin();
            GameMan.instance.coinTempAdd();
            collision.gameObject.SetActive(false);


        }
    }
    #endregion
}
