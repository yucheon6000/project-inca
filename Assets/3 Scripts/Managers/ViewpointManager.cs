using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewpointManager : MonoBehaviour
{
    [SerializeField]
    private KeyCode chageViewpointKeyCode = KeyCode.V;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private List<ViewpointInfo> viewpointInfos = new List<ViewpointInfo>();
    private int viewpointInfosIndex = 0;


    [ContextMenu("Add Current Viewpoint Info")]
    private void AddCurrentViewpointInfo()
    {
        ViewpointInfo info = new ViewpointInfo(
            camera.transform.localPosition, camera.transform.localRotation,
            camera.orthographic, camera.fieldOfView
        );

        viewpointInfos.Add(info);
    }

    private void Update()
    {
        if (Input.GetKeyDown(chageViewpointKeyCode))
        {
            viewpointInfosIndex++;
            if (viewpointInfosIndex >= viewpointInfos.Count)
                viewpointInfosIndex = 0;

            ViewpointInfo nextInfo = viewpointInfos[viewpointInfosIndex];

            camera.transform.localPosition = nextInfo.localPosition;
            camera.transform.localRotation = nextInfo.localRotation;
            camera.orthographic = nextInfo.orthographic;
            camera.fieldOfView = nextInfo.fieldOfView;
        }
    }

    [Serializable]
    public struct ViewpointInfo
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public bool orthographic;
        public float fieldOfView;

        public ViewpointInfo(Vector3 localPosition, Quaternion localRotation, bool orthographic, float fieldOfView)
        {
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            this.orthographic = orthographic;
            this.fieldOfView = fieldOfView;
        }
    }
}
