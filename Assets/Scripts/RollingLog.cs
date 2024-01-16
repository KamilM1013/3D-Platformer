using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingLog : MonoBehaviour
{
    // ref variables
    Animator _animator;
    public GameObject _log;
    public Collider _collider;

    // Start is called before the first frame update
    void Start()
    {
        _animator = _log.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _collider.enabled = false;

            _animator.SetTrigger("AnimTrigger");
            Destroy(_log, 4f);
        }
    }
}
