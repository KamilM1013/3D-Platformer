using UnityEngine;

public class SnakeAlertCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Snake snake = transform.parent.GetComponent<Snake>();
            snake.OnAlertTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Snake snake = transform.parent.GetComponent<Snake>();
        snake.OnAlertTriggerExit(other);
    }
}
