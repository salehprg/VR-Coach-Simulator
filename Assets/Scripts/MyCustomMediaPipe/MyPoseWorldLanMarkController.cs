using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

public class MyPoseWorldLanMarkController : AnnotationController<MyPoseListAnnotation>
{
    [SerializeField] private float _hipHeightMeter = 0.9f;
    [SerializeField] private Vector3 _scale = new Vector3(100, 100, 100);
    [SerializeField] private bool _visualizeZ = true;

    public IList<Landmark> _currentTarget;

    protected override void Start()
    {
        base.Start();
        transform.localPosition = new Vector3(0, _hipHeightMeter * _scale.y, 0);
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z - (GameManager.instance.rotate90Degree ? 90 : 0), transform.rotation.w);
    }

    public void DrawNow(IList<Landmark> target)
    {
        _currentTarget = target;
        SyncNow();
    }

    public void DrawNow(LandmarkList target)
    {
        DrawNow(target?.Landmark);
    }

    public void DrawLater(IList<Landmark> target)
    {
        UpdateCurrentTarget(target, ref _currentTarget);
    }

    public void DrawLater(LandmarkList target)
    {
        DrawLater(target?.Landmark);
    }

    protected override void SyncNow()
    {
        isStale = false;
        annotation.Draw(_currentTarget, _scale, GameManager.instance.visualizeZ);
    }
}
