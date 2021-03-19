using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class TreasureTrackerCamera : MonoBehaviour
{
    #region Singleton
    public static TreasureTrackerCamera mainCamera;
    #endregion


    [Header("Camera Controls")]
    public bool isTrackingPlayer = true;
    [Range(0.0f, 1.0f)]
    public float horizontalSpeed = 0.75f;
    [Range(0.0f, 1.0f)]
    public float verticalSpeed = 0.75f;
    [Range(0.0f, 1.0f)]
    public float mouseSpeed = 0.1f;
    [Range(0.0f, 1.0f)]
    public float zoomSpeed = 0.5f;
    public float focusSize = 0.3f;
    public float focusEnd = 1f;
    public float zoomStrength = 3;

    public float mouseMultiplier = 0.5f;
    public Vector2 mouseVerticalMinMax = new Vector2(-10, 50);


    [Header("Targets")]
    public Transform player;
    public float verticalOffset = 0f;
    [HideInInspector]
    public Camera cam;
    Transform tOrigin;
    Transform tOffset;
    Transform tTarget;
    Transform tFindPlayer;

    DepthOfField tDOF;

    private Vector3 verticalPos = Vector3.zero;
    private Vector3 originPos = Vector3.zero;    
    private Vector3 hVelocity = Vector3.zero;
    private Vector3 vVelocity = Vector3.zero;

    private float zoomLevel;
    private float smoothZoomLevel;
    private float zoomVelocity = 0;
    private float maxFoV;
    private bool zoomToggle = false;

    private void Awake()
    {
        mainCamera = this;
        cam = transform.GetChild(0).GetComponent<Camera>();
        tOrigin = transform.GetChild(1);
        tOffset = tOrigin.GetChild(0);
        tTarget = transform.GetChild(2);
        tFindPlayer = cam.transform.GetChild(0);
    }


    // Start is called before the first frame update
    void Start()
    {
        //cam.transform.LookAt(tOrigin.position + new Vector3(0, verticalOffset, 0));

        verticalPos.y = cam.transform.position.y;
        originPos = tOrigin.position;

        if (GameObject.FindGameObjectWithTag("GlobalVolume").GetComponent<Volume>().profile.TryGet<DepthOfField>(out DepthOfField dof))
        {
            tDOF = dof;
        }

        maxFoV = cam.fieldOfView;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetButtonDown("Middle Mouse"))
        {
            zoomToggle = !zoomToggle;
            if (zoomToggle == true)
            {
                zoomLevel = 1;
            }
            else
            {
                zoomLevel = 0;
            }
        }

        //Smooth Zoom Level
        float finalZoom = Mathf.Clamp(zoomLevel + Input.GetAxis("Mouse ScrollWheel"),0f,1f) ;
        float dampZoom = Mathf.SmoothDamp(smoothZoomLevel, finalZoom, ref zoomVelocity, zoomSpeed);
        smoothZoomLevel = dampZoom;


        if (isTrackingPlayer) //Automatic Follow
        {
            // Orient Origin
            tOrigin.LookAt(new Vector3(player.position.x, tOrigin.position.y, player.position.z));
            
            //Smoothstep Camera Position
            Vector3 finalPos = new Vector3(tOffset.position.x, verticalPos.y, tOffset.position.z);
            Vector3 smoothPos = Vector3.SmoothDamp(cam.transform.position, finalPos, ref hVelocity, horizontalSpeed);
            cam.transform.position = smoothPos;

            //Smoothstep Vertical Target position
            float finalV = player.position.y + verticalOffset;
            float smoothV = Mathf.SmoothStep(tTarget.position.y, finalV, verticalSpeed);
            tTarget.position = new Vector3(player.position.x, smoothV, player.position.z);

            //Get Vertical Rotation by looking at player
            tFindPlayer.LookAt(tTarget);
            float rotX = tFindPlayer.eulerAngles.x;

            //Aim Camera at origin and replace vertical rotation
            cam.transform.LookAt(tOrigin);
            cam.transform.eulerAngles = new Vector3(rotX, cam.transform.eulerAngles.y, 0);

        } 
        else //Manual Control
        {
            //Smoothstep pivot rotation
            Vector3 finalRot = new Vector3(ClampAngle(tOrigin.eulerAngles.x + Input.GetAxis("Mouse Y") * mouseMultiplier, mouseVerticalMinMax.x, mouseVerticalMinMax.y), tOrigin.eulerAngles.y + Input.GetAxis("Mouse X") * mouseMultiplier, 0);
            Vector3 smoothRot = Vector3.SmoothDamp(finalRot, tOrigin.eulerAngles,ref hVelocity, mouseSpeed);
            smoothRot.x = ClampAngle(tOrigin.eulerAngles.x + Input.GetAxis("Mouse Y") * mouseMultiplier, mouseVerticalMinMax.x, mouseVerticalMinMax.y);
            tOrigin.eulerAngles = smoothRot;

            //Apply camera position & offset
            cam.transform.position = tOffset.position + tOffset.TransformVector(verticalPos);

            ////Camera target Smoothstep
            //Vector3 finalTarget = Vector3.Lerp(tOrigin.position, player.position + new Vector3(0,verticalOffset,0), zoomLevel);//   new Vector3(Mathf.Lerp(tOrigin.position.x , player.position.x, zoomLevel), player.position.y, Mathf.Lerp(tOrigin.position.z, player.position.z, zoomLevel));;
            //Vector3 smoothTarget = Vector3.SmoothDamp(tTarget.position, finalTarget, ref vVelocity, horizontalSpeed);
            //tTarget.position = smoothTarget;

            //cam.transform.LookAt(tTarget);

            //Origin Position Smoothstep
            //Vector3 finalPos = Vector3.Lerp(originPos, player.position + new Vector3(0, verticalOffset, 0), zoomLevel);
            //Vector3 smoothPos = Vector3.SmoothDamp(tOrigin.position, finalPos, ref vVelocity, horizontalSpeed);

            //Zoom Target
            Vector3 finalTarget = player.position + new Vector3(0, verticalOffset, 0);
            Vector3 smoothTarget = Vector3.SmoothDamp(tTarget.position, finalTarget, ref vVelocity, zoomSpeed);
            tTarget.position = smoothTarget;

            tOrigin.position = Vector3.Lerp(originPos, tTarget.position, smoothZoomLevel);


            cam.transform.LookAt(tOrigin);
        }

        cam.fieldOfView = Mathf.Lerp(maxFoV, maxFoV/zoomStrength, smoothZoomLevel);


        float focusDistance = Vector3.Distance(cam.transform.position, player.position);
        tDOF.nearFocusEnd.Override(focusDistance - (Mathf.Lerp(focusSize, focusSize/zoomStrength,smoothZoomLevel) / 2));
        tDOF.farFocusStart.Override(focusDistance + (Mathf.Lerp(focusSize, focusSize*2 / zoomStrength, smoothZoomLevel) / 2));
        tDOF.farFocusEnd.Override(focusDistance + Mathf.Lerp(focusEnd, focusEnd/zoomStrength, smoothZoomLevel));

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;
    }
}
