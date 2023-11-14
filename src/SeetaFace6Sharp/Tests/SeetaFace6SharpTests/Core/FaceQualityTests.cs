using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeetaFace6Sharp;
using SeetaFace6SharpTests.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeetaFace6Sharp.Tests
{
    [TestClass()]
    public class FaceQualityTests
    {
        [TestMethod()]
        public void DetectTest()
        {
            using var bitmap = SKBitmap.Decode(@"images/more.jpg").ToFaceImage();
            using FaceQuality faceQuality = new FaceQuality();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceLandmarker faceMark = new FaceLandmarker();

            var info = faceDetector.Detect(bitmap).First();
            var markPoints = FaceMarkUtil.GetFaceMarkPoint(faceDetector, faceMark, bitmap);

            for (int i = 0; i < 100; i++)
            {
                var poseResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.Pose);

                var clarityResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.Clarity);

                var resolutionResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.Resolution);

                var integrityExResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.Integrity);

                var brightnessResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.Brightness);

                

                var clarityExResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.ClarityEx);

                var structureeResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.Structure);

                

                var poseExeResult = faceQuality.Detect(bitmap, info, markPoints, QualityType.PoseEx);

            }
        }
    }
}