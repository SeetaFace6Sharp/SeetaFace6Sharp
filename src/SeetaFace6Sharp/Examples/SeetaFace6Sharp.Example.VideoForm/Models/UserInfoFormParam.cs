using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Example.VideoForm.Models
{
    public class UserInfoFormParam
    {
        public Bitmap Bitmap { get; set; }

        public UserInfo UserInfo { get; set; }

        public bool ReadOnly { get; set; }
    }
}
