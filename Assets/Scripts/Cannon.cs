using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    // ref variables
    Animator _animator;
    AudioManager _audioManager;
    public GameObject _destroyEffect;
    public GameObject _projectilePrefab;
    public Transform _player;

    public int _damage = 1;
    public float _launchInterval = 4.0f;  // Time between projectile launches
    public float _distanceToPlayer = 20;

    bool _isAlert;

    // variables to store optimized setter/getter parameter IDs
    int _isAttackingHash;

    CharacterController _characterController;
    //Vector3 _lastKnownPlayerPosition; // Store the last known player position

    private void Awake()
    {
        _characterController = FindObjectOfType<CharacterController>();
        _animator = GetComponent<Animator>();
        _audioManager = FindObjectOfType<AudioManager>();

        // set the parameter hash references
        _isAttackingHash = Animator.StringToHash("Shoot");
    }

    private void Start()
    {
        StartCoroutine(LaunchProjectiles());
    }

    private void Update()
    {
        if (_isAlert == true)
        {
            Vector3 lookAt = _player.position;
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
        }
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
            _animator.SetTrigger("Shoot");

            _audioManager.Play("Cannon");

            GameObject newProjectile = Instantiate(_projectilePrefab, transform.position, transform.rotation);

            Projectile projectile = newProjectile.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Launch();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isAlert = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isAlert = false;
        }
    }
}
