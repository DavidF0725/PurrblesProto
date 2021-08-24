using UnityEngine;

public class FollowCamPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    [Range(1, 10)] public float smoothFactor;
    public Vector3 minValues, maxValues;


    private void FixedUpdate()
    {
        Follow();
    }
    void Follow()
    {
        //TODO:
        //Define minimum x,y,z values and maximum x,y,z values

        Vector3 targetPosition = target.position + offset;
        //Check if the targetPosition is out of bounds or not
        //Limit it to the minimum and maximum values
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z));


        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, smoothFactor * Time.fixedDeltaTime);
        transform.position = targetPosition;
    }
}
