using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour {
    [Header("Player Components")]
    private Rigidbody2D rb;
    private BoxCollider2D playerCollider2D;
    private InputActions inputActions;

    [Header("Movement Variables")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 8f;
    [SerializeField] private float velocityExponent = 0.05f;
    [SerializeField] private float groundedLinearDrag = 0.2f;
    private float movementDirection;

    [Header("Environment Interaction")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float groundColliderRaycastLength = .2f;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpHeight = 12f;
    private bool isGrounded => IsGrounded();
    private float lastGroundedTime = 0f;
    private float lastJumpTime = 0f;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        playerCollider2D = GetComponentInChildren<BoxCollider2D>();
        inputActions = new InputActions();
    }

    private void FixedUpdate() {
        MoveCharacter();
        ApplyGroundedLinearDrag();
    }

    // Capture left/right movement value using Unity Input System
    private void On_1DMove(InputValue input) {
        movementDirection = input.Get<float>();
    }

    // Handle Jump input using Unity Input System
    private void OnJump() {
        if (isGrounded) {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastGroundedTime = 0f;
            lastJumpTime = 0f;
        }
    }

    private void MoveCharacter() {
        // Calculate desired velocity in direction of motion
        float targetSpeed = movementDirection * speed;
        // Calculate difference between target and current velocity
        float speedDiff = targetSpeed - rb.velocity.x;
        // Alter acceleration rate based on direction
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        // Apply acceleration by raising to power for to increase with higher speeds
        // Multiply by sign to ensure correct direction
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velocityExponent) * Mathf.Sign(speedDiff);

        // Apply force to rigidbody in the X direction
        rb.AddForce(movement * Vector2.right);
    }

    private void ApplyGroundedLinearDrag() {
        // Ensure we're grounded and not input movement controls
        if (isGrounded && movementDirection == 0) {
            // Apply minimum of acceleration or linear drag to movement direction
            float linearDragStrength = MathF.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(groundedLinearDrag));
            linearDragStrength *= Mathf.Sign(rb.velocity.x);

            // Apply linear drag in opposite direction to velocity to smooth decelerate, change direction, or stop
            rb.AddForce(Vector2.right * -linearDragStrength, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded() {
        // Apply small adjustment inwards to prevent isGrounded from being triggered when player is touching a wall/ceiling
        Vector3 wallCollisionModifier = new Vector3(0.1f, 0, 0);
        // Use raycast to check for collisions with ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider2D.bounds.center, playerCollider2D.bounds.size - wallCollisionModifier, 0f, Vector3.down, groundColliderRaycastLength, groundLayerMask);
        // Draw collider box
        //Debug.DrawRay(playerCollider2D.bounds.center, Vector3.down * (playerCollider2D.bounds.extents.y + groundCollisionDistance), Color.green);
        return raycastHit.collider != null;

    }

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }
}
