using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace NeuToDo.Models {
    public class Course : ObservableObject {
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