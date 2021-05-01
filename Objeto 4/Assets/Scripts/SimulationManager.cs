using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimulationManager : MonoBehaviour
{
    struct Cube
    {
        public Vector3 position;
        public Color color;
        public float mass, speed, deltaT, t;
    }

    [SerializeField] TMP_InputField objectsInSimulationInput;
    [SerializeField] TMP_InputField minMassInput;
    [SerializeField] TMP_InputField maxMassInput;
    public int objectCount;
    public float minMass;
    public float maxMass;
    public GameObject cube;
    public GameObject[] cubeHolders;
    Vector3 startPosition;
    Cube[] cubeData;
    [SerializeField] ComputeShader cubeController;
    [SerializeField] int iteractions = 50;
    [SerializeField] bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = new Vector3(1,1,1);
    }

    public void StartSimulation()
    {
        if (started == false)
        {
            objectCount = int.Parse(objectsInSimulationInput.text);
            minMass = float.Parse(minMassInput.text);
            maxMass = float.Parse(maxMassInput.text);
            cubeData = new Cube[objectCount];
            cubeHolders = new GameObject[objectCount];

            for (int i = 0; i < objectCount; i++)
            {
                float offsetX = -objectCount / 2 + i;
                cubeHolders[i] = Instantiate(cube, new Vector3(offsetX * 1.5f, startPosition.y, startPosition.z), Quaternion.identity);
                cubeData[i].mass = Random.Range(minMass, maxMass);
                cubeData[i].position = cubeHolders[i].transform.position;
            }
            started = true;
        }
        int totalsize = 4 * sizeof(float) + 3 * sizeof(float) + 4 * sizeof(float);
        ComputeBuffer computeBuffer = new ComputeBuffer(cubeData.Length, totalsize);
        computeBuffer.SetData(cubeData);
        cubeController.SetBuffer(0, "cubes", computeBuffer);
        cubeController.SetInt("iteractions", iteractions);
        cubeController.Dispatch(0, cubeData.Length / 10, 1, 1);
        computeBuffer.GetData(cubeData);
        for (int i = 0; i < cubeHolders.Length; i++)
        {
            cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", cubeData[i].color);
            cubeHolders[i].transform.position = cubeData[i].position;
        }
        computeBuffer.Dispose();
    }
}
