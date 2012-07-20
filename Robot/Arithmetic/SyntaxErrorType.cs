/*************************************************************************
 *
 * Copyright (c) 2009-2012 Xuld. All rights reserved.
 * 
 * Project Url: http://work.xuld.net/coreplus
 * 
 * This source code is part of the Project CorePlus for .Net.
 * 
 * This code is licensed under CorePlus License.
 * See the file License.html for the license details.
 * 
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * 
 *************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorePlus.RunTime {


    /// <summary>
    /// 表示一个语法分析错误类型。
    /// </summary>
    public enum SyntaxErrorType {

        /// <summary>
        /// 多余某个操作符。
        /// </summary>
        Unexpected,

        /// <summary>
        /// 缺少一个操作符。
        /// </summary>
        Expected,

        /// <summary>
        /// 不合法的使用。
        /// </summary>
        Invalid,

        /// <summary>
        /// 不识别的操作符。
        /// </summary>
        Unrecognised,

        /// <summary>
        /// 语法错误。
        /// </summary>
        SyntaxError,

        /// <summary>
        /// 语句未结束。
        /// </summary>
        Unclosed,

        /// <summary>
        /// 其它错误。
        /// </summary>
        Other,

        /// <summary>
        /// 语句提前结束。
        /// </summary>
        Break


    }

}
