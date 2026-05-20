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

        // ¡SOLUCIÓN AL ERROR! Busca el Animator tanto en el objeto como en sus hijos visuales
        anim = GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogWarning("AranaIA: No se encontró ningún componente Animator en este objeto ni en sus hijos.");
        }

        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;

        GameObject eObj = GameObject.Find("---EnemyController---");
        if (eObj != null) enemyCtrl = eObj.GetComponent<EnemyController>();
    }

    void Update()
    {
        // Si la araña está atacando o esperando para atacar, reducimos el temporizador globalmente
        if (timerAtaque > 0)
        {
            timerAtaque -= Time.deltaTime;
        }

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
            if (anim != null) anim.SetFloat("Speed", 0f);
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
            if (dir != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * 5f);
            }
            transform.position += transform.forward * velocidad * Time.deltaTime;

            if (anim != null) anim.SetFloat("Speed", 1f);
        }
        else
        {
            // Está en rango de ataque
            if (anim != null) anim.SetFloat("Speed", 0f);
            Atacar();
        }

        // Se cansa si pasa mucho tiempo o se aleja demasiado
        if (timerPersecucion <= 0 || dist > distanciaDeteccion + 5f)
        {
            estaPersiguiendo = false;
        }
    }

    void Atacar()
    {
        // Ataca solo si el temporizador ya llegó a cero
        if (timerAtaque <= 0)
        {
            if (anim != null) anim.SetTrigger("Attack");
            enemyCtrl.HacerDanioAlJugador(1);
            timerAtaque = 2f; // Espera 2 segundos exactos para volver a morder
        }
    }

    public void DealDamage()
    {
        Debug.Log("DealDamage ejecutado");
    }
}