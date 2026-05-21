using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

public class ControllerScene2 : MonoBehaviour
{
    public static ControllerScene2 Instancia;

    [Header("UI del HUD (Requisito 5.4)")]
    public TextMeshProUGUI textoMecanismos;
    public TextMeshProUGUI textoIntentosFallidos;
    public TextMeshProUGUI textoTiempo;
    public Slider barraProgresoVisual;

    [Header("EVENTO 1 - Puerta de Piedra")]
    public Transform puertaMetalica;
    public AudioSource audioPuerta; // Sonido de goznes

    [Header("EVENTO 2 - Plataforma Elevadora")]
    public Transform plataformaElevadora;

    [Header("EVENTO 3 - Iluminación Mágica")]
    public Light[] lucesAmbiente; // Luces para hacer Fade-In

    [Header("EVENTO 4 - Mecanismo Giratorio")]
    public Transform manivelaMadera; // Objeto cosmético engranaje

    [Header("EVENTO 5 - Portal Final")]
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
        if (portalFinal != null) portalFinal.SetActive(false);

        // Apagar luces inicialmente para el Evento 3
        foreach (Light l in lucesAmbiente)
        {
            if (l != null) l.intensity = 0f;
        }

        ActualizarHUDVisual();
    }

    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        ActualizarTextoTiempo();

        if (girarManivelaCosmetico && manivelaMadera != null)
        {
            // Efecto visual de rotación continua del engranaje
            manivelaMadera.Rotate(Vector3.up * 120f * Time.deltaTime);
        }
    }

    public int ObtenerPasoActual()
    {
        return pasoActual;
    }

    public void RecibirObjetoEnAltar(int numeroAltar, string nombreObjeto)
    {
        // Si mandan un código de error (-1) desde el PlayerCarrySystem
        if (numeroAltar == -1)
        {
            failedAttempts++;
            ActualizarHUDVisual();
            return;
        }

        // Validación estricta del orden secuencial de los 5 Altares (0 al 4)
        if (numeroAltar == pasoActual)
        {
            pasoActual++;
            ActualizarHUDVisual();
            DispararEventoSecuencial(pasoActual);
        }
        else
        {
            failedAttempts++;
            ActualizarHUDVisual();

            // Buscar el altar actual que disparó el error para forzar su feedback visual rojo
            AltarTrigger[] altares = Object.FindObjectsByType<AltarTrigger>(FindObjectsSortMode.None);
            foreach (AltarTrigger alt in altares)
            {
                if (alt.numeroDeEsteAltar == numeroAltar)
                {
                    alt.ForzarFeedbackRojo();
                    break;
                }
            }
        }
    }

    void DispararEventoSecuencial(int paso)
    {
        switch (paso)
        {
            case 1: // EVENTO 1: Puerta de Piedra
                if (audioPuerta != null) audioPuerta.Play();
                if (puertaMetalica != null) StartCoroutine(MoverObjeto(puertaMetalica, puertaMetalica.position + Vector3.down * 4f, 2f));
                break;

            case 2:
                // En lugar de moverse a un destino fijo permanente, ejecuta la secuencia de ida y vuelta
                if (plataformaElevadora != null)
                {
                    StartCoroutine(MoverObjetoIdaYVuelta(plataformaElevadora, plataformaElevadora.position + Vector3.up * 5f, 2f, 3f));
                }
                break;

            case 3:
                // EVENTO 3: Activación inmediata de la iluminación mágica
                EncenderLucesAmbiente();
                break;

            case 4: // EVENTO 4: Mecanismo Giratorio (Giro cosmético inmersivo)
                girarManivelaCosmetico = true;
                StartCoroutine(DetenerGiroMecanicoDespuesDeTiempo(3f));
                break;

            case 5: // EVENTO 5: Portal Final
                if (portalFinal != null) portalFinal.SetActive(true);
                FinalizarEscenaYGuardarJSON();
                break;
        }
    }

    IEnumerator MoverObjetoIdaYVuelta(Transform obj, Vector3 destino, float duracionMovimiento, float tiempoEsperaArriba)
    {
        float t = 0;
        Vector3 origenFijo = obj.position;

        // --- FASE 1: SUBIDA ---
        while (t < duracionMovimiento)
        {
            obj.position = Vector3.Lerp(origenFijo, destino, t / duracionMovimiento);
            t += Time.deltaTime;
            yield return null;
        }
        obj.position = destino;

        // --- FASE 2: ESPERA EN EL PUNTO ALTO ---
        yield return new WaitForSeconds(tiempoEsperaArriba);

        // --- FASE 3: BAJADA ---
        t = 0;
        while (t < duracionMovimiento)
        {
            obj.position = Vector3.Lerp(destino, origenFijo, t / duracionMovimiento);
            t += Time.deltaTime;
            yield return null;
        }
        obj.position = origenFijo;
    }

    void EncenderLucesAmbiente()
    {
        foreach (Light l in lucesAmbiente)
        {
            if (l != null)
            {
                // 1. Activamos el componente para que emita energía
                l.enabled = true;

                // 2. FORZAMOS un valor alto de intensidad para romper el cero del inicio
                l.intensity = 5.0f;

                // 3. Si es un Point Light (foco esférico), aumentamos su rango para que cubra la habitación
                l.range = 20.0f;
            }
        }
        Debug.Log("¡El código ha activado las luces con intensidad 5.0f de forma estricta!");
    }

    IEnumerator MoverObjeto(Transform obj, Vector3 destino, float duracion)
    {
        float t = 0;
        Vector3 origen = obj.position;
        while (t < duracion)
        {
            obj.position = Vector3.Lerp(origen, destino, t / duracion);
            t += Time.deltaTime;
            yield return null;
        }
        obj.position = destino;
    }

    IEnumerator FadeInLuces(float duracion)
    {
        float t = 0;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float fraccion = t / duracion;
            foreach (Light l in lucesAmbiente)
            {
                if (l != null) l.intensity = Mathf.Lerp(0f, 2.5f, fraccion); // Ajusta 2.5f según la fuerza deseada
            }
            yield return null;
        }
    }

    IEnumerator DetenerGiroMecanicoDespuesDeTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        // Dejamos que siga girando o se detenga según prefieras. El requisito pide que gire.
    }

    void ActualizarHUDVisual()
    {
        if (textoMecanismos != null) textoMecanismos.text = "Mecanismos activados: " + pasoActual + "/5";
        if (textoIntentosFallidos != null) textoIntentosFallidos.text = "Intentos fallidos: " + failedAttempts;

        if (barraProgresoVisual != null)
        {
            barraProgresoVisual.minValue = 0;
            barraProgresoVisual.maxValue = 5;
            barraProgresoVisual.value = pasoActual;
        }
    }

    void ActualizarTextoTiempo()
    {
        if (textoTiempo != null)
        {
            int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60f);
            int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60f);
            textoTiempo.text = string.Format("Tiempo: {0}:{1:00}", minutos, segundos);
        }
    }

    public void FinalizarEscenaYGuardarJSON()
    {
        string ruta = Path.Combine(Application.persistentDataPath, "GameData.json");
        string jsonOutput = "{\n" +
            "  \"playerName\": \"Arqueologo\",\n" +
            "  \"lastScenePlayed\": \"Camara de Activacion\",\n" +
            "  \"scene2\": {\n" +
            $"    \"objectsPlacedCorrectly\": {pasoActual},\n" +
            "    \"totalObjectsToPlace\": 5,\n" +
            $"    \"failedAttempts\": {failedAttempts},\n" +
            "    \"portalUnlocked\": true,\n" +
            $"    \"timeElapsedSeconds\": {tiempoTranscurrido}\n" +
            "  }\n" +
            "}";
        File.WriteAllText(ruta, jsonOutput);
        Debug.Log("JSON de la Escena 2 guardado con éxito.");
    }
}