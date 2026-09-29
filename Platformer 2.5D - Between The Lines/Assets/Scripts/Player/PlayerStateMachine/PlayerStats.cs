using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movements")]
    public float movementSpeed = 10f;

    [Range(1f, 100f)] public float maxRunSpeed = 10f;
    [Range(0.25f, 50f)] public float groundAcceleration = 5f;
    [Range(0.25f, 50f)] public float groundDeceleration = 20f;
    [Range(0.25f, 50f)] public float airAcceleration = 5f;
    [Range(0.25f, 50f)] public float airDeceleration = 5f;
    [Range(0.25f, 50f)] public float maxAirControl = 5f;

    [Header("Jump")]
    public float jumpHeight = 6.5f;
    [Range(1f, 1.1f)] public float jumpHeightCompensationFactor = 1.054f;
    public float timeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)] public float gravityOnReleaseMultiplier  = 2f;
    public float maxFallSpeed = 26f;
    [Range(1f,2f)] public float fastFallGravityMultiplier  = 1.5f;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float timeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float apexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;

    [Header("Wall Jump")]
    public float timeTillWallJumpApex = 0.35f;
    public float wallJumpHeight;
    public float wallImpulsion = 5f;

    [Header("InAir")]
    public float inAirMovementSpeed = 3f;

    [Header("OnWall")]
    [Range(0,10)] public float OnWallVelocityY;
    [Range(0,10)] public float WallJumpForceGravityMultiplier;
    [Range(0,1)] public float DeadZoneGravityMultiplier;
    public int MaxWallJump = 2;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }
    public float InitialWallJumpVelocity { get; private set; }
    
    public float WallJumpGravity { get; private set; }

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        AdjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        float lWallJumpHeight = wallJumpHeight * jumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / (timeTillJumpApex * timeTillJumpApex);
        WallJumpGravity = -(2f * lWallJumpHeight) / (timeTillWallJumpApex * timeTillWallJumpApex);
        InitialJumpVelocity = Mathf.Abs(Gravity) * timeTillJumpApex;
        InitialWallJumpVelocity = Mathf.Abs(WallJumpGravity) * timeTillWallJumpApex;
    }
}
