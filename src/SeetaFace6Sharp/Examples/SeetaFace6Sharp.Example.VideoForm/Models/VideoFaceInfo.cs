using System.Drawing;

namespace SeetaFace6Sharp.Example.VideoForm.Models
{
    internal class VideoFaceInfo
    {
        public int Pid { get; set; }

        public bool HasMask { get; set; }

        public FaceRect Location { get; set; }

        public RectangleF Rectangle
        {
            get
            {
                return new RectangleF(this.Location.X, this.Location.Y, this.Location.Width, this.Location.Height);
            }
        }

        public int Age { get; set; }

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
