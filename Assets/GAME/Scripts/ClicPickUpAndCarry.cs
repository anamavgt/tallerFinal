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

        if (col != null)
        {
            col.isTrigger = true;
        }

        transform.SetParent(puntoCargaTarget);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void SoltarEnAltar()
    {
        estaSiendoCargado = false;
        transform.SetParent(null);
        enabled = false;
    }
}