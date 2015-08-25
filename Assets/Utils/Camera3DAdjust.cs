using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class Camera3DAdjust : MonoBehaviour {

    public float designWidth, designHeight;

    void Awake()
    {
        Camera camera = Camera.main;
        if (designHeight != 0f && Screen.height != 0f)
        {
            float radioDesign = designWidth / designHeight;
            float radioScreen = (float)Screen.width / Screen.height;
            if (radioDesign < radioScreen)
            {
                camera.fieldOfView *= radioDesign / radioScreen;
                camera.fieldOfView = (float)System.Math.Round(camera.fieldOfView+0.05f, 2);
            }
        }
    }

}
