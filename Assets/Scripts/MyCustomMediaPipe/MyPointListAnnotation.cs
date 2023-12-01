using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;
using Color = UnityEngine.Color;
using mplt = Mediapipe.LocationData.Types;

public class MyPointListAnnotation : ListAnnotation<MyPointAnnotation>
{
    [SerializeField] private Color _color = Color.green;
    [SerializeField] private float _radius = 15.0f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
        {
            ApplyColor(_color);
            ApplyRadius(_radius);
        }
    }
#endif

    public void SetColor(Color color)
    {
        _color = color;
        ApplyColor(_color);
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
        ApplyRadius(_radius);
    }

    public void Draw(IList<Vector3> targets)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(target); }
            });
        }
    }

    public void Draw(IList<Landmark> targets, Vector3 scale, bool visualizeZ = true)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null)
                {

                    annotation.Draw(target, scale, visualizeZ);
                }
            });
        }
    }

    public void Draw(LandmarkList targets, Vector3 scale, bool visualizeZ = true)
    {
        Draw(targets.Landmark, scale, visualizeZ);
    }

    public void Draw(IList<NormalizedLandmark> targets, bool visualizeZ = true)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(target, visualizeZ); }
            });
        }
    }

    public void Draw(NormalizedLandmarkList targets, bool visualizeZ = true)
    {
        Draw(targets.Landmark, visualizeZ);
    }

    public void Draw(IList<mplt.RelativeKeypoint> targets, float threshold = 0.0f)
    {
        if (ActivateFor(targets))
        {
            CallActionForAll(targets, (annotation, target) =>
            {
                if (annotation != null) { annotation.Draw(target, threshold); }
            });
        }
    }

    protected override MyPointAnnotation InstantiateChild(bool isActive = true)
    {
        var annotation = base.InstantiateChild(isActive);
        annotation.SetColor(_color);
        annotation.SetRadius(_radius);
        return annotation;
    }

    private void ApplyColor(Color color)
    {
        foreach (var point in children)
        {
            if (point != null) { point.SetColor(color); }
        }
    }

    private void ApplyRadius(float radius)
    {
        foreach (var point in children)
        {
            if (point != null) { point.SetRadius(radius); }
        }
    }
}
