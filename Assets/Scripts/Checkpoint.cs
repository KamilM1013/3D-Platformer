using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // declare reference variables
    PlayerManager _playerManager;

    public GameObject _pickupEffect;

    private void Awake()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            _playerManager.SetCheckpoint(transform.position);

            Instantiate(_pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
