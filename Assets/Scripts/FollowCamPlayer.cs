#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class FollowCamPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1, 10)] public float smoothFactor;
    public bool followGround = false;
    float goalAltitude;

    [HideInInspector]
    public Vector3 minValues, maxValues;


    //Editors Fields
    [HideInInspector]
    public bool setupComplete = false;
    public enum SetupState { NONE, Step1, Step2}
    [HideInInspector]
    public SetupState setupState= SetupState.NONE;

    void Start()
    {
        if(followGround)
        {
            goalAltitude = target.position.y;
        }
    }

    void OnEnable()
    {
        Fox.HasLanded += UpdateCameraAltitude;
    }

    void OnDisable()
    {
        Fox.HasLanded -= UpdateCameraAltitude;
    }

    void UpdateCameraAltitude()
    {
        if (!followGround)
            return;
        goalAltitude = target.position.y;
    }

    private void FixedUpdate()
    {
        Follow();
    }
    void Follow()
    {
        Vector3 targetPosition = target.position + offset;

        //If follow ground, modify the y value accordingly
        if (followGround)
            targetPosition.y = goalAltitude + offset.y;

        //Check if the targetPosition is out of bounds or not
        //Limit it to the minimum and maximum values
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z));


        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = targetPosition;
    }

    public void ResetValues()
    {
        setupComplete = false;
        minValues = Vector3.zero;
        maxValues = Vector3.zero;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FollowCamPlayer))]
public class CameraFollowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Assign the MonoBehaviour target script
        var script = (FollowCamPlayer)target;
        //Check if values are setup or not

        //Spacing 
        GUILayout.Space(20);


        //This to decorate in the inspector
        GUILayout.Label("Camera Boundary Settings");
        //If they are setup display the Minimum and Maximum values along with preview button
        //Also have a reset buton for the values
        if(script.setupComplete)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Minimum Values");
            GUILayout.Label("Maximum Values");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"X = {script.minValues.x}");
            GUILayout.Label($"X = {script.maxValues.x}");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Y = {script.minValues.y}");
            GUILayout.Label($"Y = {script.maxValues.y}");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if(GUILayout.Button("View Minimum"))
            {
                //Snap the camera view to the minimum values
                Camera.main.transform.position = script.minValues;
            }
            if (GUILayout.Button("View Maximum"))
            {
                //Snap the camera view to the maximum values
                Camera.main.transform.position = script.maxValues;
            }
            GUILayout.EndHorizontal();

            //Reset the view to the player
            if (GUILayout.Button("Focus on Player"))
            {
                Vector3 targetPos = script.target.position+script.offset;
                targetPos.z = script.minValues.z;
                Camera.main.transform.position = targetPos;
            }

            if (GUILayout.Button("Reset Camera Values"))
            {
                //Reset the setupcomplete bool
                //reset the min and max vector3 values
                script.ResetValues();
            }
        }

        //If they are not setup display a start setup button
        else
        {
            //Step 0 : Show the start wizard button
            if(script.setupState == FollowCamPlayer.SetupState.NONE)
            {
                if (GUILayout.Button("Start setting Camera Values"))
                {
                    //Change the state to step1
                    script.setupState = FollowCamPlayer.SetupState.Step1;
                }
            }
            //Step 1 : Setup the bottom left boundary (minimum values)
            else if (script.setupState == FollowCamPlayer.SetupState.Step1)
            {
                //Instruction on what to do
                GUILayout.Label($"1- Select your main Camera");
                GUILayout.Label($"2- Move it to the bottom left bound limit of your level");
                GUILayout.Label($"3- Click the 'Set Minimum Values' button ");
                //Button to set the min values
                if (GUILayout.Button("View Minimum values"))
                {
                    //Set the minimum values of the camera limit
                    script.minValues = Camera.main.transform.position;
                    //Change to step 2
                    script.setupState = FollowCamPlayer.SetupState.Step2;
                }
            }
            //Step 2 : Setup the top right boundary (maximum values)
            else if (script.setupState == FollowCamPlayer.SetupState.Step2)
            {
                //Instruction on what to do
                GUILayout.Label($"1- Select your main Camera");
                GUILayout.Label($"2- Move it to the top right bound limit of your level");
                GUILayout.Label($"3- Click the 'Set Maximum Values' button ");
                //Button to set the max values
                if (GUILayout.Button("View Maximum values"))
                {
                    //Snap the camera view to the maximum values
                    script.maxValues = Camera.main.transform.position;
                    //Set the state to NONE
                    script.setupState = FollowCamPlayer.SetupState.NONE;
                    //Enable the steupComplete bool
                    script.setupComplete = true;

                    //Reset view to Player
                    Vector3 targetPos = script.target.position + script.offset;
                    targetPos.z = script.minValues.z;
                    Camera.main.transform.position = targetPos;
                }
            }                      
        }



    }
}
#endif