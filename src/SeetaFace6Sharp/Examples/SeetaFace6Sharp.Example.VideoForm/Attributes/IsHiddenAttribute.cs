﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeetaFace6Sharp.Example.VideoForm.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsHiddenAttribute : Attribute
    {
        public IsHiddenAttribute()
        {

        }

        public IsHiddenAttribute(bool isHidden)
        {
            IsHidden = isHidden;
        }

        public bool IsHidden { get; set; } = false;
    }
}
