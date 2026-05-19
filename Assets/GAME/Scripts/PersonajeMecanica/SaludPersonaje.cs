using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // Requerido para la corrutina del flash rojo

public class SaludPersonaje : MonoBehaviour
{
    [Header("Referencias del Jugador")]
    public GameObject objetoPlayer;
    private CharacterController controller;

    [Header("Configuracion de Vidas UI")]
    public int vidasMaximas = 3;
    [SerializeField] private int vidasActuales;
    public Image[] corazonesUI;

    [Header("Configuracion de Danio")]
    public float tiempoInvulnerabilidad = 1.5f;
    private float timerInvulnerabilidad;
    private bool esInvulnerable;

    // --- REQUISITOS DEL TALLER (FEEDBACK VISUAL Y AUDIBLE) ---
    [Header("Feedback de Danio")]
    public AudioSource audioSource;
    public AudioClip sonidoDanio;
    public Image pantallaRojaUI;
    public float duracionFlashRojo = 0.25f; // Corregido con 'f' para float

    // --- CONEXIÓN CON EL MENÚ DE GAME OVER ---
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
        Debug.Log("Juego Iniciado. Vidas actuales: " + vidasActuales);
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
        if (esInvulnerable) return;

        vidasActuales -= cantidad;
        vidasActuales = Mathf.Clamp(vidasActuales, 0, vidasMaximas);
        ActualizarUI();

        Debug.Log("¡Danio recibido! Vida restante: " + vidasActuales);

        if (audioSource != null && sonidoDanio != null)
        {
            audioSource.PlayOneShot(sonidoDanio);
        }

        if (pantallaRojaUI != null)
        {
            StartCoroutine(EfectoPantallaRoja());
        }

        if (vidasActuales <= 0)
        {
            Morir();
        }
        else
        {
            esInvulnerable = true;
            timerInvulnerabilidad = tiempoInvulnerabilidad;
            EjecutarRespawn();
        }
    }

    private IEnumerator EfectoPantallaRoja()
    {
        pantallaRojaUI.gameObject.SetActive(true);
        pantallaRojaUI.color = new Color(1f, 0f, 0f, 0.35f);

        yield return new WaitForSeconds(duracionFlashRojo);

        pantallaRojaUI.gameObject.SetActive(false);
    }

    void EjecutarRespawn()
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

    void ActualizarUI()
    {
        for (int i = 0; i < corazonesUI.Length; i++)
        {
            if (corazonesUI[i] != null)
                corazonesUI[i].enabled = (i < vidasActuales);
        }
    }

    void Morir()
    {
        Debug.Log("Muerte definitiva. Deteniendo juego y abriendo menu de Game Over.");

        if (menuGameOverComponente != null)
        {
            vidasActuales = vidasMaximas;
            ActualizarUI();

            InventarioJugador inventario = FindFirstObjectByType<InventarioJugador>();
            if (inventario != null)
                inventario.VaciarInventario();

            if (GameManager.instance != null)
                GameManager.instance.ResetearEstadoCompleto();

            EjecutarRespawn();

            menuGameOverComponente.ActivarMenuGameOver();
        }
        else
        {
            Debug.LogWarning("No se asigno el MenuGameOver en el inspector. Reiniciando escena por defecto.");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}