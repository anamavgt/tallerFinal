using UnityEngine;
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Ahora permite cambiar de escena

public class MenuConInstrucciones : MonoBehaviour
{
    [Header("Paneles Principales")]
    public GameObject panelMenuPrincipal;
    public GameObject panelGameOver;

    [Header("Sub-Panel de Instrucciones")]
    public GameObject subPanelInstrucciones;

    [Header("Configuracion de Carga de Escena")]
    // ¡ESTA ES LA CASILLA QUE VA A APARECER EN TU INSPECTOR A LA DERECHA!
    public string nombreEscenaAClargar = "Nivel1";

    void Start()
    {
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (subPanelInstrucciones != null) subPanelInstrucciones.SetActive(false);

        // Congelamos el juego al principio para que lean tranquilos
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Metodo para el boton JUGAR (Ahora si cambia de escena)
    /// </summary>
    public void OpcionJugar()
    {
        // Apagamos todo el menu principal
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);

        // Descongelamos el juego
        Time.timeScale = 1f;
        Debug.Log("Cargando escena desde el inspector: " + nombreEscenaAClargar);

        // Cambiamos a la escena que tu escribas en el cuadro del inspector
        if (!string.IsNullOrEmpty(nombreEscenaAClargar))
        {
            SceneManager.LoadScene(nombreEscenaAClargar);
        }
        else
        {
            Debug.LogError("¡OJO! No escribiste ningun nombre de escena en el Inspector.");
        }
    }

    public void AbrirInstrucciones()
    {
        if (subPanelInstrucciones != null) subPanelInstrucciones.SetActive(true);
    }

    public void CerrarInstrucciones()
    {
        if (subPanelInstrucciones != null) subPanelInstrucciones.SetActive(false);
    }
}