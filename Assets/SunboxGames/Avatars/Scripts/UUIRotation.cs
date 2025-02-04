using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Chat.Demo;

namespace Sunbox.Avatars {
    public class UUIRotation :  MonoBehaviourPunCallbacks, IBeginDragHandler, IDragHandler, IEndDragHandler {
        
        public GameObject Avatar;

        public float RotationSpeed = 1;
        public float RotationDamping = 0.1f;

        public bool AutoRotate = false;
        public float AutoRotateSpeed = 1f;
    
        private float _rotationVelocity;
        private bool _dragged;

        private Vector3 _rotationVector = Vector3.up;

        void Start(){
            if (photonView.IsMine)
                Debug.Log("AvatarRotation photonView.IsMine");
                GameObject userAvatar = AvatarManager.Instance.GetAvatarForUser(PhotonNetwork.LocalPlayer.UserId);
                Debug.Log(userAvatar != null);
                if (userAvatar != null && Avatar == null) {
                    Avatar = userAvatar;
                }
        }
        
        
    
        public void OnBeginDrag(PointerEventData eventData) {
            if (EventSystem.current.IsPointerOverGameObject(eventData.pointerId)) return;
            _dragged = true;
        }
    
        public void OnDrag(PointerEventData eventData) {
            _rotationVelocity = eventData.delta.x * RotationSpeed;
            Avatar.transform.Rotate(_rotationVector, -_rotationVelocity, Space.World);
        }
    
        public void OnEndDrag(PointerEventData eventData) {
            _dragged = false;
        }
    
        private void Update() {
            if (Avatar != null) {
                    if (!_dragged && !Mathf.Approximately(_rotationVelocity, 0)) {
                    float deltaVelocity = Mathf.Min(
                        Mathf.Sign(_rotationVelocity) * Time.deltaTime * RotationDamping,
                        Mathf.Sign(_rotationVelocity) * _rotationVelocity
                    );
                    _rotationVelocity -= deltaVelocity;
                    Avatar.transform.Rotate(_rotationVector, -_rotationVelocity, Space.World);
                }

                if (!_dragged && AutoRotate) {
                    Avatar.transform.Rotate(_rotationVector, AutoRotateSpeed * Time.deltaTime, Space.World);
                }
            }
        }

        public void ToggleAutorotate() {
            AutoRotate = !AutoRotate;
        }
    }
}
