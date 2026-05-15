using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventarioJugador : MonoBehaviour
{
    [Header("Configuracion Visual UI")]

    public Image[] slotsIconos;

    [Header("Estado del Inventario")]
    public int objetosRecolectados = 0;

    public List<string> nombresObjetos = new List<string>();

    void Start()
    {

        foreach (Image img in slotsIconos)
        {
            if (img != null) img.enabled = false;
        }
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
}