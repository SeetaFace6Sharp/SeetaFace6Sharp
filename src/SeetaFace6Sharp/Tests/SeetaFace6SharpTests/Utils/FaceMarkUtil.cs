using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeetaFace6Sharp;

namespace SeetaFace6SharpTests.Utils
{
    public static class FaceMarkUtil
    {
        public static FaceMarkPoint[] GetFaceMarkPoint(FaceDetector faceDetector, FaceLandmarker faceMark, FaceImage bitmap)
        {
            var infos = faceDetector.Detect(bitmap);
            var info = infos.First();
            return faceMark.Mark(bitmap, info);
        }
    }
}
