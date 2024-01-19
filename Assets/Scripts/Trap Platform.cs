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

    public GameObject _splashEffectPrefab;
    private GameObject _splashEffectInstance;

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

        // Instantiate the effect if it hasn't been instantiated yet
        if (_splashEffectInstance == null)
        {
            _splashEffectInstance = Instantiate(_splashEffectPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        }
        else
        {
            // Reposition the existing effect
            _splashEffectInstance.transform.position = transform.position;
            _splashEffectInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            _splashEffectInstance.SetActive(true);
        }

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

        // Deactivate the effect if it exists
        if (_splashEffectInstance != null)
        {
            _splashEffectInstance.SetActive(false);
        }
    }

    // You can call this method to reset the platform if needed
    public void ResetTrapPlatform()
    {
        ResetPlatform();
    }
}
