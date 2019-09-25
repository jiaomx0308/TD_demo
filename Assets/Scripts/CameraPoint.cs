using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    public static CameraPoint Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "CameraPoint.tif");
    }
}
