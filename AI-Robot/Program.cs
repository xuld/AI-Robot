using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Xuld.Robot {
    public class Program {
        static void Main(string[] args) {

            Console.Title = "AI-Robot 自动聊天机器人";
            Console.WriteLine("输入任何文字并回车...");

            // 创建一个简单的机器人并回复。
            IRobot robot = new SimpleRobot();
            string s;

            while ((s = Console.ReadLine()) != null) {
                string answer = robot.Answer(s, new NameValueCollection());

                if (answer == null) {
                    Console.WriteLine("(无法回复)");
                } else {
                    Console.WriteLine(answer);
                }

            }

        }
    }
}
