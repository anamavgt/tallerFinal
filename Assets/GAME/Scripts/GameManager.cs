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
            portalDeSalida.SetActive(true); // El portal aparece mágicamente
        }
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}