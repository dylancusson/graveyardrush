using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    /*
     * I used the Unity 6 updated movement system. It captures all directions of movement at once, so it didn't make
     * sense for me to split that out into separate scripts that would cause triple the calls to the same movement capture.
     * MouseLook, Spin, and FPSInput are all contained here as individual functions.
     */
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform cameraTransform; // Assign your child camera object here in the Inspector
    [SerializeField] private bool invertY = false;
    [SerializeField] private float maxYAngle = 89f; // Prevents camera flipping

    [FormerlySerializedAs("_baseSpeed")] [SerializeField]
    private float baseSpeed = 6f;

    private float _speed;
    [SerializeField] private float gravity = 9.81f;
    public GameSettings gameSettings;

    private CharacterController _controller;
    private PlayerInput _playerInput;
    private float _pitch = 0f;
    private bool canMove = true;

    private InputAction _lookAction;
    private InputAction _attack;
    private InputAction _moveAction;
    private InputAction _pauseAction;
    private Vector2 _moveDirection;

    private bool _canShoot = true;
    private float _nextFireTime = 0f;
    public float fireRate = 0.5f;
    private bool _isPaused = false;
    public float cooldown = 0.5f;

    [SerializeField] private float maxDistance = 300f;
    [SerializeField] private Vector3 boxSizeMultiplier = Vector3.one * 0.5f;

    [SerializeField] private LayerMask enemyLayer;

    private Collider col;
    private RaycastHit hit;
    private bool hitDetected;

    private int _health;
    private int _maxHealth;

    private Vector3 impact = Vector3.zero;
    public float mass = 3.0f;
    public float _knockbackForce = 3f;

    public AudioClip blastSound;
    public AudioClip reloadSound;
    private AudioSource _audioSource;

    public TMP_Text healthText;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _lookAction = _playerInput.actions["Look"];
        _moveAction = _playerInput.actions["Move"];
        _attack = _playerInput.actions["Attack"];
        _pauseAction =  _playerInput.actions["Pause"];
        canMove = true;
        _speed = baseSpeed;
        sensitivity = gameSettings.mouseSensitivity;
        col = GetComponent<Collider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _maxHealth = 2 + (int)gameSettings.difficultyLevel;
        _health = _maxHealth;
        gameSettings.score = 0;
        _audioSource =  GetComponent<AudioSource>();
        healthText.text = $"Health: {_health} / {_maxHealth}";
    }

    private void Awake()
    {
        Messenger.AddListener(GameEvent.GAME_PAUSED, OnPause);
        Messenger.AddListener(GameEvent.GAME_UNPAUSED, OnUnPause);

    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.GAME_PAUSED, OnPause);
        Messenger.RemoveListener(GameEvent.GAME_UNPAUSED, OnUnPause);
    }

    private void OnPause()
    {
        
        _canShoot = false;
        canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void OnUnPause()
    {
        
        _canShoot = true;
        canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TakeDamage(int dmg)
    {
        if (_health - dmg <= 0)
        {
            _canShoot = false;
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Messenger.Broadcast(GameEvent.GAME_OVER);
            Debug.Log("Game over!");
        }
        else
        {
            _health -= dmg;
            healthText.text = $"Health: {_health} / {_maxHealth}";
            Messenger.Broadcast(GameEvent.TOOK_DAMAGE);
            Debug.Log("Player took " + dmg);
            Debug.Log("Health: " + _health + "/" + _maxHealth);
        }
    }

void Update()
    {
        if (_pauseAction.WasPressedThisFrame() && !_isPaused)
        {
            Messenger.Broadcast(GameEvent.GAME_PAUSED);
            _isPaused = true;
        }

        else if (_pauseAction.WasPressedThisFrame() && _isPaused)
        {
            Messenger.Broadcast(GameEvent.GAME_UNPAUSED);
            _isPaused = false;
        }
        
        else if (canMove)
        {
            if (impact.magnitude > 0.2f) {
                _controller.Move(impact * Time.deltaTime);
            }

            // 2. Consume the energy over time (Friction/Decay)
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
            ProcessLook();
            ProcessMove();
            if (_attack.WasPressedThisFrame() && _canShoot && Time.time >= _nextFireTime)
            {
                Fire();
                _nextFireTime = Time.time + fireRate;
            }
        }
        ApplyGravity();
        
    }

    

    public void AddKnockback(Vector3 direction, float force) {
        direction.Normalize();
        if (direction.y < 0) direction.y = -direction.y; // Keep the player on the ground
        impact += direction * force / mass;
        Debug.Log("Knockback: " + impact.magnitude);
    }

    private void Fire()
    {
        Vector3 halfExtents = Vector3.Scale(transform.localScale, boxSizeMultiplier);
        Vector3 origin = col.bounds.center;
        Vector3 direction = transform.forward;
        
        _audioSource.PlayOneShot(blastSound);
        
        RaycastHit[] hits = Physics.BoxCastAll(
            origin, 
            halfExtents, 
            direction, 
            transform.rotation, 
            maxDistance, 
            enemyLayer
        );

        // 1. Create a set to hold unique enemies found in this blast
        HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

        // 2. First pass: Identify all unique enemies
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
            {
                hitEnemies.Add(enemy); // Duplicates are automatically ignored here
            }
        }

        // 3. Second pass: Apply damage/knockback to each unique enemy ONCE
        foreach (Enemy enemy in hitEnemies)
        {
            Debug.Log("Hit unique zombie: " + enemy.name);
            
            Vector3 punchDirection = (enemy.transform.position - transform.position).normalized;
            enemy.AddKnockback(punchDirection, 10f);
            enemy.TakeDamage(1); 
        }
    }
    
    private void ProcessLook()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();
        // Calculate vertical rotation (pitch)
        _pitch += lookInput.y * sensitivity * (invertY ? 1 : -1) * Time.deltaTime; 
        _pitch = Mathf.Clamp(_pitch, -maxYAngle, maxYAngle);

        // Apply pitch to the camera (local rotation for up/down)
        cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        // Calculate horizontal rotation (yaw) and apply to the player body (global rotation for left/right)
        transform.Rotate(lookInput.x * sensitivity * Vector3.up * Time.deltaTime);
    }

    private void ProcessMove()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();

        // Calculate direction relative to where the player is facing
        Vector3 moveDir = transform.forward * input.y + transform.right * input.x;

        // Use the CharacterController to handle movement and collisions
        _controller.Move(moveDir * _speed * Time.deltaTime);

    }

    private void ApplyGravity()
    {
        _controller.Move(Vector3.down * gravity * Time.deltaTime);
    }
    
    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    private void ResetShoot()
    {
        _canShoot = true;
    }
    
    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if (hitDetected)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.localScale);
        }
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * maxDistance, transform.localScale);
        }
    }
}
