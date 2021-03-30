using System;
using System.Collections.Generic;

namespace Sarachan.UniTiya.Commands
{
    /// <summary>
    /// 命令接口
    /// </summary>
    /// <typeparam name="T">执行命令的主体</typeparam>
    public interface ICommand<in T>
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="subject"></param>
        void Execute(T subject);
    }
}
