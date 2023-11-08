using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemy : MonoBehaviour
{
    public int _damage = 1;
    public GameObject _destroyEffect;

    // declare reference variables
    PlayerStateMachine _playerStateMachine;

    private void Awake()
    {
        _playerStateMachine = FindObjectOfType<PlayerStateMachine>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
