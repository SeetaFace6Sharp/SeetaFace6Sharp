

using System;
using System.Diagnostics;
using System.Linq;
using SkiaSharp;

namespace SeetaFace6Sharp.PerformanceTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using SKBitmap bitmap = SKBitmap.Decode(@"images/Jay_3.jpg");
            using FaceImage faceImage = bitmap.ToFaceImage();

            using FaceDetector faceDetector = new FaceDetector(new FaceDetectConfig()
            {
                MaxHeight = 1000,
                MaxWidth = 1000,
                FaceSize = 80
            });
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Light));
            FaceInfo[] faceInfo = faceDetector.Detect(faceImage);
            FaceMarkPoint[] points = faceMark.Mark(faceImage, faceInfo[0]);

            double[] expTime = new double[5];
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < expTime.Length; i++)
            {
                stopwatch.Restart();
                RecognizeTest(faceImage, points);
                stopwatch.Stop();

                Console.WriteLine($"第{i + 1}次测试耗时：{stopwatch.ElapsedMilliseconds}ms");
                expTime[i] = stopwatch.ElapsedMilliseconds;
            }
            var avgTime = expTime.OrderByDescending(p => p).Skip(1).Take(expTime.Length - 2).Average();
            Console.WriteLine($"平均耗时为：{avgTime}ms");
        }

        static FaceRecognizer faceRecognizer = new FaceRecognizer(new FaceRecognizeConfig());

        static void RecognizeTest(FaceImage faceImage, FaceMarkPoint[] points)
        {
            var result = faceRecognizer.Extract(faceImage, points);
        }
    }
}
