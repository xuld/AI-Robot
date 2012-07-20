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
using System.Collections;
using System.Collections.Generic;
using CorePlus.IO;

namespace CorePlus.RunTime {

	/// <summary>
	/// 提供数学表达式的实时计算功能。
	/// </summary>
	/// <remarks>
	/// 要求表达式以空格作为分隔符
	/// 转换表达式折分为：
	/// 变量及数值 ,变量不允许为@
	/// 字符串“”
	/// 运算符号{+、-、*、/、++、+=、--、-=、*=、/=、!、!=、&gt;、&gt;=、&gt;&gt;、&lt;、&lt;=、&lt;&gt;、|、|=、||、&amp;、&amp;=、&amp;&amp;}
	/// 括号{包括(、)}
    /// 
    /// <example>
    /// 以下示例演示了如何使用 <see cref="Arithmetic"/> 计算表达式。
    /// <code>
    /// using System;
    /// using CorePlus.RunTime;
    /// 
    /// class Sample{
    /// 
    ///     public static void Main(){
    ///         Arithmetic calc = new Arithmetic();
    ///         double result = (double)calc.Compute("1 + 2sin 4");
    ///         Console.Write(result);
    ///         Console.Read();
    ///     }
    /// }
    /// </code>
    /// 一个表达式中的函数可自定义。
    /// 以下演示了如何自定义函数。
    /// <code>
    /// using System;
    /// using CorePlus.RunTime;
    /// 
    /// class Sample{
    /// 
    ///     public static void Main(){
    ///         Arithmetic calc = new Arithmetic();
    ///         calc.Operators.Add(new Arithmetic.FunctionCallOperator("in", v => v + 1);
    ///         double result = (double)calc.Compute("1 + in 4");
    ///         Console.Write(result); //  1 + ( 4 + 1 ) == 6
    ///         Console.Read();
    ///     }
    /// }
    /// </code>
    /// </example>
    /// 默认计算器直接支持 Boolean Int32 Double Char String 这 4 个类型。因此下列表达式合法:
    /// 1 + ( 2 == 2 )   // 2
    /// "1" + 'd'  //  "1d"
    /// true || false // true
	/// </remarks>
	public sealed class Arithmetic {

		#region 算

