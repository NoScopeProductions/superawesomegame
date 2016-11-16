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
            get { return _angle; }
            private set { _angle = value; }
        }

        void Start()
        {
            Angle = (_maxAngle - _minAngle) / 2 + _minAngle;
        }

        void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;
            CheckInput();
            DecreaseInputCooldown();
        }

        private void DecreaseInputCooldown()
        {
            CurrentCooldown -= Time.deltaTime;
            if (CurrentCooldown <= 0)
            {
                CurrentCooldown = 0f;
            }
        }

        private void CheckInput()
        {
            if (CurrentCooldown <= 0f)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    IncreaseAngle();
                    CurrentCooldown = InputCooldown;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    DecreaseAngle();
                    CurrentCooldown = InputCooldown;
                }
            }
        }

        private void IncreaseAngle()
        {
            Angle++;

            if (Angle > _maxAngle)
            {
                Angle = _maxAngle;
            }
        }

        private void DecreaseAngle()
        {
            Angle--;

            if(Angle < _minAngle)
            {
                Angle = _minAngle;
            }
        }
    }
}
