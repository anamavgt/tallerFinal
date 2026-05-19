using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Estado del Juego")]
    public int artefactosNecesarios = 6;
    public int artefactosActuales = 0;
    public bool puedeGanar = false;

    [Header("Referencias")]
    public GameObject portalDeSalida;

    // Lista interna de todos los objetos recolectables de la escena
    private List<ObjetoRecolectable> todosLosObjetos = new List<ObjetoRecolectable>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegistrarObjeto(ObjetoRecolectable obj)
    {
        if (!todosLosObjetos.Contains(obj))
            todosLosObjetos.Add(obj);
    }

    public void SumarArtefacto()
    {
        artefactosActuales++;
        Debug.Log("Artefactos en GameManager: " + artefactosActuales);
        if (artefactosActuales >= artefactosNecesarios)
            ActivarVictoria();
    }

    void ActivarVictoria()
    {
        puedeGanar = true;
        Debug.Log("¡META ALCANZADA! El portal debería abrirse.");
        if (portalDeSalida != null)
        {
            PortalSalida portal = portalDeSalida.GetComponent<PortalSalida>();
            if (portal != null)
                portal.ActivarPortal();
            else
                Debug.LogError("El portal no tiene el script PortalSalida.");
        }
    }

    public void ResetearEstadoCompleto()
    {
        // Reinicia contador de artefactos
        artefactosActuales = 0;
        puedeGanar = false;

        // Reactiva y reposiciona todos los objetos recolectables
        foreach (ObjetoRecolectable obj in todosLosObjetos)
        {
            if (obj != null)
                obj.Resetear();
        }

        Debug.Log("Estado del mundo reseteado. Objetos devueltos al origen.");
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}