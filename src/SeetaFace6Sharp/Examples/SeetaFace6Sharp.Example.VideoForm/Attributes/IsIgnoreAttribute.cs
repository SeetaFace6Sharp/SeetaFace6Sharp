using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Example.VideoForm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsIgnoreAttribute : Attribute
    {
        public IsIgnoreAttribute()
        {

        }

        public IsIgnoreAttribute(bool isIgnore)
        {
            IsIgnore = isIgnore;
        }

        public bool IsIgnore { get; set; } = true;
    }
}
