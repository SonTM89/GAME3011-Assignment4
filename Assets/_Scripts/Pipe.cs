using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [Header("Pipe Attributes")]
    public int[] connectorValues;
    public float rotationSpeed = 0;

    [Header("References")]
    public GameManager gameMgr;

    float realRotation;

    // Start is called before the first frame update
    void Start()
    {
        gameMgr = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if(transform.eulerAngles.z != realRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, realRotation), rotationSpeed);
        }
    }


    // Rotating Pipe by Left Mouse clicking
    void OnMouseDown()
    {
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            // Calculate number of connectors after rotation to change value of total current connectors

            int connectorCount = -gameMgr.CalculateConnectors((int)transform.position.x, (int)transform.position.y);

            RotatePipe();

            connectorCount += gameMgr.CalculateConnectors((int)transform.position.x, (int)transform.position.y);

            gameMgr.gameBoard.curConnectors += connectorCount;
        }  
    }


    // Pipe Rotating
    public void RotatePipe()
    {
        realRotation += 90;

        if(realRotation == 360)
        {
            realRotation = 0;
        }

        RotateValue();
    }


    // Change value of connectors to suit with rotating action
    public void RotateValue()
    {
        int upConnectorValue = connectorValues[(int)Position.UP];

        for(int i = 0; i < connectorValues.Length; i++)
        {
            if (i + 1 < connectorValues.Length)
            {
                connectorValues[i] = connectorValues[i + 1];
            }
            else
            {
                connectorValues[i] = upConnectorValue;
            }
        }
    }
}
