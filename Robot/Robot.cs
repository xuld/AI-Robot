using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xuld.Robot {

    /// <summary>
    /// 表示一个机器人。
    /// </summary>
    public class Robot:IDisposable {

        public virtual string Process(string message, string who) {
            return message;
        }

        public virtual void Dispose() {

        }
    }

}
