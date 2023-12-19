using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class BlackholesManager : MonoBehaviour
{

    public GameObject blackholePrefab;
    public GameObject rightController;
    public GameObject leftController;

    public GameObject[] blackholeTargets;
    public List<Blackhole> blackholes = new List<Blackhole>();

    public GameObject blackHolePrefab_in;
    public GameObject blackHolePrefab_out;

    // Start is called before the first frame update
    void Start()
    {
        GameObject blackholeObject = Instantiate(blackHolePrefab_out, new Vector3(0, 0, 0), Quaternion.identity);
        Blackhole blackhole1 = blackholeObject.AddComponent<Blackhole>();
        blackholeObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        blackholes.Add(blackhole1);

        GameObject blackholeObject2 = Instantiate(blackHolePrefab_out, new Vector3(5, 0, 0), Quaternion.identity);
        Blackhole blackhole2 = blackholeObject2.AddComponent<Blackhole>();
        blackholeObject2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        blackholes.Add(blackhole2);
    }

    //GameObject currentlyEditingBlackhole;
    //Vector3 editingStartPosition;

    // Update is called once per frame
    void Update()
    {
        UpdateShaders();
    }

    private void UpdateShaders() {

        int length = Math.Min(100, blackholes.Count);
        if (length == 0) {
            return;
        }
        float[] blackholePositionsX = new float[length];
        float[] blackholePositionsY = new float[length];
        float[] blackholePositionsZ = new float[length];
        float[] blackholeRotationsX = new float[length];
        float[] blackholeRotationsY = new float[length];
        float[] blackholeRotationsZ = new float[length];
        float[] blackholeStrengths = new float[length];
        for (int i = 0; i < length; i++)
        {
            Blackhole blackhole = blackholes[i];
            blackholePositionsX[i] = blackhole.position.x;
            blackholePositionsY[i] = blackhole.position.y;
            blackholePositionsZ[i] = blackhole.position.z;
            blackholeRotationsX[i] = blackhole.rotation.x;
            blackholeRotationsY[i] = blackhole.rotation.y;
            blackholeRotationsZ[i] = blackhole.rotation.z;
            blackholeStrengths[i] = blackhole.strength;
        }
        foreach(GameObject blackholeTarget in blackholeTargets) {
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholePositionX", blackholePositionsX);
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholePositionY", blackholePositionsY);
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholePositionZ", blackholePositionsZ);
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholeRotationX", blackholeRotationsX);
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholeRotationY", blackholeRotationsY);
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholeRotationZ", blackholeRotationsZ);
            blackholeTarget.GetComponent<Renderer>().material.SetFloatArray("_BlackholeStrength", blackholeStrengths);
        }
    }

    // private void MergeColliders()
    // {
    //     foreach(GameObject blackHole1 in blackHoles)
    //     {
    //         foreach(GameObject blackHole2 in blackHoles)
    //         {
    //             if(blackHole1 != blackHole2)
    //             {
    //                 if (Vector3.Distance(blackHole1.transform.position, blackHole2.transform.position) < .1)
    //                 {
    //                     if (blackHole1.transform.localScale.x > blackHole2.transform.localScale.x)
    //                     {
    //                         //destroy blackHole2
    //                         Destroy(blackHole2);
    //                         blackHoles.Remove(blackHole2);
    //                         return;
    //                     }
    //                     else
    //                     {
    //                         //destroy blackHole1
    //                         Destroy(blackHole1);
    //                         blackHoles.Remove(blackHole1);
    //                         return;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    public void MakeNewHole(float distBetweenGrabs, Vector3 midpoint)
    {
        GameObject newBlackHoleObject = Instantiate(blackHolePrefab_out, midpoint, UnityEngine.Quaternion.identity);
        // get 3d angle and change to quaternion

        Blackhole blackhole = newBlackHoleObject.AddComponent<Blackhole>();
        blackholes.Add(blackhole);

    }
    public void TransformNewHole(Vector3 midpoint, float scale, Quaternion rotation)
    {
        //get gameobject of last blackhole
        GameObject lastBlackhole = blackholes.Last().gameObject;
        lastBlackhole.transform.position = midpoint;
        lastBlackhole.transform.localScale = new Vector3(scale, scale, scale);
        lastBlackhole.transform.rotation = rotation;
    }

}
