using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlatform : MonoBehaviour
{
    private Rigidbody _rb;
    public float _fallDelay = 2.0f;  // Adjust this delay as needed
    public float _resetDelay = 6.0f;
    private Vector3 _initialPosition;
    private bool _hasPlayerStepped = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _initialPosition = transform.position;

        // Disable the Rigidbody at the start to prevent falling
        _rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasPlayerStepped)
        {
            _hasPlayerStepped = true;
            StartCoroutine(FallAfterDelay());
        }
    }

    IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(_fallDelay);

        // Enable the Rigidbody to allow falling
        _rb.isKinematic = false;

        yield return new WaitForSeconds(_resetDelay);

        // reset platform
        ResetTrapPlatform();
    }

    private void ResetPlatform()
    {
        // Reset the platform to its initial position
        _rb.isKinematic = true;
        transform.position = _initialPosition;
        _hasPlayerStepped = false;
    }

    // You can call this method to reset the platform if needed
    public void ResetTrapPlatform()
    {
        ResetPlatform();
    }
}
