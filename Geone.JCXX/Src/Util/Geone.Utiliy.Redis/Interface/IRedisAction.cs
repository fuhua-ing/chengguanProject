using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace Geone.Utiliy.Redis
{
    public delegate dynamic DoRedisConection(IRedisClient redisConnection);

    public delegate void DoRedisTransaction(IRedisClient redisConnection, IRedisTransaction redisTransaction);

    public delegate void DoRedisSubscription(IRedisSubscription redisSubscription);

    public delegate void DoMessage(string channel, string value);

    public interface IRedisAction
    {
        string RedisName { get; set; }

        IRedisAction SetName(string name);

        dynamic ExeRedisAction(DoRedisConection action);

        bool ExeRedisAction(DoRedisTransaction action);

        void ExeRedisAction(DoRedisSubscription action);

        void Save();

        void SaveAsync();

        #region 通常

        string Get(string key);

        string Get(List<string> keys);

        bool? Set(string key, string value);

        bool? Set(string key, string value, DateTime dt);

        bool? Set(string key, string value, TimeSpan sp);

        bool? Set(Dictionary<string, string> map);

        bool? ModifyExpire(string key, DateTime dt);

        bool? ModifyExpire(string key, TimeSpan tp);

        bool? ModifyRename(string oldkey, string newkey);

        bool? Remove(string key);

        bool? Contains(string key);

        bool? FlushDb();

        bool? FlushAll();

        #endregion 通常

        #region 哈希

        Dictionary<string, string> GetFromHash(string hashid);

        long? GetCountFromHash(string hashid);

        List<string> GetKeysFromHash(string hashid);

        List<string> GetValuesFromHash(string hashid);

        string GetValueFromHash(string hashid, string key);

        List<string> GetValuesFromHash(string hashid, string[] keys);

        bool? SetValueInHash(string hashid, string key, string value);

        bool? RemoveValueFromHash(string hashid, string key);

        bool? ContainsInHash(string hashid, string key);

        #endregion 哈希

        #region 通常集合-List<T>

        string GetFromList(string listid, int star);

        List<string> GetFromList(string listid, int star, int end);

        List<string> GetFromList(string listid);

        long? GetCountFromList(string listid);

        bool? AddToList(string listid, string value);

        bool? AddToList(string listid, List<string> values);

        long? RemoveFromList(string listid, string value);

        bool? RemoveAllFromList(string listid);

        #endregion 通常集合-List<T>

        #region 唯一集合-HashSet<T>

        HashSet<string> GetFromSet(string setid);

        string GetRandomFromSet(string setid);

        HashSet<string> GetFromSets(string[] setids);

        long? GetCountFromSet(string setid);

        bool? AddToSet(string setid, string item);

        bool? AddToSet(string setid, List<string> values);

        bool? RemoveFromSet(string setid, string item);

        string RemoveRandomFromSet(string setid);

        bool? MoveFromSetToSet(string fromsetid, string tosetid, string value);

        bool? UnionFromSets(string tosetid, string[] setids);

        bool? DifferencesFromSets(string tosetid, string fromsetid, string[] setids);

        #endregion 唯一集合-HashSet<T>

        #region 排序集合-SortedSet<T>

        List<string> GetFromSortedSet(string setid);

        List<string> GeFromSortedSetDesc(string setid);

        IDictionary<string, double> GetWithScoresFromSortedSet(string setid);

        List<string> GetFromSortedSet(string setid, double fromscore, double toscore);

        List<string> GeFromSortedSetDesc(string setid, double fromscore, double toscore);

        IDictionary<string, double> GetWithScoresFromSortedSet(string setid, double fromscore, double toscore);

        IDictionary<string, double> GetWithScoresFromSortedSetDesc(string setid, double fromscore, double toscore);

        long? GetCountFromSortedSet(string setid);

        long? GetCountFromSortedSet(string setid, double fromScore, double toScore);

        double? GetScoreInSortedSet(string setid, string value);

        bool? AddToSortedSet(string setid, string value, double score);

        bool? AddToSortedSet(string setid, List<string> values, double score);

        bool? RemoveFromSortedSet(string setid, string value);

        long? RemoveFromSortedSetByScore(string setid, double fromscore, double toscore);

        string RemoveFromSortedSetByMaxScore(string setid);

        string RemoveFromSortedSetByMinScore(string setid);

        bool? ContainsInSortedSet(string setid, string item);

        long? IntersectFromSortedSets(string tosetid, string[] setids);

        long? UnionFromSortedSets(string tosetid, string[] setids);

        #endregion 排序集合-SortedSet<T>

        #region 队列

        bool? Enqueue(string listid, string value);

        string Dequeue(string listid);

        string BlockingDequeue(string listid, TimeSpan? sp);

        #endregion 队列

        #region 栈

        bool? Push(string listid, string value);

        string Pop(string listid);

        string BlockingPop(string listid, TimeSpan? sp);

        string PopAndPush(string fromlistid, string tolistid);

        string BlockingPopAndPush(string fromlistid, string tolistid, TimeSpan? sp);

        #endregion 栈
    }
}