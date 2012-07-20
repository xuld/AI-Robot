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
using System.Text;

namespace CorePlus.IO {

    /// <summary>
    /// 表示一个用于读取字符串或控制台的工具。
    /// </summary>
    /// <remarks>
    /// 这个类实现了基本的控制台输入功能。
    /// </remarks>
    /// <example>
    /// 以下示例演示了如何使用 Scanner 类实现输入2个整数，然后输出和 。
    /// <code>
    /// using CorePlus.Logging;
    /// 
    /// class Sample {
    /// 
    ///     public static void Main(){
    ///         int a, b;
    ///         Scanner cin = new Scanner();
    ///         while(cin.Read(out a).Read(out b))
    ///             Logger.Write( a + b );
    ///     }
    ///     
    /// }
    /// </code>
    /// </example>
    public class Scanner {

        #region 变量

        /// <summary>
        /// 位置。
        /// </summary>
        int _position;

        /// <summary>
        /// 缓存字符串。
        /// </summary>
        string _cache;

		/// <summary>
		/// 停止输入记号。
		/// </summary>
        const int FLOW = -6;

        /// <summary>
        /// 初始化 <see cref="Scanner"/> 的新实例。
        /// </summary>
        /// <param name="cache">缓存。</param>
        /// <param name="position">位置。</param>
        public Scanner(string cache = null, int position = 0) {
            _cache = cache ?? String.Empty;
            _position = position;
        }


        #endregion

        #region 私有

        /// <summary>
        /// 准备缓存。
        /// </summary>
        /// <param name="flag">是否为负数。</param>
        /// <returns>长度。</returns>
        int ReadCache(out bool flag) {
            if (_position == FLOW) {
                flag = false;
                return -1;
            }

            int len = _cache.Length;
            while (_position < len && _cache[_position] == ' ')
                _position++;
            if (_position >= len) {

                _position = 0;
                do {
                    _cache = ReadLine();
                    if (_cache == null) {
                        _position = FLOW;
                        flag = false;
                        return -1;
                    }
                } while ((len = _cache.Length) == 0);
            }

            flag = _cache[_position] == '-';
            if (flag || _cache[_position] == '+')
                _position++;

            return len;
        }

        /// <summary>
        /// 准备缓存。
        /// </summary>
        /// <returns>长度。</returns>
        int ReadCache() {
            if (_position == FLOW) {
                return -1;
            }
            int len = _cache.Length;
            while (_position < len && _cache[_position] == ' ')
                _position++;
            if (_position >= len) {

                _position = 0;

                do {
                    _cache = ReadLine();
                    if (_cache == null) {
                        _position = FLOW;
                        return -1;
                    }
                } while ((len = _cache.Length) == 0);
            }
            return len;
        }

        #endregion

        #region 获取

		/// <summary>
		/// 获取一个 Unicode 字符。
		/// </summary>
		/// <returns>一个 Unicode 字符。</returns>
        protected virtual int OnRead() {
            return Console.Read();
        }

		/// <summary>
		/// 获取一行字符串。
		/// </summary>
		/// <returns>字符串。</returns>
        protected virtual string OnReadLine() {
            return Console.ReadLine();
        }

        /// <summary>
        /// 获取一个 Unicode 字符。
        /// </summary>
        /// <returns>一个 Unicode 字符。</returns>
        public char ReadChar() {
            int c = OnRead();
            if(  c == -1){
                _position = FLOW;
                return '\0';
            }
            return (char) c;
        }

        /// <summary>
        /// 获取一行字符串。
        /// </summary>
        /// <returns>字符串。</returns>
        public string ReadLine() {
            string v = OnReadLine();
            if(v == null){
                _position = FLOW;
            }
            return v;
        }

        /// <summary>
        /// 获取一个整数。
        /// </summary>
        /// <returns>整数。</returns>
        public int ReadInt() {
            bool flag;
            int len = ReadCache(out flag);
            if (len == -1) return 0;
            int r = 0;
            for (char c; _position < len; _position++) {
                c = _cache[_position];
                if (c >= '0' && c <= '9') {
                    r = r * 10 + (c - '0');
                    continue;
                }
                while (_position < len && _cache[_position++] != ' ') ;
                break;
            }

            if (flag) r = -r;
            return r;
        }

        /// <summary>
        ///  获取一个64位有符号的整数。
        /// </summary>
        /// <returns>  一个64位有符号的整数。</returns>
        public long ReadLong() {
            bool flag;
            int len = ReadCache(out flag);
            if (len == -1) return 0;
            long r = 0;
            for (char c; _position < len; _position++) {
                c = _cache[_position];
                if (c >= '0' && c <= '9') {
                    r = r * 10 + (c - '0');
                    continue;
                }
                while (_position < len && _cache[_position++] != ' ') ;
                break;
            }

            if (flag) r = -r;
            return r;
        }

