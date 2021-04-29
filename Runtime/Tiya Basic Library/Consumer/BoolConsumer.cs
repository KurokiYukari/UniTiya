namespace Sarachan.UniTiya.Consumer
{
    public class BoolConsumer : IConsumer
    {
        public bool CanConsume { get; set; } = true;

        public bool Consume()
        {
            if (CanConsume)
            {
                CanConsume = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
