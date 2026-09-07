using UnityEngine;

namespace Platformer.Mechanics
{
    public class KinematicObject : MonoBehaviour
    {
        public float minGroundNormalY = 0.65f;

        public float gravityModifier = 1f;

        public float fallGravityMultiplier = 2f;

        public Vector2 velocity;

        public bool IsGrounded { get; private set; }

        protected Vector2 targetVelocity;
        protected Vector2 groundNormal;
        protected Rigidbody2D body;
        protected ContactFilter2D contactFilter;

        protected RaycastHit2D[] hitBuffer =
            new RaycastHit2D[16];

        protected const float minMoveDistance =
            0.001f;

        protected const float shellRadius =
            0.01f;

        public void Bounce(float value)
        {
            velocity.y = value;
        }

        public void Bounce(Vector2 dir)
        {
            velocity = dir;
        }

        public void Teleport(Vector3 position)
        {
            if (body == null)
                return;

            body.position = position;

            velocity = Vector2.zero;

            body.linearVelocity =
                Vector2.zero;
        }

        protected virtual void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();

            if (body != null)
            {
                body.bodyType =
                    RigidbodyType2D.Kinematic;
            }
        }

        protected virtual void OnDisable()
        {
            if (body != null)
            {
                body.bodyType =
                    RigidbodyType2D.Dynamic;
            }
        }

        protected virtual void Start()
        {
            contactFilter.useTriggers = false;

            contactFilter.SetLayerMask(
                Physics2D.GetLayerCollisionMask(
                    gameObject.layer
                )
            );

            contactFilter.useLayerMask = true;
        }

        protected virtual void Update()
        {
            targetVelocity = Vector2.zero;

            ComputeVelocity();
        }

        protected virtual void ComputeVelocity()
        {
        }

        protected virtual void FixedUpdate()
        {
            Health health =
                GetComponent<Health>();

            // Dead player does not move
            if (
                health != null &&
                !health.IsAlive
            )
            {
                velocity = Vector2.zero;
                targetVelocity = Vector2.zero;

                return;
            }

            if (body == null)
                return;

            // Apply gravity
            Vector2 gravity =
                Physics2D.gravity *
                gravityModifier;

            bool falling =
                Vector2.Dot(
                    velocity,
                    gravity
                ) > 0f;

            if (falling)
            {
                velocity +=
                    gravity *
                    fallGravityMultiplier *
                    Time.deltaTime;
            }
            else
            {
                velocity +=
                    gravity *
                    Time.deltaTime;
            }

            // Horizontal movement
            velocity.x =
                targetVelocity.x;

            IsGrounded = false;

            Vector2 deltaPosition =
                velocity *
                Time.deltaTime;

            PerformMovement(
                Vector2.right *
                deltaPosition.x,
                false
            );

            PerformMovement(
                Vector2.up *
                deltaPosition.y,
                true
            );
        }

        void PerformMovement(
            Vector2 move,
            bool yMovement
        )
        {
            float distance =
                move.magnitude;

            if (
                distance >
                minMoveDistance
            )
            {
                int count =
                    body.Cast(
                        move,
                        contactFilter,
                        hitBuffer,
                        distance +
                        shellRadius
                    );

                for (
                    int i = 0;
                    i < count;
                    i++
                )
                {
                    Vector2 currentNormal =
                        hitBuffer[i].normal;

                    if (
                        currentNormal.y *
                        gravityModifier >
                        minGroundNormalY
                    )
                    {
                        IsGrounded = true;

                        if (yMovement)
                        {
                            groundNormal =
                                currentNormal;

                            currentNormal.x = 0;
                        }
                    }

                    if (IsGrounded)
                    {
                        float projection =
                            Vector2.Dot(
                                velocity,
                                currentNormal
                            );

                        if (projection < 0)
                        {
                            velocity -=
                                projection *
                                currentNormal;
                        }
                    }
                    else if (!yMovement)
                    {
                        velocity.x = 0;
                    }

                    float modifiedDistance =
                        hitBuffer[i].distance -
                        shellRadius;

                    distance =
                        Mathf.Min(
                            modifiedDistance,
                            distance
                        );
                }
            }

            body.position +=
                move.normalized *
                distance;
        }
    }
}