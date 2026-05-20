using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalSalida : MonoBehaviour
{
    [Header("Configuracion de Destino")]
    public string nombreEscenaSiguiente = "Nivel2";

    [Header("Referencias Visuales y Efectos")]
    public GameObject[] objetosVisuales;
    public ParticleSystem efectosParticulas;
    public Collider portalCollider;

    [Header("Audios de Apertura (Se ejecutan en simultaneo)")]
    public AudioSource fuenteAudioApertura;
    public AudioClip sonidoListo;              // * Sonido de "listo"
    public AudioClip sonidoActivacion;          // * Sonido de activacion (timbre, campana, sonido mágico)
    public AudioClip sonidoGoznesChirriando;    // * Sonido de goznes chirriando
    public AudioClip sonidoMecanico;           // * Acompañado de sonido mecánico

    [Header("Audios de Transicion (Al entrar al portal)")]
    public AudioSource fuenteAudioViaje;
    public AudioClip sonidoWhoosh;              // * Sonido de teletransportación (whoosh)
    public AudioClip sonidoEpico;               // * Sonido épico

    private bool yaSeUso = false;

    void Start()
    {
        if (portalCollider == null)
            portalCollider = GetComponent<Collider>();

        OcultarPortal();
    }

    // Lo llama el GameManager automáticamente al llegar a los 10 artefactos
    public void ActivarPortal()
    {
        MostrarPortal();

        // REQUISITO: Reproducir todos los efectos de sonido de la compuerta/portal abriéndose
        if (fuenteAudioApertura != null)
        {
            if (sonidoListo != null) fuenteAudioApertura.PlayOneShot(sonidoListo);
            if (sonidoActivacion != null) fuenteAudioApertura.PlayOneShot(sonidoActivacion);
            if (sonidoGoznesChirriando != null) fuenteAudioApertura.PlayOneShot(sonidoGoznesChirriando);
            if (sonidoMecanico != null) fuenteAudioApertura.PlayOneShot(sonidoMecanico);
        }
    }

    void OcultarPortal()
    {
        foreach (GameObject visual in objetosVisuales)
            if (visual != null) visual.SetActive(false);

        if (efectosParticulas != null) efectosParticulas.Stop();
        if (portalCollider != null) portalCollider.isTrigger = false;
    }

    void MostrarPortal()
    {
        foreach (GameObject visual in objetosVisuales)
            if (visual != null) visual.SetActive(true);

        if (efectosParticulas != null) efectosParticulas.Play();
        if (portalCollider != null) portalCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaSeUso) return;

        if (other.CompareTag("Player") || other.name.Contains("RabbitCharacter"))
        {
            if (GameManager.instance != null && GameManager.instance.puedeGanar)
            {
                yaSeUso = true;
                StartCoroutine(SecuenciaTeletransporte());
            }
        }
    }

    IEnumerator SecuenciaTeletransporte()
    {
        // REQUISITO: Feedback auditivo de viaje exitoso
        if (fuenteAudioViaje != null)
        {
            if (sonidoWhoosh != null) fuenteAudioViaje.PlayOneShot(sonidoWhoosh);
            if (sonidoEpico != null) fuenteAudioViaje.PlayOneShot(sonidoEpico);
        }

        yield return new WaitForSeconds(2f); // Pantalla se oscurece durante 2 segundos

        if (GameManager.instance != null)
        {
            GameManager.instance.GuardarProgresoJSON(3);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaSiguiente);
    }
}