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
            return -10 * Mathf.Log(transform.localScale.x);
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

    private GameObject blackhole_in;
    private GameObject blackhole_out;

    void Start()
    {
        // get prefab blackhole_in
        blackhole_in = Instantiate(Resources.Load("blackhole_in") as GameObject);
        blackhole_out = Instantiate(Resources.Load("blackhole_out") as GameObject);
        blackhole_in.transform.parent = transform;
        blackhole_in.transform.localPosition = new Vector3(0, 0, 0);
        blackhole_out.transform.parent = transform;
        blackhole_out.transform.localPosition = new Vector3(0, 0, 0);

        // add collider
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.radius = 0.1f;
    }
    // update
    void Update()
    {
        if (transform.localScale.x > 1)
        {
            blackhole_in.SetActive(false);
            blackhole_out.SetActive(true);
            float diff = 0.5f + 0.1f * Mathf.Log(transform.localScale.x);
            blackhole_out.transform.localScale = new Vector3(diff, diff, diff);
        }
        else
        {
            blackhole_in.SetActive(true);
            float diff = 0.5f + -0.1f * Mathf.Log(transform.localScale.x);
            blackhole_in.transform.localScale = new Vector3(diff, diff, diff);
            blackhole_out.SetActive(false);
        }
    }
}
