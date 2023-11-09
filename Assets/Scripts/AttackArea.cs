using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    PlayerManager _playerManager;
    PlayerStateMachine _playerStateMachine;

    public GameObject _checkpointEffect;

    private void Start()
    {
        Transform parentTransform = transform.parent;
        _playerManager = parentTransform.GetComponent<PlayerManager>();
        _playerStateMachine = parentTransform.GetComponent<PlayerStateMachine>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

        }
        else if (other.CompareTag("Checkpoint") && _playerStateMachine.IsAttacking)
        {
            Debug.Log("Triggered");
            _playerManager.SetCheckpoint(other.transform.position);

            Instantiate(_checkpointEffect, other.transform.position, other.transform.rotation);

            Destroy(other.gameObject);
        }
    }
}