		/// <summary>
		/// 表达式分割为列表形式。
		/// </summary>
		/// <param name="expression">表达式。</param>
		/// <returns>表达式。</returns>
		/// <exception cref="ArgumentNullException"><paramref name="expression" /> 为空。</exception>
        /// <exception cref="ArithmeticException"><paramref name="expression" /> 不是合法的表达式。</exception>
		List<Token> ParseExpression(string expression) {
			List<Token> list = new List<Token>();
			int count = expression.Length - 1;
			char c;
			string opt = null;
			bool? flag = null;
			TokenType lastTokenType = TokenType.LeftBucket;
			for (int i = 0; i <= count; i++) {
				c = expression[i];

				#region 数字

				if ((c >= '0' && c <= '9') || c == '.') {
					opt = Scanner.ReadNumber(expression, ref i);
					list.Add(new Token {
						Value = opt,
						Type = lastTokenType = opt.IndexOf('.') == -1 ? TokenType.Integer : TokenType.Double
					});
					continue;
				}

				#endregion

				#region 变量

				if (c == '$') {
					if (lastTokenType == TokenType.Integer || lastTokenType == TokenType.Double) {
						list.Add(new Token {
							Value = "*",
							Type = TokenType.Operator
						});
					}
					i++;
					list.Add(new Token {
						Value = Scanner.ReadValue(expression, ref i, rrr => char.IsLetter(rrr) || rrr == '.'),
						Type = lastTokenType = TokenType.Variant
					});
					continue;
				}
				
				#endregion

				#region 字符

				if (char.IsLetter(c)) {
					if (lastTokenType == TokenType.Integer || lastTokenType == TokenType.Double) {
						list.Add(new Token {
							Value = "*",
							Type = TokenType.Operator
						});
					}
					opt = Scanner.ReadValue(expression, ref i, char.IsLetter);
					if (opt == "null")
						list.Add(new Token {
							Value = String.Empty,
							Type = lastTokenType = TokenType.String
						});
					else
						list.Add(new Token {
							Value = opt,
							Type = lastTokenType = opt == "true" || opt == "false" ? TokenType.Boolean : TokenType.Operator
						});
					continue;
				}

				#endregion

				#region 字符判别
				switch (c) {
					#region ( )
					case '(':
						list.Add(new Token {
							Type = lastTokenType = TokenType.LeftBucket
						});
						continue;
					case ')':
						list.Add(new Token {
							Type = lastTokenType = TokenType.RightBucket
						});
						continue;
					case ' ':
						continue;
					#endregion

					#region + - * / %
					case '+':
					case '-':
						if (lastTokenType == TokenType.Operator) {
							FillBucket(list, ref flag);
						}
						if (i >= count) {
							opt = c.ToString();
						} else {
							char c1 = expression[++i];
							if (c1 == c || c1 == '=') {
								opt = new String(new char[] { c, c1 });
							} else {
								opt = c.ToString();
								if (c1 != ' ')
									i--;
							}
						}


						break;
					case '*':
					case '/':
					case '%':
						if (i >= count) {
							opt = c.ToString();
						} else {
							if (expression[++i] == '=') {
								opt = new String(new char[] { c, '=' });
							} else {
								opt = c.ToString();
								if (expression[i] != ' ')
									i--;
							}
						}
						break;
					#endregion

					#region 字符串

					case '\"':
					case '\'':
						Token t = new Token();
						t.Type = lastTokenType = c == '\"' ? TokenType.String : TokenType.Char;
						t.Value = Scanner.ReadString(expression, ref i);
						if (t.Value == null)
							throw new SyntaxException("\"未关闭");
						list.Add(t);
						continue;

					#endregion

					#region > < =
					case '>':
						if (i >= count) {
							opt = ">";
						} else
							switch (c = expression[++i]) {
								case '>':
								case '=':
									opt = ">" + c;
									break;
								case ' ':
									opt = ">";
									break;
								default:
									opt = ">";
									i--;
									break;
							}
						break;
					case '<':
						if (i >= count) {
							opt = "<";
						} else
							switch (c = expression[++i]) {
								case '<':
								case '=':
									opt = "<" + c;
									break;
								case '>':
									opt = "!=";
									break;
								case ' ':
									opt = "<";
									break;
								default:
									opt = "<";
									i--;
									break;
							}
						break;
					case '~':
						if (i >= count) {
							opt = "~";
						} else
							switch (expression[++i]) {
								case '=':
									opt = "~=";
									break;
								default:
									opt = "~";
									i--;
									break;
							}
						break;
					case '=':
						if (i >= count) {
							opt = "==";
						} else
							switch (c = expression[++i]) {
								case '=':
								case ' ':
									opt = "==";
									break;
								case '>':
									opt = "=>";
									break;
								default:
									opt = "==";
									i--;
									break;
							}
						break;
					#endregion

					#region ! | &
					case '!':
						if (i >= count) {
							opt = "!";
						} else
							switch (c = expression[++i]) {
								case '=':
									opt = "!=";
									break;
								default:
									opt = "!";
									i--;
									break;
							}
						break;
					case '|':
						if (i >= count) {
							opt = "|";
						} else {
							switch (c = expression[++i]) {
								case '=':
									opt = "!=";
									break;
								case '|':
									opt = "||";
									break;
								case ' ':
									opt = "|";
									break;
								default:
									opt = "|";
									i--;
									break;
							}
						}
						break;
					case '&':
						if (i >= count) {
							opt = "&";
						} else {
							switch (c = expression[++i]) {
								case ' ':
									opt = "&";
									break;
								case '=':
									opt = "&=";
									break;
								case '&':
									opt = "&&";
									break;
								default:
									opt = "&";
									i--;
									break;
							}
						}
						break;
					#endregion

					#region 其它

					case '\\':
						opt = "\\";
						break;
					case '^':
						opt = "^";
						break;
					#endregion

					default:
						throw new SyntaxException("不能处理字符 " + c, SyntaxErrorType.Unexpected);
				}

				if (flag == true) {
					list.Add(new Token {
						Type = TokenType.RightBucket
					});
					flag = null;
				} else if (flag == false)
					flag = true;

				list.Add(new Token {
					Value = opt,
					Type = lastTokenType = TokenType.Operator
				});

				#endregion
			}

			if (flag != null) {
				list.Add(new Token {
					Type = TokenType.RightBucket
				});
			}

			return list;
		}

