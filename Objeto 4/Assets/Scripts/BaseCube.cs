using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCube : MonoBehaviour
{

    struct Cube {
        public Vector3 position;
        public Color color;
        public float mass, speed, deltaT, t;
    }

    public float mass;
    [SerializeField] ComputeShader cubeController = default;
    public int iteractions;

    // Start is called before the first frame update
    void Start()
    {
        cubeController.SetFloat("mass", mass);
        cubeController.SetFloat("position", transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        cubeController.SetFloat("t", Time.time); 
    }
}
