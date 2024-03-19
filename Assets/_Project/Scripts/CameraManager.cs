using Cinemachine;
using KBCore.Refs;
using System;
using System.Collections;
using UnityEngine;

namespace Platformer {
    public class CameraManager : ValidatedMonoBehaviour {

        [Header("References")]
        [SerializeField, Anywhere] InputReader input;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;

        [Header("Settings")]
        [SerializeField, Range(0.5f, 3f)] float speedMultiplier = 1f;

        bool isRMBPressed;
        bool isDeviceMouse;
        bool cameraMovementLook;

        private void OnEnable() {
            input.Look += OnLook;
            input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        private void OnDisable() {
            input.Look -= OnLook;
            input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }

        private void OnDisableMouseControlCamera() {
            isRMBPressed = false;


            // Unlock the cursor and make it visible
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;

            // Reset the camera asix to prevent jumping when re-enabling mouse control
            freeLookVCam.m_XAxis.m_InputAxisValue = 0f;
            freeLookVCam.m_YAxis.m_InputAxisValue = 0f;
        }

        private void OnEnableMouseControlCamera() {
            isRMBPressed = true;

            // Lock the cursor to the center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            StartCoroutine(DisableMouseForFrame());
        }

        IEnumerator DisableMouseForFrame() {
            cameraMovementLook = true;
            yield return new WaitForEndOfFrame();
            cameraMovementLook = false;
        }

        private void OnLook(Vector2 cameraMovement, bool isDeviceMouse) {
            if (cameraMovementLook) return;

            if (isDeviceMouse && !isRMBPressed) return;

            // If the device is mouse use fixedDeltaTime, otherwise use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            // Set the camera asix values
            freeLookVCam.m_XAxis.m_InputAxisValue = cameraMovement.x * speedMultiplier * deviceMultiplier;
            freeLookVCam.m_YAxis.m_InputAxisValue = cameraMovement.y * speedMultiplier * deviceMultiplier;
        }
    }
}
