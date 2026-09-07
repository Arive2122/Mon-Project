using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        public float maxSpeed = 7f;
        public float jumpTakeOffSpeed = 3f;

        public JumpState jumpState = JumpState.Grounded;

        private bool stopJump;

        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;

        public bool controlEnabled = true;

        // ==========================================
        // PLAYER NUMBER
        // ==========================================

        // Player 1:
        // A = Left
        // D = Right
        // W = Jump
        // G = Gravity
        //
        // Player 2:
        // Left Arrow = Left
        // Right Arrow = Right
        // Up Arrow = Jump
        // Slash = Gravity
        //
        // Player 3:
        // J = Left
        // L = Right
        // I = Jump
        // K = Gravity
        //
        // Player 4:
        // Numpad 4 = Left
        // Numpad 6 = Right
        // Numpad 8 = Jump
        // Numpad 5 = Gravity

        public int playerNumber = 1;

        // ==========================================
        // GUN
        // ==========================================

        public Transform gunHolder;
        public Transform firePoint;

        // ==========================================
        // MOVEMENT
        // ==========================================

        bool jump;
        Vector2 move;

        // ==========================================
        // COMPONENTS
        // ==========================================

        SpriteRenderer spriteRenderer;
        internal Animator animator;

        // ==========================================
        // MODEL
        // ==========================================

        readonly PlatformerModel model =
            Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        // ==========================================
        // GRAVITY
        // ==========================================

        public float gravityCooldown = 5f;

        private float gravityCooldownTimer = 0f;

        // True when player is upside down.
        private bool inverted = false;

        // ==========================================
        // DEATH
        // ==========================================

        public bool IsDead =>
            health != null && !health.IsAlive;

        // ==========================================
        // AWAKE
        // ==========================================

        void Awake()
        {
            health = GetComponent<Health>();

            audioSource =
                GetComponent<AudioSource>();

            collider2d =
                GetComponent<Collider2D>();

            spriteRenderer =
                GetComponent<SpriteRenderer>();

            animator =
                GetComponent<Animator>();
        }

        // ==========================================
        // UPDATE
        // ==========================================

        protected override void Update()
        {
            // ==========================================
            // DEAD PLAYER
            // ==========================================

            if (IsDead)
            {
                move = Vector2.zero;
                jump = false;

                return;
            }

            // ==========================================
            // GRAVITY COOLDOWN
            // ==========================================

            if (gravityCooldownTimer > 0f)
            {
                gravityCooldownTimer -= Time.deltaTime;

                if (gravityCooldownTimer < 0f)
                {
                    gravityCooldownTimer = 0f;
                }
            }

            // ==========================================
            // CONTROLS
            // ==========================================

            if (controlEnabled)
            {
                KeyCode leftKey;
                KeyCode rightKey;
                KeyCode jumpKey;
                KeyCode gravityKey;

                // ======================================
                // PLAYER 1
                // ======================================

                if (playerNumber == 1)
                {
                    leftKey = KeyCode.A;
                    rightKey = KeyCode.D;
                    jumpKey = KeyCode.W;
                    gravityKey = KeyCode.G;
                }

                // ======================================
                // PLAYER 2
                // ======================================

                else if (playerNumber == 2)
                {
                    leftKey = KeyCode.LeftArrow;
                    rightKey = KeyCode.RightArrow;
                    jumpKey = KeyCode.UpArrow;
                    gravityKey = KeyCode.Slash;
                }

                // ======================================
                // PLAYER 3
                // ======================================

                else if (playerNumber == 3)
                {
                    leftKey = KeyCode.J;
                    rightKey = KeyCode.L;
                    jumpKey = KeyCode.I;
                    gravityKey = KeyCode.K;
                }

                // ======================================
                // PLAYER 4
                // ======================================

                else
                {
                    leftKey = KeyCode.Keypad4;
                    rightKey = KeyCode.Keypad6;
                    jumpKey = KeyCode.Keypad8;
                    gravityKey = KeyCode.Keypad5;
                }

                // ==========================================
                // MOVEMENT
                // ==========================================

                move.x = 0f;

                if (Input.GetKey(leftKey))
                {
                    move.x -= 1f;
                }

                if (Input.GetKey(rightKey))
                {
                    move.x += 1f;
                }

                // ==========================================
                // JUMP
                // ==========================================

                if (
                    jumpState == JumpState.Grounded &&
                    Input.GetKeyDown(jumpKey)
                )
                {
                    jumpState =
                        JumpState.PrepareToJump;
                }
                else if (Input.GetKeyUp(jumpKey))
                {
                    stopJump = true;

                    Schedule<PlayerStopJump>()
                        .player = this;
                }

                // ==========================================
                // GRAVITY REVERSAL
                // ==========================================

                if (
                    Input.GetKeyDown(gravityKey) &&
                    gravityCooldownTimer <= 0f
                )
                {
                    // Reverse gravity.

                    gravityModifier *= -1f;

                    // Stop vertical velocity.

                    velocity.y = 0f;

                    // Player is in flight.

                    jumpState =
                        JumpState.InFlight;

                    // Start cooldown.

                    gravityCooldownTimer =
                        gravityCooldown;

                    // Toggle inverted state.

                    inverted = !inverted;

                    // ======================================
                    // FLIP CHARACTER VERTICALLY
                    // ======================================

                    if (spriteRenderer != null)
                    {
                        spriteRenderer.flipY =
                            inverted;
                    }

                    // ======================================
                    // UPDATE GUN
                    // ======================================

                    UpdateGunRotation();

                    Debug.Log(
                        "PLAYER " +
                        playerNumber +
                        " GRAVITY SWITCHED: " +
                        gravityModifier +
                        " | INVERTED: " +
                        inverted
                    );
                }
            }
            else
            {
                move.x = 0f;
            }

            // ==========================================
            // UPDATE JUMP
            // ==========================================

            UpdateJumpState();

            // ==========================================
            // BASE UPDATE
            // ==========================================

            base.Update();
        }

        // ==========================================
        // JUMP STATE
        // ==========================================

        void UpdateJumpState()
        {
            jump = false;

            switch (jumpState)
            {
                case JumpState.PrepareToJump:

                    jumpState =
                        JumpState.Jumping;

                    jump = true;

                    stopJump = false;

                    break;

                case JumpState.Jumping:

                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>()
                            .player = this;

                        jumpState =
                            JumpState.InFlight;
                    }

                    break;

                case JumpState.InFlight:

                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>()
                            .player = this;

                        jumpState =
                            JumpState.Landed;
                    }

                    break;

                case JumpState.Landed:

                    jumpState =
                        JumpState.Grounded;

                    break;
            }
        }

        // ==========================================
        // COMPUTE VELOCITY
        // ==========================================

        protected override void ComputeVelocity()
        {
            // ==========================================
            // JUMP
            // ==========================================

            if (jump && IsGrounded)
            {
                velocity.y =
                    jumpTakeOffSpeed *
                    model.jumpModifier *
                    gravityModifier;

                jump = false;
            }

            // ==========================================
            // STOP JUMP EARLY
            // ==========================================

            else if (stopJump)
            {
                stopJump = false;

                if (
                    gravityModifier > 0f &&
                    velocity.y > 0f
                )
                {
                    velocity.y *=
                        model.jumpDeceleration;
                }
                else if (
                    gravityModifier < 0f &&
                    velocity.y < 0f
                )
                {
                    velocity.y *=
                        model.jumpDeceleration;
                }
            }

            // ==========================================
            // PLAYER HORIZONTAL FACING
            // ==========================================

            if (spriteRenderer != null)
            {
                if (move.x > 0.01f)
                {
                    spriteRenderer.flipX = false;
                }
                else if (move.x < -0.01f)
                {
                    spriteRenderer.flipX = true;
                }
            }

            // ==========================================
            // GUN DIRECTION
            // ==========================================

            UpdateGunRotation();

            // ==========================================
            // FIREPOINT
            // ==========================================

            if (firePoint != null)
            {
                if (firePoint.parent != null)
                {
                    firePoint.rotation =
                        firePoint.parent.rotation;
                }
            }

            // ==========================================
            // ANIMATOR
            // ==========================================

            if (animator != null)
            {
                animator.SetBool(
                    "grounded",
                    IsGrounded
                );

                animator.SetFloat(
                    "velocityX",
                    Mathf.Abs(velocity.x) /
                    maxSpeed
                );
            }

            // ==========================================
            // HORIZONTAL MOVEMENT
            // ==========================================

            targetVelocity =
                move * maxSpeed;
        }

        // ==========================================
        // GUN ROTATION
        // ==========================================

        private void UpdateGunRotation()
        {
            if (gunHolder == null)
            {
                return;
            }

            // ==========================================
            // X ROTATION
            // ==========================================
            //
            // Normal gravity  = 0
            // Reverse gravity = 180

            float xRotation =
                inverted ? 180f : 0f;

            // ==========================================
            // Y ROTATION
            // ==========================================
            //
            // Facing right = 180
            // Facing left  = 0

            float yRotation = 0f;

            if (move.x > 0.01f)
            {
                yRotation = 180f;
            }
            else if (move.x < -0.01f)
            {
                yRotation = 0f;
            }
            else
            {
                if (spriteRenderer != null)
                {
                    yRotation =
                        spriteRenderer.flipX
                        ? 0f
                        : 180f;
                }
            }

            // ==========================================
            // SET EXACT ROTATION
            // ==========================================

            gunHolder.localEulerAngles =
                new Vector3(
                    xRotation,
                    yRotation,
                    0f
                );
        }

        // ==========================================
        // DEATH BORDER
        // ==========================================

        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            Debug.Log(
                "PLAYER " +
                playerNumber +
                " TRIGGER HIT: " +
                other.gameObject.name
            );

            if (other.CompareTag("DeathBorder"))
            {
                Debug.Log(
                    "PLAYER " +
                    playerNumber +
                    " DEATH BORDER DETECTED!"
                );

                if (
                    health != null &&
                    health.IsAlive
                )
                {
                    Debug.Log(
                        "SCHEDULING PLAYER " +
                        playerNumber +
                        " DEATH"
                    );

                    Schedule<PlayerDeath>();
                }
            }
        }

        // ==========================================
        // TRIGGER EXIT
        // ==========================================

        private void OnTriggerExit2D(
            Collider2D other
        )
        {
            Debug.Log(
                "PLAYER " +
                playerNumber +
                " TRIGGER EXIT: " +
                other.gameObject.name
            );
        }

        // ==========================================
        // JUMP STATES
        // ==========================================

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}