using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Google.Protobuf;
using Mediapipe.Unity;
using Mediapipe;
using Logger = Mediapipe.Unity.Logger;

public class MyCustomGraph : GraphRunner
{
    public enum ModelComplexity
    {
        Lite = 0,
        Full = 1,
        Heavy = 2,
    }

    private OutputStream<DetectionVectorPacket, List<Detection>> _palmDetectionsStream;
    private OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>> _handRectsFromPalmDetectionsStream;
    private OutputStream<NormalizedLandmarkListVectorPacket, List<NormalizedLandmarkList>> _handLandmarksStream;
    private OutputStream<LandmarkListVectorPacket, List<LandmarkList>> _handWorldLandmarksStream;
    private OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>> _handRectsFromLandmarksStream;
    private OutputStream<ClassificationListVectorPacket, List<ClassificationList>> _handednessStream;
    private OutputStream<DetectionPacket, Detection> _poseDetectionStream;
    private OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList> _poseLandmarksStream;
    private OutputStream<LandmarkListPacket, LandmarkList> _poseWorldLandmarksStream;
    private OutputStream<ImageFramePacket, ImageFrame> _segmentationMaskStream;
    private OutputStream<NormalizedRectPacket, NormalizedRect> _roiFromLandmarksStream;

    private const string _PalmDetectionsStreamName = "palm_detections";
    private const string _HandRectsFromPalmDetectionsStreamName = "hand_rects_from_palm_detections";
    private const string _HandLandmarksStreamName = "hand_landmarks";
    private const string _HandWorldLandmarksStreamName = "hand_world_landmarks";
    private const string _HandRectsFromLandmarksStreamName = "hand_rects_from_landmarks";
    private const string _HandednessStreamName = "handedness";
    private const string _InputStreamName = "input_video";
    private const string _PoseDetectionStreamName = "pose_detection";
    private const string _PoseLandmarksStreamName = "pose_landmarks";
    private const string _PoseWorldLandmarksStreamName = "pose_world_landmarks";
    private const string _SegmentationMaskStreamName = "segmentation_mask";
    private const string _RoiFromLandmarksStreamName = "roi_from_landmarks";

    public bool trackHand = true;
    public bool trackPose = true;

    public bool smoothLandmarks = true;
    public bool enableSegmentation = true;
    public bool smoothSegmentation = true;

