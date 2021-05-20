using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;


public class TreasureTrackerCamera : MonoBehaviour
{
    #region Singleton
    public static TreasureTrackerCamera mainCamera;
    #endregion
    public bool _usingV2 = true;
    
    
    //public bool _isHittingPlayer;

    [Header("Camera Controls")]
    public bool isTrackingPlayer = true;
    public Vector2 VerticalAngleMinMax = new Vector2(-65, 15);
    [Range(0.0f, 1.0f)]
    public float horizontalDampen = 0.75f;
    [Range(0.0f, 1.0f)]
    public float verticalDampen = 0.75f;
    [Range(0.0f, 1.0f)]
    public float mouseDampen = 0.1f;
    public float mouseSensitivity = 0.5f;
    public float joystickSensitivity = 1f;
    

    [Header("Zoom Controls")]
    [Range(0.0f, 1.0f)]
    public float zoomLevel;
    [Range(0.0f, 1.0f)]
    public float zoomToggleSpeed = 0.75f;
    [Range(0.0f, 1.0f)]
    public float zoomTrackingDampen = 0.25f;
    public float zoomSensitivity = 1.5f;
    public bool _enableToggle = false;
    public float zoomStrength = 3;
    public float envSize;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.Linear(0,0,1,1);

    [Header("Autofocus Controls")]
    public LayerMask afLayers;
    [Range(0.0f, 1.0f)]
    public float autofocusDampen = 0.2f;
    public bool _enableZoomCoyote = true;
    public int afCoyoteFrames = 60;
    public float focusSize = 0.3f;
    public float nearFocusStrength = 1f;
    public float farFocusStrength = 1f;
    public bool _showAFDebug = false;
    public bool _afOnPlayer;

    [Header("Targets")]
    public Transform player;
    public float verticalOffset = 0f;
    [HideInInspector]
    public Camera cam;
    Transform tOrigin;
    Transform tOffset;
    Transform tTarget;
    Transform tFindPlayer;
    MeshRenderer playerModel;

    DepthOfField tDOF;

    private Vector3 verticalPos = Vector3.zero;
    private Vector3 originPos = Vector3.zero;    
    private Vector3 hVelocity = Vector3.zero;
    private Vector3 vVelocity = Vector3.zero;

    private float smoothZoomLevel;
    private float zoomVelocity = 0;
    private float maxFoV;
    private bool zoomToggle = false;

    private Vector3 dampPlayerPos;
    private Vector3 smoothPlayerPos;


    private Ray afRay;
    private RaycastHit afHit;
    private GameObject afDebug;
    private float focusDistance;
    private float dampFocus;
    private float smoothFocus;
    private float focusVelocity = 0;
    private int focusCoyote = 0;

    //Input 
    [Header("Input")]
    public string horizontalCamAxis = "Camera Horizontal";
    public string verticalCamAxis = "Camera Vertical";
    public string horizontalCamAxisGamepad = "Camera Horizontal Gamepad";
    public string verticalCamAxisGamepad = "Camera Vertical Gamepad";
    public string zoomAxis = "Zoom Axis";
    public string zoomToggleButton = "Zoom Toggle";
    
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

        if (_showAFDebug)
        {
            afDebug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            afDebug.name = "afDebug";
            afDebug.transform.SetParent(tFindPlayer);
            afDebug.GetComponent<SphereCollider>().enabled = false;
        }

        playerModel = player.GetComponent<DemoPlayerController>().playerModel.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*if(Input.GetButtonDown(zoomToggleButton))
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

        print(Input.GetAxis("Mouse ScrollWheel"));
        //Smooth Zoom Level
        float finalZoom = Mathf.Clamp(zoomLevel + Input.GetAxis("Mouse ScrollWheel"),0f,1f) ;
        float dampZoom = Mathf.SmoothDamp(smoothZoomLevel, finalZoom, ref zoomVelocity, zoomToggleSpeed);
        smoothZoomLevel = Mathf.Clamp(dampZoom + Input.GetAxis("Mouse ScrollWheel"), 0f, 1f);

