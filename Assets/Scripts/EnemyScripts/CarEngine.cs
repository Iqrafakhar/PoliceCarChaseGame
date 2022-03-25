using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    //parent objet of the path
    public Transform path;
    private List<Transform> nodes;
    private int currentNode = 0;

    //Wheel Movement
    private float MaxsteerAngle = 45f;
    public WheelCollider wheelfl;
    public WheelCollider wheelfr;
    public float maxMotorTorque = 250f;
    public float currentSpeed;
    public float maxSpeed = 100f;

    //Stsbility
    public Vector3 centerOfMass;

    //Brakes
    public bool isBrake = false;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer;
    public Renderer carRenderer2;
    public WheelCollider wheelrf;
    public WheelCollider wheelrr;
    public float maxBrakeTorque = 150f;

    //Sensors
    [Header("Sensors")]
    public float SensorLength = 5;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.6f, 2.1f);
    public float frontSideSensor = 0.6f;
    public float frontSensorAngle = 40f;
    private bool avoiding = false;

    //we need to get the all node for the path we used it in path script so let's get that 
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Sensors();
        ApplySteer();
        Movement();
        WayPoint();
        Braking();
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidingMultiplier = 0;
        avoiding = false;
    
        //front right side sensor 
        sensorStartPos += transform.right * frontSideSensor;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidingMultiplier -= 1f;
            }
        }
        

        //front right side angle  sensor 
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidingMultiplier -= 0.5f;
            }
        }
        

        //front  left side sensor 
        sensorStartPos -= 2 * frontSideSensor * transform.right;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidingMultiplier += 1f;
            }
        }
        

        //front left side angle  sensor 
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, SensorLength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidingMultiplier += 0.5f;
            }
        }
        //Front center sensors
        if (avoidingMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, SensorLength))
            {
                if (!hit.collider.CompareTag("Terrain"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    if(hit.normal.x < 0)
                    {
                        avoidingMultiplier = -1;
                    }
                    else
                    {
                        avoidingMultiplier = 1;
                    }
                }
            }
        }

        if (avoiding)
        {
            wheelfl.steerAngle = MaxsteerAngle * avoidingMultiplier;
            wheelfr.steerAngle = MaxsteerAngle * avoidingMultiplier;
        }
    }

    private void ApplySteer()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        //relativeVector = relativeVector / relativeVector.magnitude;
        //we need to devide only x to get 1| -1 so we don't have to do the upper line
        float newSteer = (relativeVector.x / relativeVector.magnitude) * MaxsteerAngle;
        wheelfl.steerAngle = newSteer;
        wheelfr.steerAngle = newSteer;
    }
    private void Movement()
    {
        currentSpeed = 2 * Mathf.PI * wheelfl.radius * wheelfl.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBrake)
        {
            wheelfl.motorTorque = maxMotorTorque;
            wheelfr.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelfl.motorTorque = 0f;
            wheelfr.motorTorque = 0f;
        }
    }

    private void WayPoint()
    {
        if(Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            if(currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    private void Braking()
    {
        /*if (Input.GetKeyDown(KeyCode.B))
        { 
            isBrake = true; 
        }*/
       if(isBrake)
        {
            carRenderer.material.mainTexture = textureBraking;
            carRenderer2.material.mainTexture = textureBraking;
            wheelrf.brakeTorque = maxBrakeTorque;
            wheelrr.brakeTorque = maxBrakeTorque;
        }
        else
        {
            carRenderer.material.mainTexture = textureNormal;
            carRenderer2.material.mainTexture = textureNormal;
            wheelrf.brakeTorque = 0f;
            wheelrr.brakeTorque = 0f;
        }
    }
}
