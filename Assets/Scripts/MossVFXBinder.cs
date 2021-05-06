using UnityEngine;
using UnityEngine.VFX;

public class MossVFXBinder : MonoBehaviour
{
    Transform playerFeet;
    VisualEffect mossVfx;

    // Start is called before the first frame update
    void Start()
    {
        playerFeet = DemoPlayerController.Player.transform.Find("Feet");
        mossVfx = this.GetComponent<VisualEffect>();
        mossVfx.SetMesh("Base Mesh", GetComponentInParent<MeshFilter>().mesh);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mossVfx.SetVector3("Player Position", playerFeet.position);
    }
}
