using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSalida : MonoBehaviour
{
    [Header("Configuracion")]
    public string nombreEscenaSiguiente = "Nivel2";

    [Header("Referencias Visuales")]
    public GameObject[] objetosVisuales;
    public Collider portalCollider;

    void Start()
    {
        OcultarPortal();
    }

    // SOLUCIÓN DEFINITIVA A LOS ERRORES 2 y 3: La función que invoca el GameManager
    public void ActivarPortal()
    {
        MostrarPortal();
        Debug.Log("Portal activado desde el GameManager.");
    }

    void OcultarPortal()
    {
        foreach (GameObject visual in objetosVisuales)
            if (visual != null) visual.SetActive(false);

        if (portalCollider != null) portalCollider.enabled = false;
    }

    void MostrarPortal()
    {
        foreach (GameObject visual in objetosVisuales)
            if (visual != null) visual.SetActive(true);

        if (portalCollider != null) portalCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null && GameManager.instance.puedeGanar)
            {
                if (string.IsNullOrEmpty(nombreEscenaSiguiente))
                {
                    Debug.LogError("PortalSalida: nombre de escena vacio.");
                    return;
                }

                // Guardamos antes de saltar
                GameManager.instance.GuardarProgresoJSON(3);
                SceneManager.LoadScene(nombreEscenaSiguiente);
            }
        }
    }
}