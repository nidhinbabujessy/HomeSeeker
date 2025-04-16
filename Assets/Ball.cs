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

    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Input.gyro.enabled = true;

        initialPosition = transform.position; // Store initial position

        startstage();
    }

    void FixedUpdate()
    {
        Vector3 gravity = Input.gyro.gravity;

        // In 2D, we can use gravity.x and gravity.y
        Vector2 movement = new Vector2(gravity.x, gravity.y) * moveSpeed;

        rb.linearVelocity = movement;
    }

    #region win

    public void startstage()
    {
        transform.position = initialPosition;     // Reset position
        rb.linearVelocity = Vector2.zero;         // Reset linear velocity
        rb.angularVelocity = 0f;                  // Optional: reset angular velocity
        rb.bodyType = RigidbodyType2D.Kinematic;  // Make it kinematic
        cage.SetActive(true);                     // Show cage
    }

    private void OnMouseDown()
    {
        if (CompareTag("Ball"))
        {
            print("ball");
            rb.bodyType = RigidbodyType2D.Dynamic;
            cage.SetActive(false);
        }
    }

    #endregion
}
