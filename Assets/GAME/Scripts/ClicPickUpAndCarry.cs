using UnityEngine;

public class ClicPickUpAndCarry : MonoBehaviour
{
    private bool estaSiendoCargado = false;
    private Transform puntoCargaTarget;
    private Rigidbody rb;
    private Collider col;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (estaSiendoCargado && puntoCargaTarget != null)
        {
            transform.position = puntoCargaTarget.position;
            transform.rotation = puntoCargaTarget.rotation;
        }
    }

    /// <summary>
    /// Metodo requerido por PlayerCarrySystem para alzar el objeto con el mouse
    /// </summary>
    public void EmpezarACargarDetras(Transform puntoEspalda)
    {
        puntoCargaTarget = puntoEspalda;
        estaSiendoCargado = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (col != null)
        {
            col.isTrigger = true;
        }

        transform.SetParent(puntoCargaTarget);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Metodo requerido por AltarTrigger al colocar exitosamente el objeto
    /// </summary>
    public void SoltarEnAltar()
    {
        estaSiendoCargado = false;
        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = true; // Se queda quieto en el altar
        }

        enabled = false; // Desactivamos el script para que no se pueda volver a alzar
    }
}