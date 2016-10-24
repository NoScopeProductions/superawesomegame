using JetBrains.Annotations;
using ShooterGame.Player;
using UnityEngine;

namespace ShooterGame.Camera
{
    public class SmoothFollowCamera : MonoBehaviour
    {
        public Transform Target { private get; set; }
        private Transform _transform;

        [SerializeField, UsedImplicitly] private Vector3 _cameraOffset;
        [SerializeField, UsedImplicitly] private float _smoothDampTime = 0.2f;

        private Vector3 _smoothDampVelocity;


        private void Awake()
        {
            this._transform = this.gameObject.transform;
        }


        private void LateUpdate()
        {
            if (this.Target != null)
            {
                this._transform.position = Vector3.SmoothDamp(this._transform.position,
                                                              this.Target.position - this._cameraOffset,
                                                              ref this._smoothDampVelocity,
                                                              this._smoothDampTime);
            }
        }
    }
}
