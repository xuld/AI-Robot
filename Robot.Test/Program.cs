using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xuld.Robot.Test {
    class Program {
        static void Main(string[] args) {

            Console.Title = "Xuld.Xuld.Robot  自动聊天机器人";
            Console.WriteLine("输入任何文字并回车...");

            Xuld.Robot.Robot robot = new Xuld.Robot.DefaultRobot();

            string s;

            while((s = Console.ReadLine()) != null) {
                //string[] result = Xuld.Robot.ChineseSplitter.Split(s);
                //string f1 = String.Join("/", result);

                string f2 = robot.Process(s, "{s}");

                Console.WriteLine(f2);

            }

        }
    }
}
