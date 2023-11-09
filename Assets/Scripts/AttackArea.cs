using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    PlayerManager _playerManager;
    PlayerStateMachine _playerStateMachine;

    public GameObject _checkpointEffect;
    public GameObject _attackEffect;

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
        }
        else if (other.CompareTag("Checkpoint") && _playerStateMachine.IsAttacking)
        {
            _playerManager.SetCheckpoint(other.transform.position);

            Instantiate(_checkpointEffect, other.transform.position, other.transform.rotation);

            Destroy(other.gameObject);
        }
    }
}
