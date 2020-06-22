﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NeuToDo.Models {
    public class Course {
        public string Name { get; set; }

        public string School { get; set; }

        public string ImgUrl { get; set; }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// 课程编号
        /// </summary>
        public string Code { get; set; }
    }
}