using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO; // REQUISITO: Requerido para operaciones de archivos (Lectura y Escritura de JSON)

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Estado del Juego")]
    public int artefactosNecesarios = 6;
    public int artefactosActuales = 0;
    public bool puedeGanar = false;

    [Header("Referencias de la Escena")]
    public GameObject portalDeSalida;

    // NUEVAS VARIABLES PARA EL REQUISITO DEL TALLER
    private float tiempoTranscurrido = 0f;
    private string rutaArchivoJSON;

    // Lista interna de todos los objetos recolectables de la escena
    private List<ObjetoRecolectable> todosLosObjetos = new List<ObjetoRecolectable>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Definimos la ruta automatica en el sistema para guardar el JSON
            rutaArchivoJSON = Path.Combine(Application.persistentDataPath, "GameData.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // REQUISITO: Medir el tiempo transcurrido en la escena
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
        Debug.Log("Artefactos en GameManager: " + artefactosActuales);
        if (artefactosActuales >= artefactosNecesarios)
            ActivarVictoria();
    }

    void ActivarVictoria()
    {
        puedeGanar = true;
        Debug.Log("META ALCANZADA El portal deberia abrirse.");
        if (portalDeSalida != null)
        {
            // Buscamos el componente del portal para avisarle que se active visualmente
            PortalSalida portal = portalDeSalida.GetComponent<PortalSalida>();
            if (portal != null)
                portal.ActivarPortal();
            else
                Debug.LogError("El portal no tiene el script PortalSalida.");
        }
    }

    /// <summary>
    /// REQUISITO OBLIGATORIO DEL TALLER: Guarda los datos en formato JSON en el disco duro.
    /// </summary>
    public void GuardarProgresoJSON(int vidasRestantes)
    {
        // 1. Creamos el objeto contenedor de datos utilizando la clase interna serializable
        DatosGuardado datos = new DatosGuardado();
        datos.artefactosRecolectados = artefactosActuales;
        datos.vidasRestantes = vidasRestantes;
        datos.completionTime = tiempoTranscurrido;
        datos.nombreEscenaActual = SceneManager.GetActiveScene().name;

        // 2. Convertimos el objeto de C# a una cadena de texto estructurada en JSON
        string textoJSON = JsonUtility.ToJson(datos, true);

        // 3. Escribimos el archivo de texto plano en la ruta del sistema
        File.WriteAllText(rutaArchivoJSON, textoJSON);

        Debug.Log("Datos serializados y guardados en JSON exitosamente");
        Debug.Log("Ruta: " + rutaArchivoJSON);
        Debug.Log("Contenido: " + textoJSON);
    }

    public void ResetearEstadoCompleto()
    {
        // Reinicia contador de artefactos y el reloj
        artefactosActuales = 0;
        tiempoTranscurrido = 0f;
        puedeGanar = false;

        // Reactiva y reposiciona todos los objetos recolectables
        foreach (ObjetoRecolectable obj in todosLosObjetos)
        {
            if (obj != null)
                obj.Resetear();
        }

        Debug.Log("Estado del mundo reseteado. Objetos devueltos al origen y reloj en cero.");
    }

    public void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

/// <summary>
/// Clase espejo estructurada (POO) contenedora de las variables a serializar.
/// </summary>
[System.Serializable]
public class DatosGuardado
{
    public int artefactosRecolectados;
    public int vidasRestantes;
    public float completionTime;
    public string nombreEscenaActual;
}