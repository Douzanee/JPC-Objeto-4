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
        public float speed, mass;
    }

    [SerializeField] TMP_InputField objectsInSimulationInput;
    [SerializeField] TMP_InputField minMassInput;
    [SerializeField] TMP_InputField maxMassInput;
    public int objectCount;
    public float minMass;
    public float maxMass;
    public GameObject cubeObj;
    public GameObject[] cubeHolders;
    Vector3 startPosition;
    Cube[] cubeData;
    [SerializeField] ComputeShader cubeController;
    [SerializeField] bool started = false;
    public Material material;

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
                cubeHolders[i] = Instantiate(cubeObj, new Vector3(offsetX * 1.5f, startPosition.y + 10, startPosition.z), Quaternion.identity);
                cubeData[i].mass = Random.Range(minMass, maxMass);
                cubeData[i].position = cubeHolders[i].transform.position;
                cubeHolders[i].GetComponent<MeshRenderer>().material = new Material(material);
                cubeData[i].color = cubeHolders[i].GetComponent<MeshRenderer>().material.color;
                cubeData[i].speed = 9.8f * Time.time;
            }
            started = true;
        }
    }
    private void Update()
    {
        if (started)
        {
            int colorSize = sizeof(float) * 4;
            int vector3Size = sizeof(float) * 3;
            int variables = sizeof(float) * 2;
            int totalSize = colorSize + vector3Size + variables;
            ComputeBuffer computeBuffer = new ComputeBuffer(cubeData.Length, totalSize);
            for (int i = 0; i < objectCount; i++)
            {
                cubeData[i].position = cubeHolders[i].transform.position;
            }
            computeBuffer.SetData(cubeData);
            cubeController.SetBuffer(0, "cubes", computeBuffer);
            cubeController.Dispatch(0, cubeData.Length / 4, cubeData.Length / 8, 1);
            computeBuffer.GetData(cubeData);
            for (int i = 0; i < cubeHolders.Length; i++)
            {
                cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", cubeData[i].color);
                cubeHolders[i].transform.position -= new Vector3(0, cubeData[i].position.y, 0) * Time.deltaTime;
            }
            computeBuffer.Dispose();

        }
    }
}
