using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class ControllerScene2 : MonoBehaviour
{
    public static ControllerScene2 Instancia;

    [Header("UI del HUD")]
    public Text textoMecanismos;
    public Text textoIntentosFallidos;
    public Text textoTiempo;
    public Slider barraProgresoVisual;

    [Header("Elementos Mecanicos de la Escena 2")]
    public Transform puertaMetalica;
    public Transform plataformaElevadora;
    public ParticleSystem particulasCaldero;
    public Transform manivelaMadera;
    public Transform puertaMadera;
    public GameObject portalFinal;

    private int pasoActual = 0;
    private int failedAttempts = 0;
    private float tiempoTranscurrido = 0f;
    private bool girarManivelaCosmetico = false;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (particulasCaldero != null) particulasCaldero.Stop();
        if (portalFinal != null) portalFinal.SetActive(false);

        ActualizarHUDVisual();
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        if (textoTiempo != null)
        {
            textoTiempo.text = "Tiempo: " + tiempoTranscurrido.ToString("F1") + "s";
        }

        if (girarManivelaCosmetico && manivelaMadera != null)
        {
            manivelaMadera.Rotate(Vector3.up * 45f * Time.deltaTime);
        }
    }

    // Funcion receptora adaptada a las variables originales de tus altares
    public void RecibirObjetoEnAltar(int numeroDeEsteAltar, string nombreDelObjetoClave)
    {
        // Caso de error enviado por el PlayerCarrySystem (-1 o "Incorrecto")
        if (numeroDeEsteAltar == -1 || nombreDelObjetoClave == "Incorrecto")
        {
            failedAttempts++;
            ActualizarHUDVisual();
            return;
        }

        // Validacion estricta del orden secuencial (0, luego 1, luego 2...)
        if (numeroDeEsteAltar != pasoActual)
        {
            Debug.Log("Orden incorrecto. Debes interactuar con el altar: " + pasoActual);
            failedAttempts++;
            ActualizarHUDVisual();
            return;
        }

        // Si el orden es correcto, avanzamos el mecanismo
        pasoActual++;
        ActualizarHUDVisual();
        EjecutarMecanismoPorPaso(pasoActual);
    }

    void ActualizarHUDVisual()
    {
        if (textoMecanismos != null) textoMecanismos.text = "Mecanismos: " + pasoActual + " / 4";
        if (textoIntentosFallidos != null) textoIntentosFallidos.text = "Fallos: " + failedAttempts;
        if (barraProgresoVisual != null) barraProgresoVisual.value = pasoActual;
    }

    void EjecutarMecanismoPorPaso(int paso)
    {
        switch (paso)
        {
            case 1:
                if (puertaMetalica != null) StartCoroutine(MoverObjeto(puertaMetalica, puertaMetalica.position + Vector3.up * 4f, 2f));
                break;
            case 2:
                if (plataformaElevadora != null) StartCoroutine(MoverObjeto(plataformaElevadora, plataformaElevadora.position + Vector3.up * 3f, 2.5f));
                break;
            case 3:
                if (particulasCaldero != null) particulasCaldero.Play();
                break;
            case 4:
                girarManivelaCosmetico = true;
                if (puertaMadera != null) StartCoroutine(MoverObjeto(puertaMadera, puertaMadera.position + Vector3.down * 4f, 2f));
                if (portalFinal != null) portalFinal.SetActive(true);
                FinalizarEscenaYGuardarJSON();
                break;
        }
    }

    IEnumerator MoverObjeto(Transform obj, Vector3 destino, float duracion)
    {
        float b = 0;
        Vector3 origen = obj.position;
        while (b < duracion)
        {
            obj.position = Vector3.Lerp(origen, destino, b / duracion);
            b += Time.deltaTime;
            yield return null;
        }
        obj.position = destino;
    }

    public void FinalizarEscenaYGuardarJSON()
    {
        string ruta = Path.Combine(Application.persistentDataPath, "GameData.json");
        string jsonOutput = "{\n" +
            "  \"playerName\": \"Arqueologo\",\n" +
            "  \"lastScenePlayed\": \"Camara de Activacion\",\n" +
            "  \"scene2\": {\n" +
            $"    \"objectsPlacedCorrectly\": {pasoActual},\n" +
            "    \"totalObjectsToPlace\": 4,\n" +
            $"    \"failedAttempts\": {failedAttempts},\n" +
            "    \"portalUnlocked\": true,\n" +
            $"    \"timeSpent\": {tiempoTranscurrido}\n" +
            "  }\n" +
            "}";

        File.WriteAllText(ruta, jsonOutput);
        Debug.Log("Progreso final guardado de la Escena 2 en JSON:\n" + jsonOutput);
    }
}