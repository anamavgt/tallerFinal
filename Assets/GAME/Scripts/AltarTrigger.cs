using UnityEngine;
using System.Collections;

public class AltarTrigger : MonoBehaviour
{
    [Header("Configuracion del Altar (Escena 2)")]
    public int numeroDeEsteAltar;
    public string nombreDelObjetoClave;
    public Transform puntoAnclajeObjeto;

    [Header("Feedback Visual Obligatorio")]
    public MeshRenderer aroBrillanteVisual;
    public Material materialNormal;
    public Material materialVerde;
    public Material materialRojo;

    private bool altarYaResuelto = false;

    public bool ValidarObjetoEntregado(string nombreObjeto)
    {
        if (altarYaResuelto) return false;
        // Valida si el nombre del objeto contiene la clave requerida
        return nombreObjeto.Contains(nombreDelObjetoClave);
    }

    public void ProcesarEntregaExitosa(ClicPickUpAndCarry objeto)
    {
        altarYaResuelto = true;
        objeto.SoltarEnAltar();

        if (aroBrillanteVisual != null && materialVerde != null)
        {
            aroBrillanteVisual.material = materialVerde;
        }

        if (puntoAnclajeObjeto != null)
        {
            objeto.transform.position = puntoAnclajeObjeto.position;
            objeto.transform.rotation = puntoAnclajeObjeto.rotation;
        }
        objeto.transform.SetParent(this.transform);

        // Notificar al controlador de la escena 2 usando el orden correcto
        if (ControllerScene2.Instancia != null)
        {
            ControllerScene2.Instancia.RecibirObjetoEnAltar(numeroDeEsteAltar, nombreDelObjetoClave);
        }
    }

    public void ForzarFeedbackRojo()
    {
        StartCoroutine(FeedbackErrorVisual());
    }

    IEnumerator FeedbackErrorVisual()
    {
        if (aroBrillanteVisual != null && materialRojo != null)
        {
            aroBrillanteVisual.material = materialRojo;
        }
        yield return new WaitForSeconds(1.5f);

        if (!altarYaResuelto && aroBrillanteVisual != null && materialNormal != null)
        {
            aroBrillanteVisual.material = materialNormal;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (altarYaResuelto) return;

        if (other.CompareTag("Player"))
        {
            PlayerCarrySystem scriptJugador = other.GetComponent<PlayerCarrySystem>();
            if (scriptJugador != null)
            {
                scriptJugador.RegistrarAltarCercano(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCarrySystem scriptJugador = other.GetComponent<PlayerCarrySystem>();
            if (scriptJugador != null)
            {
                scriptJugador.RemoverAltarCercano(this);
            }
        }
    }
}