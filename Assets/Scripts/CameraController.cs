using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera cam;
    float originalSize;
    [SerializeField] float panSpeed;
    [SerializeField] float scrollSpeed;
    float xDir;
    float yDir;
    Vector3 movement;


    private void Start()
    {
        originalSize = cam.orthographicSize;
    }
    void Update()        
    {
        
        xDir = Input.GetAxis("Horizontal");
        yDir = Input.GetAxis("Vertical");
        movement = new Vector3(xDir, yDir, 0);

        transform.position += movement * panSpeed * Time.deltaTime;

        float scoll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize +=  -scoll * scrollSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 2f, 20f);

        //use mathf.clamp to limit how far the camera can pan
        //auto pan to enemy when its their turn?
    }
}
