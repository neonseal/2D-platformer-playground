using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private InputActions inputActions;

    [Header("Movement Variables")]
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float linearDrag = 7f;
    private float movementDirection;
    private float speed = 0f;
    private bool changingDirection => (playerBody.velocity.x > 0f && movementDirection < 0f) || (playerBody.velocity.x < 0f && movementDirection > 0f);

    private void Awake() {
        playerBody = GetComponent<Rigidbody2D>();
        inputActions = new InputActions();
    }

    private void Update() {
        movementDirection = inputActions.Player._1DMove.ReadValue<float>();
    }

    private void FixedUpdate() {
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

    private void OnEnable() {
        inputActions.Enable();
    }

    private void OnDisable() {
        inputActions.Disable();
    }
}