        /// <summary>
        /// 获取一个16位有符号的整数。
        /// </summary>
        /// <returns>一个16位有符号的整数。</returns>
        public short ReadShort() {
            return (short)ReadInt();
        }

        /// <summary>
        /// 获取一个8位有符号整数。
        /// </summary>
        /// <returns>一个8位有符号整数。</returns>
        [CLSCompliant(false)]
        public sbyte ReadSByte() {
            return (sbyte)ReadInt();
        }

        /// <summary>
        /// 获取一个8位无符号整数。
        /// </summary>
        /// <returns>一个8位无符号整数。</returns>
        public byte ReadByte() {
            return (byte)ReadInt();
        }

        /// <summary>
        /// 获取一个32位无符号整数。
        /// </summary>
        /// <returns>一个32位无符号整数。</returns>
        [CLSCompliant(false)]
        public uint ReadUInt() {
            bool flag;
            int len = ReadCache(out flag);
            if (len == -1) return 0;
            uint r = 0;
            for (char c; _position < len; _position++) {
                c = _cache[_position];
                if (c >= '0' && c <= '9') {
                    r = r * 10 + (uint)(c - '0');
                    continue;
                }
                while (_position < len && _cache[_position++] != ' ') ;
                break;
            }

            if (flag) unchecked { r = (uint)-r; }
            return r;
        }

        /// <summary>
        /// 获取一个64位无符号整数。
        /// </summary>
        /// <returns>一个64位无符号整数。</returns>
        [CLSCompliant(false)]
        public ulong ReadULong() {
            bool flag;
            int len = ReadCache(out flag);
            ulong r = 0;
            for (char c; _position < len; _position++) {
                c = _cache[_position];
                if (c >= '0' && c <= '9') {
                    r = r * 10 + (ulong)(c - '0');
                    continue;
                }
                while (_position < len && _cache[_position++] != ' ') ;
                break;
            }
            if (flag)
                throw new InvalidCastException("unsigned long无法转换为负数。");
            return r;
        }

        /// <summary>
        /// 获取一个16位无符号整数。
        /// </summary>
        /// <returns>一个16位无符号整数。</returns>
        [CLSCompliant(false)]
        public ushort ReadUShort() {
            return (ushort)ReadUInt();
        }

        /// <summary>
        /// 获取一个单精度浮点数字。
        /// </summary>
        /// <returns>一个单精度浮点数字。</returns>
        public float ReadFloat() {
            bool flag;
            int len = ReadCache(out flag);
            if (len == -1) return 0;
            float r = 0;
            float d = 1;
            for (char c; _position < len; _position++) {
                c = _cache[_position];
                if (c >= '0' && c <= '9') {
                    r = r * 10 + (c - '0');
                    continue;
                }
                if (c == '.' && d == 1) {
                    while (++_position < len) {
                        d *= .1F;
                        c = _cache[_position];
                        if (c >= '0' && c <= '9') {
                            r += (c - '0') * d;
                            continue;
                        }
                        break;
                    }
                    _position--;
                }
                while (_cache[_position] != ' ' && ++_position < len) ;
                break;
            }

            if (flag) r = -r;
            return r;
        }

        /// <summary>
        /// 获取一个双精度浮点数。
        /// </summary>
        /// <returns>一个双精度浮点数。</returns>
        public double ReadDouble() {
            bool flag;
            int len = ReadCache(out flag);
            if (len == -1) return 0;
            double r = 0;
            double d = 1;
            for (char c; _position < len; _position++) {
                c = _cache[_position];
                if (c >= '0' && c <= '9') {
                    r = r * 10 + (c - '0');
                    continue;
                }
                if (c == '.' && d == 1) {
                    while (++_position < len) {
                        d *= .1F;
                        c = _cache[_position];
                        if (c >= '0' && c <= '9') {
                            r += (c - '0') * d;
                            continue;
                        }
                        break;
                    }
                    _position--;
                } while (_cache[_position] != ' ' && ++_position < len) ;
                break;
            }

            if (flag) r = -r;
            return r;
        }

        /// <summary>
        /// 获取一个字符串。
        /// </summary>
        /// <returns>字符串。</returns>
        public string ReadString() {
            int len = ReadCache();
            if (len == -1) return null;
            int start = _position, end = -1;
            for (; _position < len; _position++) {
                if (_cache[_position] == ' ') {
                    end = _position;
                    break;
                }
            }
            return end == -1 ? _cache.Substring(start) : _cache.Substring(start, end - start);
        }

        /// <summary>
        /// 获取一个十进制数。
        /// </summary>
        /// <returns>十进制数。</returns>
        public decimal ReadDecimal() {
            string s = ReadString();
            return s == null ? 0 : decimal.Parse(s);
        }

        /// <summary>
        /// 获取一个日期。
        /// </summary>
        /// <returns>日期。</returns>
        public DateTime ReadDateTime() {
            string s = ReadString();
            return s == null ? DateTime.Now : DateTime.Parse(s);
        }

