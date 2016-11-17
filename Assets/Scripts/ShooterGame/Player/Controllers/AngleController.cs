using JetBrains.Annotations;
using Photon;
using UnityEngine;

namespace ShooterGame.Player.Controllers
{
    public class AngleController : PunBehaviour
    {
        private const float INPUT_COOLDOWN = 0.08f;
        private float _currentCooldown;

        [SerializeField, UsedImplicitly]
        private int _minAngle;

        [SerializeField, UsedImplicitly]
        private int _maxAngle;

        [SerializeField] //TODO: Temporary so we can see the value in the inspector, once the HUD is in place we don't need to expose this variable and Angle can be an auto-property.
        private int _angle;

        public int Angle
        {
            get { return this._angle; }
            private set { this._angle = value; }
        }

        private void Start()
        {
            this.Angle = (this._maxAngle - this._minAngle) / 2 + this._minAngle;
            this._currentCooldown = 0f;
        }

        private void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;
            this.CheckInput();
            this.DecreaseInputCooldown();
        }

        private void DecreaseInputCooldown()
        {
            this._currentCooldown -= Time.deltaTime;
            if (this._currentCooldown <= 0)
            {
                this._currentCooldown = 0f;
            }
        }

        private void CheckInput()
        {
            if (!(this._currentCooldown <= 0f)) return;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                this.IncreaseAngle();
                this._currentCooldown = INPUT_COOLDOWN;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                this.DecreaseAngle();
                this._currentCooldown = INPUT_COOLDOWN;
            }
        }

        private void IncreaseAngle()
        {
            this.Angle++;

            if (this.Angle > this._maxAngle)
            {
                this.Angle = this._maxAngle;
            }
        }

        private void DecreaseAngle()
        {
            this.Angle--;

            if(this.Angle < this._minAngle)
            {
                this.Angle = this._minAngle;
            }
        }
    }
}
