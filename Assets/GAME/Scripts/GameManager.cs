using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Estado del Juego")]
    public int artefactosNecesarios = 6;
    public int artefactosActuales = 0;
    public bool puedeGanar = false;

    [Header("Referencias")]
    public GameObject portalDeSalida; // Arrastra el portal aquí cuando lo tengamos

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SumarArtefacto()
    {
        artefactosActuales++;
        Debug.Log("Artefactos en GameManager: " + artefactosActuales);

        if (artefactosActuales >= artefactosNecesarios)
        {
            ActivarVictoria();
        }
    }

    void ActivarVictoria()
    {
        puedeGanar = true;
        Debug.Log("¡META ALCANZADA! El portal debería abrirse.");

        if (portalDeSalida != null)
        {
            // En vez de SetActive, llama al método del script
            PortalSalida portal = portalDeSalida.GetComponent<PortalSalida>();
            if (portal != null)
                portal.ActivarPortal();
            else
                Debug.LogError("El portal no tiene el script PortalSalida.");
        }
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}