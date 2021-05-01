using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] TMP_InputField objectsInSimulationInput;
    [SerializeField] TMP_InputField minMassInput;
    [SerializeField] TMP_InputField maxMassInput;
    

    public int objectCount;
    public float minMass;
    public float maxMass;
    public GameObject cube;
    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = new Vector3(1,1,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSimulation()
    {
        objectCount = int.Parse(objectsInSimulationInput.text);
        for (int i = 0; i < objectCount; i++) 
        {

            float offsetX = -objectCount / 2 + i;
            Instantiate(cube, new Vector3(offsetX*1.5f, startPosition.y, startPosition.z), Quaternion.identity);
        }
        // Compute.mixMass = minMass;
        // Compute.maxMass = maxMass;
    }
}
