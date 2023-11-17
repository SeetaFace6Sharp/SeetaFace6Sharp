using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeetaFace6Sharp;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Tests
{
    [TestClass()]
    public class QualityOfLBNTests
    {
        [TestMethod()]
        public void DetectTest1()
        {
            using var bitmap = SKBitmap.Decode(@"images/more.jpg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using QualityOfLBN qualityOfLBN = new QualityOfLBN();

            var info = faceDetector.Detect(bitmap).First();
            var result = qualityOfLBN.Detect(bitmap, info);

            qualityOfLBN.Dispose();
        }

        [TestMethod()]
        public void DetectTest2()
        {
            using var bitmap = SKBitmap.Decode(@"images/more.jpg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Normal));
            using QualityOfLBN qualityOfLBN = new QualityOfLBN();

            var info = faceDetector.Detect(bitmap).First();
            var points = faceMark.Mark(bitmap, info);
            var result = qualityOfLBN.Detect(bitmap, points);

            qualityOfLBN.Dispose();
        }

        [TestMethod()]
        public void DetectResultTheSameAsOldTest1()
        {
            using var bitmap = SKBitmap.Decode(@"images/Jay_3.jpg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Normal));
            using QualityOfLBN qualityOfLBN = new QualityOfLBN();

            var info = faceDetector.Detect(bitmap).First();
            var points = faceMark.Mark(bitmap, info);
            var result1 = qualityOfLBN.Detect(bitmap, points);

            qualityOfLBN.Dispose();

            using var qua = new FaceQuality();
            var result2 = qua.Detect(bitmap, info, points, QualityType.ClarityEx);


        }
    }
}