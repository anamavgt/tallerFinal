using UnityEngine;
using UnityEngine.SceneManagement; // Requerido para reiniciar escenas

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
    /// Recarga la escena activa actual restableciendo el flujo del juego.
    /// </summary>
    public void ReintentarEscena()
    {
        // IMPORTANTE: Siempre restaurar el tiempo antes de cambiar o recargar escenas
        Time.timeScale = 1f;

        // Obtenemos el nombre de la escena en la que murió el jugador
        string escenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(escenaActual);
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