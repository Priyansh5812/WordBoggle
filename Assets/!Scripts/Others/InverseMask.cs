using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InverseMask : Image
{
    private Material _cachedMaterial;

    public override Material materialForRendering
    {
        get
        {
            if (_cachedMaterial == null)
            {
                _cachedMaterial = new Material(base.materialForRendering);
                _cachedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            return _cachedMaterial;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetMaterialDirty();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (_cachedMaterial != null)
        {
            DestroyImmediate(_cachedMaterial);
            _cachedMaterial = null;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_cachedMaterial != null)
        {
            DestroyImmediate(_cachedMaterial);
            _cachedMaterial = null;
        }
    }
}