		static void FillBucket(List<Token> list, ref bool? flag) {
			if (flag != null) {
				list.Add(new Token {
					Type = TokenType.RightBucket
				});
			}
			list.Add(new Token {
				Type = TokenType.LeftBucket
			});


			flag = false;

		}

		/// <summary>
		/// 中缀表达式转换为后缀表达式。
		/// </summary>
		/// <param name="ex">表达式。</param>
		/// <returns>表达式。</returns>
		List<Token> ConvertToPostfix(List<Token> ex) {

			int s = 0;
			Stack<Token> sOperator = new Stack<Token>();
			Token t;
			int count = ex.Count;
			int i = 0;
			while (i < count) {
				t = ex[i++];

				//·读到左括号时总是将它压入栈中
				if (t.Type == TokenType.LeftBucket) {
					//  ex[s++] = t;
					sOperator.Push(t);
				} else if (t.Type == TokenType.RightBucket) {
					if (sOperator.Count == 0)
						throw new SyntaxException("多余的 )", SyntaxErrorType.Unexpected);
					while (sOperator.Count > 0) {
						if (sOperator.Peek().Type == TokenType.LeftBucket) {
							sOperator.Pop();
							break;
						}

						ex[s++] = sOperator.Pop();
					}
				} else if (t.IsVar) {
					ex[s++] = t;
				} else {

					//·当读到运算符t时，
					//　　　　　a.将栈中所有优先级高于或等于t的运算符弹出，送到输出队列中； 
					//　　　　　b.t进栈
					Operator tt, p = GetOperators(t.Value);

					while (sOperator.Count > 0 && sOperator.Peek().Type != TokenType.LeftBucket && p.Priority >= (tt = GetOperators(sOperator.Peek().Value)).Priority) {
						if (tt.IsSingle && p.IsSingle)
							break;
						ex[s++] = sOperator.Pop();
					}
					sOperator.Push(t);
				}

			}

			//中缀表达式全部读完后，若栈中仍有运算符，将其送到输出队列中
			while (sOperator.Count > 0) {
				ex[s++] = sOperator.Pop();
			}

			while (count-- > s)
				ex.RemoveAt(count);
			return ex;
		}

		/// <summary>
		/// 计算后缀表达式。
		/// </summary>
		/// <param name="expression">表达式。</param>
		/// <returns>值。</returns>
		/// <exception cref="SyntaxException">表达式不符合运算规则。</exception>
		object ComputePostfix(List<Token> expression) {
			//·建立一个栈S
			Stack s = new Stack();
			foreach (Token t in expression) {
				switch (t.Type) {
					case TokenType.Integer:
						s.Push(int.Parse(t.Value, System.Globalization.CultureInfo.CurrentCulture));
						break;
					case TokenType.Operator:
						Operator opt = GetOperators(t.Value);
						if (s.Count == 0)
							throw new SyntaxException("缺少操作数。");
						if (opt.IsSingle) {
							s.Push(opt.Compute(s.Pop()));
						} else {
							object v = s.Pop();
							if (s.Count > 0) {
								if (s.Peek() != null)
									s.Push(opt.Compute(s.Pop(), v));
								else {
									s.Pop();
									s.Push(opt.Compute(v));
								}
							} else
								s.Push(opt.Compute(v));
						}
						break;
					case TokenType.Char:
						s.Push((int)char.Parse(t.Value));
						break;
					case TokenType.Double:
						s.Push(double.Parse(t.Value, System.Globalization.CultureInfo.CurrentCulture));
						break;
					case TokenType.LeftBucket:
						s.Push(null);
						break;
					case TokenType.String:
						s.Push(t.Value);
						break;
					case TokenType.Boolean:
						s.Push(bool.Parse(t.Value));
						break;
					case TokenType.Variant:
						s.Push(_variants[t.Value]);
						break;
				}


			}



			return s.Pop();

		}

		/// <summary>
		/// 计算表达式。
		/// </summary>
		/// <param name="expression">表达式。</param>
		/// <returns>计算的结果。</returns>
		/// <exception cref="SyntaxException">表达式不符合运算规则。</exception>
		/// <exception cref="ArithmeticException">表达式不符合运算规则。</exception>
		/// <exception cref="ArgumentNullException">表达式是空字符串。</exception>
		/// <exception cref="DivideByZeroException">被 0 除。</exception>
        public object Compute(string expression) {
			return ComputePostfix(ConvertToPostfix(ParseExpression(expression)));
		}

