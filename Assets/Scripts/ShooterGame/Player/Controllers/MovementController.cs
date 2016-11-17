using Photon;
using UnityEngine;

namespace ShooterGame.Player.Controllers
{
    [RequireComponent(typeof(CharacterController2D))]
    public class MovementController : PunBehaviour
    {
        // movement config
        [SerializeField] private readonly float _gravity = -25f;
        [SerializeField] private readonly float _runSpeed = 8f;
        [SerializeField] private readonly float _groundDamping = 20f; // how fast do we change direction? higher means faster
        [SerializeField] private readonly float _inAirDamping = 0f;

        private float _normalizedHorizontalSpeed;

        private CharacterController2D _controller;
        private Vector3 _velocity;

        public void Awake()
        {
            this._controller = this.GetComponent<CharacterController2D>();
            this._normalizedHorizontalSpeed = 0f;
        }

        public void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;

            if (this._controller.IsGrounded)
                this._velocity.y = 0;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                this._normalizedHorizontalSpeed = 1;
                if (this.transform.localScale.x < 0f)
                    this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                this._normalizedHorizontalSpeed = -1;
                if (this.transform.localScale.x > 0f)
                    this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);

            }
            else
            {
                this._normalizedHorizontalSpeed = 0;
            }

            var smoothedMovementFactor = this._controller.IsGrounded ? this._groundDamping : this._inAirDamping; // how fast do we change direction?
            this._velocity.x = Mathf.Lerp(this._velocity.x, this._normalizedHorizontalSpeed * this._runSpeed, Time.deltaTime * smoothedMovementFactor);

            // apply gravity before moving
            this._velocity.y += this._gravity * Time.deltaTime;

            this._controller.Move(this._velocity * Time.deltaTime);

            // grab our current _velocity to use as a base for all calculations
            this._velocity = this._controller._velocity;
        }

    }
}
