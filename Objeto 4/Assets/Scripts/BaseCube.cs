using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCube : MonoBehaviour
{
    public float mass;
    [SerializeField] ComputeShader cubeController = default;

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
