using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Estado del Juego")]
    public int artefactosNecesarios = 10;
    public int artefactosActuales = 0;
    public bool puedeGanar = false;

    [Header("Referencias de la Escena")]
    public GameObject portalDeSalida;

    [Header("Referencias del Taller 4.4 (HUD UI)")]
    public TextMeshProUGUI textoContador;
    public TextMeshProUGUI textoEstado;
    public TextMeshProUGUI textoTemporal;

    [Header("Sonidos de Recoleccion")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoRecoleccion; // * Sonido de recolección proporciona feedback auditivo

    private float tiempoTranscurrido = 0f;
    private string rutaArchivoJSON;
    private List<ObjetoRecolectable> todosLosObjetos = new List<ObjetoRecolectable>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            rutaArchivoJSON = Path.Combine(Application.persistentDataPath, "GameData.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (textoTemporal != null)
            textoTemporal.gameObject.SetActive(false);

        ActualizarInterfazHUD();
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
    }

    public void RegistrarObjeto(ObjetoRecolectable obj)
    {
        if (!todosLosObjetos.Contains(obj))
            todosLosObjetos.Add(obj);
    }

    public void SumarArtefacto()
    {
        artefactosActuales++;

        // * Sonido de recolección proporciona feedback auditivo
        if (fuenteAudio != null && sonidoRecoleccion != null)
        {
            fuenteAudio.PlayOneShot(sonidoRecoleccion);
        }

        StartCoroutine(MostrarMensajeTemporal());
        ActualizarInterfazHUD();

        if (artefactosActuales >= artefactosNecesarios)
            ActivarVictoria();
    }

    void ActualizarInterfazHUD()
    {
        if (textoContador != null)
        {
            textoContador.text = "Artefactos: " + artefactosActuales + "/" + artefactosNecesarios;
        }

        if (textoEstado != null)
        {
            int faltantes = artefactosNecesarios - artefactosActuales;
            if (faltantes > 0)
            {
                textoEstado.text = "Recolecta " + faltantes + " artefactos más para continuar";
            }
            else
            {
                textoEstado.text = "¡Portal activo! Ve hacia la salida.";
            }
        }
    }

    IEnumerator MostrarMensajeTemporal()
    {
        if (textoTemporal != null)
        {
            textoTemporal.text = "+1 Artefacto recolectado";
            textoTemporal.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            textoTemporal.gameObject.SetActive(false);
        }
    }

    void ActivarVictoria()
    {
        puedeGanar = true;
        if (portalDeSalida != null)
        {
            PortalSalida portal = portalDeSalida.GetComponent<PortalSalida>();
            if (portal != null)
                portal.ActivarPortal(); // Detona la apertura y los sonidos del portal
        }
    }

    public void GuardarProgresoJSON(int vidasPorDefecto = 3)
    {
        int vidasActualesDelJugador = vidasPorDefecto;

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            SaludPersonaje scriptSalud = jugador.GetComponent<SaludPersonaje>();
            if (scriptSalud != null)
            {
                vidasActualesDelJugador = scriptSalud.VidasParaJSON;
            }
        }

        DatosGuardado datos = new DatosGuardado();
        datos.artefactosRecolectados = artefactosActuales;
        datos.vidasRestantes = vidasActualesDelJugador;
        datos.completionTime = tiempoTranscurrido;
        datos.nombreEscenaActual = SceneManager.GetActiveScene().name;

        string textoJSON = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaArchivoJSON, textoJSON);
    }

    public void ResetearEstadoCompleto()
    {
        artefactosActuales = 0;
        tiempoTranscurrido = 0f;
        puedeGanar = false;
        ActualizarInterfazHUD();

        foreach (ObjetoRecolectable obj in todosLosObjetos)
        {
            if (obj != null) obj.Resetear();
        }
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public class DatosGuardado
{
    public int artefactosRecolectados;
    public int vidasRestantes;
    public float completionTime;
    public string nombreEscenaActual;
}