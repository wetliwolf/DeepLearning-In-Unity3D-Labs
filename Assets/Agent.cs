using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneticAlgorithm;
using System.Threading;

public class Agent : MonoBehaviour {

    const float multiplier = .3f;
    public float speed = 0, rotate = 0;
    Rigidbody rigidbody;
    public GameObject eye_sensor;
    public Material neuronMaterial;
    RaycastHit hit;
    public sbyte[] genome;

    double health = 1, startTime, points = 0;

    public int genomeId = -1;
    public GeneticController gController;
    public double[] inputs, outputs;
    public double fitness;

    public LayerMask lm;

    const int rays = 20;
    const float deg = 120;

    void Start() {
        //Physics.IgnoreLayerCollision(8, 8, true);
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        startTime = Time.time;
        inputs = new double[rays * 3];
        outputs = new double[1];

        if (genomeId == 0) gController.printANetwork(genomeId, neuronMaterial);
    }

    void Update() {
        
        for (int i = 0; i < rays * 3; i += 3)
        {
            if (Physics.Raycast(eye_sensor.transform.position, Quaternion.Euler(0, -deg / 2 + i * (deg / rays) / 2, 0) * transform.forward, out hit, 15, lm))
            {
                Color color = Color.red;

                if (hit.transform.tag == "point")
                {
                    inputs[i] = double.Parse(hit.transform.name) / 2;
                    inputs[i + 1] = 0;
                    inputs[i + 2] = 0;
                    color = Color.green;
                }
                else if (hit.transform.tag == "wall")
                {
 