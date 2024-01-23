using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    // ref variables
    Animator _animator;
    AudioManager _audioManager;
    public Collider _collider;


    private void Awake()
    {
        _animator = FindObjectOfType<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetTrigger("AnimTrigger");
            _collider.enabled = false;

            _audioManager.Play("Zipline");

            StartCoroutine(WaitForAnim());
        }
    }

    IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(12);
        _animator.SetTrigger("AnimReverseTrigger");
        _collider.enabled = true;
    }
}
