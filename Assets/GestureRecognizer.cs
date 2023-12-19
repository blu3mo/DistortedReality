using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Unity.XR.CoreUtils;
using Unity.Mathematics;



#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Gesture
{
    public string gestureName;
    [HideInInspector]
    public List<Vector3> positionsPerFinger; // Relative to hand
    
    [HideInInspector]
    public UnityEvent onRecognized;

    [HideInInspector]
    public float time = 0.0f;

    public Gesture(string name, List<Vector3> positions)
    {
        gestureName = name;
        positionsPerFinger = positions;
        onRecognized = new UnityEvent();
    }
}

//[DisallowMultipleComponent]
public class GestureRecognizer : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField] private List<Gesture> savedGestures = new List<Gesture>();
    [SerializeField] private float threshold = 0.05f;
    [SerializeField] private float delay = 0.2f;
    //[SerializeField] private UnityEvent onNothingDetected = default;

    [Header("Objects")]
    [SerializeField] private GameObject hand_left = default;
    [SerializeField] private GameObject grip_left = default;
    [SerializeField] private GameObject[] fingers_left = default;
    [SerializeField] private GameObject hand_right = default;
    [SerializeField] private GameObject grip_right = default;
    [SerializeField] private GameObject[] fingers_right = default;

    [Header("Debugging")]
    [SerializeField] private Gesture rightGrabDetected = default;
    [SerializeField] private Gesture leftGrabDetected = default;
    //[SerializeField] private Vector3 grabTranslation;
    //[SerializeField] public float distBetweenGrabs;
    //[SerializeField] private Vector3 grabVelocity;

    private List<GameObject> blackHoles = new List<GameObject>();
    private GameObject newBlackHole;
    private Vector3 _previousGrabPosition;
    private quaternion _previousGrabRotation;
    private float _startingDistandceBetweenGrabs;
    private Vector3 _startingGrabAxis;
    private Gesture grabLeft=null, grabRight=null;
    private bool _previousDoubleGrab = false;

    private BlackholesManager blackholesManager;

    private void Start()
    {
        blackholesManager = GetComponent<BlackholesManager>();

        // wait for 3 sec, then save left and right grab
        Invoke("SaveLeftGrab", 3f);
        Invoke("SaveRightGrab", 3f);
    }

    private void Update()
    {

        //gestureDetected = Recognize();
        leftGrabDetected = RecognizeLeft();
        rightGrabDetected = RecognizeRight();
        Vector3 grabPosition = Vector3.zero;
        quaternion grabRotation = quaternion.identity;

        //if leftGrabDetected and rightGrabDetected then spawn black hole in the middle
        if (leftGrabDetected != null && rightGrabDetected != null)
        {
            Vector3 midpoint = (grip_left.transform.position + grip_right.transform.position) / 2;
            float distBetweenGrabs = Vector3.Distance(grip_left.transform.position, grip_right.transform.position);
            Vector3 grabAxis = (grip_left.transform.position - grip_right.transform.position).normalized;
            if (_previousDoubleGrab==false)
            {
                _startingGrabAxis = grabAxis;
                _startingDistandceBetweenGrabs = distBetweenGrabs;
                blackholesManager.MakeNewHole(distBetweenGrabs, midpoint);
            }
            else
            {
                float scale = math.pow(distBetweenGrabs / _startingDistandceBetweenGrabs,2);
                Quaternion rotation = Quaternion.FromToRotation(_startingGrabAxis, grabAxis);
                // multiply by 0.2
                rotation = Quaternion.Slerp(rotation, Quaternion.identity, 0.8f);
                blackholesManager.TransformNewHole(midpoint, scale, rotation);
            }
            _previousDoubleGrab = true;            
        }
        else
        {
            if (leftGrabDetected != null)
            {
                grabPosition = grip_left.transform.position;
                grabRotation = grip_left.transform.rotation;
            }

            if (rightGrabDetected != null)
            {
                grabPosition = grip_right.transform.position;
                grabRotation = grip_right.transform.rotation;
            }

            if (_previousGrabPosition != Vector3.zero && grabPosition != Vector3.zero)
            {
                Vector3 grabTranslation = grabPosition - _previousGrabPosition;
                //Debug.Log("grabTranslation: " + grabTranslation);
                quaternion grabTurn = grabRotation * Quaternion.Inverse(_previousGrabRotation);
                grabTurn = Quaternion.Slerp(grabTurn, Quaternion.identity, 0.9f);
                Collider[] colliders = Physics.OverlapSphere(grabPosition, 0.1f);
                foreach (Collider col in colliders)
                {
                    col.gameObject.transform.Translate(grabTranslation, Space.World);
                    col.gameObject.transform.rotation = grabTurn * col.gameObject.transform.rotation;
                }
            }
            _previousDoubleGrab = false;
        }
        _previousGrabPosition = grabPosition;
        _previousGrabRotation = grabRotation;
        //MergeColliders();

    }

    public void SaveLeftGrab()
    {
        List<Vector3> positions = fingers_left.Select(t => hand_left.transform.InverseTransformPoint(t.transform.position)).ToList();
        grabLeft = new Gesture("Left Grab", positions);
        savedGestures.Add(grabLeft);
    }
       public void SaveRightGrab()
    {
        List<Vector3> positions = fingers_right.Select(t => hand_right.transform.InverseTransformPoint(t.transform.position)).ToList();
        grabRight = new Gesture("Right Grab", positions);
        savedGestures.Add(grabRight);
    }
    private Gesture RecognizeLeft()
    {
        bool discardGesture = false;
        float minSumDistances = Mathf.Infinity;
        Gesture bestCandidate = null;

        if (grabLeft != null && fingers_left.Length == grabLeft.positionsPerFinger.Count)
        {
            float sumDistances = 0f;
            // For each finger
            for (int f = 0; f < fingers_left.Length; f++)
            {
                Vector3 fingerRelativePos = hand_left.transform.InverseTransformPoint(fingers_left[f].transform.position);
                // If at least one finger does not enter the threshold we discard the gesture
                if (Vector3.Distance(fingerRelativePos, grabLeft.positionsPerFinger[f]) > threshold)
                {
                    discardGesture = true;
                    grabLeft.time = 0.0f;
                    break;
                }
                // If all the fingers entered, then we calculate the total of their distances
                sumDistances += Vector3.Distance(fingerRelativePos, grabLeft.positionsPerFinger[f]);
            }
            // If we have to discard the gesture, we skip it
            if (discardGesture)
            {
                discardGesture = false;
            }
            else{
            // If it is valid and the sum of its distances is less than the existing record, it is replaced because it is a better candidate 
                if (sumDistances < minSumDistances)
                {
                    if (bestCandidate != null)
                        bestCandidate.time = 0.0f;

                    minSumDistances = sumDistances;
                    bestCandidate = grabLeft;
                }
            }
        }

        if (bestCandidate != null)
        {
            bestCandidate.time += Time.deltaTime;

            if (bestCandidate.time < delay)
                bestCandidate = null;
        }
        return bestCandidate;
    }

    private Gesture RecognizeRight()
    {
        bool discardGesture = false;
        float minSumDistances = Mathf.Infinity;
        Gesture bestCandidate = null;

        //check if grabRight is null
        if (grabRight != null && fingers_right.Length == grabRight.positionsPerFinger.Count)
        {
            float sumDistances = 0f;

            // For each finger
            for (int f = 0; f < fingers_right.Length; f++)
            {
                Vector3 fingerRelativePos = hand_right.transform.InverseTransformPoint(fingers_right[f].transform.position);

                // If at least one finger does not enter the threshold we discard the gesture
                if (Vector3.Distance(fingerRelativePos, grabRight.positionsPerFinger[f]) > threshold)
                {
                    discardGesture = true;
                    grabRight.time = 0.0f;
                    break;
                }

                // If all the fingers entered, then we calculate the total of their distances
                sumDistances += Vector3.Distance(fingerRelativePos, grabRight.positionsPerFinger[f]);
            }

            // If we have to discard the gesture, we skip it
            if (discardGesture) 
                discardGesture = false;
            else{
            // If it is valid and the sum of its distances is less than the existing record, it is replaced because it is a better candidate 
                if (sumDistances < minSumDistances)
                {
                    if (bestCandidate != null)
                        bestCandidate.time = 0.0f;

                    minSumDistances = sumDistances;
                    bestCandidate = grabRight;
                }
            }
        }

        if (bestCandidate != null)
        {
            bestCandidate.time += Time.deltaTime;

            if (bestCandidate.time < delay)
                bestCandidate = null;
        }

        return bestCandidate;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GestureRecognizer))]
public class CustomInspectorGestureRecognizer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GestureRecognizer gestureRecognizer = (GestureRecognizer)target;

        if (GUILayout.Button("Save Left Grab"))
        {
           gestureRecognizer.SaveLeftGrab();
        }
        if (GUILayout.Button("Save Right Grab"))
        {
            gestureRecognizer.SaveRightGrab();
        }
        return;
    }
}
#endif