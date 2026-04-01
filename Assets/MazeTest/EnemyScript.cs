using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public float health = 100f;
    private NavMeshAgent agent;
    private Transform player;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }


    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    public void TakeDamage(float damage)
    {
        
        health -= damage;
        Debug.Log("Enemy took damage: " + damage);

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }
}
