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
    [SerializeField] TextMeshProUGUI timer;
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
    [SerializeField] ComputeShader colorController;
    [SerializeField] bool started = false;
    public Material material;
    ComputeBuffer computeBuffer;
    ComputeBuffer colorBuffer;
    private int finalized, recolored;
    private float localSpeed;
    Color[] cubeColors, colorsVerify;
    public GameObject gpuButton;
    public GameObject cpuButton;
    [SerializeField] int iteractions;
    float startTime;

    private bool cpustarted = false;
    private bool checkEnd;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = Vector3.up * 100;
    }
    public void StartSimulationCPU()
    {
        cpuButton.SetActive(false);
        objectCount = int.Parse(objectsInSimulationInput.text);
        minMass = float.Parse(minMassInput.text);
        maxMass = float.Parse(maxMassInput.text);
        cubeHolders = new GameObject[objectCount];
        
        Color startColor;
        cubeColors = new Color[objectCount];
        colorsVerify = new Color[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            startColor = Random.ColorHSV();
            float offsetX = -objectCount / 2 + i;
            cubeHolders[i] = Instantiate(cubeObj, new Vector3(offsetX * 1.5f, startPosition.y, startPosition.z - 5), Quaternion.identity);
            cubeHolders[i].GetComponent<MeshRenderer>().material = new Material(material);
            cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            cubeColors[i] = cubeHolders[i].GetComponent<MeshRenderer>().material.GetColor("_Color");
        }

        startTime = Time.time;
        timer.text = startTime + "";

        cpustarted = true;

    }
    public void StartSimulation()
    {
        gpuButton.SetActive(false);

        if (started == false)
        {
            objectCount = int.Parse(objectsInSimulationInput.text);
            minMass = float.Parse(minMassInput.text);
            maxMass = float.Parse(maxMassInput.text);
            cubeData = new Cube[objectCount];
            cubeHolders = new GameObject[objectCount];
            cubeColors = new Color[objectCount];
            colorsVerify = new Color[objectCount];

            for (int i = 0; i < objectCount; i++)
            {
                float offsetX = -objectCount / 2 + i;
                cubeHolders[i] = Instantiate(cubeObj, new Vector3(offsetX * 1.5f, startPosition.y, startPosition.z), Quaternion.identity);
                cubeHolders[i].GetComponent<MeshRenderer>().material = new Material(material);
                cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                cubeColors[i] = cubeHolders[i].GetComponent<MeshRenderer>().material.GetColor("_Color");
                cubeData[i].mass = Random.Range(minMass, maxMass);
                cubeData[i].color = cubeHolders[i].GetComponent<MeshRenderer>().material.color;
                cubeData[i].dTime = 0;
                cubeData[i].position = cubeHolders[i].transform.position;
            }
            int colorSize = sizeof(float) * 4;
            int vector3Size = sizeof(float) * 3;
            int variables = sizeof(float) * 6;
            totalSize = colorSize + vector3Size + variables;

            started = true;
        }
        startTime = Time.time;
        timer.text = startTime + "";
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }



        if (started)
        {
            newTime += 1 * Time.fixedDeltaTime;
            for (int i = 0; i < cubeHolders.Length; i++)
            {
                cubeData[i].dTime = newTime - oldTime;
            }
            computeBuffer = new ComputeBuffer(cubeData.Length, totalSize);

            computeBuffer.SetData(cubeData);
            
            cubeController.SetInt("iteractions", iteractions);
            cubeController.SetBuffer(0, "cubes", computeBuffer);
            cubeController.Dispatch(0, cubeData.Length / 20, 1, 1);       
            computeBuffer.GetData(cubeData);
            computeBuffer.Dispose();        

            for (int i = 0; i < cubeHolders.Length; i++)
            {
                if (cubeHolders[i].transform.position.y > 2)
                {
                    cubeHolders[i].transform.position -= new Vector3(0, cubeData[i].position.y, 0) * Time.deltaTime;
                }
                else
                {                                       
                    finalized += 1;
                }
            }
            oldTime = newTime;

            if (finalized == cubeHolders.Length)
            {
                started = false;
                checkEnd = true;

                colorBuffer = new ComputeBuffer(cubeData.Length, totalSize);
                colorBuffer.SetData(cubeData);

                colorController.SetInt("iteractions", iteractions);
                colorController.SetBuffer(0, "cubes", colorBuffer);
                colorController.Dispatch(0, cubeData.Length / 8, 1, 1);
                colorBuffer.GetData(cubeData);
                colorBuffer.Dispose();

                for (int i = 0; i < cubeHolders.Length; i++)
                {
                    cubeHolders[i].transform.position = new Vector3(cubeHolders[i].transform.position.x, 1, cubeHolders[i].transform.position.z);
                    cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", cubeData[i].color);
                }
            }
        }

        if (cpustarted)
        {
            newTime += 1 * Time.fixedDeltaTime;
            localSpeed += newTime * 9.8f;

            for (int i = 0; i < cubeHolders.Length; i++)
            {
                if (cubeHolders[i].transform.position.y > 2)
                {
                    cubeHolders[i].transform.position -= new Vector3(0, localSpeed, 0) * Time.deltaTime;
                }
                else
                {

                    cubeHolders[i].transform.position = new Vector3(cubeHolders[i].transform.position.x, 1, cubeHolders[i].transform.position.z);

                    //for (int j = 0; j < iteractions; j++)
                    //{
                        cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                    //}

                    finalized += 1;
                }

            }
            if (finalized == cubeHolders.Length)
            {
                cpustarted = false;
                checkEnd = true;
            }        

            oldTime = newTime;
        }

        if (checkEnd)
        {
            for (int i = 0; i < objectCount; i++)
            {
                if (cubeHolders[i].GetComponent<MeshRenderer>().material.GetColor("_Color") != cubeColors[i])
                {
                    colorsVerify[i] = cubeColors[i];
                }
            }
            foreach (Color c in colorsVerify)
            {
                if (c != null)
                {
                    recolored++;
                }
            }
            if (recolored == objectCount)
            {
                StopTimer();
            }
        }
    }

    void StopTimer()
    {
        timer.text = Time.time - startTime + "";
    }
}
