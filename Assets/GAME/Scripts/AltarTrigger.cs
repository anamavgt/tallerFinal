using UnityEngine;
using System.Collections;

public class AltarTrigger : MonoBehaviour
{
    [Header("Configuración del Altar (Escena 2)")]
    public int numeroDeEsteAltar;
    public string nombreDelObjetoClave;
    public Transform puntoAnclajeObjeto;

    [Header("Feedback Visual Obligatorio")]
    public MeshRenderer aroBrillanteVisual;
    public Material materialNormal;
    public Material materialVerde;
    public Material materialRojo;

    private bool altarYaResuelto = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    
    
    }
    
    // Update is called once per frame
    void Update() { 
    
    
    
    }

    // SOLUCIÓN AL ERROR: Esta es la función que busca PlayerCarrySystem para el clic
    public bool ValidarObjetoEntregado(string nombreObjeto)
    {
        if (altarYaResuelto) return false;
        return nombreObjeto.Contains(nombreDelObjetoClave);
    }

    // SOLUCIÓN AL ERROR: Esta función procesa la entrega física al hacer clic correcto
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

        // Notificar al controlador de la escena
        if (ControllerScene2.Instancia != null)
        {
            ControllerScene2.Instancia.RecibirObjetoEnAltar(numeroDeEsteAltar, nombreDelObjetoClave);
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
        yield return new WaitForSeconds(1.2f);
        if (!altarYaResuelto && aroBrillanteVisual != null)
        {
            aroBrillanteVisual.material = materialNormal;
        }
    }
}