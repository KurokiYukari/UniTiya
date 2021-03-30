namespace Sarachan.UniTiya
{
    /// <summary>
    /// Item 的管理器。
    /// </summary>
    public interface IItemManager
    {
        /// <summary>
        /// 获取一个 Item 种类
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IItem this[int ID] { get; }

        /// <summary>
        /// 获取一个 Item 种类
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        IItem GetItemByID(int itemID);

        /// <summary>
        /// 创建 itemID 种类 item 的一个实例，并返回该 Item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        IItem CreateItemByID(int itemID);
    }
}
