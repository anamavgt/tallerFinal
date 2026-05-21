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

    public void EmpezarACargarDetras(Transform puntoEspalda)
    {
        puntoCargaTarget = puntoEspalda;
        estaSiendoCargado = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (col != null) col.isTrigger = true;

        transform.SetParent(puntoCargaTarget);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void RebotarPorError(Vector3 posicionRebote)
    {
        estaSiendoCargado = false;
        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            // Fuerza física de rebote hacia el suelo
            rb.AddForce((posicionRebote - transform.position).normalized * 5f, ForceMode.Impulse);
        }

        if (col != null) col.isTrigger = false;
    }

    public void SoltarEnAltar()
    {
        estaSiendoCargado = false;
        transform.SetParent(null);
        if (rb != null) rb.isKinematic = true;
        enabled = false;
    }
}