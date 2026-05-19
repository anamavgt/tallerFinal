using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class ControllerScene2 : MonoBehaviour
{
    // Instancia estática para que AltarTrigger lo encuentre con el nuevo nombre
    public static ControllerScene2 Instancia;

    [Header("UI del HUD")]
    public Text textoMecanismos;
    public Text textoIntentosFallidos;
    public Text textoTiempo;
    public Slider barraProgresoVisual;

    [Header("Elementos Mecánicos de la Escena 2")]
    public Transform puertaMetalica;
    public Transform plataformaElevadora;

    // CAMBIADO: Ahora es de tipo ParticleSystem para tu caldero
    public ParticleSystem particulasCaldero;

    public Transform manivelaMadera;
    public Transform puertaMadera;
    public GameObject portalFinal;

    // Variables internas de control
    private int pasoActual = 0;
    private int failedAttempts = 0;
    private float tiempoTranscurrido = 0f;
    private bool girarManivelaCosmetico = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // CAMBIADO: Al iniciar, detenemos el efecto para que empiece apagado
        if (particulasCaldero != null) particulasCaldero.Stop();

        if (portalFinal != null) portalFinal.SetActive(false);
        ActualizarHUD();
    }

    // Update is called once per frame
    void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        ActualizarHUD();

        if (girarManivelaCosmetico && manivelaMadera != null)
        {
            manivelaMadera.Rotate(Vector3.up * 60f * Time.deltaTime);
        }
    }

    // El AltarTrigger llamará a esta función usando el nuevo nombre del controlador
    public void RecibirObjetoEnAltar(int numeroAltar, string nombreObjeto)
    {
        if (numeroAltar != pasoActual + 1)
        {
            RegistrarFallo();
            return;
        }

        switch (numeroAltar)
        {
            case 1:
                if (nombreObjeto == "Token") RegistrarPasoExitoso(1); else RegistrarFallo();
                break;
            case 2:
                if (nombreObjeto == "Palanca") RegistrarPasoExitoso(2); else RegistrarFallo();
                break;
            case 3:
                if (nombreObjeto == "Pocion") RegistrarPasoExitoso(3); else RegistrarFallo();
                break;
            case 4:
                if (nombreObjeto == "Manivela") RegistrarPasoExitoso(4); else RegistrarFallo();
                break;
        }
    }

    void RegistrarPasoExitoso(int paso)
    {
        pasoActual = paso;
        if (barraProgresoVisual != null) barraProgresoVisual.value = (float)pasoActual / 4f;

        switch (pasoActual)
        {
            case 1:
                StartCoroutine(MoverObjeto(puertaMetalica, puertaMetalica.position + Vector3.up * 4f, 2f));
                break;
            case 2:
                StartCoroutine(MoverObjeto(plataformaElevadora, plataformaElevadora.position + Vector3.up * 4.5f, 2f));
                break;
            case 3:
                // CAMBIADO: En vez de la corrutina de luz, encendemos las partículas directamente
                if (particulasCaldero != null)
                {
                    particulasCaldero.Play();
                }
                break;
            case 4:
                girarManivelaCosmetico = true;
                StartCoroutine(MoverObjeto(puertaMadera, puertaMadera.position + Vector3.right * 3.5f, 2f));
                if (portalFinal != null) portalFinal.SetActive(true);
                break;
        }
    }

    void RegistrarFallo()
    {
        failedAttempts++;
    }

    void ActualizarHUD()
    {
        if (textoMecanismos != null) textoMecanismos.text = $"Mecanismos: {pasoActual}/4";
        if (textoIntentosFallidos != null) textoIntentosFallidos.text = $"Fallos: {failedAttempts}";

        int min = Mathf.FloorToInt(tiempoTranscurrido / 60f);
        int seg = Mathf.FloorToInt(tiempoTranscurrido % 60f);
        if (textoTiempo != null) textoTiempo.text = string.Format("{0:0}:{1:00}", min, seg);
    }

    IEnumerator MoverObjeto(Transform obj, Vector3 destino, float duracion)
    {
        if (obj == null) yield break;
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

    // Nota: Dejé esta corrutina abajo por si acaso la necesitas para otra cosa, 
    // pero ya no se ejecuta en el paso 3 para evitar errores con las partículas.
    IEnumerator FadeInLuz(Light luz, float maxIntensidad, float duracion)
    {
        if (luz == null) yield break;
        float b = 0;
        while (b < duracion)
        {
            luz.intensity = Mathf.Lerp(0f, maxIntensidad, b / duracion);
            b += Time.deltaTime;
            yield return null;
        }
        luz.intensity = maxIntensidad;
    }

    public void FinalizarEscenaYGuardarJSON()
    {
        string ruta = Path.Combine(Application.persistentDataPath, "GameData.json");

        string jsonOutput = "{\n" +
            "  \"playerName\": \"Arqueólogo\",\n" +
            "  \"lastScenePlayed\": \"Cámara de Activación\",\n" +
            "  \"scene2\": {\n" +
            $"    \"objectsPlacedCorrectly\": {pasoActual},\n" +
            "    \"totalObjectsToPlace\": 4,\n" +
            $"    \"failedAttempts\": {failedAttempts},\n" +
            "    \"portalUnlocked\": true,\n" +
            "    \"completed\": true,\n" +
            $"    \"completionTime\": {Mathf.RoundToInt(tiempoTranscurrido)}\n" +
            "  }\n" +
            "}";

        File.WriteAllText(ruta, jsonOutput);
        SceneManager.LoadScene("PantallaVictoria");
    }
}