		/// <summary>
		/// 初始化 <see cref="CorePlus.RunTime.Arithmetic"/> 的新实例。
		/// </summary>
		public Arithmetic() {
			Setup();
		}

		#endregion

		#region 其它

		/// <summary>
		/// 变量集合。
		/// </summary>
		[Serializable]
		public class VariantCollection : System.Collections.Specialized.NameObjectCollectionBase, ICollection {

			/// <summary>
			/// 获取或设置指定名字的变量值。
			/// </summary>
			/// <param name="varName">变量名，不包括 $ 。</param>
			/// <returns>指定变量的值。</returns>
			/// <exception cref="ArithmeticException">变量不存在。</exception>
			public object this[string varName] {
				get {
					object v = BaseGet(varName);
					if (v == null)
						throw new KeyNotFoundException("变量" + varName + "未定义。");
					return v;
				}
				set {
					BaseSet(varName, value);
				}
			}

			/// <summary>
			/// 从特定的 <see cref="T:System.Array"/> 索引处开始，将 <see cref="T:System.Collections.ICollection"/> 的元素复制到一个 <see cref="T:System.Array"/> 中。
			/// </summary>
			/// <param name="array">作为从 <see cref="T:System.Collections.ICollection"/> 复制的元素的目标位置的一维 <see cref="T:System.Array"/>。<see cref="T:System.Array"/> 必须具有从零开始的索引。</param>
			/// <param name="index"><paramref name="array"/> 中从零开始的索引，在此处开始复制。</param>
			/// <exception cref="T:System.ArgumentNullException">
			/// 	<paramref name="array"/> 为 null。 </exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			/// 	<paramref name="index"/> 小于零。 </exception>
			/// <exception cref="T:System.ArgumentException">
			/// 	<paramref name="array"/> 是多维的。- 或 - <paramref name="index"/> 等于或大于 <paramref name="array"/> 的长度。- 或 - 源 <see cref="T:System.Collections.ICollection"/> 中的元素数目大于从 <paramref name="index"/> 到目标 <paramref name="array"/> 末尾之间的可用空间。 </exception>
			/// <exception cref="T:System.ArgumentException">源 <see cref="T:System.Collections.ICollection"/> 的类型无法自动转换为目标 <paramref name="array"/> 的类型。 </exception>
			public void CopyTo(Array array, int index) {
				foreach (string vars in base.BaseGetAllValues())
					array.SetValue(vars, index++);
			}

		}

		/// <summary>
		/// 当前的变量集合。
		/// </summary>
		VariantCollection _variants = new VariantCollection();

		/// <summary>
		/// 获取当前的变量。
		/// </summary>
		public VariantCollection Variants {
			get {
				return _variants;
			}
		}

		/// <summary>
		/// 表示表达式。
		/// </summary>
		struct Token {

			/// <summary>
			/// 表达式的类型。
			/// </summary>
			public TokenType Type;

			/// <summary>
			/// 表达式的值。
			/// </summary>
			public string Value;

			/// <summary>
			/// 获取当前表达式是否为变量。
			/// </summary>
			public bool IsVar {
				get {
					return Type != TokenType.Operator && Type != TokenType.LeftBucket && Type != TokenType.RightBucket;
				}
			}

			/// <summary>
			/// 返回该实例的完全限定类型名。
			/// </summary>
			/// <returns>
			/// 包含完全限定类型名的 <see cref="T:System.String"/>。
			/// </returns>
			public override string ToString() {
				return Value;
			}
		}

		/// <summary>
		/// 表示一个表达式的类型。
		/// </summary>
		enum TokenType {

			/// <summary>
			/// 操作符。
			/// </summary>
			Operator,

			/// <summary>
			/// 变量。
			/// </summary>
			Variant,

			/// <summary>
			/// 整数。
			/// </summary>
			Integer,

			/// <summary>
			/// 布尔。
			/// </summary>
			Boolean,

			/// <summary>
			/// 浮点数。
			/// </summary>
			Double,

			/// <summary>
			/// 字符串。
			/// </summary>
			String,

			/// <summary>
			/// 字符。
			/// </summary>
			Char,

