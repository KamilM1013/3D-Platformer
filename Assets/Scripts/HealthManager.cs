using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int _maxHealth;
    int _currentHealth;

    PlayerStateMachine _playerStateMachine;
    CharacterController _characterController;
    Animator _animator;

    public float _invincibilityLength;
    float _invincibilityCounter;

    public Renderer _playerRenderer;
    float _flashCounter;
    public float _flashLength = 0.1f;

    bool _isRespawning;
    Vector3 _checkpoint;
    public float _respawnLength;

    int _isDeadHash;

    private void Awake()
    {
        _isDeadHash = Animator.StringToHash("isDead");
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;

        _playerStateMachine = GetComponent<PlayerStateMachine>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _checkpoint = _characterController.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_invincibilityCounter > 0)
        {
            _invincibilityCounter -= Time.deltaTime;
            
            _flashCounter -= Time.deltaTime;

            if (_flashCounter <= 0)
            {
                _playerRenderer.enabled = !_playerRenderer.enabled;
                _flashCounter = _flashLength;
            }

            if (_invincibilityCounter <= 0)
            {
                _playerRenderer.enabled = true;
            }
        }
    }

    public void TakeDamage(int damage, Vector3 direction)
    {
        if (_invincibilityCounter <= 0)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                _animator.SetBool(_isDeadHash, true);
                Respawn();
            }
            else
            {
                //_playerStateMachine.Knockback(direction);
                _invincibilityCounter = _invincibilityLength;
                _playerRenderer.enabled = false;
                _flashCounter = _flashLength;
            }
        }
    }

    public void Respawn()
    {
        if (!_isRespawning)
        {
            StartCoroutine("RespawnCoRoutine");
        }
    }

    public IEnumerator RespawnCoRoutine()
    {
        _isRespawning = true;
        //_characterController.gameObject.SetActive(false);

        yield return new WaitForSeconds(_respawnLength);

        //_characterController.gameObject.SetActive(true);
        //_animator.Rebind();
        //_animator.Update(0f);
        _characterController.enabled = false;
        _characterController.transform.position = _checkpoint;
        _characterController.enabled = true;
        _currentHealth = _maxHealth;
        _animator.SetBool(_isDeadHash, false);

        _isRespawning = false;
    }

    public void AddLife(int lifeAmount)
    {
        _currentHealth += lifeAmount;
    }
}
