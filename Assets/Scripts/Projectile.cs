using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    AudioManager _audioManager;

    public float _speed = 1.75f;     // Projectile speed
    public int _damage = 1;         // Damage dealt by the projectile
    public float _offsetEnemy = 2;  // Offset from the enemy
    public float _offsetPlayerHeight = 1.7f;

    Vector3 _launchPosition;  // Starting position
    Vector3 _direction;      // Direction in which the projectile moves

    CharacterController _characterController;
    Vector3 _lastKnownPlayerPosition; // Store the last known player position

    public GameObject _hitEffect;
    public Rigidbody _rb;

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Method to launch the projectile
    public void Launch()
    {
        // Set the starting position
        _launchPosition = transform.position;
        _launchPosition.y += _offsetEnemy;
        transform.position = _launchPosition;

        _lastKnownPlayerPosition = _characterController.transform.position;
        Vector3 playerPosition = _lastKnownPlayerPosition; // Use the last known player position
        playerPosition.y += _offsetPlayerHeight;

        // Calculate the direction to the target
        _direction = (playerPosition - transform.position).normalized;

        // Calculate the initial velocity to achieve the desired arc
        float launchAngle = 45f; // Adjust this angle as needed for your desired arc
        float g = Mathf.Abs(Physics.gravity.y); // Magnitude of gravity
        float initialSpeed = _speed * Mathf.Sqrt((playerPosition - transform.position).magnitude * g / Mathf.Sin(2 * launchAngle * Mathf.Deg2Rad));

        // Apply the initial velocity to the Rigidbody
        _rb.velocity = _direction * initialSpeed;
    }

    // Handle collision with other objects
    private void OnTriggerEnter(Collider other)
    {
        // Check if the projectile hits an object with a "Player" tag
        if (other.CompareTag("Player"))
        {
            // Deal damage to the player or trigger other interactions
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.TakeDamage(_damage, _direction);
            }

            // Instantiate hit effect
            Instantiate(_hitEffect, transform.position, transform.rotation);

            // Destroy the projectile when it hits the player
            Destroy(gameObject);

            _audioManager.Play("ProjectileHit");
        }
        else if (other.CompareTag("Terrain"))
        {
            // Instantiate hit effect
            Instantiate(_hitEffect, transform.position, transform.rotation);

            // Destroy the projectile when it hits anything other than the player
            Destroy(gameObject);

            _audioManager.Play("ProjectileHit");
        }
    }
}
