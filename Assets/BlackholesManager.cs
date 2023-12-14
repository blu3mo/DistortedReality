using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholesManager : MonoBehaviour
{

    public GameObject blackholePrefab;
    public GameObject rightController;
    public GameObject leftController;

    public GameObject[] blackholeTargets;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    GameObject currentlyEditingBlackhole;
    Vector3 editingStartPosition;

    // Update is called once per frame
    void Update()
    {
        // when vr controller index trigger gets pushed, create a blackhole and record its movement until release.
        // use XRI to get the controller button press
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1");
            // get the controller position
            Vector3 controllerPosition = rightController.transform.position;
            // create a blackhole
            GameObject blackhole = Instantiate(blackholePrefab, controllerPosition, Quaternion.identity);
            blackhole.transform.parent = this.transform;
            blackhole.GetComponent<UpdateBlackholeShaders>().blackholeTargets = blackholeTargets;
            currentlyEditingBlackhole = blackhole;
        } else if (Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Fire1 up");
            currentlyEditingBlackhole = null;
        }

        if (currentlyEditingBlackhole != null)
        {
            // get the controller position
            Vector3 controllerPosition = rightController.transform.position;
            // get the blackhole position
            Vector3 blackholePosition = currentlyEditingBlackhole.transform.position;
            // get the distance between the two
            Vector3 distance = controllerPosition - blackholePosition;
            // set the blackhole scale to the distance
            currentlyEditingBlackhole.GetComponent<UpdateBlackholeShaders>().blackholeStrength = distance.magnitude;
            // get child object with particle system, and change the scale of that too
            currentlyEditingBlackhole.transform.GetChild(0).localScale = new Vector3(distance.magnitude, distance.magnitude, distance.magnitude);
        }

    }
}
