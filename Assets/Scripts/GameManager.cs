using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public int currentAcorns;
    public TextMeshProUGUI acornsText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddAcorns(int acornsToAdd)
    {
        currentAcorns += acornsToAdd;
        acornsText.text = "Acorns: " + currentAcorns;
    }
}
