using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public SaludPersonaje saludJugador;

    void Start()
    {
        if (saludJugador == null)
        {
            GameObject ctrl = GameObject.Find("---CharacterController---");
            if (ctrl != null) saludJugador = ctrl.GetComponent<SaludPersonaje>();
        }
    }

    public void HacerDanioAlJugador(int puntos)
    {
        if (saludJugador != null) saludJugador.RecibirDanio(puntos);
    }
}