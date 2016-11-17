using UnityEngine;
using Photon;
using System;


namespace ShooterGame.Player
{
    public class AngleController : PunBehaviour
    {
        public float InputCooldown = 0.08f;
        private float CurrentCooldown = 0f;

        [SerializeField]
        private int _minAngle;

        [SerializeField]
        private int _maxAngle;

        [SerializeField]
        private int _angle;

        public int Angle
        {
            get { return this._angle; }
            private set { this._angle = value; }
        }

        void Start()
        {
            this.Angle = (this._maxAngle - this._minAngle) / 2 + this._minAngle;
        }

        void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;
            this.CheckInput();
            this.DecreaseInputCooldown();
        }

        private void DecreaseInputCooldown()
        {
            this.CurrentCooldown -= Time.deltaTime;
            if (this.CurrentCooldown <= 0)
            {
                this.CurrentCooldown = 0f;
            }
        }

        private void CheckInput()
        {
            if (this.CurrentCooldown <= 0f)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    this.IncreaseAngle();
                    this.CurrentCooldown = this.InputCooldown;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    this.DecreaseAngle();
                    this.CurrentCooldown = this.InputCooldown;
                }
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
