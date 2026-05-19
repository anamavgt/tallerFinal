using UnityEngine;

public class ObjetoRecolectable : MonoBehaviour
{
    [Header("Configuracion del Objeto")]
    public string nombreDelObjeto;
    public Sprite iconoParaInventario;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    void Awake()
    {
        // Guarda posición y rotación originales al iniciar
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
    }

    void Start()
    {
        // Start() siempre corre después de que todos los Awake() terminaron,
        // así el GameManager ya existe con seguridad
        if (GameManager.instance != null)
            GameManager.instance.RegistrarObjeto(this);
        else
            Debug.LogError("No hay GameManager en la escena al registrar: " + nombreDelObjeto);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventarioJugador inventario = FindFirstObjectByType<InventarioJugador>();
            if (inventario != null)
            {
                inventario.AgregarObjeto(iconoParaInventario, nombreDelObjeto);
                Debug.Log("¡CONSEGUIDO! Recolectado: " + nombreDelObjeto);
                gameObject.SetActive(false); // Desactiva en vez de destruir
            }
            else
            {
                Debug.LogError("OJO: El script InventarioJugador NO existe en la escena.");
            }
        }
    }

    public void Resetear()
    {
        // Vuelve a su posición original y se reactiva
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;
        gameObject.SetActive(true);
    }
}