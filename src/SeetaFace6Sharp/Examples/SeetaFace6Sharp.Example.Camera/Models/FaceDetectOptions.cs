using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Example.Camera.Models
{
    public class FaceDetectOptions
    {
        public bool IsTrack { get; set; } = true;

        public bool PropertyDetect { get; set; } = true;

        public bool MaskDetect { get; set; }
    }
}
