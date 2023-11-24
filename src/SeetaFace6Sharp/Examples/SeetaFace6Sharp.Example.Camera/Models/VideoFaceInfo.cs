using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Example.Camera.Models
{
    public class VideoFaceInfo
    {
        public int Pid { get; set; }

        public bool IsMasked { get; set; }

        public bool IsDetectMask { get; set; }

        public bool HasProperty { get; set; }

        public FaceRect Location { get; set; }

        public RectangleF Rectangle
        {
            get
            {
                return new RectangleF(this.Location.X, this.Location.Y, this.Location.Width, this.Location.Height);
            }
        }

        public int Age { get; set; }

        public QualityResult Clarity { get; set; }

        public Gender Gender { get; set; }

        public string GenderDescribe
        {
            get
            {
                switch (this.Gender)
                {
                    case Gender.Male:
                        return "男";
                    case Gender.Female:
                        return "女";
                    case Gender.Unknown:
                        return "未知";
                }
                return "未知";
            }
        }

        public string Name { get; set; }
    }
}
