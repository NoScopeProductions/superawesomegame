using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace ShooterGame.Player
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [SelectionBase]
    public class PlayerController : Photon.MonoBehaviour
    {
        [SerializeField]
        private  Vector2 _velocity = Vector2.zero;

        [SerializeField, UsedImplicitly] private int _rayCount = 8; //TODO: don't need to expose this.

    
        [SerializeField, UsedImplicitly] private float _maximumClimbAngle = 30f;

        private const float WALK_SPEED = 6f;
        private const float GRAVITY = 4f;
        private const float TERMINAL_VELOCITY = 12f;

        private float GroundCheckDelta
        {
            get { return this._bounds.height + 0.5f; }
        }

        private const float MARGIN = 0f;
    

        private Transform _spriteTransform;
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody;
        private PlayerStats _stats;

        //private float _xMax = float.MaxValue;
        //private float _yMax = float.MaxValue; //might not need since there is no vertical movement
        //private float _yMin = float.MinValue;
        //private float _xMin = float.MinValue;


        [SerializeField]
        private bool _isGrounded;
        [SerializeField]
        private bool _isOnSlope;

        private readonly List<RaycastHit2D> _raycastHits = new List<RaycastHit2D>();

        private Rect _bounds;
    


        private Vector2 TopLeft
        {
            get
            {
                return this._bounds.min + Vector2.up *this._bounds.height + MARGIN * (Vector2.right + Vector2.down);
            }
        }

        private Vector2 TopRight
        {
            get
            {
                return this._bounds.max + MARGIN * (Vector2.left + Vector2.down);
            }
        }

        private Vector2 BottomLeft
        {
            get
            {
                return this._bounds.min +  + MARGIN * (Vector2.right + Vector2.up);
            }
        }

        private Vector2 BottomRight
        {
            get
            {
                return this._bounds.max + Vector2.down*this._bounds.height + MARGIN * (Vector2.left + Vector2.up);
            }
        }

        private void Awake()
        {
            this._collider = this.GetComponent<BoxCollider2D>();
            this._rigidbody = this.GetComponent<Rigidbody2D>();

            this._spriteTransform = this.GetComponentsInChildren<Transform>().First(t => t.name == "SpriteGraphics");
            _stats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;

            this._bounds.x = this._collider.bounds.min.x;
            this._bounds.y = this._collider.bounds.min.y;
            this._bounds.width = this._collider.bounds.size.x;
            this._bounds.height = this._collider.bounds.size.y;

            //_velocity.x = _velocity.y = 0f;
            this.ApplyMovementInput();
        
            if (this._isGrounded == false)
            {
                this.ApplyGravity();
            }

            this.CheckGrounded();

        
            this.AlignSpriteToGround();

            this.UpdatePosition();
        }
    
        private void ApplyGravity()
        {
            this._velocity.y += -GRAVITY*Time.fixedDeltaTime;
            this._velocity.y = Mathf.Clamp(this._velocity.y, -TERMINAL_VELOCITY*Time.fixedDeltaTime, float.MaxValue);
        }

        private void ApplyMovementInput()
        {
            //TODO: Apply only when grounded...Can probably only fire one raycast instead of _rayCount
            float input = Input.GetAxisRaw("Horizontal");

            this.GetRaycastInfo(this.TopLeft, this.TopRight, Vector2.down, 100f);

            var shortestRay = this._raycastHits.Where(hit => hit.collider != null)
                .OrderBy(hit => hit.fraction)
                .FirstOrDefault(hit => Math.Abs(hit.fraction) > 0.0001f);

            float angle = Vector2.Angle(shortestRay.normal, Vector2.up);

            this._velocity.x = input * WALK_SPEED * Time.fixedDeltaTime * Mathf.Cos(angle * Mathf.Deg2Rad);
        }

        private void UpdatePosition()
        {
            var newPos = new Vector2
            {
                x = this._rigidbody.position.x + this._velocity.x,
                y = this._rigidbody.position.y + this._velocity.y
            };

            this._rigidbody.MovePosition(newPos);
        }

        private void CheckGrounded()
        {
            this.GetRaycastInfo(this.TopLeft, this.TopRight, Vector2.down, this._bounds.height + this.GroundCheckDelta);

            if (this._raycastHits.Any(hit => hit.collider != null))
            {
                var shortestRay = this._raycastHits.Where(hit => hit.collider != null)
                    .OrderBy(hit => hit.fraction)
                    .FirstOrDefault(hit => Math.Abs(hit.fraction) > 0.0001f);

                if (shortestRay.distance < this.GroundCheckDelta)
                {
                    this._velocity.y = 0;
                    this._isGrounded = true;

                    this._isOnSlope = Math.Abs(Vector2.Angle(shortestRay.normal, Vector2.up)) > 0.0001f;

                    float targetY = shortestRay.point.y + this._bounds.height/2;

                    if (this._isOnSlope)
                    {
                        var startPoint = this.TopLeft + Vector2.right*this._bounds.width/2;
                        var middleRay = Physics2D.Raycast(startPoint, Vector2.down, this._bounds.height + 0.3f);
                        Debug.DrawLine(this._bounds.center, middleRay.point, Color.green);

                        if (middleRay.collider != null)
                        {
                            targetY = middleRay.point.y + this._bounds.height/2;
                        }
                    }
                    this._rigidbody.position = new Vector2(this._rigidbody.position.x, targetY);
                }
                else
                {
                    this._isGrounded = false;
                    this._isOnSlope = false;
                }
            }
            else
            {
                this._isGrounded = false;
                this._isOnSlope = false;
            }
        }

        private void GetRaycastInfo(Vector2 startPoint, Vector2 endPoint, Vector2 direction, float distance)
        {
            this._raycastHits.Clear();

            for (int i = 0; i < this._rayCount; i++)
            {
                float lerpAmount = (float) i/(this._rayCount - 1);
                var raycastOrigin = Vector2.Lerp(startPoint, endPoint, lerpAmount);

                var hitInfo = Physics2D.Raycast(raycastOrigin, direction, distance);

                this._raycastHits.Add(hitInfo);
                if (hitInfo.collider != null)
                {
                    Debug.DrawLine(raycastOrigin, hitInfo.point, Color.cyan);
                }
            
            }
        }

        private void AlignSpriteToGround()
        {
            //TODO: get sprite transform width/height?
            var midPoint = this.TopLeft + Vector2.right * this._bounds.width / 2;
            var hitInfo = Physics2D.Raycast(midPoint, Vector2.down, this._bounds.height + 0.3f);

            this._spriteTransform.up = hitInfo.normal;

            //Align to ground
            //Vector2 startPoint = this._spriteTransform.position - this._spriteTransform.right * 0.5f - this._spriteTransform.up * 0.5f;
            //startPoint.y += MARGIN;
            //startPoint.x += MARGIN;

            //Vector2 endPoint = this._spriteTransform.position + this._spriteTransform.right * 0.5f - this._spriteTransform.up * 0.5f;
            //endPoint.y += MARGIN;
            //endPoint.x -= MARGIN;

            //var hitInfoStart = Physics2D.Raycast(startPoint, Vector2.down, 4f);
            //var hitInfoEnd = Physics2D.Raycast(endPoint, Vector2.down, 4f);
            //var hitInfos = new[] { hitInfoStart, hitInfoEnd };
            //switch (hitCount)
            //{
            //    case 2:
            //        var alignAxis = hitInfoEnd.point - hitInfoStart.point;
            //        this._spriteTransform.up = Vector3.Cross(alignAxis, Vector3.back).normalized;
            //        break;
            //    case 1:
            //        var hitInfo = hitInfos.First(h => h.collider != null);
            //        this._spriteTransform.up = hitInfo.normal;
            //        break;
            //    default:
            //        this._spriteTransform.up = Vector2.up;
            //        break;
            //}

        }
    }
}