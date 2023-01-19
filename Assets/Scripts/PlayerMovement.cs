using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("Components")]
    private Rigidbody2D playerBody;
    private InputActions inputActions;

    [Header("Movement Variables")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float linearDrag = 7f;
    private float movementDirection;
    private bool changingDirection => (playerBody.velocity.x > 0f && movementDirection < 0f) || (playerBody.velocity.x < 0f && movementDirection > 0f);

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpDuration = 5f;
    [SerializeField] private float gravitySuppresionTimer = 1f;
    [SerializeField] private float gravityModifier = 1f;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Environment Interaction")]
    [SerializeField] private float collisionCapsuleRadius = 1f;
    private BoxCollider2D playerCollider;
    private bool grounded;

    private void Awake() {
        playerBody = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
        playerCollider = GetComponentInChildren<BoxCollider2D>();
    }

    private void Update() {
        movementDirection = inputActions.Player._1DMove.ReadValue<float>();
    }

    private void FixedUpdate() {
        IsGrounded();
        MoveCharacter();
        ApplyLinearDrag();
    }

    // Increase speed towards maximum or maintain maximum velocity
    private void MoveCharacter() {
        if (Mathf.Abs(playerBody.velocity.x) < maxSpeed) {
            Vector2 movementVector = new Vector2(movementDirection, 0f);
            playerBody.AddForce(movementVector * acceleration * Time.fixedDeltaTime, ForceMode2D.Impulse);
        } else {
            playerBody.velocity = new Vector2(Mathf.Sign(playerBody.velocity.x) * maxSpeed, playerBody.velocity.y);
        }
    }

    // Apply linear drag when decelerating to stop or changing directions to switch smoothly
    private void ApplyLinearDrag() {
        if (Mathf.Abs(movementDirection) < 0.4f || changingDirection) {
            playerBody.drag = linearDrag;
        } else {
            playerBody.drag = 0f;
        }
    }

    private void OnJump() {
        Debug.Log("JUMP");
        //if (grounded) {
        playerBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        //}
    }

    private void IsGrounded() {
        Vector3 groundColliderVector = new Vector3(playerCollider.bounds.center.x, playerCollider.bounds.min.y - 0.1f, playerCollider.bounds.center.z);
        grounded = Physics.CheckCapsule(playerCollider.bounds.center, groundColliderVector, collisionCapsuleRadius);
        if (grounded) {
            Debug.Log("GROUNDED: " + grounded);
        }        
    }

    private void OnDrawGizmos() {
        Vector3 groundColliderVector = new Vector3(.bounds.center.x, playerCollider.bounds.min.y - 0.1f, playerCollider.bounds.center.z);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + groundColliderVector * collisionCapsuleRadius);
    }

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }
}
