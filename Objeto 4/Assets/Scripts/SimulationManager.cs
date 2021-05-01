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
    public GameObject cubeHolder;
    Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = new Vector3(1,1,1);
    }

    public void StartSimulation()
    {
        objectCount = int.Parse(objectsInSimulationInput.text);
        minMass = float.Parse(minMassInput.text);
        maxMass = float.Parse(maxMassInput.text);

        for (int i = 0; i < objectCount; i++) 
        {

            float offsetX = -objectCount / 2 + i;
            cubeHolder = Instantiate(cube, new Vector3(offsetX*1.5f, startPosition.y, startPosition.z), Quaternion.identity);
            cubeHolder.GetComponent<BaseCube>().mass = Random.Range(minMass, maxMass);
        }
        // Compute.mixMass = minMass;
        // Compute.maxMass = maxMass;
    }
}