        if (Input.GetButtonDown(zoomToggleButton) && _enableToggle)
        {
            if (zoomLevel < 0.5f)
            {
                zoomLevel = 0.85f;
            }
            else
            {
                zoomLevel = 0.15f;
            }
        }
        zoomLevel = Mathf.Clamp(zoomLevel + Input.GetAxis(zoomAxis),0f,1f);

        float dampZoom = Mathf.SmoothDamp(smoothZoomLevel, zoomLevel, ref zoomVelocity, zoomToggleSpeed);
        smoothZoomLevel = dampZoom;
        */

        if (!_usingV2) //Old system
        {
            //Zoom Toggle
            if (Input.GetButtonDown(zoomToggleButton))
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

            //print(Input.GetAxis("Mouse ScrollWheel"));
            //Smooth Zoom Level
            float finalZoom = Mathf.Clamp(zoomLevel + Input.GetAxis(zoomAxis), 0f, 1f);
            float dampZoom = Mathf.SmoothDamp(smoothZoomLevel, finalZoom, ref zoomVelocity, zoomToggleSpeed);
            smoothZoomLevel = dampZoom;

            if (isTrackingPlayer) //Automatic Follow
            {
                // Orient Origin
                tOrigin.LookAt(new Vector3(player.position.x, tOrigin.position.y, player.position.z));
            
                //Smoothstep Camera Position
                Vector3 finalPos = new Vector3(tOffset.position.x, verticalPos.y, tOffset.position.z);
                Vector3 smoothPos = Vector3.SmoothDamp(cam.transform.position, finalPos, ref hVelocity, horizontalDampen);
                cam.transform.position = smoothPos;

                //Smoothstep Vertical Target position
                float finalV = player.position.y + verticalOffset;
                float smoothV = Mathf.SmoothStep(tTarget.position.y, finalV, verticalDampen);
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
                Vector3 finalRot = new Vector3(0, tOrigin.eulerAngles.y + (Input.GetAxis(horizontalCamAxis) * mouseSensitivity) + (Input.GetAxis(horizontalCamAxisGamepad) * joystickSensitivity), 0);
                Vector3 smoothRot = Vector3.SmoothDamp(finalRot, tOrigin.eulerAngles,ref hVelocity, mouseDampen);
                smoothRot.x = ClampAngle(tOrigin.eulerAngles.x + (Input.GetAxis(verticalCamAxis) * mouseSensitivity) + (Input.GetAxis(verticalCamAxisGamepad) * joystickSensitivity), VerticalAngleMinMax.x, VerticalAngleMinMax.y);
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
                Vector3 smoothTarget = Vector3.SmoothDamp(tTarget.position, finalTarget, ref vVelocity, zoomTrackingDampen);
                tTarget.position = smoothTarget;

                tOrigin.position = Vector3.Lerp(originPos, tTarget.position, smoothZoomLevel);


                cam.transform.LookAt(tOrigin);
            }

            cam.fieldOfView = Mathf.Lerp(maxFoV, maxFoV/zoomStrength, smoothZoomLevel);
        }
        else //new system
        {
            //Zoom V2   
            if (Input.GetButtonDown(zoomToggleButton) && _enableToggle)
            {
                if (zoomLevel < 0.5f)
                {
                    zoomLevel = 0.85f;
                }
                else
                {
                    zoomLevel = 0.15f;
                }
            }
            zoomLevel = Mathf.Clamp(zoomLevel + Input.GetAxis(zoomAxis) * zoomSensitivity, 0f, 1f);

            float dampZoom = Mathf.SmoothDamp(smoothZoomLevel, zoomLevel, ref zoomVelocity, zoomToggleSpeed);
            smoothZoomLevel = dampZoom;

            //Smoothstep pivot rotation
            Vector3 finalRot = new Vector3(0, tOrigin.eulerAngles.y + (Input.GetAxis(horizontalCamAxis) * mouseSensitivity) + (Input.GetAxis(horizontalCamAxisGamepad) * joystickSensitivity), 0);
            Vector3 smoothRot = Vector3.SmoothDamp(finalRot, tOrigin.eulerAngles, ref hVelocity, mouseDampen);
            smoothRot.x = ClampAngle(tOrigin.eulerAngles.x + (Input.GetAxis(verticalCamAxis) * mouseSensitivity) + (Input.GetAxis(verticalCamAxisGamepad) * joystickSensitivity), VerticalAngleMinMax.x, VerticalAngleMinMax.y);
            tOrigin.eulerAngles = smoothRot;

            //Apply camera position & offset
            cam.transform.position = tOffset.position + tOffset.TransformVector(verticalPos);

            //Smooth player tracking position
            dampPlayerPos = Vector3.SmoothDamp(smoothPlayerPos, player.position + new Vector3(0, verticalOffset, 0), ref vVelocity, zoomTrackingDampen);
            smoothPlayerPos = dampPlayerPos;

            //Lerp target between origin and player
            Vector3 finalTarget = Vector3.Lerp(tOrigin.position, smoothPlayerPos, zoomCurve.Evaluate(smoothZoomLevel));

            cam.fieldOfView = Mathf.Lerp(maxFoV, (maxFoV / zoomStrength) / (Vector3.Distance(player.position,cam.transform.position)/ envSize), smoothZoomLevel); //Zoom FOV adjusted based on distance of player to camera so player always stays the same size

            cam.transform.LookAt(finalTarget);
        }




        

