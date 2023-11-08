using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowerEnemy : MonoBehaviour
{
    public int _damage = 1;
    public GameObject _destroyEffect;
    public GameObject _projectilePrefab;
    public float _launchInterval = 4.0f;  // Time between projectile launches
    public float _distanceToPlayer = 20;

    PlayerStateMachine _playerStateMachine;
    CharacterController _characterController;
    //Vector3 _lastKnownPlayerPosition; // Store the last known player position

    private void Awake()
    {
        _playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        _characterController = FindObjectOfType<CharacterController>();
    }

    private void Start()
    {
        StartCoroutine(LaunchProjectiles());
    }

    private IEnumerator LaunchProjectiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(_launchInterval);

            if (_characterController != null && Vector3.Distance(_characterController.transform.position, transform.position) < _distanceToPlayer)
            {
                LaunchProjectileTowardsPlayer();
            }
        }
    }

    private void LaunchProjectileTowardsPlayer()
    {
        if (_projectilePrefab != null && _characterController != null)
        {
            if (_projectilePrefab != null)
            {
                GameObject newProjectile = Instantiate(_projectilePrefab, transform.position, transform.rotation);

                Projectile projectile = newProjectile.GetComponent<Projectile>();

                if (projectile != null)
                {
                    projectile.Launch();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_playerStateMachine.IsAttackPressed)
        {
            Vector3 hitDirection = other.transform.position - transform.position;
            hitDirection = hitDirection.normalized;

            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            playerManager.TakeDamage(_damage, hitDirection);
        }
        else if (other.CompareTag("Player") && _playerStateMachine.IsAttackPressed)
        {
            Instantiate(_destroyEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
