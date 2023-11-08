using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine _playerStateMachine = other.GetComponent<PlayerStateMachine>();
            _playerStateMachine.RunMultiplier = 1.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine playerStateMachine = other.GetComponent<PlayerStateMachine>();
            playerStateMachine.RunMultiplier = 4.0f;
        }
    }
}
