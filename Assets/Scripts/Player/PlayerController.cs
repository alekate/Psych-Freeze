using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Clase10
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Transform cameraPivot; 

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> clips = new();

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float maxSlopeAngle = 75f;

        [Header("Jump Settings")]
        private bool isGrounded;
        private int jumps = 0;
        [SerializeField] private int maxJumps = 2;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float groundCheckDistance = 1.1f;
        private LayerMask groundLayer;

        [Header("Rotation Settings")]
        public float sensX = 7f;
        public float sensY = 7f;
        private Vector2 turn;
        private float mouseXDelta;

        [Header("Audio Settings")]
        [SerializeField] private float stepInterval = 0.7f;

        private Rigidbody rb;
        private float audioTimer = 0f;
        private Terrain terrain;

        private int currentStepClipIndex = 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            terrain = Terrain.activeTerrain;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            groundLayer = LayerMask.GetMask("Ground");

            cameraPivot = transform.Find("Camera Pivot");
        }


        private void FixedUpdate()
        {
            CheckGrounded();
            CameraMovement();
            HandleMovement();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && jumps < maxJumps)
            {
                Jump();
            }
        }

        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

            if (isGrounded && !wasGrounded)
            {
                jumps = 0;
            }
        }

        private void Jump()
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumps++;
        }

        public void SetRigidbody(Rigidbody rigid)
        {
            rb = rigid;
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Usar cameraPivot en lugar de cameraTransform
            Vector3 inputDirection = (cameraPivot.forward * vertical + cameraPivot.right * horizontal);
            inputDirection.y = 0;
            inputDirection.Normalize();

            if (inputDirection.sqrMagnitude > 0.01f && CanMove(inputDirection))
            {
                Vector3 move = inputDirection * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);

                PlayFootstepSound();
            }
        }

        private bool CanMove(Vector3 direction)
        {
            if (terrain == null)
            {
                terrain = Terrain.activeTerrain;

                if (terrain == null)
                {
                    return true; // o false si preferís que no se mueva sin terreno
                }
            }

            Vector3 currentPos = rb.position;
            Vector3 futurePos = currentPos + direction * 5f;

            float currentHeight = terrain.SampleHeight(currentPos);
            float nextHeight = terrain.SampleHeight(futurePos);

            Vector3 normalizedPos = GetNormalizedTerrainPos(currentPos);
            Vector3 normal = terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.z);
            float slopeAngle = Vector3.Angle(normal, Vector3.up);

            return slopeAngle <= maxSlopeAngle || nextHeight <= currentHeight;
        }

        private void PlayFootstepSound()
        {
            audioTimer += Time.deltaTime;
            if (audioTimer < stepInterval)
                return;

            audioTimer = 0f;

            if (clips.Count == 0)
                return;

            audioSource.clip = clips[currentStepClipIndex];
            audioSource.Play();

            currentStepClipIndex = (currentStepClipIndex + 1) % clips.Count;
        }

        private Vector3 GetNormalizedTerrainPos(Vector3 worldPos)
        {
            Vector3 terrainPos = terrain.transform.position;
            Vector3 size = terrain.terrainData.size;

            return new Vector3(
                (worldPos.x - terrainPos.x) / size.x,
                0,
                (worldPos.z - terrainPos.z) / size.z
            );
        }

        void CameraMovement()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensX;
            float mouseY = Input.GetAxis("Mouse Y") * sensY;

            turn.x += mouseX;
            turn.y -= mouseY; // invertir para comportamiento normal

            turn.y = Mathf.Clamp(turn.y, -35f, 35f);

            // Rotar el jugador en el eje Y (horizontal)
            transform.localRotation = Quaternion.Euler(0f, turn.x, 0f);

            // Rotar el cameraPivot en el eje X (vertical)
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(turn.y, 0f, 0f);
            }
        }

    }
}
