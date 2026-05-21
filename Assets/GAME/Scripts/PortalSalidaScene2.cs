using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class PortalSalidaScene2 : MonoBehaviour
{
    [Header("Configuración de Destino (Requisito 6)")]
    [Tooltip("Nombre de la pantalla de victoria o créditos.")]
    public string nombreEscenaSiguiente = "Victoria";

    [Header("Referencias Visuales y Efectos")]
    [Tooltip("El objeto del portal (esfera luminosa que parpadea).")]
    public GameObject cuerpoVisualPortal;
    public ParticleSystem efectosParticulas;
    public Collider portalCollider;

    [Header("Audios de Transición (Sección 6)")]
    public AudioSource fuenteAudioViaje;
    public AudioClip sonidoWhoosh;
    public AudioClip sonidoEpico;

    private bool jugadorEnRango = false;
    private bool yaSeUso = false;
    private Transform transformJugador;

    void Start()
    {
        if (portalCollider == null) portalCollider = GetComponent<Collider>();

        // El portal inicia invisible u oculto hasta que el ControllerScene2 lo active (Evento 5)
        if (portalCollider != null) portalCollider.isTrigger = true;
    }

    void Update()
    {
        if (!jugadorEnRango || yaSeUso) return;

        // REQUISITO SECCIÓN 6: Medición estricta de distancia en tiempo real (menor a 2 unidades)
        if (transformJugador != null)
        {
            float distanciaReal = Vector3.Distance(transform.position, transformJugador.position);

            // Si está en rango y presiona la tecla "E"
            if (distanciaReal <= 2.0f && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                yaSeUso = true;
                StartCoroutine(SecuenciaTeletransporte());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectar al jugador (por Tag o nombre del conejo)
        if (other.CompareTag("Player") || other.name.Contains("RabbitCharacter"))
        {
            transformJugador = other.transform;
            jugadorEnRango = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("RabbitCharacter"))
        {
            jugadorEnRango = false;
            transformJugador = null;
        }
    }

    IEnumerator SecuenciaTeletransporte()
    {
        // 1. Guardar y registrar los datos finales en el JSON automáticamente antes de salir
        if (ControllerScene2.Instancia != null)
        {
            ControllerScene2.Instancia.FinalizarEscenaYGuardarJSON();
        }

        // 2. Feedback auditivo: Efecto Whoosh y sonido Épico en simultáneo
        if (fuenteAudioViaje != null)
        {
            if (sonidoWhoosh != null) fuenteAudioViaje.PlayOneShot(sonidoWhoosh);
            if (sonidoEpico != null) fuenteAudioViaje.PlayOneShot(sonidoEpico);
        }

        // 3. Animación de Fade-Out de la cámara de 1.5 segundos (Espera en corrutina)
        float tiempo = 0f;
        while (tiempo < 1.5f)
        {
            tiempo += Time.deltaTime;
            // Aquí puedes añadir una llamada a un elemento UI de fundido a negro si lo tienes
            yield return null;
        }

        // 4. Cambio de escena final mediante SceneManager
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}