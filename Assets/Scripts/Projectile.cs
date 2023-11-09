using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float _speed = 10.0f;    // Projectile speed
    public int _damage = 1;        // Damage dealt by the projectile
    public float _offsetEnemy = 1;      // Offset from the enemy
    public float _offsetPlayerHeight = 1.7f;
    bool _launch = false;

    private Vector3 _launchPosition; // Starting position
    private Vector3 _direction;     // Direction in which the projectile moves

    CharacterController _characterController;
    Vector3 _lastKnownPlayerPosition; // Store the last known player position

    public GameObject _hitEffect;
    Rigidbody _rb;

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_launch)
        {
            // You can adjust the velocity here based on the calculated _direction
            _rb.velocity = _direction * _speed;
        }
    }

    // Method to launch the projectile
    public void Launch()
    {
        // Set the starting position
        _launchPosition = transform.position;
        _launchPosition.x += _offsetEnemy;
        transform.position = _launchPosition;

        _lastKnownPlayerPosition = _characterController.transform.position;
        Vector3 playerPosition = _lastKnownPlayerPosition; // Use the last known player position
        playerPosition.y += _offsetPlayerHeight; 

        // Calculate the direction to the target
        _direction = (playerPosition - transform.position).normalized;

        _launch = true;
    }

    // Handle collision with other objects
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
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
        }
        else if (other.CompareTag("Dead Zone"))
        {
            Debug.Log("Else Statement");
            // Instantiate hit effect
            Instantiate(_hitEffect, transform.position, transform.rotation);

            // Destroy the projectile when it hits anything other than the player
            Destroy(gameObject);
        }
    }
}
