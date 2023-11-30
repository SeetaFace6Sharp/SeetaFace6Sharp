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
    public class FaceTrackerTests
    {
        [TestMethod()]
        public void TrackVideoTest()
        {
            using var bitmap = SKBitmap.Decode(@"images/mask_01.jpeg").ToFaceImage();
            using FaceDetector faceDetector = new FaceDetector();
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Light));
            using FaceTracker faceTracker = new FaceTracker(new FaceTrackerConfig(bitmap.Width, bitmap.Height));

            for (int i = 0; i < 1000; i++)
            {
                var result = faceTracker.TrackVideo(bitmap, i);

                if (i%10 == 0)
                {
                    faceTracker.Reset();
                }

                Console.WriteLine($"视频追踪结果，Pid：{result[0].Pid}");
            }
        }
    }
}