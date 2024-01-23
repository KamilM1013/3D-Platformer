using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AttackArea : MonoBehaviour
{
    PlayerManager _playerManager;
    PlayerStateMachine _playerStateMachine;
    AudioManager _audioManager;
    GameManager _gameManager;

    public GameObject _crateEffect;
    public GameObject _attackEffect;
    public GameObject _fireworksEffect;
    public GameObject _fireworksEffect2;

    private List<GameObject> _triggeredCheckpoints = new List<GameObject>();
    private List<GameObject> _triggeredCrates = new List<GameObject>();

    private void Awake()
    {
        _audioManager = FindObjectOfType<AudioManager>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        Transform parentTransform = transform.parent;
        _playerManager = parentTransform.GetComponent<PlayerManager>();
        _playerStateMachine = parentTransform.GetComponent<PlayerStateMachine>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && _playerStateMachine.IsAttacking)
        {
            Instantiate(_attackEffect, other.transform.position, other.transform.rotation);

            Destroy(other.gameObject);

            _audioManager.Play("HitEnemy");
        }
        else if (other.CompareTag("Checkpoint") && _playerStateMachine.IsAttacking && !_triggeredCheckpoints.Contains(other.gameObject))
        {
            _triggeredCheckpoints.Add(other.gameObject); // Mark checkpoint as triggered

            _playerManager.SetCheckpoint(other.transform.position);
            _playerManager.TriggerCheckpointUI();

            Instantiate(_crateEffect, other.transform.position, other.transform.rotation);

            Destroy(other.gameObject);

            _audioManager.Play("HitCrate");
            _audioManager.Play("Checkpoint");

            _gameManager.AddPeanuts(8);
            _gameManager.AddCrates(1);
        }
        else if (other.CompareTag("Crate") && _playerStateMachine.IsAttacking && !_triggeredCrates.Contains(other.gameObject))
        {
            _triggeredCrates.Add(other.gameObject); // Mark crate as triggered

            Instantiate(_crateEffect, other.transform.position, other.transform.rotation);

            Destroy(other.gameObject);

            _audioManager.Play("HitCrate");

            _gameManager.AddPeanuts(1);
            _gameManager.AddCrates(1);
        }
        else if (other.CompareTag("QuestionCrate") && _playerStateMachine.IsAttacking)
        {

            Instantiate(_crateEffect, other.transform.position, other.transform.rotation);
            Instantiate(_fireworksEffect, other.transform.position, other.transform.rotation);
            Instantiate(_fireworksEffect2, other.transform.position, other.transform.rotation);

            Destroy(other.gameObject);

            _audioManager.Play("HitCrate");
            _audioManager.Play("FireWorks");

            _gameManager.AddCrates(1);
        }
    }
}
