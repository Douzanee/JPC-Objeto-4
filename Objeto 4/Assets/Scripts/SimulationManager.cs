using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] InputField objectsInSimulationInput;
    [SerializeField] InputField minMassInput;
    [SerializeField] InputField maxMassInput;

    public int objectCount;
    public float minMass;
    public float maxMass;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSimulation()
    {
        objectCount = objectsInSimulationInput.text.ToInt();

        // Compute.mixMass = minMass;
        // Compute.maxMass = maxMass;
    }
}
