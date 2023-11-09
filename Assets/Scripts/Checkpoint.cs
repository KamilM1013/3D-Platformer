using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // declare reference variables
    PlayerStateMachine _playerStateMachine;
    PlayerManager _playerManager;

    public GameObject _pickupEffect;

    private void Awake()
    {
        _playerStateMachine = FindObjectOfType<PlayerStateMachine>();
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Attack Area") && _playerStateMachine.IsAttackPressed)
        {
            _playerManager.SetCheckpoint(transform.position);

            Instantiate(_pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }*/
}
