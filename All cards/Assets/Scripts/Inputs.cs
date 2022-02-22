using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public KeyCode select;
    public KeyCode cameraUp;
    public KeyCode cameraDown;
    public KeyCode cameraLeft;
    public KeyCode cameraRight;

    // Start is called before the first frame update
    void Start()
    {
        select = KeyCode.Mouse0;
        cameraUp = KeyCode.W;
        cameraDown = KeyCode.S;
        cameraLeft = KeyCode.A;
        cameraRight = KeyCode.D;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
