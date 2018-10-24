namespace Geone.Utiliy.Redis
{
    public interface IQueueAction : IRedisAction
    {
        long? Publish(string channel, string value);

        void Start(params string[] channels);

        void Subscribe(DoMessage @do, params string[] channels);

        void UnSubscribe(params string[] channels);
    }
}