// Set an off-center projection, where perspective's vanishing
// point is not necessarily in the center of the screen.
//
// left/right/top/bottom define near plane size, i.e.
// how offset are corners of camera's near plane.
// Tweak the values and you can see camera's frustum change.

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraView : MonoBehaviour
{
    Camera cam;
    public float height = 0.7f;
    public float width = 1f;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        //stretch view//
        cam.ResetProjectionMatrix();
        var m = cam.projectionMatrix;

        m.m11 *= height;
        m.m00 *= width;
        cam.projectionMatrix = m;
    }
}