using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSalida : MonoBehaviour
{
    [Header("Configuracion")]
    public string nombreEscenaSiguiente;

    [Header("Referencias Visuales")]
    public GameObject[] objetosVisuales; // Arrastra aquí los hijos visuales del portal
    public Collider portalCollider;       // Arrastra aquí el Collider del portal

    void Start()
    {
        OcultarPortal();
    }

    public void ActivarPortal()
    {
        MostrarPortal();
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
                    Debug.LogError("PortalSalida: nombre de escena vacío.");
                    return;
                }
                SceneManager.LoadScene(nombreEscenaSiguiente);
            }
        }
    }
}