using UnityEngine;

public class ObjetoDañino : MonoBehaviour
{
    public int dañoAQuitar = 1;

    private SaludPersonaje scriptSalud;

    void Start()
    {
        GameObject controlador = GameObject.Find("---CharacterController---");
        if (controlador != null)
        {
            scriptSalud = controlador.GetComponent<SaludPersonaje>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (scriptSalud != null)
            {
                scriptSalud.RecibirDanio(dañoAQuitar);
            }
        }
    }
}