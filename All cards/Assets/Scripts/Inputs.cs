using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public static class Inputs
{
    public static KeyCode select;
    public static KeyCode move;
    public static KeyCode cameraUp;
    public static KeyCode cameraDown;
    public static KeyCode cameraLeft;
    public static KeyCode cameraRight;

    // Start is called before the first frame update
    public static void InitInputs()
    {
        select = KeyCode.Mouse0;
        move = KeyCode.Mouse1;
        cameraUp = KeyCode.W;
        cameraDown = KeyCode.S;
        cameraLeft = KeyCode.A;
        cameraRight = KeyCode.D;
    }
}
