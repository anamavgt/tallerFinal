using UnityEngine;
using UnityEngine.InputSystem;

namespace StylizedCharacterPackDemo
{
    public class BasicCameraFollowScene2 : MonoBehaviour
    {
        public InputActionAsset InputAsset;
        public Transform Followed;

        [Header("Configuración de Vista en Tercera Persona")]
        public float DistanciaAtras = 2.2f;

        public Vector3 OffsetPersonaje = new Vector3(0f, 1.45f, 0f);

        [Header("Velocidad y Sensibilidad")]
        public float RotateSpeed = 100.0f;
        public float SensibilidadMouse = 0.05f;

        [Header("Límites de Ángulo Vertical")]
        public float AnguloMinimo = -20.0f;
        public float AnguloMaximo = 45.0f;

        private InputAction m_LookAction;
        private float m_RotacionX = 15.0f; 
        private float m_RotacionY = 180.0f;

        void Start()
        {
            transform.SetParent(null);
            transform.localScale = Vector3.one;

            Camera cam = GetComponent<Camera>();
            if (cam != null) cam.fieldOfView = 60f;

            m_LookAction = InputAsset.FindAction("Look");
            m_LookAction.Enable();

            Vector3 rotacionActual = transform.localEulerAngles;
            m_RotacionX = 15.0f;
            m_RotacionY = rotacionActual.y;
        }

        void LateUpdate()
        {
            if (Followed == null) return;

            Vector2 lookInput = m_LookAction.ReadValue<Vector2>();

            m_RotacionY += lookInput.x * RotateSpeed * SensibilidadMouse * Time.deltaTime;
            m_RotacionX -= lookInput.y * RotateSpeed * SensibilidadMouse * Time.deltaTime;

            m_RotacionX = Mathf.Clamp(m_RotacionX, AnguloMinimo, AnguloMaximo);

            Quaternion rotacionObjetivo = Quaternion.Euler(m_RotacionX, m_RotacionY, 0.0f);

            Vector3 posicionCentro = Followed.position + OffsetPersonaje;
            Vector3 posicionFinal = posicionCentro - (rotacionObjetivo * Vector3.forward * DistanciaAtras);

            transform.rotation = rotacionObjetivo;
            transform.position = posicionFinal;
        }
    }
}