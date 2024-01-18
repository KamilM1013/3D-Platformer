using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    // declare reference variables
    PlayerStateMachine _playerStateMachine;
    CharacterController _characterController;
    Animator _animator;
    public Renderer _playerRenderer;
    public Image blackScreen;
    public GameObject _checkpointText;
    Snake[] _snakes;

    // health variables
    public int _maxHealth;
    int _currentHealth;

    // invincibility variables
    public float _invincibilityLength;
    float _invincibilityCounter;

    // flash variables
    float _flashCounter;
    public float _flashLength = 0.1f;

    // respawn variables
    bool _isRespawning;
    Vector3 _checkpoint;
    public float _respawnLength;

    int _isDeadHash;

    // fade variables
    bool _isFadeToBlack;
    bool _isFadeFromBlack;
    public float _fadeSpeed;
    public float _waitForFade;

    // getters and setters
    public bool IsDead => _currentHealth <= 0;

    void Awake()
    {
        _isDeadHash = Animator.StringToHash("die");

        _playerStateMachine = GetComponent<PlayerStateMachine>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _snakes = FindObjectsOfType<Snake>();

        _checkpoint = _characterController.transform.position;
    }

    void Start()
    {
        _currentHealth = _maxHealth;
    }

    void Update()
    {
        HandleInvincibilityFlash();
        HandleScreenFading();
    }

    void HandleInvincibilityFlash()
    {
        if (_invincibilityCounter > 0)
        {
            _invincibilityCounter -= Time.deltaTime;
            _flashCounter -= Time.deltaTime;

            if (_flashCounter <= 0)
            {
                TogglePlayerRenderer();
                _flashCounter = _flashLength;
            }

            if (_invincibilityCounter <= 0)
            {
                ShowPlayerRenderer();
            }
        }
    }

    void HandleScreenFading()
    {
        if (_isFadeToBlack)
        {
            FadeToBlack();
        }

        if (_isFadeFromBlack)
        {
            FadeFromBlack();
        }
    }

    void TogglePlayerRenderer()
    {
        _playerRenderer.enabled = !_playerRenderer.enabled;
    }

    void ShowPlayerRenderer()
    {
        _playerRenderer.enabled = true;
    }

    void FadeToBlack()
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 1f, _fadeSpeed * Time.deltaTime));

        if (blackScreen.color.a == 1)
        {
            _isFadeToBlack = false;
        }
    }

    void FadeFromBlack()
    {
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 0f, _fadeSpeed * Time.deltaTime));

        if (blackScreen.color.a == 0)
        {
            _isFadeFromBlack = false;
        }
    }

    public void TakeDamage(int damage, Vector3 direction)
    {
        if (!IsDead)
        {
            if (_invincibilityCounter <= 0)
            {
                _currentHealth -= damage;

                if (_currentHealth <= 0)
                {
                    _animator.SetTrigger(_isDeadHash);
                    Respawn();
                }
                else
                {
                    _invincibilityCounter = _invincibilityLength;
                    _playerRenderer.enabled = false;
                    _flashCounter = _flashLength;
                }
            }
        }
    }

    public void Respawn()
    {
        if (!_isRespawning)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(_respawnLength);

        _isFadeToBlack = true;

        yield return new WaitForSeconds(_waitForFade);

        // Reset snake alert states
        foreach (Snake snake in _snakes)
        {
            snake.ResetAlertState();
        }

        // Now, start fading out
        _isFadeToBlack = false;
        _isFadeFromBlack = true;

        _isRespawning = false;

        // Move the character during screen fading
        _characterController.enabled = false;
        _characterController.transform.position = _checkpoint;
        _characterController.enabled = true;
        _currentHealth = _maxHealth;
        _playerStateMachine.RunMultiplier = 5.0f;

        _invincibilityCounter = _invincibilityLength;
        _playerRenderer.enabled = false;
        _flashCounter = _flashLength;

    }

    public void AddLife(int lifeAmount)
    {
        _currentHealth += lifeAmount;
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        _checkpoint = newCheckpoint;
    }

    public void TriggerCheckpointUI()
    {
        StartCoroutine(FlashCheckpointUI()); 
    }

    private IEnumerator FlashCheckpointUI()
    {
        _checkpointText.SetActive(true);
        yield return new WaitForSeconds(3);
        _checkpointText.SetActive(false);
    }
}