			/// <summary>
			/// 左括号。
			/// </summary>
			LeftBucket,

			/// <summary>
			/// 右括号。
			/// </summary>
			RightBucket

		}

		/// <summary>
		/// 表示一个操作符。
		/// </summary>
		public abstract class Operator:IComparable<Operator> {

			#region 属性

			/// <summary>
			/// 获取或设置当前操作法的优先等级。
			/// </summary>
			public int Priority {
				get;
				set;
			}

			/// <summary>
			/// 获取或设置当前操作符的名字。
			/// </summary>
			public string Name {
				get;
				set;
			}

			/// <summary>
			/// 获取指示当前操作符是否是单目操作的布尔值。
			/// </summary>
			public abstract bool IsSingle {
				get;
			}


			#endregion

			#region 方法

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.Operator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="priority">优先等级。</param>
			protected Operator(string name, int priority) {
				Name = name;
				Priority = priority;
			}

			/// <summary>
			/// 返回表示当前 <see cref="T:System.Object"/> 的 <see cref="T:System.String"/>。
			/// </summary>
			/// <returns>
			/// 	<see cref="T:System.String"/>，表示当前的 <see cref="T:System.Object"/>。
			/// </returns>
			public override string ToString() {
				return Name;
			}

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public abstract object Compute(object right);

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="SyntaxException">无法计算。</exception>
			public abstract object Compute(object left, object right);

			/// <summary>
			/// 当计算发生错误时运行。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			protected object OnError(object left, object right) {
				throw new ArithmeticException(String.Format("无法计算表达式 {{{0} {1} {2}}} 的值。", left, Name, right));
			}

			/// <summary>
			/// 当计算发生错误时运行。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>值。</returns>
			protected object OnError(object right) {
                throw new ArithmeticException(String.Format("无法计算表达式 {{{0}{1}}} 的值。", Name, right));
			}

			#endregion

            #region IComparable<Operator> 成员

            /// <summary>
            /// 比较当前对象和同一类型的另一对象。
            /// </summary>
            /// <param name="other">与此对象进行比较的对象。</param>
            /// <returns>一个 32 位有符号整数，指示要比较的对象的相对顺序。返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 大于零 此对象大于 other。</returns>
            public int CompareTo(Operator other) {
                return Name.CompareTo(other);
            }

            #endregion
        }

		/// <summary>
		/// 表示四则计算的操作符。
		/// </summary>
		public class ArithmeticOperator : Operator {

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.ArithmeticOperator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="priority">优先等级。</param>
			/// <param name="intC">计算的委托。</param>
			/// <param name="doubleC">计算的委托。</param>
            /// <exception cref="ArgumentNullException"><paramref name="intC" /> 或 <paramref name="doubleC" /> 为空。</exception>
			public ArithmeticOperator(string name, int priority, Func<int, int, int> intC, Func<double, double, double> doubleC)
				: base(name, priority) {
				_getter1 = intC;
				_getter2 = doubleC;
			}

			Func<int, int, int> _getter1;

			Func<double, double, double> _getter2;

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public override object Compute(object right) {

				switch (Name) {
					case "+":
						return right;
					case "-":
						if (right is int)
							return -(int)right;
						if (right is double)
							return -(double)right;
						if (right is bool)
							return (bool)right ? -1 : 0;
						break;

				}

				return OnError(right);

			}

			/// <summary>
			/// 获取指示当前操作符是否是单目操作的布尔值。
			/// </summary>
			/// <value></value>
			public override bool IsSingle {
				get {
					return false;
				}
			}

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="SyntaxException">无法计算。</exception>
			public override object Compute(object left, object right) {
				if (left is bool)
					left = (bool)left ? 1 : 0;
				if (right is bool)
					right = (bool)right ? 1 : 0;
				if (left is int)
					if (right is int)
						return _getter1((int)left, (int)right);
					else
						left = (double)(int)left;
				if (left is double)
					if (right is double)
						return _getter2((double)left, (double)right);
					else if (right is int)
						return _getter2((double)left, (double)(int)right);

                if (Name == "+" && right != null) {
                    return String.Concat(left.ToString(), right.ToString());

                }

				return OnError(left, right);

			}

		}

