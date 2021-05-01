using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public struct Cube
{
    public Vector3 position;
    public Color color;
}

public class SimulationManager : MonoBehaviour
{
    [SerializeField] TMP_InputField objectsInSimulationInput;
    [SerializeField] TMP_InputField minMassInput;
    [SerializeField] TMP_InputField maxMassInput;
    [SerializeField] ComputeShader cubeController;
    

    public int objectCount;
    public float minMass;
    public float maxMass;
    public GameObject cubeGO;

    List<GameObject> cubes = new List<GameObject>();
    Vector3 startPosition = Vector3.up;

    private Cube[] data;

    public void StartSimulation()
    {
        objectCount = int.Parse(objectsInSimulationInput.text);
        data = new Cube[objectCount];

        for (int i = 0; i < objectCount; i++) 
        {
            GameObject cube = new GameObject("Cube");
            float offsetX = -objectCount / 2 + i;
            cube.transform.position = new Vector3(offsetX, startPosition.y, startPosition.z);
            cubes.Add(cube);

            Cube cubeData = new Cube();
            cubeData.position = cube.transform.position;
            data[i] = cubeData;
        }
        // Compute.mixMass = minMass;
        // Compute.maxMass = maxMass;

        int totalSize = sizeof(float) * 3 + sizeof(float) * 4;
        ComputeBuffer buffer = new ComputeBuffer(data.Length, totalSize);
        buffer.SetData(data);
        cubeController.SetBuffer(0, "cubes", buffer);
        cubeController.Dispatch(0, data.Length/10, 1, 1);
    }
}
