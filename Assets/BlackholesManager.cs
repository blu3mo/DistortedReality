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
        // GameObject blackholeObject = Instantiate(blackHolePrefab_out, new Vector3(0, 0, 0), Quaternion.identity);
        // Blackhole blackhole1 = blackholeObject.AddComponent<Blackhole>();
        // blackholeObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        // blackholes.Add(blackhole1);

        // GameObject blackholeObject2 = Instantiate(blackHolePrefab_out, new Vector3(5, 0, 0), Quaternion.identity);
        // Blackhole blackhole2 = blackholeObject2.AddComponent<Blackhole>();
        // blackholeObject2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        // blackholes.Add(blackhole2);
    }

    //GameObject currentlyEditingBlackhole;
    //Vector3 editingStartPosition;

    // Update is called once per frame
    void Update()
    {
        UpdateShaders();
    }

    private void UpdateShaders() {

        int length = 100;//Math.Min(100, blackholes.Count);
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
        for (int i = 0; i < Math.Min(100, blackholes.Count); i++)
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

    private void MergeColliders()
    {
        foreach(Blackhole blackHole1 in blackholes)
        {
            foreach(Blackhole blackHole2 in blackholes)
            {
                if(blackHole1 != blackHole2)
                {
                    if (Vector3.Distance(blackHole1.transform.position, blackHole2.transform.position) < .2)
                    {
                        if (blackHole1.transform.localScale.x > blackHole2.transform.localScale.x)
                        {
                            //add scale and rotation of blackhole 2 to blackhole 1.
                            blackHole1.transform.localScale += blackHole2.transform.localScale;
                            blackHole1.transform.rotation *= blackHole2.transform.rotation;
                            //destroy blackHole2
                            Destroy(blackHole2.gameObject);
                            blackholes.Remove(blackHole2);
                            return;
                        }
                        else
                        {
                            //add scale and rotation of blackhole 1 to blackhole 2.
                            blackHole2.transform.localScale += blackHole1.transform.localScale;
                            blackHole2.transform.rotation *= blackHole1.transform.rotation;
                            //destroy blackHole1
                            Destroy(blackHole1.gameObject);
                            blackholes.Remove(blackHole1);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void MakeNewHole(float distBetweenGrabs, Vector3 midpoint)
    {
        GameObject newBlackHoleObject = new GameObject();//Instantiate(blackHolePrefab_out, midpoint, UnityEngine.Quaternion.identity);
        newBlackHoleObject.transform.position = midpoint;
        newBlackHoleObject.transform.localScale = new Vector3(1, 1, 1);
        newBlackHoleObject.transform.rotation = UnityEngine.Quaternion.identity;
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
