using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeetaFace6Sharp;
using SkiaSharp;

namespace SeetaFace6SharpTests.Core
{
    [TestClass()]
    public class FaceDetectorTest
    {
        [TestMethod()]
        public void FaceDetectorTest1()
        {
            using var bitmap = SKBitmap.Decode(@"images/Jay_3.jpg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            var info = faceDetector.Detect(bitmap).First();
        }
    }
}
