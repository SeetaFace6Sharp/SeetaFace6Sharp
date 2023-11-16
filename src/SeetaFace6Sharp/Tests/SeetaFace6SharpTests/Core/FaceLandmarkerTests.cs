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
    public class FaceLandmarkerTests
    {
        [TestMethod()]
        public void MarkV2Test()
        {
            using var bitmap = SKBitmap.Decode(@"images/mask_01.jpeg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Light));

            var info = faceDetector.Detect(bitmap).First();
            var markPoints = faceMark.MarkV2(bitmap, info);

        }
    }
}