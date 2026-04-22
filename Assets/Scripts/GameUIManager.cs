using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [Header("Core References")]
    public CricketBallController ballController;

    [Header("Meter UI")]
    public RectTransform meterIndicator;
    public float meterSpeed = 3f;
    public float meterHeightRange = 200f; // Adjust based on your UI height

    [Header("UI Buttons")]
    public Button swingButton;
    public Button spinButton;
    public Button bowlButton;
    public Button changeSideButton;

    [Header("UI Feedback (Optional)")]
    public TextMeshProUGUI modeText;

    private float pingPongValue = 0f;
    private bool isAiming = true;

    // 1 for Right/Off-spin, -1 for Left/Leg-spin
    private float deliveryDirection = 1f;

    void Start()
    {
        // Wire up all buttons automatically via code
        SetupButtonListeners();

        // Set default state
        SetSwingMode();
    }

    void Update()
    {
        if (isAiming)
        {
            // Smoothly bounce between -1 and 1
            pingPongValue = Mathf.Sin(Time.time * meterSpeed);

            // Move the black line up and down
            meterIndicator.anchoredPosition = new Vector2(0, pingPongValue * meterHeightRange);
        }
    }

    // --- SETUP LISTENERS ---
    private void SetupButtonListeners()
    {
        // This replaces the need to use the Unity Inspector "On Click" menu completely
        if (swingButton != null) swingButton.onClick.AddListener(SetSwingMode);
        if (spinButton != null) spinButton.onClick.AddListener(SetSpinMode);
        if (bowlButton != null) bowlButton.onClick.AddListener(BowlBall);
        if (changeSideButton != null) changeSideButton.onClick.AddListener(ChangeSide);
    }

    // --- BUTTON FUNCTIONS ---

    public void SetSwingMode()
    {
        if (!isAiming) return;
        ballController.currentDeliveryType = CricketBallController.DeliveryType.Swing;
        if (modeText != null) modeText.text = "Mode: SWING";
    }

    public void SetSpinMode()
    {
        if (!isAiming) return;
        ballController.currentDeliveryType = CricketBallController.DeliveryType.Spin;
        if (modeText != null) modeText.text = "Mode: SPIN";
    }

    public void BowlBall()
    {
        if (!isAiming) return;

        isAiming = false; // Stop the meter indicator

        // Lock the UI so the player can't spam click while the ball is flying
        swingButton.interactable = false;
        spinButton.interactable = false;
        bowlButton.interactable = false;

        // Calculate power (Center = 100%, Edges = 0%)
        float distanceFromCenter = Mathf.Abs(pingPongValue);
        float powerStrength = 1f - distanceFromCenter;

        // Apply direction and power
        float finalEffectValue = powerStrength * deliveryDirection;

        ballController.swingStrength = finalEffectValue;
        ballController.spinStrength = finalEffectValue;

        // Trigger physics
        ballController.BowlDelivery();
    }

    public void ChangeSide()
    {
        // Flip the direction math between 1 (Right) and -1 (Left)
        deliveryDirection *= -1f;

        string sideInfo = (deliveryDirection == 1f) ? "Right Side" : "Left Side";
        Debug.Log("Changed Bowling Side to: " + sideInfo);

        ResetGame();
    }

    public void ResetGame()
    {
        isAiming = true;

        // Re-enable the UI buttons
        swingButton.interactable = true;
        spinButton.interactable = true;
        bowlButton.interactable = true;

        ballController.ResetBall();
    }
}