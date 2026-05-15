using UnityEngine;

public class ObjetoRecolectable : MonoBehaviour
{
    [Header("Configuracion del Objeto")]
    public string nombreDelObjeto;
    public Sprite iconoParaInventario;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // BUSQUEDA UNIVERSAL: Busca el inventario en TODA la escena
            InventarioJugador inventario = FindFirstObjectByType<InventarioJugador>();

            if (inventario != null)
            {
                inventario.AgregarObjeto(iconoParaInventario, nombreDelObjeto);
                Debug.Log("¡CONSEGUIDO! Recolectado: " + nombreDelObjeto);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("OJO: El script InventarioJugador NO existe en la escena.");
            }
        }
    }
}