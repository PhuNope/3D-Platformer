using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Utillities;

namespace Platformer {
    public partial class PlayerController : ValidatedMonoBehaviour {

        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 6f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0;
        [SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 3f;

        const float ZeroF = 0f;

        Transform mainCam;

        float currentSpeed;
        float velocity;
        float jumpVelocity;

        Vector3 movement;

        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;

        // Animation parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        private void Awake() {
            mainCam = Camera.main.transform;
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            // Invoke event when observed tranform is teleported, adjusting freeLookVCam's position accordingly
            freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);

            rb.freezeRotation = true;

            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };

            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        private void Start() => input.EnablePlayerActions();

        private void OnEnable() {
            input.Jump += OnJump;
        }

        private void OnDisable() {
            input.Jump -= OnJump;
        }

        private void OnJump(bool performed) {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrouned) {
                jumpTimer.Start();
            } else if (!performed && jumpTimer.IsRunning) {
                jumpTimer.Stop();
            }
        }

        private void Update() {
            movement = new Vector3(input.Direction.x, 0, input.Direction.y);

            HandleTimers();
            UpdateAnimator();
        }

        private void FixedUpdate() {
            HandleJump();
            HandleMovement();
        }

        private void UpdateAnimator() {
            animator.SetFloat(Speed, currentSpeed);
        }

        private void HandleTimers() {
            foreach (var timer in timers) {
                timer.Tick(Time.deltaTime);
            }
        }

        private void HandleJump() {
            // If not jumping and grounded, keep jump velocity  at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrouned) {
                jumpVelocity = ZeroF;
                jumpTimer.Stop();
                return;
            }

            // If jumping or falling, calculate velocity
            if (jumpTimer.IsRunning) {
                // Progress point for initial burst of velocity
                float launchPoint = 0.9f;
                if (jumpTimer.Progress > launchPoint) {
                    // Calculate the velocity required to reach the jump height using physics equations v = sqrt(2gh), h = 1/2 * g * pow(t, 2), v = gt
                    jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                } else {
                    // Gradually apply less velocity as the jump progresses
                    jumpVelocity += (1 - jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
                }
            } else {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            // Apply velocity
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }

        private void HandleMovement() {
            // Rotate movement direction to match camera rotation
            Vector3 adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;

            if (adjustedDirection.magnitude > ZeroF) {
                HandleRotation(adjustedDirection);

                HandleHorizontalMovement(adjustedDirection);

                SmoothSpeed(adjustedDirection.magnitude);
            } else {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for a snappy stop
                rb.velocity = new Vector3(ZeroF, rb.velocity.y, ZeroF);
            }
        }

        private void HandleHorizontalMovement(Vector3 adjustedDirection) {
            // Move the player 
            Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        private void HandleRotation(Vector3 adjustedDirection) {
            // Adjust rotation to match movement direction
            Quaternion targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }

        private void SmoothSpeed(float value) {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}
