using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerCarrySystem : MonoBehaviour
{
    [Header("Configuración de Carga")]
    public float distanciaMaximaAgarrar = 4.0f;
    public Transform puntoCargaEspalda;

    [Header("UI de Error (Requisito Obligatorio)")]
    public Text textoZonaIncorrecta;

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
                altarCercanoActual.ProcesarEntregaExitosa(objetoCargadoActualmente);
                objetoCargadoActualmente = null;
                altarCercanoActual = null;
            }
            else
            {
                MostrarErrorZona();
            }
        }
        else if (IntentarSoltarPorClicDirectoAlAltar())
        {
            objetoCargadoActualmente = null;
        }
        else
        {
            MostrarErrorZona();
        }
    }

    bool IntentarSoltarPorClicDirectoAlAltar()
    {
        if (camaraPrincipal == null) return false;

        Vector2 posicionMouse = Mouse.current.position.ReadValue();
        Ray rayo = camaraPrincipal.ScreenPointToRay(posicionMouse);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaMaximaAgarrar))
        {
            AltarTrigger altarClickeado = hit.transform.GetComponent<AltarTrigger>();
            if (altarClickeado != null)
            {
                if (altarClickeado.ValidarObjetoEntregado(objetoCargadoActualmente.gameObject.name))
                {
                    altarClickeado.ProcesarEntregaExitosa(objetoCargadoActualmente);
                    return true;
                }
            }
        }
        return false;
    }

    void MostrarErrorZona()
    {
        if (corrutinaMensaje != null) StopCoroutine(corrutinaMensaje);
        corrutinaMensaje = StartCoroutine(MostrarMensajeErrorUI());

        // MODIFICADO: Llamada segura para registrar el error sin romper el orden secuencial
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