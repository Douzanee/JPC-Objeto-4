using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    struct Cube
    {
        public Vector3 position;
        public Color color;
        public float speed, mass, v1, v2, dTime, force;
    }

    [SerializeField] TMP_InputField objectsInSimulationInput;
    [SerializeField] TMP_InputField minMassInput;
    [SerializeField] TMP_InputField maxMassInput;
    public int objectCount;
    public int totalSize;
    public float minMass;
    public float maxMass;
    public float oldTime, newTime;
    public GameObject cubeObj;
    public GameObject[] cubeHolders;
    Vector3 startPosition;
    Cube[] cubeData;
    [SerializeField] ComputeShader cubeController;
    [SerializeField] bool started = false;
    public Material material;
    ComputeBuffer computeBuffer;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = Vector3.up * 100;
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

            Color startColor;

            for (int i = 0; i < objectCount; i++)
            {
                startColor = Random.ColorHSV();
                float offsetX = -objectCount / 2 + i;
                cubeHolders[i] = Instantiate(cubeObj, new Vector3(offsetX * 1.5f, startPosition.y + 10, startPosition.z), Quaternion.identity);
                cubeData[i].mass = Random.Range(minMass, maxMass);
                cubeData[i].position = cubeHolders[i].transform.position;
                cubeHolders[i].GetComponent<MeshRenderer>().material = new Material(material);
                cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", startColor);
                cubeData[i].color = cubeHolders[i].GetComponent<MeshRenderer>().material.color;
                //cubeData[i].speed = 9.8f * Time.deltaTime;
                cubeData[i].dTime = 0;
                cubeData[i].position = cubeHolders[i].transform.position;
            }
            int colorSize = sizeof(float) * 4;
            int vector3Size = sizeof(float) * 3;
            int variables = sizeof(float) * 6;
            totalSize = colorSize + vector3Size + variables;

            started = true;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
        if (started)
        {
            newTime += 1 * Time.deltaTime;
            for (int i = 0; i < objectCount; i++)
            {
                cubeData[i].dTime = newTime - oldTime;
            }
            computeBuffer = new ComputeBuffer(cubeData.Length, totalSize);
            computeBuffer.SetData(cubeData);
            cubeController.SetBuffer(0, "cubes", computeBuffer);
            cubeController.Dispatch(0, cubeData.Length / 12, cubeData.Length / 3, 1);
            computeBuffer.GetData(cubeData);
            for (int i = 0; i < cubeHolders.Length; i++)
            {
                if (cubeHolders[i].transform.position.y > 1)
                {
                    cubeHolders[i].transform.position -= new Vector3(0, cubeData[i].position.y, 0) * Time.deltaTime;
                }
                else
                {
                    cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", cubeData[i].color);
                }
            }
            computeBuffer.Dispose();
            oldTime = newTime;

        }
    }
}
