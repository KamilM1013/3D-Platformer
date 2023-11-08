using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MonoBehaviour
{

    public int _value;

    public GameObject _pickupEffect;

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
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<GameManager>().AddAcorns(_value);

            Instantiate(_pickupEffect, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
