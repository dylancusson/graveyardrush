using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    private int _health;
    private int _maxHealth;
    private Vector3 impact = Vector3.zero;
    public float mass = 3.0f;
    private float _knockbackForce = 30f;
    public GameSettings gameSettings;
    NavMeshAgent agent;
    public Animator animator;
    

    private Transform _playerPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _maxHealth = 4 - (int)gameSettings.difficultyLevel;
        _health = _maxHealth;
        agent = GetComponent<NavMeshAgent>();
         _playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        MoveToPlayer();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(1);
                Vector3 punchDirection = (other.transform.position - transform.position).normalized;
                player.AddKnockback(punchDirection, _knockbackForce);
            }
        }
    }

    private void MoveToPlayer()
    {
        
        agent.SetDestination(_playerPos.position);
        Invoke(nameof(MoveToPlayer), 1f);
        
    }
    
    public void AddKnockback(Vector3 direction, float force) {
        direction.Normalize();
        if (direction.y < 0) direction.y = -direction.y; // Keep the player on the ground
        impact += direction * force / mass;
        Debug.Log("Knockback: " + impact.magnitude);
        StartCoroutine(KnockbackRoutine(direction, force));
    }
    
    private int lastHitFrame = -1;

    public void TakeDamage(int amount)
    {
        // If we've already been hit this frame, ignore this call
        if (Time.frameCount == lastHitFrame) return;

        lastHitFrame = Time.frameCount;
        _health -= amount;
        Debug.Log($"Zombie took {amount}. Health: {_health}");

        if (_health <= 0)
        {
            Messenger.Broadcast(GameEvent.ENEMY_DEAD);
            animator.SetTrigger("isDead");
            Invoke("Die", 1f);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
    
    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        agent.enabled = false; // Turn off AI so physics can work
    
        float timer = 0;
        while (timer < 0.2f) // Apply force for 0.2 seconds
        {
            transform.position += direction * force * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
    
        agent.enabled = true; // Turn AI back on
        agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
    }
    
}