        /// <summary>
        /// 获取一个布尔值。
        /// </summary>
        /// <returns>一个布尔值。</returns>
        public bool ReadBool() {
            string v = ReadString();
            return v == "1" || (v != null && v.Equals("true", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取一个时间间隔。
        /// </summary>
        /// <returns>时间间隔。</returns>
        public TimeSpan ReadTimeSpan() {
            string s = ReadString();
            return s == null ? TimeSpan.Zero : TimeSpan.Parse(ReadString());
        }


        #endregion

        #region 输出获取

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out bool v) {
            v = ReadBool();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner ReadLine(out string v) {
            v = ReadLine();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out char v) {
            v = ReadChar();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out int v) {
            v = ReadInt();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out short v) {
            v = ReadShort();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out long v) {
            v = ReadLong();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
		[CLSCompliant(false)]
        public Scanner Read(out sbyte v) {
            v = ReadSByte();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out byte v) {
            v = ReadByte();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
		[CLSCompliant(false)]
        public Scanner Read(out ulong v) {
            v = ReadULong();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
		[CLSCompliant(false)]
        public Scanner Read(out uint v) {
            v = ReadUInt();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out float v) {
            v = ReadFloat();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
		[CLSCompliant(false)]
        public Scanner Read(out ushort v) {
            v = ReadUShort();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out decimal v) {
            v = ReadDecimal();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out double v) {
            v = ReadDouble();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out TimeSpan v) {
            v = ReadTimeSpan();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out DateTime v) {
            v = ReadDateTime();
            return this;
        }

        /// <summary>
        /// 读取一个值。
        /// </summary>
        /// <param name="v">输出的变量名。</param>
        /// <returns>读取器，用于连续操作。</returns>
        public Scanner Read(out string v) {
            v = ReadString();
            return this;
        }

		/// <summary>
		/// 实现从 <see cref="Scanner"/> 到 <see cref="System.Boolean"/> 的隐性的转换。
		/// </summary>
		/// <param name="scanner">读取器。</param>
		/// <returns>转换的结果。</returns>
        public static implicit operator bool(Scanner scanner) {
            return scanner._position != FLOW;
        }

        #endregion

		#region 工具

		/// <summary>
		/// 从一个字符串中读取有效的字符串。
		/// </summary>
		/// <param name="input">输入的值。</param>
		/// <param name="startIndex">开始出现引号的位置。</param>
		/// <returns>一个字符串。如有错误，则返回空。</returns>
		public static string ReadString(string input, ref int startIndex) {
			if (input == null)
				return null;
			int ls = input.Length;
			if (ls <= startIndex)
				return String.Empty;
			char c = input[startIndex];
			StringBuilder sb = new StringBuilder(input.Length - startIndex);
			for (; ++startIndex < ls; ) {
				char t = input[startIndex];
				if (t == '\\') {
					if (++startIndex < ls) {
						switch (input[startIndex]) {
							case 'n':
								sb.Append('\n');
								break;
							case 'r':
								sb.Append('\r');
								break;
							case 't':
								sb.Append('\t');
								break;
							case 'a':
								sb.Append('\a');
								break;
							default:
								sb.Append(input[startIndex]);
								break;
						}
					} else
						return null;

				} else if (t == c) {
					return sb.ToString();

				} else
					sb.Append(t);

			}

			return null;
		}

		/// <summary>
		/// 从一个字符串中读取有效的数字。
		/// </summary>
		/// <param name="input">输入的值。</param>
		/// <param name="startIndex">开始出现引号的位置。</param>
		/// <returns>一个字符串。如有错误，则返回空。</returns>
		public static string ReadNumber(string input, ref int startIndex) {
			return ReadValue(input, ref startIndex, c => (c >= '0' && c <= '9') || c == '.');
		}

		/// <summary>
		/// 从一个字符串中读取有效的数字。
		/// </summary>
		/// <param name="input">输入的值。</param>
		/// <param name="startIndex">开始出现引号的位置。</param>
		/// <returns>一个字符串。如有错误，则返回空。</returns>
		public static string ReadLiteral(string input, ref int startIndex) {
			return ReadValue(input, ref startIndex, char.IsLetterOrDigit);
		}

		/// <summary>
		/// 从一个字符串中读取有效的数字。
		/// </summary>
		/// <param name="input">输入的值。</param>
		/// <param name="startIndex">开始出现引号的位置。</param>
		/// <param name="predicate">判断是否继续读的委托。</param>
		/// <returns>一个字符串。如有错误，则返回空。</returns>
		public static string ReadValue(string input, ref int startIndex, Predicate<char> predicate) {
			if (input == null)
				return null;
			int ls = input.Length;
			if (ls <= startIndex)
				return null;
			int i = startIndex;

			for (; ++startIndex < ls; )
				if (predicate(input[startIndex])) {

				} else {
					break;
				}

			return input.Substring(i, startIndex-- - i);
		}

		#endregion

    }

}
