﻿using FreeSql.DataAnnotations;
using System;
using System.ComponentModel;
using SeetaFace6Sharp.Example.VideoForm.Attributes;

namespace SeetaFace6Sharp.Example.VideoForm.Models
{
    [Table(Name = "user_info")]
    public class UserInfo
    {
        [IsReadonly(true)]
        [Description("Id")]
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Description("姓名")]
        [Column(StringLength = 150, IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Description("年龄")]
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Description("性别")]
        public GenderEnum Gender { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        [Column(StringLength = 300, IsNullable = true)]
        public string Remark { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Description("电话")]
        [Column(StringLength = 20, IsNullable = true)]
        public string Phone { get; set; }

        /// <summary>
        /// 图片（Base64）编码
        /// </summary>
        [IsHidden(true)]
        [Column(DbType = "text")]
        public string Image { get; set; }

        /// <summary>
        /// 人脸识别数据
        /// </summary>
        [IsHidden(true)]
        [Column(DbType = "text", IsNullable = true)]
        [Description("人脸识别数据")]
        public string Extract { get; set; }

        [IsIgnore]
        public bool IsDelete { get; set; }

        [IsReadonly(true)]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        [IsReadonly(true)]
        [Description("更新时间")]
        [Column(IsNullable = true)]
        public DateTime? UpdateTime { get; set; }

        private float[] _extractData = null;

        /// <summary>
        /// 获取人脸识别数据（懒加载）
        /// </summary>
        /// <returns></returns>
        public float[] GetExtractData()
        {
            if (_extractData != null)
            {
                return _extractData;
            }
            if (string.IsNullOrWhiteSpace(Extract))
            {
                return new float[0];
            }
            string[] dataStr = this.Extract.Split(';');
            float[] data = new float[dataStr.Length];
            for (int i = 0; i < dataStr.Length; i++)
            {
                data[i] = float.Parse(dataStr[i]);
            }
            _extractData = data;
            return _extractData;
        }
    }
}