    public event EventHandler<OutputEventArgs<Detection>> OnPoseDetectionOutput
    {
        add => _poseDetectionStream.AddListener(value);
        remove => _poseDetectionStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<NormalizedLandmarkList>> OnPoseLandmarksOutput
    {
        add => _poseLandmarksStream.AddListener(value);
        remove => _poseLandmarksStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<LandmarkList>> OnPoseWorldLandmarksOutput
    {
        add => _poseWorldLandmarksStream.AddListener(value);
        remove => _poseWorldLandmarksStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<ImageFrame>> OnSegmentationMaskOutput
    {
        add => _segmentationMaskStream.AddListener(value);
        remove => _segmentationMaskStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<NormalizedRect>> OnRoiFromLandmarksOutput
    {
        add => _roiFromLandmarksStream.AddListener(value);
        remove => _roiFromLandmarksStream.RemoveListener(value);
    }

    public ModelComplexity modelComplexity = ModelComplexity.Full;
    public int maxNumHands = 2;

    private float _minDetectionConfidence = 0.5f;
    public float minDetectionConfidence
    {
        get => _minDetectionConfidence;
        set => _minDetectionConfidence = Mathf.Clamp01(value);
    }

    private float _minTrackingConfidence = 0.5f;
    public float minTrackingConfidence
    {
        get => _minTrackingConfidence;
        set => _minTrackingConfidence = Mathf.Clamp01(value);
    }

    public event EventHandler<OutputEventArgs<List<Detection>>> OnPalmDetectectionsOutput
    {
        add => _palmDetectionsStream.AddListener(value);
        remove => _palmDetectionsStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<List<NormalizedRect>>> OnHandRectsFromPalmDetectionsOutput
    {
        add => _handRectsFromPalmDetectionsStream.AddListener(value);
        remove => _handRectsFromPalmDetectionsStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<List<NormalizedLandmarkList>>> OnHandLandmarksOutput
    {
        add => _handLandmarksStream.AddListener(value);
        remove => _handLandmarksStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<List<LandmarkList>>> OnHandWorldLandmarksOutput
    {
        add => _handWorldLandmarksStream.AddListener(value);
        remove => _handWorldLandmarksStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<List<NormalizedRect>>> OnHandRectsFromLandmarksOutput
    {
        add => _handRectsFromLandmarksStream.AddListener(value);
        remove => _handRectsFromLandmarksStream.RemoveListener(value);
    }

    public event EventHandler<OutputEventArgs<List<ClassificationList>>> OnHandednessOutput
    {
        add => _handednessStream.AddListener(value);
        remove => _handednessStream.RemoveListener(value);
    }


    public override void StartRun(ImageSource imageSource)
    {
        if (runningMode.IsSynchronous())
        {
            if (trackHand)
            {
                _palmDetectionsStream.StartPolling().AssertOk();
                _handRectsFromPalmDetectionsStream.StartPolling().AssertOk();
                _handLandmarksStream.StartPolling().AssertOk();
                _handWorldLandmarksStream.StartPolling().AssertOk();
                _handRectsFromLandmarksStream.StartPolling().AssertOk();
                _handednessStream.StartPolling().AssertOk();
            }

            if (trackPose)
            {
                _poseDetectionStream.StartPolling().AssertOk();
                _poseLandmarksStream.StartPolling().AssertOk();
                _poseWorldLandmarksStream.StartPolling().AssertOk();
                _segmentationMaskStream.StartPolling().AssertOk();
                _roiFromLandmarksStream.StartPolling().AssertOk();
            }
        }
        StartRun(BuildSidePacket(imageSource));
    }

    public override void Stop()
    {
        if (trackHand)
        {
            _palmDetectionsStream?.Close();
            _palmDetectionsStream = null;
            _handRectsFromPalmDetectionsStream?.Close();
            _handRectsFromPalmDetectionsStream = null;
            _handLandmarksStream?.Close();
            _handLandmarksStream = null;
            _handWorldLandmarksStream?.Close();
            _handWorldLandmarksStream = null;
            _handRectsFromLandmarksStream?.Close();
            _handRectsFromLandmarksStream = null;
            _handednessStream?.Close();
            _handednessStream = null;
        }

        if (trackPose)
        {
            _poseDetectionStream?.Close();
            _poseDetectionStream = null;
            _poseLandmarksStream?.Close();
            _poseLandmarksStream = null;
            _poseWorldLandmarksStream?.Close();
            _poseWorldLandmarksStream = null;
            _segmentationMaskStream?.Close();
            _segmentationMaskStream = null;
            _roiFromLandmarksStream?.Close();
            _roiFromLandmarksStream = null;
        }

        base.Stop();
    }

    public void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
        AddTextureFrameToInputStream(_InputStreamName, textureFrame);
    }

    public bool TryGetNext(out List<Detection> palmDetections, out List<NormalizedRect> handRectsFromPalmDetections, out List<NormalizedLandmarkList> handLandmarks,
                           out List<LandmarkList> handWorldLandmarks, out List<NormalizedRect> handRectsFromLandmarks, out List<ClassificationList> handedness,
                           out Detection poseDetection, out NormalizedLandmarkList poseLandmarks, out LandmarkList poseWorldLandmarks, out ImageFrame segmentationMask,
                           out NormalizedRect roiFromLandmarks, bool allowBlock = true)
    {
        var currentTimestampMicrosec = GetCurrentTimestampMicrosec();
        var r1 = TryGetNext(_palmDetectionsStream, out palmDetections, allowBlock, currentTimestampMicrosec);
        var r2 = TryGetNext(_handRectsFromPalmDetectionsStream, out handRectsFromPalmDetections, allowBlock, currentTimestampMicrosec);
        var r3 = TryGetNext(_handLandmarksStream, out handLandmarks, allowBlock, currentTimestampMicrosec);
        var r4 = TryGetNext(_handWorldLandmarksStream, out handWorldLandmarks, allowBlock, currentTimestampMicrosec);
        var r5 = TryGetNext(_handRectsFromLandmarksStream, out handRectsFromLandmarks, allowBlock, currentTimestampMicrosec);
        var r6 = TryGetNext(_handednessStream, out handedness, allowBlock, currentTimestampMicrosec);

        var r7 = TryGetNext(_poseDetectionStream, out poseDetection, allowBlock, currentTimestampMicrosec);
        var r8 = TryGetNext(_poseLandmarksStream, out poseLandmarks, allowBlock, currentTimestampMicrosec);
        var r9 = TryGetNext(_poseWorldLandmarksStream, out poseWorldLandmarks, allowBlock, currentTimestampMicrosec);
        var r10 = TryGetNext(_segmentationMaskStream, out segmentationMask, allowBlock, currentTimestampMicrosec);
        var r11 = TryGetNext(_roiFromLandmarksStream, out roiFromLandmarks, allowBlock, currentTimestampMicrosec);

        return r1 || r2 || r3 || r4 || r5 || r6 || r7 || r8 || r9 || r10 || r11;
    }

    protected override IList<WaitForResult> RequestDependentAssets()
    {
        if (trackHand && !trackPose)
        {
            return new List<WaitForResult> {
                WaitForHandLandmarkModel(),
                WaitForAsset("handedness.txt"),
                WaitForPalmDetectionModel()
            };
        }
        if (trackPose && !trackHand)
        {
            return new List<WaitForResult> {
                WaitForAsset("pose_detection.bytes"),
                WaitForPoseLandmarkModel()
            };
        }
        else if (trackHand && trackPose)
        {
            return new List<WaitForResult> {
                WaitForHandLandmarkModel(),
                WaitForAsset("handedness.txt"),
                WaitForPalmDetectionModel(),
                WaitForAsset("pose_detection.bytes"),
                WaitForPoseLandmarkModel()
            };
        }
        return new List<WaitForResult> {
                WaitForAsset("pose_detection.bytes"),
                WaitForPoseLandmarkModel()
            };
    }

    protected override Status ConfigureCalculatorGraph(CalculatorGraphConfig config)
    {
        if (runningMode == RunningMode.NonBlockingSync)
        {
            if (trackHand)
            {
                _palmDetectionsStream = new OutputStream<DetectionVectorPacket, List<Detection>>(
                    calculatorGraph, _PalmDetectionsStreamName, config.AddPacketPresenceCalculator(_PalmDetectionsStreamName), timeoutMicrosec);
                _handRectsFromPalmDetectionsStream = new OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>>(
                    calculatorGraph, _HandRectsFromPalmDetectionsStreamName, config.AddPacketPresenceCalculator(_HandRectsFromPalmDetectionsStreamName), timeoutMicrosec);
                _handLandmarksStream = new OutputStream<NormalizedLandmarkListVectorPacket, List<NormalizedLandmarkList>>(
                    calculatorGraph, _HandLandmarksStreamName, config.AddPacketPresenceCalculator(_HandLandmarksStreamName), timeoutMicrosec);
                _handWorldLandmarksStream = new OutputStream<LandmarkListVectorPacket, List<LandmarkList>>(
                    calculatorGraph, _HandWorldLandmarksStreamName, config.AddPacketPresenceCalculator(_HandWorldLandmarksStreamName), timeoutMicrosec);
                _handRectsFromLandmarksStream = new OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>>(
                    calculatorGraph, _HandRectsFromLandmarksStreamName, config.AddPacketPresenceCalculator(_HandRectsFromLandmarksStreamName), timeoutMicrosec);
                _handednessStream = new OutputStream<ClassificationListVectorPacket, List<ClassificationList>>(
                    calculatorGraph, _HandednessStreamName, config.AddPacketPresenceCalculator(_HandednessStreamName), timeoutMicrosec);
            }

            if (trackPose)
            {
                _poseDetectionStream = new OutputStream<DetectionPacket, Detection>(
            calculatorGraph, _PoseDetectionStreamName, config.AddPacketPresenceCalculator(_PoseDetectionStreamName), timeoutMicrosec);
                _poseLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(
                    calculatorGraph, _PoseLandmarksStreamName, config.AddPacketPresenceCalculator(_PoseLandmarksStreamName), timeoutMicrosec);
                _poseWorldLandmarksStream = new OutputStream<LandmarkListPacket, LandmarkList>(
                    calculatorGraph, _PoseWorldLandmarksStreamName, config.AddPacketPresenceCalculator(_PoseWorldLandmarksStreamName), timeoutMicrosec);
                _segmentationMaskStream = new OutputStream<ImageFramePacket, ImageFrame>(
                    calculatorGraph, _SegmentationMaskStreamName, config.AddPacketPresenceCalculator(_SegmentationMaskStreamName), timeoutMicrosec);
                _roiFromLandmarksStream = new OutputStream<NormalizedRectPacket, NormalizedRect>(
                    calculatorGraph, _RoiFromLandmarksStreamName, config.AddPacketPresenceCalculator(_RoiFromLandmarksStreamName), timeoutMicrosec);
            }
        }
        else
        {
            if (trackHand)
            {
                _palmDetectionsStream = new OutputStream<DetectionVectorPacket, List<Detection>>(calculatorGraph, _PalmDetectionsStreamName, true, timeoutMicrosec);
                _handRectsFromPalmDetectionsStream = new OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>>(calculatorGraph, _HandRectsFromPalmDetectionsStreamName, true, timeoutMicrosec);
                _handLandmarksStream = new OutputStream<NormalizedLandmarkListVectorPacket, List<NormalizedLandmarkList>>(calculatorGraph, _HandLandmarksStreamName, true, timeoutMicrosec);
                _handWorldLandmarksStream = new OutputStream<LandmarkListVectorPacket, List<LandmarkList>>(calculatorGraph, _HandWorldLandmarksStreamName, true, timeoutMicrosec);
                _handRectsFromLandmarksStream = new OutputStream<NormalizedRectVectorPacket, List<NormalizedRect>>(calculatorGraph, _HandRectsFromLandmarksStreamName, true, timeoutMicrosec);
                _handednessStream = new OutputStream<ClassificationListVectorPacket, List<ClassificationList>>(calculatorGraph, _HandednessStreamName, true, timeoutMicrosec);
            }
            if (trackPose)
            {
                _poseDetectionStream = new OutputStream<DetectionPacket, Detection>(calculatorGraph, _PoseDetectionStreamName, true, timeoutMicrosec);
                _poseLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(calculatorGraph, _PoseLandmarksStreamName, true, timeoutMicrosec);
                _poseWorldLandmarksStream = new OutputStream<LandmarkListPacket, LandmarkList>(calculatorGraph, _PoseWorldLandmarksStreamName, true, timeoutMicrosec);
                _segmentationMaskStream = new OutputStream<ImageFramePacket, ImageFrame>(calculatorGraph, _SegmentationMaskStreamName, true, timeoutMicrosec);
                _roiFromLandmarksStream = new OutputStream<NormalizedRectPacket, NormalizedRect>(calculatorGraph, _RoiFromLandmarksStreamName, true, timeoutMicrosec);
            }
        }

        using (var validatedGraphConfig = new ValidatedGraphConfig())
        {
            var status = validatedGraphConfig.Initialize(config);

            if (!status.Ok()) { return status; }

            var extensionRegistry = new ExtensionRegistry() { TensorsToDetectionsCalculatorOptions.Extensions.Ext, ThresholdingCalculatorOptions.Extensions.Ext };
            var cannonicalizedConfig = validatedGraphConfig.Config(extensionRegistry);
            var tensorsToDetectionsCalculators = cannonicalizedConfig.Node.Where((node) => node.Calculator == "TensorsToDetectionsCalculator").ToList();
            var thresholdingCalculators = cannonicalizedConfig.Node.Where((node) => node.Calculator == "ThresholdingCalculator").ToList();

            foreach (var calculator in tensorsToDetectionsCalculators)
            {
                if (calculator.Options.HasExtension(TensorsToDetectionsCalculatorOptions.Extensions.Ext))
                {
                    var options = calculator.Options.GetExtension(TensorsToDetectionsCalculatorOptions.Extensions.Ext);
                    options.MinScoreThresh = minDetectionConfidence;
                    Logger.LogInfo(TAG, $"Min Detection Confidence = {minDetectionConfidence}");
                }
            }

            foreach (var calculator in thresholdingCalculators)
            {
                if (calculator.Options.HasExtension(ThresholdingCalculatorOptions.Extensions.Ext))
                {
                    var options = calculator.Options.GetExtension(ThresholdingCalculatorOptions.Extensions.Ext);
                    options.Threshold = minTrackingConfidence;
                    Logger.LogInfo(TAG, $"Min Tracking Confidence = {minTrackingConfidence}");
                }
            }
            return calculatorGraph.Initialize(cannonicalizedConfig);
        }
    }

    private WaitForResult WaitForPoseLandmarkModel()
    {
        switch (modelComplexity)
        {
            case ModelComplexity.Lite: return WaitForAsset("pose_landmark_lite.bytes");
            case ModelComplexity.Full: return WaitForAsset("pose_landmark_full.bytes");
            case ModelComplexity.Heavy: return WaitForAsset("pose_landmark_heavy.bytes");
            default: throw new InternalException($"Invalid model complexity: {modelComplexity}");
        }
    }

    private WaitForResult WaitForHandLandmarkModel()
    {
        switch (modelComplexity)
        {
            case ModelComplexity.Lite: return WaitForAsset("hand_landmark_lite.bytes");
            case ModelComplexity.Full: return WaitForAsset("hand_landmark_full.bytes");


            default: throw new InternalException($"Invalid model complexity: {modelComplexity}");
        }
    }

    private WaitForResult WaitForPalmDetectionModel()
    {
        switch (modelComplexity)
        {
            case ModelComplexity.Lite: return WaitForAsset("palm_detection_lite.bytes");
            case ModelComplexity.Full: return WaitForAsset("palm_detection_full.bytes");
            default: throw new InternalException($"Invalid model complexity: {modelComplexity}");
        }
    }

    private SidePacket BuildSidePacket(ImageSource imageSource)
    {
        var sidePacket = new SidePacket();

        SetImageTransformationOptions(sidePacket, imageSource, false);
        sidePacket.Emplace("model_complexity", new IntPacket((int)modelComplexity));
        sidePacket.Emplace("num_hands", new IntPacket(maxNumHands));

        Logger.LogInfo(TAG, $"Model Complexity = {modelComplexity}");
        Logger.LogInfo(TAG, $"Max Num Hands = {maxNumHands}");

        // TODO: refactoring
        // The orientation of the output image must match that of the input image.
        var isInverted = Mediapipe.Unity.CoordinateSystem.ImageCoordinate.IsInverted(imageSource.rotation);
        var outputRotation = imageSource.rotation;
        var outputHorizontallyFlipped = !isInverted && imageSource.isHorizontallyFlipped;
        var outputVerticallyFlipped = (!runningMode.IsSynchronous() && imageSource.isVerticallyFlipped) ^ (isInverted && imageSource.isHorizontallyFlipped);

        if ((outputHorizontallyFlipped && outputVerticallyFlipped) || outputRotation == RotationAngle.Rotation180)
        {
            outputRotation = outputRotation.Add(RotationAngle.Rotation180);
            outputHorizontallyFlipped = !outputHorizontallyFlipped;
            outputVerticallyFlipped = !outputVerticallyFlipped;
        }

        sidePacket.Emplace("output_rotation", new IntPacket((int)outputRotation));
        sidePacket.Emplace("output_horizontally_flipped", new BoolPacket(outputHorizontallyFlipped));
        sidePacket.Emplace("output_vertically_flipped", new BoolPacket(outputVerticallyFlipped));

        Logger.LogDebug($"output_rotation = {outputRotation}, output_horizontally_flipped = {outputHorizontallyFlipped}, output_vertically_flipped = {outputVerticallyFlipped}");

        sidePacket.Emplace("model_complexity", new IntPacket((int)modelComplexity));
        sidePacket.Emplace("smooth_landmarks", new BoolPacket(smoothLandmarks));
        sidePacket.Emplace("enable_segmentation", new BoolPacket(enableSegmentation));
        sidePacket.Emplace("smooth_segmentation", new BoolPacket(smoothSegmentation));

        Logger.LogInfo(TAG, $"Model Complexity = {modelComplexity}");
        Logger.LogInfo(TAG, $"Smooth Landmarks = {smoothLandmarks}");
        Logger.LogInfo(TAG, $"Enable Segmentation = {enableSegmentation}");
        Logger.LogInfo(TAG, $"Smooth Segmentation = {smoothSegmentation}");

        return sidePacket;
    }
}
