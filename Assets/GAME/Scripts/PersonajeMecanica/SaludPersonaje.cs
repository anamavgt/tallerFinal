using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        Debug.Log("¡Dañado! Vida restante: " + vidasActuales);

        vidasActuales = Mathf.Clamp(vidasActuales, 0, vidasMaximas);
        ActualizarUI();

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
        Debug.Log("Muerte definitiva. Reseteando estado del juego.");

        // Restaura las vidas
        vidasActuales = vidasMaximas;
        ActualizarUI();

        // Vacía el inventario
        InventarioJugador inventario = FindFirstObjectByType<InventarioJugador>();
        if (inventario != null)
            inventario.VaciarInventario();

        // Resetea objetos del mundo y contadores
        if (GameManager.instance != null)
            GameManager.instance.ResetearEstadoCompleto();

        // Respawnea al jugador en su posición inicial
        EjecutarRespawn();
    }
}