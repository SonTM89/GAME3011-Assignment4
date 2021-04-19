using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    public int[] value;

    public float rotationSpeed = 0;

    float realRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.root.eulerAngles.z != realRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, realRotation), rotationSpeed);
        }
    }


    void OnMouseDown()
    {
        RotatePipe();
    }


    public void RotatePipe()
    {
        realRotation += 90;

        if(realRotation == 360)
        {
            realRotation = 0;
        }

        

        RotateValue();
    }


    public void RotateValue()
    {
        int firstVal = value[0];

        for(int i = 0; i < value.Length; i++)
        {
            if (i + 1 < value.Length)
            {
                value[i] = value[i + 1];
            }
            else
            {
                value[i] = firstVal;
            }
        }
    }
}
