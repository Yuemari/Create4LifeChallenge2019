using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [Tooltip("The Layers which represent gameobjects that the Character Controller can be grounded on.")]
        public LayerMask groundedLayerMask;
        [Tooltip("The distance down to check for ground.")]
        public float groundedRaycastDistance = 0.1f;

        Rigidbody2D m_Rigidbody2D;
        BoxCollider2D boxCollider;
        Vector2 m_PreviousPosition;
        Vector2 m_CurrentPosition;
        Vector2 m_NextMovement;
        ContactFilter2D m_ContactFilter;
        RaycastHit2D[] m_HitBuffer = new RaycastHit2D[5];
        RaycastHit2D[] m_FoundHits = new RaycastHit2D[3];
        Collider2D[] m_GroundColliders = new Collider2D[3];
        Vector2[] m_RaycastPositions = new Vector2[3];

        public bool IsGrounded { get; protected set; }
        public bool IsCeilinged { get; protected set; }
        public bool IsHittingLeftWall { get; protected set; }
        public bool IsHittingRightWall { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Rigidbody2D Rigidbody2D { get { return m_Rigidbody2D; } }
        public Collider2D[] GroundColliders { get { return m_GroundColliders; } }
        public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }


        void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();

            m_CurrentPosition = m_Rigidbody2D.position;
            m_PreviousPosition = m_Rigidbody2D.position;

            m_ContactFilter.layerMask = groundedLayerMask;
            m_ContactFilter.useLayerMask = true;
            m_ContactFilter.useTriggers = false;

            Physics2D.queriesStartInColliders = false;
        }

        void FixedUpdate()
        {
            m_PreviousPosition = m_Rigidbody2D.position;
            m_CurrentPosition = m_PreviousPosition + m_NextMovement;
            Velocity = (m_CurrentPosition - m_PreviousPosition) / Time.deltaTime;

            m_Rigidbody2D.MovePosition(m_CurrentPosition);
            m_NextMovement = Vector2.zero;

            CheckCapsuleEndCollisions();
            CheckCapsuleEndCollisions(false);
            CheckSideCollider();
            CheckSideCollider(true);
        }

        /// <summary>
        /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
        /// </summary>
        /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
        public void Move(Vector2 movement)
        {
            m_NextMovement += movement;
        }

        /// <summary>
        /// This moves the character without any implied velocity.
        /// </summary>
        /// <param name="position">The new position of the character in global space.</param>
        public void Teleport(Vector2 position)
        {
            Vector2 delta = position - m_CurrentPosition;
            m_PreviousPosition += delta;
            m_CurrentPosition = position;
            m_Rigidbody2D.MovePosition(position);
        }

        /// <summary>
        /// This updates the state of IsGrounded.  It is called automatically in FixedUpdate but can be called more frequently if higher accurracy is required.
        /// </summary>
        public void CheckCapsuleEndCollisions(bool bottom = true)
        {
            Vector2 raycastDirection;
            Vector2 raycastStart;
            float raycastDistance;

            if (boxCollider == null)
            {
                raycastStart = m_Rigidbody2D.position + Vector2.up;
                raycastDistance = 1f + groundedRaycastDistance;

                if (bottom)
                {
                    raycastDirection = Vector2.down;

                    m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                    m_RaycastPositions[1] = raycastStart;
                    m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
                }
                else
                {
                    raycastDirection = Vector2.up;

                    m_RaycastPositions[0] = raycastStart + Vector2.left * 0.4f;
                    m_RaycastPositions[1] = raycastStart;
                    m_RaycastPositions[2] = raycastStart + Vector2.right * 0.4f;
                }
            }
            else
            {
                raycastStart = m_Rigidbody2D.position + boxCollider.offset;
                raycastDistance = boxCollider.size.x * 0.5f + groundedRaycastDistance * 2f;

                if (bottom)
                {
                    raycastDirection = Vector2.down;
                    Vector2 raycastStartBottomCentre = raycastStart + Vector2.down * (boxCollider.size.y * 0.5f - boxCollider.size.x * 0.5f);

                    m_RaycastPositions[0] = raycastStartBottomCentre + Vector2.left * boxCollider.size.x * 0.5f;
                    m_RaycastPositions[1] = raycastStartBottomCentre;
                    m_RaycastPositions[2] = raycastStartBottomCentre + Vector2.right * boxCollider.size.x * 0.5f;
                }
                else
                {
                    raycastDirection = Vector2.up;
                    Vector2 raycastStartTopCentre = raycastStart + Vector2.up * (boxCollider.size.y * 0.5f - boxCollider.size.x * 0.5f);

                    m_RaycastPositions[0] = raycastStartTopCentre + Vector2.left * boxCollider.size.x * 0.5f;
                    m_RaycastPositions[1] = raycastStartTopCentre;
                    m_RaycastPositions[2] = raycastStartTopCentre + Vector2.right * boxCollider.size.x * 0.5f;
                }
            }

            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                int count = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

                

                if (bottom)
                {
                    //Debug.Log("OnFloor Raycast:" + count);
                    m_FoundHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
                    m_GroundColliders[i] = m_FoundHits[i].collider;
                }
                else
                {
                    IsCeilinged = false;

                }
            }

            if (bottom)
            {
                Vector2 groundNormal = Vector2.zero;
                int hitCount = 0;

                for (int i = 0; i < m_FoundHits.Length; i++)
                {
                    if (m_FoundHits[i].collider != null)
                    {
                        groundNormal += m_FoundHits[i].normal;
                        hitCount++;
                    }
                }

                //Debug.Log("bottom HitCounts:"+hitCount);
                if (hitCount > 0)
                {
                    groundNormal.Normalize();
                }

                Vector2 relativeVelocity = Velocity;
                //Debug.LogFormat("GroundNormal:{0} relativeVelocity:{1} ", groundNormal, relativeVelocity.y);

                if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
                {
                    IsGrounded = false;
                }
                else
                {
                    IsGrounded = relativeVelocity.y <= 0f;
                    //Debug.LogFormat("RelativeVel:{0} isgrounded:{1}",relativeVelocity,IsGrounded);

                    if (boxCollider != null)
                    {
                        if (m_GroundColliders[1] != null)
                        {
                            float capsuleBottomHeight = m_Rigidbody2D.position.y + boxCollider.offset.y - boxCollider.size.y * 0.5f;
                            float middleHitHeight = m_FoundHits[1].point.y;

                           // Debug.LogFormat("isGrounded:{3} middleHitHeight:{0} capsuelBottomH:{1} groundedRaycastDistance:{2}", middleHitHeight , capsuleBottomHeight, groundedRaycastDistance,IsGrounded);

                            IsGrounded &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                        }
                    }
                }
            }

            for (int i = 0; i < m_HitBuffer.Length; i++)
            {
                m_HitBuffer[i] = new RaycastHit2D();
            }
        }

        public void CheckSideCollider(bool directionRight = false)
        {
            Vector2 raycastDirection;
            Vector2 raycastStart;
            float raycastDistance;

            if (boxCollider == null)
            {
                raycastStart = m_Rigidbody2D.position + Vector2.up;
                raycastDistance = 1f + groundedRaycastDistance;

                raycastDirection = Vector2.left;

                m_RaycastPositions[0] = raycastStart + Vector2.up * 0.4f;
                m_RaycastPositions[1] = raycastStart;
                m_RaycastPositions[2] = raycastStart + Vector2.down * 0.4f;
            }
            else
            {
                raycastStart = m_Rigidbody2D.position + boxCollider.size * 0.5f;
                raycastDistance = boxCollider.size.x * 0.5f;

                raycastDirection = (directionRight ? Vector2.right : Vector2.left);
                Vector2 raycastStartBottomCentre = raycastStart + Vector2.left * (boxCollider.size.y * 0.4f - boxCollider.size.x * 0.4f);

                m_RaycastPositions[0] = raycastStartBottomCentre + Vector2.up * boxCollider.size.x * 0.4f;
                m_RaycastPositions[1] = raycastStartBottomCentre;
                m_RaycastPositions[2] = raycastStartBottomCentre + Vector2.down * boxCollider.size.x * 0.4f;
            }

            for (int i = 0; i < m_RaycastPositions.Length; i++)
            {
                int count = Physics2D.Raycast(m_RaycastPositions[i], raycastDirection, m_ContactFilter, m_HitBuffer, raycastDistance);

                //Debug.Log("onwall Raycast:" + count);
                m_FoundHits[i] = count > 0 ? m_HitBuffer[0] : new RaycastHit2D();
                m_GroundColliders[i] = m_FoundHits[i].collider;

            }

            Vector2 wall = Vector2.zero;
            int hitCount = 0;

            for (int i = 0; i < m_FoundHits.Length; i++)
            {
                if (m_FoundHits[i].collider != null)
                {
                    wall += m_FoundHits[i].normal;
                    hitCount++;
                }
            }

            //Debug.Log("wall HitCounts:" + hitCount);
            if (hitCount > 0)
            {
                wall.Normalize();
            }

            Vector2 relativeVelocity = Velocity;
           // Debug.LogFormat("GroundNormal:{0} relativeVelocity:{1} ", wall, relativeVelocity.x);

            if (Mathf.Approximately(wall.x, 0f) && Mathf.Approximately(wall.y, 0f))
            {
                if (directionRight)
                {
                    IsHittingRightWall = false;
                }
                else
                {
                    IsHittingLeftWall = false;
                }
            }
            else
            {
                if (directionRight)
                {
                    IsHittingRightWall = relativeVelocity.x > 0f;
                }
                else
                {
                    IsHittingLeftWall = relativeVelocity.x < 0f;
                }
                
                Debug.LogFormat("RelativeVel:{0} {1}wall:{2}", relativeVelocity, (directionRight?"Right":"Left"), (directionRight?IsHittingRightWall:IsHittingLeftWall));

                if (boxCollider != null)
                {
                    if (m_GroundColliders[1] != null)
                    {
                        float capsuleBottomHeight = m_Rigidbody2D.position.x + boxCollider.offset.x - boxCollider.size.x * 0.5f;
                        float middleHitHeight = m_FoundHits[1].point.x;

                        //Debug.LogFormat("leftwall:{3} middleHitHeight:{0} capsuelBottomH:{1} groundedRaycastDistance:{2}", middleHitHeight, capsuleBottomHeight, groundedRaycastDistance, IsHittingLeftWall);

                        if (directionRight)
                        {
                            IsHittingRightWall &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                        }
                        else
                        {
                            IsHittingLeftWall &= middleHitHeight < capsuleBottomHeight + groundedRaycastDistance;
                        }
                        
                    }
                }
            }

            for (int i = 0; i < m_HitBuffer.Length; i++)
            {
                m_HitBuffer[i] = new RaycastHit2D();
            }
        }
    }
}