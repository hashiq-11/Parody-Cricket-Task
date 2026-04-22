using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CricketBallController : MonoBehaviour
{
    public enum BallState { Aiming, InAir, PostBounce }
    public enum DeliveryType { Swing, Spin }

    [Header("Current State")]
    public BallState currentState = BallState.Aiming;
    public DeliveryType currentDeliveryType = DeliveryType.Swing;

    [Header("Targeting")]
    public Transform bounceMarker;

    [Header("Physics Settings")]
    public float forwardSpeed = 8f;

    [Header("Swing Settings (Phase I)")]
    [Range(-1f, 1f)] public float swingStrength = 0.5f;
    public float swingMultiplier = 15f;

    [Header("Spin Settings (Phase II)")]
    [Range(-1f, 1f)] public float spinStrength = 0.5f;
    public float spinAngleMultiplier = 25f; // How sharp the turn is at bounce

    private Rigidbody rb;
    private float currentLateralVelocity = 0f;
    private Vector3 startingPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        startingPosition = transform.position; // Save this to easily reset the ball
    }

    void Update()
    {
        // Throw the ball
        if (currentState == BallState.Aiming && Input.GetKeyDown(KeyCode.Space))
        {
            BowlDelivery();
        }

        // Quick reset button for testing (Press R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBall();
        }
    }

    public void BowlDelivery()
    {
        currentState = BallState.InAir;
        rb.useGravity = true;

        Vector3 direction = (bounceMarker.position - transform.position).normalized;
        direction.y += 0.15f;

        rb.linearVelocity = direction * forwardSpeed;
    }

    void FixedUpdate()
    {
        // Phase I Logic: Only curve if it's a Swing delivery [cite: 36, 108]
        if (currentState == BallState.InAir && currentDeliveryType == DeliveryType.Swing)
        {
            currentLateralVelocity += swingStrength * swingMultiplier * Time.fixedDeltaTime;

            Vector3 vel = rb.linearVelocity;
            vel.x += currentLateralVelocity * Time.fixedDeltaTime;
            rb.linearVelocity = vel;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentState == BallState.InAir && collision.gameObject.CompareTag("Pitch"))
        {
            currentState = BallState.PostBounce;

            if (currentDeliveryType == DeliveryType.Swing)
            {
                // Phase I: Swing stops instantly [cite: 61]
                currentLateralVelocity = 0f;
            }
            else if (currentDeliveryType == DeliveryType.Spin)
            {
                // Phase II: Spin changes direction exactly at bounce [cite: 124, 125]
                Vector3 velocityAtBounce = rb.linearVelocity;

                // Calculate the sharp angle turn
                float turnAngle = spinStrength * spinAngleMultiplier;
                Quaternion spinRotation = Quaternion.Euler(0f, turnAngle, 0f);

                // Apply the new rotated straight-line velocity [cite: 144]
                rb.linearVelocity = spinRotation * velocityAtBounce;
            }
        }
    }

    // Helper method to let you test rapidly
    public void ResetBall()
    {
        currentState = BallState.Aiming;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingPosition;
        currentLateralVelocity = 0f;
    }
}