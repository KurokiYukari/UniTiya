using System.Linq;
using UnityEngine;

namespace Sarachan.UniTiya.Consumer
{
    /// <summary>
    /// 一切 Cost 的接口
    /// </summary>
    public interface IConsumer
    {
        bool CanConsume { get; }
        bool Consume();
    }

    /// <summary>
    /// IConsumer 的 ScriptableObject 基引用
    /// </summary>
    public abstract class ScriptableConsumerRef : ScriptableObject, IConsumer
    {
        protected abstract IConsumer Consumer { get; }

        public bool CanConsume => Consumer.CanConsume;

        public bool Consume() => Consumer.Consume();
    }

    /// <summary>
    /// IConsumer 的 MonoBehaviuor 基引用
    /// </summary>
    public abstract class BehaviourConsumerRef : MonoBehaviour, IConsumer
    {
        protected abstract IConsumer Consumer { get; }

        public bool CanConsume => Consumer.CanConsume;

        public bool Consume() => Consumer.Consume();
    }
}