        //Ray-based autofocus
       
        //Create Ray, if fully zoomed out, focus on the middle of the screen
        if ( zoomLevel < 0.2f)
        {
            afRay = new Ray(cam.transform.position,cam.transform.forward);
        }
        else
        {
            tFindPlayer.LookAt(playerModel.transform.position);
            afRay = new Ray(tFindPlayer.position, tFindPlayer.forward);
        }

        //cast Ray, take distance of hit
        if (Physics.Raycast(afRay, out afHit, afLayers))
        {
            focusDistance = Vector3.Distance(cam.transform.position, afHit.point);
            //print("Name: " + afHit.collider.name + ", Tag: " + afHit.collider.tag + ", Layer: " + afHit.collider.gameObject.layer);
        }
        else
        {
            focusDistance = Vector3.Distance(cam.transform.position, player.position); //fallback focus distance if ray hits nothing
        }

        focusCoyote++;
        if (afHit.collider.CompareTag("Player"))    //If player in view, lock autofocus (no smoothing)
        {
            //focusDistance = Vector3.Distance(cam.transform.position, player.position);
            smoothFocus = focusDistance;
            _afOnPlayer = true;
            focusCoyote = 0;
        }
        else if(focusCoyote < afCoyoteFrames && _enableZoomCoyote)
        {
            smoothFocus = Vector3.Distance(cam.transform.position, player.position);
            _afOnPlayer = true;
        }
        else // Smooth AF when player not in view
        {
            dampFocus = Mathf.SmoothDamp(smoothFocus, focusDistance, ref focusVelocity, autofocusDampen);
            smoothFocus = dampFocus;
            _afOnPlayer = false;
        }

        if (_showAFDebug)
        {
            afDebug.transform.localPosition = new Vector3(0, 0, smoothFocus);
        }

        //apply focus distances
        tDOF.nearFocusStart.Override(smoothFocus - Mathf.Lerp(nearFocusStrength, (nearFocusStrength/2)/zoomStrength,smoothZoomLevel));
        tDOF.nearFocusEnd.Override(smoothFocus - (Mathf.Lerp(focusSize, focusSize/zoomStrength,smoothZoomLevel) / 2));
        tDOF.farFocusStart.Override(smoothFocus + (Mathf.Lerp(focusSize, focusSize*2 / zoomStrength, smoothZoomLevel) / 2));
        tDOF.farFocusEnd.Override(smoothFocus + Mathf.Lerp(farFocusStrength, farFocusStrength/zoomStrength, smoothZoomLevel));

        //print(Input.GetAxis(zoomAxis));


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
