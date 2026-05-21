using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class PlayerCarrySystem : MonoBehaviour
{
    [Header("Configuración de Carga")]
    public float distanciaMaximaAgarrar = 4.0f;
    public Transform puntoCargaEspalda;

    [Header("UI de Error")]
    public TextMeshProUGUI textoZonaIncorrecta;

    private ClicPickUpAndCarry objetoCargadoActualmente = null;
    private Camera camaraPrincipal;
    private AltarTrigger altarCercanoActual = null;
    private Coroutine corrutinaMensaje;

    void Start()
    {
        camaraPrincipal = Camera.main;

        if (puntoCargaEspalda == null)
        {
            puntoCargaEspalda = transform.Find("PuntoCargaEspalda");
        }

        if (textoZonaIncorrecta != null)
        {
            textoZonaIncorrecta.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (objetoCargadoActualmente == null)
            {
                IntentarAgarrarObjeto();
            }
            else
            {
                IntentarSoltarObjetoConClic();
            }
        }
    }

    void IntentarAgarrarObjeto()
    {
        if (camaraPrincipal == null) return;

        Vector2 posicionMouse = Mouse.current.position.ReadValue();
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionMouse);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, 100f))
        {
            ClicPickUpAndCarry scriptObjeto = hit.transform.GetComponent<ClicPickUpAndCarry>();

            if (scriptObjeto != null)
            {
                float distanciaReal = Vector3.Distance(transform.position, hit.transform.position);
                if (distanciaReal <= distanciaMaximaAgarrar)
                {
                    objetoCargadoActualmente = scriptObjeto;
                    objetoCargadoActualmente.EmpezarACargarDetras(puntoCargaEspalda);
                }
            }
        }
    }

    void IntentarSoltarObjetoConClic()
    {
        if (altarCercanoActual != null)
        {
            if (altarCercanoActual.ValidarObjetoEntregado(objetoCargadoActualmente.gameObject.name))
            {
                // Obtenemos el paso correcto actual de forma segura desde el controlador
                int pasoEsperado = ObtenerPasoActual();

                // Validación estricta: El número de altar debe coincidir con el orden de la secuencia activa
                if (altarCercanoActual.numeroDeEsteAltar == pasoEsperado)
                {
                    altarCercanoActual.ProcesarEntregaExitosa(objetoCargadoActualmente);
                    objetoCargadoActualmente = null;
                    altarCercanoActual = null;
                }
                else
                {
                    // Si el objeto corresponde al altar pero NO es su turno en la secuencia
                    ForzarReboteObjetoInvalido();
                }
            }
            else
            {
                // Si el objeto ni siquiera pertenece a este tipo de altar
                altarCercanoActual.ForzarFeedbackRojo();
                ForzarReboteObjetoInvalido();
            }
        }
        else
        {
            // Si el jugador hace clic en cualquier lugar vacío del mapa mientras carga algo
            ForzarReboteObjetoInvalido();
        }
    }

    int ObtenerPasoActual()
    {
        // Consulta el script de control central de la Escena 2 para saber qué número de altar toca activar
        if (ControllerScene2.Instancia != null)
        {
            return ControllerScene2.Instancia.ObtenerPasoActual();
        }
        return 0;
    }

    void ForzarReboteObjetoInvalido()
    {
        if (objetoCargadoActualmente != null)
        {
            // Ejecuta el rebote físico hacia atrás del personaje (Requisito 5.2)
            objetoCargadoActualmente.RebotarPorError(transform.position - transform.forward * 2f + Vector3.up * 0.5f);
            objetoCargadoActualmente = null;
        }

        if (corrutinaMensaje != null) StopCoroutine(corrutinaMensaje);
        corrutinaMensaje = StartCoroutine(MostrarMensajeErrorUI());

        if (ControllerScene2.Instancia != null)
        {
            ControllerScene2.Instancia.RecibirObjetoEnAltar(-1, "Incorrecto");
        }
    }

    IEnumerator MostrarMensajeErrorUI()
    {
        if (textoZonaIncorrecta != null)
        {
            textoZonaIncorrecta.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            textoZonaIncorrecta.gameObject.SetActive(false);
        }
    }

    public void RegistrarAltarCercano(AltarTrigger altar) { altarCercanoActual = altar; }
    public void RemoverAltarCercano(AltarTrigger altar) { if (altarCercanoActual == altar) altarCercanoActual = null; }
}