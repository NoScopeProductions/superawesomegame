using Photon;
using ShooterGame.Constants;
using ShooterGame.Extensions;
using ShooterGame.Projectile;
using UnityEngine;

namespace ShooterGame.Player
{
    [RequireComponent(typeof(AngleController))]
    public class ShootingController : PunBehaviour
    {
        private const float POWER_GAIN_PER_SECOND = 1f;
        private const float MAX_POWER = 4.0f;

        public float _power;
        private float Power
        {
            get { return this._power; }
            set { this._power = value; }
        }

        private AngleController AngleController { get; set; }

        private void Awake()
        {
            this.AngleController = this.GetComponent<AngleController>();
            this.Power = 0f;
        }

        private void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;

            //TODO: Check if it is currently our turn before accepting input

            if (Input.GetKey(KeyCode.S))
            {
                this.Power += POWER_GAIN_PER_SECOND * Time.deltaTime;
                if (this.Power >= MAX_POWER)
                {
                    this.Power = MAX_POWER;
                }
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                this.FireProjectile(this.Power);
                this.Power = 0f;
            }
        }

        private void FireProjectile(float power)
        {
            int angle = (int) this.AngleController.Angle;

            Debug.Log("Fire with power: " + power + " and angle: " + angle + "!");


            var direction = Vector2.right.Rotate(angle).normalized;

            //TODO: set projectile position based on sprite/gun position
            var projectilePosition = this.transform.position + Vector3.up*0.4f;
            var projectile = PhotonNetwork.Instantiate(PrefabNames.BASE_PROJECTILE, projectilePosition, Quaternion.identity, 0);

            projectile.GetComponent<BaseProjectileController>().Shoot(direction, power);
        }
    }
}
