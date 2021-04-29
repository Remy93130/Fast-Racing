namespace FastRacing
{
    using UnityEngine;

    public class CameraController : SimpleGameStateObserver
    {
        [SerializeField] Transform m_Target;
        [SerializeField] private float translateSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Vector3 offset;
        Transform m_Transform;
        Vector3 m_InitPosition;

        void ResetCamera()
        {
            m_Transform.position = m_InitPosition;
        }

        protected override void Awake()
        {
            base.Awake();
            m_Transform = transform;
            m_InitPosition = m_Transform.position;
        }

        void FixedUpdate()
        {
            if (!GameManager.Instance.IsPlaying) return;

            HandleTranslation();
            HandleRotation();
           

        }

        private void HandleTranslation()
        {
            var targetPosition = m_Target.TransformPoint(offset);
            transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
        }

        private void HandleRotation()
        {
            var direction = m_Target.position - transform.position;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

        }

        protected override void GameMenu(GameMenuEvent e)
        {
            ResetCamera();
        }
    }
}