		/// <summary>
		/// 表示转为布尔的操作符。
		/// </summary>
		public class BoolOperator : Operator {

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.BoolOperator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="priority">优先等级。</param>
			public BoolOperator(string name, int priority)
				: base(name, priority) {
			}

			/// <summary>
			/// 获取指示当前操作符是否是单目操作的布尔值。
			/// </summary>
			/// <value></value>
			public override bool IsSingle {
				get {
					return Name == "!";
				}
			}

			#region 单目运算


			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public override object Compute(object right) {
				if (Name == "!") {
					if (right is bool)
						return !(bool)right;
					if (right is int)
						return (int)right == 0;
					if (right is double)
						return (double)right == 0;
					return right.ToString().Length == 0;
				}
				return OnError(right);
			}


			#endregion

			#region 双目运算

			/// <summary>
			/// 计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="DivideByZeroException">被 0 除。</exception>
			/// <exception cref="SyntaxException">不支持的运算符。</exception>
			object Compute(double left, double right) {
				switch (Name) {
					case "=":
					case "==":
						return (left == right);
					case "!=":
						return (left != right);
					case ">":
						return (left.CompareTo(right) > 0);
					case ">=":
						return (left.CompareTo(right) >= 0);
					case "<":
						return (left.CompareTo(right) < 0);
					case "<=":
						return (left.CompareTo(right) <= 0);
					case "||":
						return left != 0 || right != 0;
					case "&&":
						return left != 0 && right != 0;
					default:
						return OnError(left, right);
				}
			}

			/// <summary>
			/// 计算表达式的值。
			/// </summary>
			///<param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			object Compute(bool left, bool right) {
				switch (Name) {

					case "||":
					case "|":
						return left || right;
					case "&&":
					case "&":
						return left && right;
					case ">":
						return left.CompareTo(right) > 0;
					case ">=":
						return left.CompareTo(right) >= 0;
					case "<":
						return left.CompareTo(right) < 0;
					case "<=":
						return left.CompareTo(right) <= 0;
					case "=":
					case "==":
						return left == right;
					case "!=":
						return left != right;
					default:
						return OnError(left, right);
				}
			}

