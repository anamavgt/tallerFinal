using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("Referencias de la Interfaz")]
    [SerializeField] private GameObject panelGameOver;

    private void Start()
    {
        // Nos aseguramos de que el panel inicie oculto al jugar
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }
    }

    /// <summary>
    /// Activa el panel de Game Over, congela el juego y muestra el cursor.
    /// </summary>
    public void ActivarMenuGameOver()
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);

            // Pausamos el tiempo para que nada se mueva de fondo
            Time.timeScale = 0f;

            // Mostramos y desbloqueamos el mouse para poder hacer clic
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Limpia el estado del juego, revive al jugador y oculta el menú restableciendo el flujo.
    /// </summary>
    public void ReintentarEscena()
    {
        Debug.Log("Reiniciando estado del jugador y del nivel sin recargar escena...");

        // 1. Buscamos el script de salud en la escena para revivir al jugador
        SaludPersonaje salud = Object.FindFirstObjectByType<SaludPersonaje>();
        if (salud != null)
        {
            // Vaciamos el inventario visual y de datos
            InventarioJugador inventario = Object.FindFirstObjectByType<InventarioJugador>();
            if (inventario != null)
            {
                inventario.VaciarInventario();
            }

            // Reseteamos el contador de artefactos del GameManager si está presente
            if (GameManager.instance != null)
            {
                GameManager.instance.ResetearEstadoCompleto();
            }

            // Le devolvemos las 3 vidas, actualizamos corazones y lo mandamos al inicio
            salud.RestaurarPersonajeTotalmente();
        }

        // 2. Ocultamos el panel de derrota
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        // 3. Restauramos el flujo del tiempo para que todo vuelva a la normalidad
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Método para el botón de salir. En un build cierra el juego.
    /// </summary>
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}