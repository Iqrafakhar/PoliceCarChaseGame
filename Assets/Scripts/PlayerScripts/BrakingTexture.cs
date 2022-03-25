using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakingTexture : MonoBehaviour
{
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer;
    public Renderer carRenderer2;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public bool isBrake = false;
    public float maxBrakeTorque = 250f;
    // Update is called once per frame
    void FixedUpdate()
    {
        Braking();
    }


    private void Braking()
    {
            if (Input.GetKeyDown(KeyCode.B))
            { 
                isBrake = true; 
            }
            if (isBrake)
            {
               carRenderer.material.mainTexture = textureBraking;
                carRenderer2.material.mainTexture = textureBraking;
                wheelRL.brakeTorque = maxBrakeTorque;
                wheelRR.brakeTorque = maxBrakeTorque;
            }
            else
            {
                carRenderer.material.mainTexture = textureNormal;
                carRenderer2.material.mainTexture = textureNormal;
                wheelRL.brakeTorque = 0f;
                wheelRR.brakeTorque = 0f;
            }
        }
    
}
