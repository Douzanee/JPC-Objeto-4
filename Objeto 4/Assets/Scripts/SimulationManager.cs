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
        public Vector3 p2;
        public Color color;
        public float v1;
        public float v2;
        public float mass;
        public float dT;
        public float dS;
    };
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
        startPosition = Vector3.up * 20;
    }
    public void StartSimulationCPU()
    {
        cpuButton.SetActive(false);
        objectCount = int.Parse(objectsInSimulationInput.text);
        minMass = float.Parse(minMassInput.text);
        maxMass = float.Parse(maxMassInput.text);
        cubeData = new Cube[objectCount];
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
            cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
            cubeColors[i] = cubeHolders[i].GetComponent<MeshRenderer>().material.GetColor("_Color");
        }
        StartTimer();

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
                cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                cubeColors[i] = cubeHolders[i].GetComponent<MeshRenderer>().material.GetColor("_Color");
                cubeData[i].mass = Random.Range(minMass, maxMass);
                cubeData[i].color = cubeHolders[i].GetComponent<MeshRenderer>().material.color;
                cubeData[i].dT = 0;
                cubeData[i].v1 = 0;
                cubeData[i].v2 = 0;
                cubeData[i].position = cubeHolders[i].transform.position;
                cubeData[i].p2 = cubeHolders[i].transform.position;
            }
            int colorSize = sizeof(float) * 4;
            int vector3Size = sizeof(float) * 3;
            int vector3SizeFelipe = sizeof(float) * 3;
            int variables = sizeof(float) * 5;
            totalSize = colorSize + vector3Size + vector3SizeFelipe + variables;


            StartTimer();

            started = true;
        }
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
                cubeData[i].dT = newTime - oldTime;
                
            }
            computeBuffer = new ComputeBuffer(cubeData.Length, totalSize);

            computeBuffer.SetData(cubeData);

            cubeController.SetBuffer(0, "cubes", computeBuffer);

            cubeController.Dispatch(0, Mathf.CeilToInt(cubeData.Length / 64), 1, 1);

            computeBuffer.GetData(cubeData);
            
            computeBuffer.Dispose();


            for (int i = 0; i < cubeHolders.Length; i++)
            {
                if (cubeHolders[i].transform.position.y > 1)
                {
                    cubeHolders[i].transform.position = new Vector3(cubeData[i].p2.x, cubeData[i].p2.y, cubeData[i].p2.z);
                    
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
                colorController.Dispatch(0, Mathf.CeilToInt(cubeData.Length / 64), 1, 1);
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
            float dT = newTime - oldTime;

            for (int i = 0; i < cubeHolders.Length; i++)
            {
                if (cubeHolders[i].transform.position.y > 1)
                {
                    cubeData[i].v2 = cubeData[i].v1 + 9.8f * dT;

                    cubeData[i].position = new Vector3(cubeData[i].position.x, (cubeData[i].v1 + cubeData[i].v2) * dT / 2, cubeData[i].position.z);

                    cubeHolders[i].transform.position -= cubeData[i].position;
                    cubeData[i].v1 = cubeData[i].v2;
                }
                else
                {

                    cubeHolders[i].transform.position = new Vector3(cubeHolders[i].transform.position.x, 1, cubeHolders[i].transform.position.z);

                    //for (int j = 0; j < iteractions; j++)
                    //{

                    for (int j = 0; j < iteractions; j++)
                    {
                        cubeHolders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Random.ColorHSV());
                        //}}
                    }

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
    public void StartTimer()
    {
        startTime = Time.realtimeSinceStartup;
    }

    void StopTimer()
    {
        timer.text = Time.realtimeSinceStartup - startTime + "";
    }
}
