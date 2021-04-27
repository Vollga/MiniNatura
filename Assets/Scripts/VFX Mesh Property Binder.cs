using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;


[VFXBinder("Mesh")]

public class VFXMeshPropertyBinder : VFXBinderBase
{
    // VFXPropertyBinding attributes enables the use of a specific
    // property drawer that populates the VisualEffect properties of a
    // certain type.
    [VFXPropertyBinding("System.Single")]
    public ExposedProperty meshProperty;

    public GameObject target;
    
    // The IsValid method need to perform the checks and return if the binding
    // can be achieved.
    public override bool IsValid(VisualEffect component)
    {
        return target != null && component.HasMesh(meshProperty);
    }

    // The UpdateBinding method is the place where you perform the binding,
    // by assuming that it is valid. This method will be called only if
    // IsValid returned true.
    public override void UpdateBinding(VisualEffect component)
    {
        component.SetMesh(meshProperty, target.GetComponent<MeshFilter>().sharedMesh);
    }
}