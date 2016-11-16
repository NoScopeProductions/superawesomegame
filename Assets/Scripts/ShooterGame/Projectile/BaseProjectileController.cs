using Photon;
using ShooterGame.Extensions;
using UnityEngine;

namespace ShooterGame.Projectile
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BaseProjectileController : PunBehaviour
    {
        private const float POWER_CONSTANT = 100f;

        private Vector2 _prevPosition;
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            this._rigidbody = this.GetComponent<Rigidbody2D>();

            this._prevPosition = Vector2.zero;

            if (this.photonView.isMine == false)
            {
                this._rigidbody.isKinematic = true;
            }
        }

        public void Shoot(Vector2 direction, float power)
        {
            //TODO: Use Physics for now..
            
            this._rigidbody.AddForce(direction * power * POWER_CONSTANT, ForceMode2D.Impulse);
        }

        private void Update()
        {
            if (this.photonView.isMine == false && PhotonNetwork.connected) return;

            var moveDirection = (Vector2) this.transform.position - this._prevPosition;

            if (moveDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            this._prevPosition = this.transform.position;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            Debug.Log("Collided with " + col.gameObject.name);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
