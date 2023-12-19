// C# Blackhole object with multiple properties
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Blackhole : MonoBehaviour
{
    // Properties
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }
    public float strength
    {
        get
        {
            return -30 * Mathf.Log(transform.localScale.x);
        }
    }
    public Vector3 rotation {
        get
        {
            Debug.Log("rotation: " + transform.rotation.eulerAngles);
            Vector3 rad = transform.rotation.eulerAngles * Mathf.Deg2Rad;
            if (rad.x > Mathf.PI)
            {
                rad.x -= 2 * Mathf.PI;
            }
            if (rad.y > Mathf.PI)
            {
                rad.y -= 2 * Mathf.PI;
            }
            if (rad.z > Mathf.PI)
            {
                rad.z -= 2 * Mathf.PI;
            }
            return rad;
        }
    }
}