			/// <summary>
			/// 计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			object Compute(string left, string right) {
				switch (Name) {
					case "=":
					case "==":
						return (left == right);
					case "~=":
						return left.Equals(right, StringComparison.OrdinalIgnoreCase);
					case "!=":
						return (left != right);
					case ">":
						return (String.Compare(left, right, StringComparison.Ordinal) > 0);
					case ">=":
						return (String.Compare(left, right, StringComparison.Ordinal) >= 0);
					case "<":
						return (String.Compare(left, right, StringComparison.Ordinal) < 0);
					case "<=":
						return (String.Compare(left, right, StringComparison.Ordinal) <= 0);
					default:
						return OnError(left, right);
				}
			}

			/// <summary>
			/// 计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="DivideByZeroException">被 0 除。</exception>
			/// <exception cref="SyntaxException">不支持的运算符。</exception>
			object Compute(int left, int right) {

				switch (Name) {
					case "=":
					case "==":
						return (left == right);
					case "!=":
						return (left != right);
					case ">":
						return (left.CompareTo(right) > 0);
					case ">=":
						return (left.CompareTo(right) >= 0);
					case "<":
						return (left.CompareTo(right) < 0);
					case "<=":
						return (left.CompareTo(right) <= 0);

					case ">>":
						return left >> right;
					case "<<":
						return left << right;
					case "|":
						return left | right;
					case "&":
						return left & right;
					case "^":
						return left ^ right;
					case "||":
						return left != 0 || right != 0;
					case "&&":
						return left != 0 && right != 0;
					default:
						return OnError(left, right);
				}
			}


			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="SyntaxException">无法计算。</exception>
			public override object Compute(object left, object right) {
				if (left is bool)
					if (right is bool)
						return Compute((bool)left, (bool)right);
					else
						left = (bool)left ? 1 : 0;
				if (left is int)
					if (right is int)
						return Compute((int)left, (int)right);
					else
						left = (double)left;
				if (left is double)
					if (right is int || right is double)
						return Compute((double)left, (double)right);

				return Compute(right.ToString(), left.ToString());

			}

			#endregion

		}

		/// <summary>
		/// 表示位操作的操作符。
		/// </summary>
		public class BitOperator : Operator {

			/// <summary>
			/// 获取指示当前操作符是否是单目操作的布尔值。
			/// </summary>
			/// <value></value>
			public override bool IsSingle {
				get {
					return false;
				}
			}

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.BitOperator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="priority">优先等级。</param>
			/// <param name="intC">返回计算结果的委托。</param>
			public BitOperator(string name, int priority, Func<int, int, int> intC)
				: base(name, priority) {
				_getter = intC;
			}

			Func<int, int, int> _getter;

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public override object Compute(object right) {
				return OnError(right);
			}

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="SyntaxException">无法计算。</exception>
			public override object Compute(object left, object right) {
				if (left is bool)
					left = (bool)left ? 1 : 0;
				if (right is bool)
					right = (bool)right ? 1 : 0;
				try {
					return _getter((int)left, (int)right);
				} catch (InvalidCastException) {
					return OnError(left, right);
				}

			}

		}

		/// <summary>
		/// 表示单表达式的操作符。
		/// </summary>
		public class SingleOperator : Operator {

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.SingleOperator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="priority">优先等级。</param>
			public SingleOperator(string name, int priority)
				: base(name, priority) {
			}



			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public override object Compute(object right) {

				switch (Name) {
					case "++":
						if (right is int)
							return (int)right + 1;
						if (right is double)
							return (double)right + 1;
						goto default;
					case "--":
						if (right is int)
							return (int)right - 1;
						if (right is double)
							return (double)right - 1;
						goto default;
					case "~":
						try {
							return ~(int)right;
						} catch (InvalidCastException) {
							goto default;
						}
					default:
						return OnError(right);

				}
			}

			/// <summary>
			/// 获取指示当前操作符是否是单目操作的布尔值。
			/// </summary>
			/// <value></value>
			public override bool IsSingle {
				get {
					return true;
				}
			}


			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="SyntaxException">无法计算。</exception>
			public override object Compute(object left, object right) {
				return OnError(left, right);
			}

		}

		/// <summary>
		/// 表示调用一个函数的操作符。
		/// </summary>
		public class FunctionCallOperator : SingleOperator {

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.FunctionCallOperator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="fn">计算的委托。</param>
			public FunctionCallOperator(string name, Func<double, double> fn)
				: base(name, 3) {
				_getter = fn;
			}

			Func<double, double> _getter;

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public override object Compute(object right) {

				if (right is int)
					return _getter((double)(int)right);
				if (right is double)
					return _getter((double)right);
				if (right is bool) {
					return _getter((bool)right ? 1.0 : 0.0);
				}
				return OnError(right);
			}

		}

		/// <summary>
		/// 表示其它操作符。
		/// </summary>
		public class OtherOperator : Operator {

			/// <summary>
			/// 初始化 <see cref="CorePlus.RunTime.Arithmetic.OtherOperator"/> 的新实例。
			/// </summary>
			/// <param name="name">名字。</param>
			/// <param name="priority">优先等级。</param>
			public OtherOperator(string name, int priority)
				: base(name, priority) {
			}

			/// <summary>
			/// 获取指示当前操作符是否是单目操作的布尔值。
			/// </summary>
			/// <value></value>
			public override bool IsSingle {
				get {
					return false;
				}
			}

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="right">要计算的值。</param>
			/// <returns>计算的结果。</returns>
			public override object Compute(object right) {

				return OnError(right);
			}

			/// <summary>
			/// 通过当前操作符计算表达式的值。
			/// </summary>
			/// <param name="left">左值。</param>
			/// <param name="right">右值。</param>
			/// <returns>值。</returns>
			/// <exception cref="SyntaxException">无法计算。</exception>
			public override object Compute(object left, object right) {
				return OnError(left, right);
			}
		}

		/// <summary>
		/// 操作符数组。
		/// </summary>
		List<Operator> _operators;

		/// <summary>
		/// 获取支持的操作符。
		/// </summary>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> 为空。</exception>
		public List<Operator> Operators {
			get {
				return _operators;
			}
		}

		/// <summary>
		/// 初始化。
		/// </summary>
		void Setup() {
			_operators = new List<Operator> {
				new OtherOperator("(", 3),
				new OtherOperator(")", 3),
				new ArithmeticOperator("*", 7, (a, b)=> a * b, (a, b)=> a * b),
				new ArithmeticOperator("/", 7, (a, b)=> a / b, (a, b)=> a / b),

				new ArithmeticOperator("+", 10, (a, b)=> a + b, (a, b)=> a + b),
				new ArithmeticOperator("-", 10, (a, b)=> a - b, (a, b)=> a - b),
				new ArithmeticOperator("\\", 10, (a, b)=> a / b, (a, b)=> (double) ( (int) (a / b - a % b / b ) )),
				new ArithmeticOperator("^", 5, (a, b)=> (int)Math.Pow((double)a, (double)b), Math.Pow),

				new BoolOperator(">", 20),
				new BoolOperator(">=", 20),
				new BoolOperator("<", 20),
				new BoolOperator("<=", 20),
				new BoolOperator("!=", 20),
				new BoolOperator("==", 20),
				new BoolOperator("~=", 20),

				new BoolOperator("!", 4),
				new BoolOperator("||", 42),
				new BoolOperator("&&", 43),

				new SingleOperator("++", 2),
				new SingleOperator("--", 2),
				new BitOperator("&", 40, (a, b) => a & b),
				new SingleOperator("~", 4),
				new BitOperator("|", 40, (a, b) => a | b),
				new BitOperator(">>", 40, (a, b) => a >> b),
				new BitOperator("<<", 40, (a, b) => a << b),

				new ArithmeticOperator("%", 7, (a, b)=> a % b, (a, b)=> a % b),

				new FunctionCallOperator("sin", Math.Sin),
				new FunctionCallOperator("cos", Math.Cos),
				new FunctionCallOperator("tan", Math.Tan),
				new FunctionCallOperator("tg", Math.Tan),
				new FunctionCallOperator("cot", d => 1 / Math.Tan(d)),
				new FunctionCallOperator("ctg", d => 1 / Math.Tan(d)),
				new FunctionCallOperator("sinh", Math.Sinh),
				new FunctionCallOperator("cosh", Math.Cosh),
				new FunctionCallOperator("tanh", Math.Tanh),
				new FunctionCallOperator("sqrt", Math.Sqrt),
				new FunctionCallOperator("round", Math.Round),
				new FunctionCallOperator("lg", Math.Log10),
				new FunctionCallOperator("ln", Math.Log),
				new FunctionCallOperator("exp", Math.Exp),
				new FunctionCallOperator("floor", Math.Floor),
				new FunctionCallOperator("ceil", Math.Ceiling),
				new FunctionCallOperator("abs", Math.Abs),
				new FunctionCallOperator("arccos", Math.Acos),
				new FunctionCallOperator("arcsin", Math.Asin),
				new FunctionCallOperator("arctan", Math.Atan),
				new FunctionCallOperator("int", Math.Truncate)
			};

            UpdateOperators();

			_operators.TrimExcess();


		}

        /// <summary>
        /// 更新缓存的操作符列表。
        /// </summary>
        public void UpdateOperators() {

            _operators.Sort((a, b) => a.Name.CompareTo(b.Name));
        }

		/// <summary>
		/// 根据名字获取操作符。
		/// </summary>
		/// <param name="name">名字。</param>
		/// <returns>操作符。</returns>
		/// <exception cref="SyntaxException">找不到需要的操作符。</exception>
		Operator GetOperators(string name) {
			int middle, t;
			int start = 0, end = _operators.Count;

			end--;


			while (start <= end) {
				middle = (start + end) / 2;

				t = _operators[middle].Name.CompareTo( name );

				if (t < 0)
					start = middle + 1;
				else if (t > 0)
					end = middle - 1;
				else
					return _operators[middle];
			}

			throw new SyntaxException("不支持的函数或操作符 " + name, SyntaxErrorType.Invalid);
		}

		#endregion

		#region 工具

		/// <summary>
		/// 计算表达式。
		/// </summary>
		/// <param name="expression">表达式。</param>
		/// <returns>计算的结果。</returns>
		/// <exception cref="SyntaxException">表达式不符合运算规则。</exception>
		/// <exception cref="ArithmeticException">表达式不符合运算规则。</exception>
		/// <exception cref="ArgumentNullException">表达式是空字符串。</exception>
		/// <exception cref="DivideByZeroException">被 0 除。</exception>
		public static object ComputeExpression(string expression) {
			return new Arithmetic().Compute(expression);
		}

		#endregion
	}
}
