using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement cameraFollow;
    public Transform target;
    public float damping;
    public Vector3 offset;    
    private Vector3 velocity = Vector3.zero;
    private void Start()
    {
        cameraFollow = this;
    }
    private void FixedUpdate()
    {
        if (HomeScreenManager.Instance.StartGame)
        {
            Vector3 moveposition = new Vector3(0, target.position.y, -10) + offset;
            transform.position = Vector3.SmoothDamp(transform.position, moveposition, ref velocity, damping);            
        }
    }
}
