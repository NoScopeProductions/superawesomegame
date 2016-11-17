#define DEBUG_CC2D_RAYS
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShooterGame.Player.Controllers
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public class CharacterController2D : MonoBehaviour
    {
        #region internal types

        private struct CharacterRaycastOrigins
        {
            public Vector3 _topLeft;
            public Vector3 _bottomRight;
            public Vector3 _bottomLeft;
        }

        public class CharacterCollisionState2D
        {
            public bool _right;
            public bool _left;
            public bool _above;
            public bool _below;
            public bool _becameGroundedThisFrame;
            public bool _wasGroundedLastFrame;
            public bool _movingDownSlope;
            public float _slopeAngle;

            public void Reset()
            {
                this._right = this._left = this._above = this._below = this._becameGroundedThisFrame = this._movingDownSlope = false;
                this._slopeAngle = 0f;
            }


            public override string ToString()
            {
                return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}",
                                     this._right, this._left, this._above, this._below, this._movingDownSlope, this._slopeAngle, this._wasGroundedLastFrame, this._becameGroundedThisFrame);
            }
        }

        #endregion


        #region events, properties and fields

        public event Action<RaycastHit2D> OnControllerCollidedEvent = o => { };
        public event Action<Collider2D> OnTriggerEnterEvent = o => { };
        public event Action<Collider2D> OnTriggerStayEvent = o => { };
        public event Action<Collider2D> OnTriggerExitEvent = o => { };


        /// <summary>
        /// when true, one way platforms will be ignored when moving vertically for a single frame
        /// </summary>
        public bool _ignoreOneWayPlatformsThisFrame;

        [SerializeField]
        [Range(0.001f, 0.3f)] private float _skinWidth = 0.02f;

        /// <summary>
        /// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
        /// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
        /// </summary>
        public float SkinWidth
        {
            get { return this._skinWidth; }
            set
            {
                this._skinWidth = value;
                this.RecalculateDistanceBetweenRays();
            }
        }


        /// <summary>
        /// mask with all layers that the player should interact with
        /// </summary>
        public LayerMask _platformMask = 0;

        /// <summary>
        /// mask with all layers that trigger events should fire when intersected
        /// </summary>
        public LayerMask _triggerMask = 0;

        /// <summary>
        /// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is because it does not support being
        /// updated anytime outside of the inspector for now.
        /// </summary>
        [SerializeField] private LayerMask _oneWayPlatformMask = 0;

        /// <summary>
        /// the max slope angle that the CC2D can climb
        /// </summary>
        /// <value>The slope limit.</value>
        [Range(0f, 90f)]
        public float _slopeLimit = 30f;

        /// <summary>
        /// the threshold in the change in vertical movement between frames that constitutes jumping
        /// </summary>
        /// <value>The jumping threshold.</value>
        public float _jumpingThreshold = 0.07f;


        /// <summary>
        /// curve for multiplying speed based on slope (negative = down slope and positive = up slope)
        /// </summary>
        public AnimationCurve _slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90f, 1.5f), new Keyframe(0f, 1f), new Keyframe(90f, 0f));

        [Range(2, 20)]
        public int _totalHorizontalRays = 8;
        [Range(2, 20)]
        public int _totalVerticalRays = 4;


        /// <summary>
        /// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
        /// to calculate the length of the ray that checks for slopes.
        /// </summary>
        private float _slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);


        [HideInInspector]
        [NonSerialized]
        public Transform _transform;
        [HideInInspector]
        [NonSerialized]
        public BoxCollider2D _boxCollider;
        [HideInInspector]
        [NonSerialized]
        public Rigidbody2D _rigidBody2D;

        [HideInInspector]
        [NonSerialized]
        public CharacterCollisionState2D _collisionState = new CharacterCollisionState2D();
        [HideInInspector]
        [NonSerialized]
        public Vector3 _velocity;
        public bool IsGrounded { get { return this._collisionState._below; } }

        private const float K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR = 0.001f;

        #endregion


        /// <summary>
        /// holder for our raycast origin corners (TR, TL, BR, BL)
        /// </summary>
        private CharacterRaycastOrigins _raycastOrigins;

        /// <summary>
        /// stores our raycast hit during movement
        /// </summary>
        private RaycastHit2D _raycastHit;

        /// <summary>
        /// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
        /// horizontally and vertically so that we can send the events after all collision state is set
        /// </summary>
        private readonly List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>(2);

        // horizontal/vertical movement data
        private float _verticalDistanceBetweenRays;
        private float _horizontalDistanceBetweenRays;

        // we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
        // the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
        private bool _isGoingUpSlope;


        #region Monobehaviour

        private void Awake()
        {
            // add our one-way platforms to our normal platform mask so that we can land on them from above
            this._platformMask |= this._oneWayPlatformMask;

            // cache some components
            this._transform = this.GetComponent<Transform>();
            this._boxCollider = this.GetComponent<BoxCollider2D>();
            this._rigidBody2D = this.GetComponent<Rigidbody2D>();

            // here, we trigger our properties that have setters with bodies
            this.SkinWidth = this._skinWidth;

            // we want to set our CC2D to ignore all collision layers except what is in our triggerMask
            for (var i = 0; i < 32; i++)
            {
                // see if our triggerMask contains this layer and if not ignore it
                if ((this._triggerMask.value & 1 << i) == 0)
                    Physics2D.IgnoreLayerCollision(this.gameObject.layer, i);
            }
        }


        public void OnTriggerEnter2D(Collider2D col)
        {
            if (this.OnTriggerEnterEvent != null)
                this.OnTriggerEnterEvent(col);
        }


        public void OnTriggerStay2D(Collider2D col)
        {
            if (this.OnTriggerStayEvent != null)
                this.OnTriggerStayEvent(col);
        }


        public void OnTriggerExit2D(Collider2D col)
        {
            if (this.OnTriggerExitEvent != null)
                this.OnTriggerExitEvent(col);
        }

        #endregion


        [System.Diagnostics.Conditional("DEBUG_CC2D_RAYS")]
        private void DrawRay(Vector3 start, Vector3 dir, Color color)
        {
            Debug.DrawRay(start, dir, color);
        }


        #region Public

        /// <summary>
        /// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
        /// stop when run into.
        /// </summary>
        /// <param name="deltaMovement">Delta movement.</param>
        public void Move(Vector3 deltaMovement)
        {
            // save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
            this._collisionState._wasGroundedLastFrame = this._collisionState._below;

            // clear our state
            this._collisionState.Reset();
            this._raycastHitsThisFrame.Clear();
            this._isGoingUpSlope = false;

            this.PrimeRaycastOrigins();


            // first, we check for a slope below us before moving
            // only check slopes if we are going down and grounded
            if (deltaMovement.y < 0f && this._collisionState._wasGroundedLastFrame)
                this.HandleVerticalSlope(ref deltaMovement);

            // now we check movement in the horizontal dir
            if (deltaMovement.x != 0f)
                this.MoveHorizontally(ref deltaMovement);

            // next, check movement in the vertical dir
            if (deltaMovement.y != 0f)
                this.MoveVertically(ref deltaMovement);

            // move then update our state
            deltaMovement.z = 0;
            this._transform.Translate(deltaMovement, Space.World);

            // only calculate velocity if we have a non-zero deltaTime
            if (Time.deltaTime > 0f)
                this._velocity = deltaMovement / Time.deltaTime;

            // set our becameGrounded state based on the previous and current collision state
            if (!this._collisionState._wasGroundedLastFrame && this._collisionState._below)
                this._collisionState._becameGroundedThisFrame = true;

            // if we are going up a slope we artificially set a y velocity so we need to zero it out here
            if (this._isGoingUpSlope)
                this._velocity.y = 0;

            // send off the collision events if we have a listener
            if (this.OnControllerCollidedEvent != null)
            {
                for (var i = 0; i < this._raycastHitsThisFrame.Count; i++)
                    this.OnControllerCollidedEvent(this._raycastHitsThisFrame[i]);
            }

            this._ignoreOneWayPlatformsThisFrame = false;
        }


        /// <summary>
        /// moves directly down until grounded
        /// </summary>
        public void WarpToGrounded()
        {
            do
            {
                this.Move(new Vector3(0, -1f, 0));
            } while (!this.IsGrounded);
        }


        /// <summary>
        /// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
        /// It is also used in the skinWidth setter in case it is changed at runtime.
        /// </summary>
        public void RecalculateDistanceBetweenRays()
        {
            // figure out the distance between our rays in both directions
            // horizontal
            var colliderUseableHeight = this._boxCollider.size.y * Mathf.Abs(this._transform.localScale.y) - (2f * this._skinWidth);
            this._verticalDistanceBetweenRays = colliderUseableHeight / (this._totalHorizontalRays - 1);

            // vertical
            var colliderUseableWidth = this._boxCollider.size.x * Mathf.Abs(this._transform.localScale.x) - (2f * this._skinWidth);
            this._horizontalDistanceBetweenRays = colliderUseableWidth / (this._totalVerticalRays - 1);
        }

        #endregion


        #region Movement Methods

        /// <summary>
        /// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
        /// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
        /// </summary>
        private void PrimeRaycastOrigins()
        {
            // our raycasts need to be fired from the bounds inset by the skinWidth
            var modifiedBounds = this._boxCollider.bounds;
            modifiedBounds.Expand(-2f * this._skinWidth);

            this._raycastOrigins._topLeft = new Vector2(modifiedBounds.min.x, modifiedBounds.max.y);
            this._raycastOrigins._bottomRight = new Vector2(modifiedBounds.max.x, modifiedBounds.min.y);
            this._raycastOrigins._bottomLeft = modifiedBounds.min;
        }


        /// <summary>
        /// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
        /// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
        /// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
        /// actually moving the player
        /// </summary>
        private void MoveHorizontally(ref Vector3 deltaMovement)
        {
            var isGoingRight = deltaMovement.x > 0;
            var rayDistance = Mathf.Abs(deltaMovement.x) + this._skinWidth;
            var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
            var initialRayOrigin = isGoingRight ? this._raycastOrigins._bottomRight : this._raycastOrigins._bottomLeft;

            for (var i = 0; i < this._totalHorizontalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y + i * this._verticalDistanceBetweenRays);

                this.DrawRay(ray, rayDirection * rayDistance, Color.red);

                // if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
                // walk up sloped oneWayPlatforms
                if (i == 0 && this._collisionState._wasGroundedLastFrame)
                    this._raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, this._platformMask);
                else
                    this._raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, this._platformMask & ~this._oneWayPlatformMask);

                if (this._raycastHit)
                {
                    // the bottom ray can hit a slope but no other ray can so we have special handling for these cases
                    if (i == 0 && this.HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(this._raycastHit.normal, Vector2.up)))
                    {
                        this._raycastHitsThisFrame.Add(this._raycastHit);
                        break;
                    }

                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.x = this._raycastHit.point.x - ray.x;
                    rayDistance = Mathf.Abs(deltaMovement.x);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingRight)
                    {
                        deltaMovement.x -= this._skinWidth;
                        this._collisionState._right = true;
                    }
                    else
                    {
                        deltaMovement.x += this._skinWidth;
                        this._collisionState._left = true;
                    }

                    this._raycastHitsThisFrame.Add(this._raycastHit);

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < this._skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
                        break;
                }
            }
        }


        /// <summary>
        /// handles adjusting deltaMovement if we are going up a slope.
        /// </summary>
        /// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
        /// <param name="deltaMovement">Delta movement.</param>
        /// <param name="angle">Angle.</param>
        private bool HandleHorizontalSlope(ref Vector3 deltaMovement, float angle)
        {
            // disregard 90 degree angles (walls)
            if (Mathf.RoundToInt(angle) == 90)
                return false;

            // if we can walk on slopes and our angle is small enough we need to move up
            if (angle < this._slopeLimit)
            {
                // we only need to adjust the deltaMovement if we are not jumping
                // TODO: this uses a magic number which isn't ideal! The alternative is to have the user pass in if there is a jump this frame
                if (deltaMovement.y < this._jumpingThreshold)
                {
                    // apply the slopeModifier to slow our movement up the slope
                    var slopeModifier = this._slopeSpeedMultiplier.Evaluate(angle);
                    deltaMovement.x *= slopeModifier;

                    // we dont set collisions on the sides for this since a slope is not technically a side collision.
                    // smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
                    // to our new x location using our good friend Pythagoras
                    deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
                    var isGoingRight = deltaMovement.x > 0;

                    // safety check. we fire a ray in the direction of movement just in case the diagonal we calculated above ends up
                    // going through a wall. if the ray hits, we back off the horizontal movement to stay in bounds.
                    var ray = isGoingRight ? this._raycastOrigins._bottomRight : this._raycastOrigins._bottomLeft;
                    RaycastHit2D raycastHit;
                    if (this._collisionState._wasGroundedLastFrame)
                        raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude, this._platformMask);
                    else
                        raycastHit = Physics2D.Raycast(ray, deltaMovement.normalized, deltaMovement.magnitude, this._platformMask & ~this._oneWayPlatformMask);

                    if (raycastHit)
                    {
                        // we crossed an edge when using Pythagoras calculation, so we set the actual delta movement to the ray hit location
                        deltaMovement = (Vector3)raycastHit.point - ray;
                        if (isGoingRight)
                            deltaMovement.x -= this._skinWidth;
                        else
                            deltaMovement.x += this._skinWidth;
                    }

                    this._isGoingUpSlope = true;
                    this._collisionState._below = true;
                }
            }
            else // too steep. get out of here
            {
                deltaMovement.x = 0;
            }

            return true;
        }


        private void MoveVertically(ref Vector3 deltaMovement)
        {
            var isGoingUp = deltaMovement.y > 0;
            var rayDistance = Mathf.Abs(deltaMovement.y) + this._skinWidth;
            var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
            var initialRayOrigin = isGoingUp ? this._raycastOrigins._topLeft : this._raycastOrigins._bottomLeft;

            // apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
            initialRayOrigin.x += deltaMovement.x;

            // if we are moving up, we should ignore the layers in oneWayPlatformMask
            var mask = this._platformMask;
            if ((isGoingUp && !this._collisionState._wasGroundedLastFrame) || this._ignoreOneWayPlatformsThisFrame)
                mask &= ~this._oneWayPlatformMask;

            for (var i = 0; i < this._totalVerticalRays; i++)
            {
                var ray = new Vector2(initialRayOrigin.x + i * this._horizontalDistanceBetweenRays, initialRayOrigin.y);

                this.DrawRay(ray, rayDirection * rayDistance, Color.red);
                this._raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, mask);
                if (this._raycastHit)
                {
                    // set our new deltaMovement and recalculate the rayDistance taking it into account
                    deltaMovement.y = this._raycastHit.point.y - ray.y;
                    rayDistance = Mathf.Abs(deltaMovement.y);

                    // remember to remove the skinWidth from our deltaMovement
                    if (isGoingUp)
                    {
                        deltaMovement.y -= this._skinWidth;
                        this._collisionState._above = true;
                    }
                    else
                    {
                        deltaMovement.y += this._skinWidth;
                        this._collisionState._below = true;
                    }

                    this._raycastHitsThisFrame.Add(this._raycastHit);

                    // this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
                    // where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
                    if (!isGoingUp && deltaMovement.y > 0.00001f)
                        this._isGoingUpSlope = true;

                    // we add a small fudge factor for the float operations here. if our rayDistance is smaller
                    // than the width + fudge bail out because we have a direct impact
                    if (rayDistance < this._skinWidth + K_SKIN_WIDTH_FLOAT_FUDGE_FACTOR)
                        break;
                }
            }
        }


        /// <summary>
        /// checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
        /// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
        /// </summary>
        /// <param name="deltaMovement">Delta movement.</param>
        private void HandleVerticalSlope(ref Vector3 deltaMovement)
        {
            // slope check from the center of our collider
            var centerOfCollider = (this._raycastOrigins._bottomLeft.x + this._raycastOrigins._bottomRight.x) * 0.5f;
            var rayDirection = -Vector2.up;

            // the ray distance is based on our slopeLimit
            var slopeCheckRayDistance = this._slopeLimitTangent * (this._raycastOrigins._bottomRight.x - centerOfCollider);

            var slopeRay = new Vector2(centerOfCollider, this._raycastOrigins._bottomLeft.y);
            this.DrawRay(slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow);
            this._raycastHit = Physics2D.Raycast(slopeRay, rayDirection, slopeCheckRayDistance, this._platformMask);
            if (this._raycastHit)
            {
                // bail out if we have no slope
                var angle = Vector2.Angle(this._raycastHit.normal, Vector2.up);

                this.SlopeAngle = angle; //TODO: Set the slope angle to negative or positive depending on facing direction so we can use it from the (future) AngleController

                if (angle == 0)
                    return;

                // we are moving down the slope if our normal and movement direction are in the same x direction
                var isMovingDownSlope = Mathf.Sign(this._raycastHit.normal.x) == Mathf.Sign(deltaMovement.x);
                if (isMovingDownSlope)
                {
                    // going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
                    var slopeModifier = this._slopeSpeedMultiplier.Evaluate(-angle);
                    // we add the extra downward movement here to ensure we "stick" to the surface below
                    deltaMovement.y += this._raycastHit.point.y - slopeRay.y - this.SkinWidth;
                    deltaMovement.x *= slopeModifier;
                    this._collisionState._movingDownSlope = true;
                    this._collisionState._slopeAngle = angle;
                }
            }
        }

        public float SlopeAngle;

        #endregion

    }
}
