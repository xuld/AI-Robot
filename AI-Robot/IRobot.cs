using System;
using System.Collections.Generic;
using System.Text;

namespace Xuld.Robot {

    /// <summary>
    /// 表示一个机器人应该实现的接口。
    /// </summary>
    public interface IRobot {

        /// <summary>
        /// 获取当前机器人的名字。
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// 获取当前机器人的作者名字。
        /// </summary>
        string Author {
            get;
        }

        /// <summary>
        /// 获取当前机器人的心情。
        /// </summary>
        string Mood {
            get;
        }

        /// <summary>
        /// 向机器人说话，返回机器人的回复。
        /// </summary>
        /// <param name="message">向机器人说话的内容</param>
        /// <param name="options">存储了说话时的其它附属信息。
        /// <list type="ul">
        /// <item>speaker: 问话的人的名字。</item>
        /// <item>speakerID: 问话的人的 ID。</item>
        /// <item>group: 所在的群。</item>
        /// <item>groupID: 所在的群的 ID。</item>
        /// <item>time: 提问的时间。</item>
        /// </list>
        /// </param>
        /// <returns>返回回答内容。返回 null 表示无法回复。</returns>
        string Answer(string message, System.Collections.Specialized.NameValueCollection options);

    }
}
