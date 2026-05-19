using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StylizedCharacterPackDemo
{
    public class BasicCameraFollowScene2 : MonoBehaviour
    {
        public InputActionAsset InputAsset;
        public Transform Followed;

        [Header("Configuración de Distancia y Altura")]
        public float StartDistance = 2.0f; // <-- FORZADO CORTO
        public Vector3 OffsetPersonaje = new Vector3(0f, 1.3f, 0f);

        [Header("Velocidad y Rotación")]
        public float RotateSpeed = 100.0f;
        public float SensibilidadMouse = 0.07f;

        public float StartVerticalRotation = 15.0f; // <-- ÁNGULO MÁS BAJO NATURAL
        public float StartHorizontalRotation = 180.0f;

        [Header("Límites de Ángulo Vertical")]
        public float AnguloMinimo = -5.0f;
        public float AnguloMaximo = 50.0f;

        private InputAction m_LookAction;
        private Transform m_TargetFollower;
        private float m_HorizontalRotation;
        private float m_VerticalRotation;

        void Start()
        {
            // Forzamos el FOV de la cámara a 50 para garantizar el zoom perfecto
            Camera cam = GetComponent<Camera>();
            if (cam != null) cam.fieldOfView = 50f;

            var targetObject = new GameObject("CameraLookRoot");
            m_TargetFollower = targetObject.transform;

            // Ignoramos el inspector viejo y forzamos los valores correctos de inicio
            m_HorizontalRotation = StartHorizontalRotation;
            m_VerticalRotation = StartVerticalRotation;
            m_TargetFollower.rotation = Quaternion.Euler(m_VerticalRotation, m_HorizontalRotation, 0);

            transform.SetParent(m_TargetFollower, false);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.back * StartDistance;

            m_LookAction = InputAsset.FindAction("Look");
            m_LookAction.Enable();
        }

        void LateUpdate()
        {
            if (Followed == null) return;

            var look = m_LookAction.ReadValue<Vector2>();

            m_HorizontalRotation += look.x * RotateSpeed * SensibilidadMouse * Time.deltaTime;
            m_VerticalRotation -= look.y * RotateSpeed * SensibilidadMouse * Time.deltaTime;

            m_VerticalRotation = Mathf.Clamp(m_VerticalRotation, AnguloMinimo, AnguloMaximo);

            while (m_HorizontalRotation < 0.0f) m_HorizontalRotation += 360.0f;
            while (m_HorizontalRotation > 360.0f) m_HorizontalRotation -= 360.0f;

            Vector3 posicionObjetivo = Followed.position + OffsetPersonaje;
            m_TargetFollower.transform.position = Vector3.Lerp(m_TargetFollower.transform.position, posicionObjetivo, Time.deltaTime * 15f);

            m_TargetFollower.transform.rotation = Quaternion.Euler(m_VerticalRotation, m_HorizontalRotation, 0.0f);
        }
    }
}