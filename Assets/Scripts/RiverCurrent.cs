using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverCurrent : MonoBehaviour
{
    public float currentSpeed = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine _playerStateMachine = other.GetComponent<PlayerStateMachine>();
            _playerStateMachine.InRiver = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStateMachine _playerStateMachine = other.GetComponent<PlayerStateMachine>();
            _playerStateMachine.InRiver = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterController controller = other.GetComponent<CharacterController>();
            if (controller != null && controller.isGrounded)
            {
                Vector3 currentDirection = -Vector3.right;
                Vector3 currentVelocity = currentDirection * currentSpeed;
                currentVelocity.y = 0f;
                controller.Move(currentVelocity * Time.deltaTime);
            }
        }
    }
}
