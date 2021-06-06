using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

public class FollowCam : MonoBehaviour
{
    [Header("- Camera Controls -")]
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


    [Header("- Zoom Controls -")]
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
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("- Autofocus Controls -")]
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

    [Header("- Targets -")]
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
    [Header("- Input -")]
    public string horizontalCamAxis = "Camera Horizontal";
    public string verticalCamAxis = "Camera Vertical";
    public string horizontalCamAxisGamepad = "Camera Horizontal Gamepad";
    public string verticalCamAxisGamepad = "Camera Vertical Gamepad";
    public string zoomAxis = "Zoom Axis";
    public string zoomToggleButton = "Zoom Toggle";

    // Start is called before the first frame update
    void Start()
    {
        player = DemoPlayerController.Player.transform;
        cam = GetComponentInChildren<Camera>();
        tOrigin = tOrigin = transform.GetChild(0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = player.transform.position;

        //Smoothstep pivot rotation
        Vector3 finalRot = new Vector3(0, tOrigin.eulerAngles.y + (Input.GetAxis(horizontalCamAxis) * mouseSensitivity) + (Input.GetAxis(horizontalCamAxisGamepad) * joystickSensitivity), 0);
        Vector3 smoothRot = Vector3.SmoothDamp(finalRot, tOrigin.eulerAngles, ref hVelocity, mouseDampen);
        smoothRot.x = ClampAngle(tOrigin.eulerAngles.x + (Input.GetAxis(verticalCamAxis) * -mouseSensitivity) + (Input.GetAxis(verticalCamAxisGamepad) * joystickSensitivity), VerticalAngleMinMax.x, VerticalAngleMinMax.y);
        tOrigin.eulerAngles = smoothRot;

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
