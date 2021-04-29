namespace Sarachan.UniTiya.Commands
{
    /// <summary>
    /// 命令处理器接口
    /// </summary>
    /// <typeparam name="T">执行命令的主体</typeparam>
    public interface ICommandProcessor<out T>
    {
        /// <summary>
        /// 向命令处理器中添加命令
        /// </summary>
        /// <param name="cmd"></param>
        void AddCommand(ICommand<T> cmd);
    }
}
