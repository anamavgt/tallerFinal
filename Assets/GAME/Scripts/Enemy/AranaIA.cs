using UnityEngine;

public class AranaIA : MonoBehaviour
{
    [Header("Ajustes de IA")]
    public float velocidad = 3f;
    public float distanciaDeteccion = 10f;
    public float distanciaAtaque = 2f;
    public float tiempoPersecucionMax = 5f;

    private Transform player;
    private Animator anim;
    private EnemyController enemyCtrl;
    private Vector3 posOriginal;
    private float timerPersecucion;
    private bool estaPersiguiendo = false;
    private float timerAtaque;

    void Start()
    {
        posOriginal = transform.position;
        anim = GetComponent<Animator>();

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

        GameObject eObj = GameObject.Find("---EnemyController---");
        if (eObj != null) enemyCtrl = eObj.GetComponent<EnemyController>();
    }

    void Update()
    {
        if (player == null || enemyCtrl == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia < distanciaDeteccion && !estaPersiguiendo)
        {
            estaPersiguiendo = true;
            timerPersecucion = tiempoPersecucionMax;
        }

        if (estaPersiguiendo)
        {
            LogicaPersecucion(distancia);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, posOriginal, (velocidad / 2) * Time.deltaTime);
            if (anim != null) anim.SetFloat("Speed", 0);
        }
    }

    void LogicaPersecucion(float dist)
    {
        timerPersecucion -= Time.deltaTime;

        if (dist > distanciaAtaque)
        {
            // Moverse hacia personaje
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0;
            transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 5f);
            transform.position += transform.forward * velocidad * Time.deltaTime;

            if (anim != null) anim.SetFloat("Speed", 1); 
        }
        else
        {
            // Esta en rango de ataque
            if (anim != null) anim.SetFloat("Speed", 0);
            Atacar();
        }

        // Se cansa si pasa mucho tiempo o se aleja
        if (timerPersecucion <= 0 || dist > distanciaDeteccion + 5f)
        {
            estaPersiguiendo = false;
        }
    }

    void Atacar()
    {
        if (timerAtaque <= 0)
        {
            if (anim != null) anim.SetTrigger("Attack");
            enemyCtrl.HacerDanioAlJugador(1);
            timerAtaque = 2f; // Espera 2 seg para volver a morder
        }
        else
        {
            timerAtaque -= Time.deltaTime;
        }
    }
}