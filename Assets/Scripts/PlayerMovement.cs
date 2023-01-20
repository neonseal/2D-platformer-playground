using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    /*[SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float jumpDuration = 5f;
    [SerializeField] private float gravitySuppresionTimer = 1f;
    [SerializeField] private float gravityModifier = 1f;*/

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Environment Interaction")]
    [SerializeField] private float groundCollisionDistance = .2f;
    private BoxCollider2D playerCollider2D;

    private void Awake() {
        playerBody = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
        playerCollider2D = GetComponentInChildren<BoxCollider2D>();
    }

    private void Update() {
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

    private void On_1DMove(InputValue input) {
        movementDirection = input.Get<float>();
    }

    private void OnJump() {
        if (IsGrounded()) {
            playerBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded() {
        Vector3 wallCollisionModifier = new Vector3(0.1f, 0, 0);
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider2D.bounds.center, playerCollider2D.bounds.size - wallCollisionModifier, 0f, Vector3.down, groundCollisionDistance, groundLayerMask);
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
