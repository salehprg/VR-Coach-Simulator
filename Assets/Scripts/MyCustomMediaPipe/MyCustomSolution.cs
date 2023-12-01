using System.Collections;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Unity;
using UnityEngine;

public class MyCustomSolution : ImageSourceSolution<MyCustomGraph>
{
    [SerializeField] private MediapipeGameManager _mediapipeGameManager;
    [SerializeField] private RectTransform _worldAnnotationArea;
    [SerializeField] private DetectionAnnotationController _poseDetectionAnnotationController;
    [SerializeField] private PoseLandmarkListAnnotationController _poseLandmarksAnnotationController;
    [SerializeField] private MyPoseWorldLanMarkController _poseWorldLandmarksAnnotationController;
    [SerializeField] private MaskAnnotationController _segmentationMaskAnnotationController;
    [SerializeField] private NormalizedRectAnnotationController _roiFromLandmarksAnnotationController;

    [SerializeField] private DetectionListAnnotationController _palmDetectionsAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromPalmDetectionsAnnotationController;
    [SerializeField] private MultiHandLandmarkListAnnotationController _handLandmarksAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromLandmarksAnnotationController;

    public MyCustomGraph.ModelComplexity modelComplexity
    {
        get => graphRunner.modelComplexity;
        set => graphRunner.modelComplexity = value;
    }

    public bool smoothLandmarks
    {
        get => graphRunner.smoothLandmarks;
        set => graphRunner.smoothLandmarks = value;
    }

    public bool enableSegmentation
    {
        get => graphRunner.enableSegmentation;
        set => graphRunner.enableSegmentation = value;
    }

    public bool smoothSegmentation
    {
        get => graphRunner.smoothSegmentation;
        set => graphRunner.smoothSegmentation = value;
    }

    public float minDetectionConfidence
    {
        get => graphRunner.minDetectionConfidence;
        set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
        get => graphRunner.minTrackingConfidence;
        set => graphRunner.minTrackingConfidence = value;
    }

    protected override void SetupScreen(ImageSource imageSource)
    {
        base.SetupScreen(imageSource);
        _worldAnnotationArea.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();
    }

    protected override void OnStartRun()
    {
        if (!runningMode.IsSynchronous())
        {
            if (graphRunner.trackPose)
            {
                graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
                graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
                graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
                graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
                graphRunner.OnRoiFromLandmarksOutput += OnRoiFromLandmarksOutput;
            }

            if (graphRunner.trackHand)
            {
                graphRunner.OnPalmDetectectionsOutput += OnPalmDetectionsOutput;
                graphRunner.OnHandRectsFromPalmDetectionsOutput += OnHandRectsFromPalmDetectionsOutput;
                graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
                // TODO: render HandWorldLandmarks annotations
                graphRunner.OnHandRectsFromLandmarksOutput += OnHandRectsFromLandmarksOutput;
                graphRunner.OnHandednessOutput += OnHandednessOutput;
            }
        }

        var imageSource = ImageSourceProvider.ImageSource;
        if (graphRunner.trackPose)
        {
            SetupAnnotationController(_poseDetectionAnnotationController, imageSource);
            SetupAnnotationController(_poseLandmarksAnnotationController, imageSource);
            SetupAnnotationController(_poseWorldLandmarksAnnotationController, imageSource);
            SetupAnnotationController(_segmentationMaskAnnotationController, imageSource);
            _segmentationMaskAnnotationController.InitScreen(imageSource.textureWidth, imageSource.textureHeight);
            SetupAnnotationController(_roiFromLandmarksAnnotationController, imageSource);

            _mediapipeGameManager.InitBoneTransfer();
        }

        if (graphRunner.trackHand)
        {
            SetupAnnotationController(_palmDetectionsAnnotationController, imageSource, true);
            SetupAnnotationController(_handRectsFromPalmDetectionsAnnotationController, imageSource, true);
            SetupAnnotationController(_handLandmarksAnnotationController, imageSource, true);
            SetupAnnotationController(_handRectsFromLandmarksAnnotationController, imageSource, true);
        }

    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
        graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
        Detection poseDetection = null;
        NormalizedLandmarkList poseLandmarks = null;
        LandmarkList poseWorldLandmarks = null;
        ImageFrame segmentationMask = null;
        NormalizedRect roiFromLandmarks = null;

        List<Detection> palmDetections = null;
        List<NormalizedRect> handRectsFromPalmDetections = null;
        List<NormalizedLandmarkList> handLandmarks = null;
        List<LandmarkList> handWorldLandmarks = null;
        List<NormalizedRect> handRectsFromLandmarks = null;
        List<ClassificationList> handedness = null;

        if (runningMode == RunningMode.Sync)
        {
            var _ = graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness,
                                            out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, true);
        }
        else if (runningMode == RunningMode.NonBlockingSync)
        {
            yield return new WaitUntil(() => graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness,
                                                                    out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, false));
        }

        if (graphRunner.trackPose)
        {
            _poseDetectionAnnotationController.DrawNow(poseDetection);
            _poseLandmarksAnnotationController.DrawNow(poseLandmarks);
            _poseWorldLandmarksAnnotationController.DrawNow(poseWorldLandmarks);
            _segmentationMaskAnnotationController.DrawNow(segmentationMask);
            _roiFromLandmarksAnnotationController.DrawNow(roiFromLandmarks);
        }

        if (graphRunner.trackHand)
        {
            _palmDetectionsAnnotationController.DrawNow(palmDetections);
            _handRectsFromPalmDetectionsAnnotationController.DrawNow(handRectsFromPalmDetections);
            _handLandmarksAnnotationController.DrawNow(handLandmarks, handedness);
            // TODO: render HandWorldLandmarks annotations
            _handRectsFromLandmarksAnnotationController.DrawNow(handRectsFromLandmarks);
        }

    }

    private void OnPalmDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
        _palmDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandRectsFromPalmDetectionsOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
        _handRectsFromPalmDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
    {
        _handLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandRectsFromLandmarksOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
        _handRectsFromLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandednessOutput(object stream, OutputEventArgs<List<ClassificationList>> eventArgs)
    {
        _handLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
    {
        _poseDetectionAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
        _poseLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
    {
        _poseWorldLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
    {
        _segmentationMaskAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnRoiFromLandmarksOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
    {
        _roiFromLandmarksAnnotationController.DrawLater(eventArgs.value);
    }
}
