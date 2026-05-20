using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaludPersonaje : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public GameObject objetoPlayer;
    private CharacterController controller;

    [Header("Configuracion de Vidas UI")]
    public int vidasMaximas = 3;
    [SerializeField] private int vidasActuales;
    public Image[] corazonesUI;

    // Propiedad para que el GameManager guarde las vidas en el JSON
    public int VidasParaJSON => vidasActuales;

    [Header("Configuracion de Danio")]
    public float tiempoInvulnerabilidad = 1.5f;
    private float timerInvulnerabilidad;
    private bool esInvulnerable;

    [Header("Feedback Auditivo del Taller")]
    public AudioSource audioSource;
    public AudioClip sonidoDanio;       // * Sonido de daño (al perder una vida)
    public AudioClip sonidoError;       // * Sonido de error (cuando intentan dañarte pero eres invulnerable)

    [Header("Feedback Visual del Taller")]
    public Image pantallaRojaUI;        // * Aura roja (Flash visual de peligro)
    public float duracionFlashRojo = 0.25f;

    [Header("Control de Menus de UI")]
    public GameOverMenu menuGameOverComponente;

    private Vector3 posicionInicial;

    void Awake()
    {
        vidasActuales = vidasMaximas;
    }

    void Start()
    {
        if (objetoPlayer != null)
        {
            posicionInicial = objetoPlayer.transform.position;
            controller = objetoPlayer.GetComponent<CharacterController>();
        }

        if (pantallaRojaUI != null)
        {
            pantallaRojaUI.gameObject.SetActive(false);
        }

        ActualizarUI();
    }

    void Update()
    {
        if (esInvulnerable)
        {
            timerInvulnerabilidad -= Time.deltaTime;
            if (timerInvulnerabilidad <= 0) esInvulnerable = false;
        }
    }

    public void RecibirDanio(int cantidad)
    {
        // Si el jugador es invulnerable, no pierde vida, pero genera feedback de error
        if (esInvulnerable)
        {
            if (audioSource != null && sonidoError != null)
            {
                audioSource.PlayOneShot(sonidoError); // * Sonido de error
            }
            return;
        }

        if (vidasActuales <= 0) return;

        vidasActuales -= cantidad;
        vidasActuales = Mathf.Clamp(vidasActuales, 0, vidasMaximas);
        ActualizarUI();

        // * Sonido de daño (al perder una vida)
        if (audioSource != null && sonidoDanio != null)
        {
            audioSource.PlayOneShot(sonidoDanio);
        }

        if (vidasActuales <= 0)
        {
            Morir();
        }
        else
        {
            if (pantallaRojaUI != null)
            {
                StartCoroutine(EfectoPantallaRoja()); // * Aura roja
            }

            esInvulnerable = true;
            timerInvulnerabilidad = tiempoInvulnerabilidad;
            EjecutarRespawn();
        }
    }

    private IEnumerator EfectoPantallaRoja()
    {
        pantallaRojaUI.gameObject.SetActive(true);
        pantallaRojaUI.color = new Color(1f, 0f, 0f, 0.35f); // Filtro rojo translúcido
        yield return new WaitForSeconds(duracionFlashRojo);
        pantallaRojaUI.gameObject.SetActive(false);
    }

    public void EjecutarRespawn()
    {
        if (objetoPlayer == null) return;
        if (controller != null) controller.enabled = false;

        RaycastHit hit;
        Vector3 origenRayo = new Vector3(posicionInicial.x, posicionInicial.y + 20f, posicionInicial.z);

        if (Physics.Raycast(origenRayo, Vector3.down, out hit, 50f))
        {
            objetoPlayer.transform.position = hit.point + new Vector3(0, 1.0f, 0);
        }
        else
        {
            objetoPlayer.transform.position = posicionInicial;
        }

        if (controller != null) controller.enabled = true;
    }

    public void ActualizarUI()
    {
        for (int i = 0; i < corazonesUI.Length; i++)
        {
            if (corazonesUI[i] != null)
                corazonesUI[i].enabled = (i < vidasActuales);
        }
    }

    public void RestaurarPersonajeTotalmente()
    {
        vidasActuales = vidasMaximas;
        ActualizarUI();
        EjecutarRespawn();
        esInvulnerable = false;
    }

    void Morir()
    {
        if (pantallaRojaUI != null)
        {
            StopAllCoroutines();
            pantallaRojaUI.gameObject.SetActive(false);
        }

        if (menuGameOverComponente != null)
        {
            menuGameOverComponente.ActivarMenuGameOver();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}