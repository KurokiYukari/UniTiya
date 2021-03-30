using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya.Consumer
{
    /// <summary>
    /// 复合 Consumer。
    /// 要求所有 Consumer 的 CanConsume 都为 true 才执行所有 Consumer 的 Consume()。
    /// </summary>
    [System.Serializable]
    public class MultiConsumer : IConsumer, IEnumerable<IConsumer>
    {
        [SerializeField] [TypeRestriction(typeof(IConsumer))] Object[] _consumerObjects;

        List<IConsumer> _consumerList;
        public List<IConsumer> ConsumerList
        {
            get
            {
                if (_consumerList == null)
                {
                    _consumerList = new List<IConsumer>();
                    _consumerList.AddRange(
                        from obj in _consumerObjects
                        where obj != null
                        select obj.ConvertTo<IConsumer>());
                }

                return _consumerList;
            }
        }

        public bool CanConsume => !ConsumerList.Any(consumer => !consumer.CanConsume);

        public bool Consume()
        {
            if (CanConsume)
            {
                foreach (var consumer in ConsumerList)
                {
                    consumer.Consume();
                }

                return true;
            }

            return false;
        }

        public IEnumerator<IConsumer> GetEnumerator() => ConsumerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
