using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Añadido por si usas TextMeshPro

public class InventarioJugador : MonoBehaviour
{
    [Header("Configuracion Visual UI")]
    public Image[] slotsIconos;

    // NUEVA CASILLA: Para arrastrar el texto del contador (ej: "0/10")
    public TextMeshProUGUI textoContadorTMP; // Si usas Text normal de Unity cambia a: public Text textoContadorLegacy;

    [Header("Estado del Inventario")]
    public int objetosRecolectados = 0;
    public List<string> nombresObjetos = new List<string>();

    void Start()
    {
        foreach (Image img in slotsIconos)
        {
            if (img != null) img.enabled = false;
        }

        ActualizarTextoVisual();
    }

    public void AgregarObjeto(Sprite iconoDelObjeto, string nombre)
    {
        if (objetosRecolectados < slotsIconos.Length)
        {
            slotsIconos[objetosRecolectados].sprite = iconoDelObjeto;
            slotsIconos[objetosRecolectados].enabled = true;

            nombresObjetos.Add(nombre);
            objetosRecolectados++;

            Debug.Log("Inventario: Agregado " + nombre + " en el slot " + (objetosRecolectados - 1));

            // Actualizamos el contador visual en la pantalla de inmediato
            ActualizarTextoVisual();

            if (GameManager.instance != null)
            {
                GameManager.instance.SumarArtefacto();
            }
            else
            {
                Debug.LogWarning("OJO: No hay un GameManager en la escena para contar los artefactos.");
            }
        }
        else
        {
            Debug.Log("Inventario lleno, no se pueden mostrar mas iconos.");
        }
    }

    public void VaciarInventario()
    {
        objetosRecolectados = 0;
        nombresObjetos.Clear();

        foreach (Image img in slotsIconos)
        {
            if (img != null)
            {
                img.sprite = null;
                img.enabled = false;
            }
        }

        ActualizarTextoVisual();
        Debug.Log("Inventario vaciado.");
    }

    // Método nuevo para reescribir el letrero en pantalla
    void ActualizarTextoVisual()
    {
        if (textoContadorTMP != null)
        {
            textoContadorTMP.text = objetosRecolectados + " / " + slotsIconos.Length;
        }
    }
}