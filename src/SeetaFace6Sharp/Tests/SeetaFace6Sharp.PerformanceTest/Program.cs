

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
            GlobalConfig.DefaultDeviceType =  DeviceType.CPU;

            //特征值检测性能测试
            RecognizeTest();

            //常规人脸比对性能测试
            CompareTest();
        }

        static double CompareTest()
        {
            var sourceVals = GetExtract(@"images/Jay_4.jpg");

            using SKBitmap bitmap = SKBitmap.Decode(@"images/Jay_3.jpg");
            using FaceImage faceImage = bitmap.ToFaceImage();

            using FaceTracker faceTracker = new FaceTracker(new FaceTrackerConfig(bitmap.Width, bitmap.Height));
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Light));


            FaceRecognizer faceRecognizer = new FaceRecognizer(new FaceRecognizeConfig());

            double[] expTime = new double[5];
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < expTime.Length; i++)
            {
                stopwatch.Restart();
                FaceTrackInfo[] faceInfo = faceTracker.Track(faceImage);
                FaceMarkPoint[] points = faceMark.Mark(faceImage, faceInfo[0]);
                var result = faceRecognizer.Extract(faceImage, points);
                bool isSamePerson = faceRecognizer.IsSelf(sourceVals, result);
                stopwatch.Stop();

                Console.WriteLine($"CompareTest第{i + 1}次测试耗时：{stopwatch.ElapsedMilliseconds}ms，结果：{isSamePerson}");
                expTime[i] = stopwatch.ElapsedMilliseconds;
            }
            var avgTime = expTime.OrderByDescending(p => p).Skip(1).Take(expTime.Length - 2).Average();
            Console.WriteLine($"平均耗时为：{avgTime}ms");
            Console.WriteLine();
            return avgTime;
        }

        static double RecognizeTest()
        {
            using SKBitmap bitmap = SKBitmap.Decode(@"images/Jay_3.jpg");
            using FaceImage faceImage = bitmap.ToFaceImage();

            using FaceDetector faceDetector = new FaceDetector(new FaceDetectConfig());
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Light));
            FaceInfo[] faceInfo = faceDetector.Detect(faceImage);
            FaceMarkPoint[] points = faceMark.Mark(faceImage, faceInfo[0]);

            FaceRecognizer faceRecognizer = new FaceRecognizer(new FaceRecognizeConfig());

            double[] expTime = new double[5];
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < expTime.Length; i++)
            {
                stopwatch.Restart();
                var result = faceRecognizer.Extract(faceImage, points);
                stopwatch.Stop();

                Console.WriteLine($"FaceRecognizer第{i + 1}次测试耗时：{stopwatch.ElapsedMilliseconds}ms");
                expTime[i] = stopwatch.ElapsedMilliseconds;
            }
            var avgTime = expTime.OrderByDescending(p => p).Skip(1).Take(expTime.Length - 2).Average();
            Console.WriteLine($"平均耗时为：{avgTime}ms");
            Console.WriteLine();
            return avgTime;
        }


        static float[] GetExtract(string path)
        {
            using SKBitmap bitmap = SKBitmap.Decode(@"images/Jay_3.jpg");
            using FaceImage faceImage = bitmap.ToFaceImage();

            using FaceDetector faceDetector = new FaceDetector(new FaceDetectConfig());
            using FaceLandmarker faceMark = new FaceLandmarker(new FaceLandmarkConfig(MarkType.Light));
            FaceInfo[] faceInfo = faceDetector.Detect(faceImage);
            FaceMarkPoint[] points = faceMark.Mark(faceImage, faceInfo[0]);

            FaceRecognizer faceRecognizer = new FaceRecognizer(new FaceRecognizeConfig());
            return faceRecognizer.Extract(faceImage, points);
        }
    }
}
