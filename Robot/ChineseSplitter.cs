using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Xuld.Robot {
    public static class ChineseSplitter {

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        struct tagICTCLAS_Result {
            public int iStartPos; //开始位置
            public int iLength; //长度

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string szPOS;//词性
            public int iPOS; //词性ID
            public int iWordID; //词ID
            public int iWordType; //词语类型，用户词汇？(0-否,1-是)
            public int iWeight;// 词语权重
        }

        [DllImport("ICTCLAS50.dll")]
        extern static bool ICTCLAS_Init(string path);

        [DllImport("ICTCLAS50.dll")]
        extern static int ICTCLAS_ParagraphProcess(string pszText, int iLength, StringBuilder pszResult, byte codeType = 0, bool bEnablePOS = false);

        [DllImport("ICTCLAS50.dll")]
        unsafe extern static int ICTCLAS_ParagraphProcessAW(string pszText, [Out, MarshalAs(UnmanagedType.LPArray)]tagICTCLAS_Result[] result, byte codeType = 0, bool bEnablePOS = true);

        static ChineseSplitter() {
            ICTCLAS_Init(".");
        }

        public unsafe static string[] Split(string value) {
            tagICTCLAS_Result[] result = new tagICTCLAS_Result[value.Length * 2];
            int i =  ICTCLAS_ParagraphProcessAW(value, result);
            return new string[0];
            //return result.Length.ToString();
        }

    }
}
