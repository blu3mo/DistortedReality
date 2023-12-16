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

    // Start is called before the first frame update
    void Start()
    {
        Blackhole blackhole1 = new Blackhole();
        blackhole1.position = new Vector3(0, 0, 0);
        blackhole1.rotation = new Vector3(2f, 0f, 0f);
        blackhole1.strength = 0f;
        blackholes.Add(blackhole1);
        // Blackhole blackhole2 = new Blackhole();
        // blackhole2.position = new Vector3(8f, 0, 0);
        // blackhole2.rotation = new Vector3(0f, 2f, 0f);
        // blackhole2.strength = 0f;
        // blackholes.Add(blackhole2);
        // Blackhole blackhole3 = new Blackhole();
        // blackhole3.position = new Vector3(-8f, 0, 0);
        // blackhole3.rotation = new Vector3(0f, 0f, 2f);
        // blackhole3.strength = 0f;
        // blackholes.Add(blackhole3);
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
        float[] blackholePositionsX = new float[length];
        float[] blackholePositionsY = new float[length];
        float[] blackholePositionsZ = new float[length];
        float[] blackholeRotationsX = new float[length];
        float[] blackholeRotationsY = new float[length];
        float[] blackholeRotationsZ = new float[length];
        float[] blackholeStrengths = new float[length];
        print(length);
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
}
