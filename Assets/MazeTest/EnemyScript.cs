using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 100f;

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
