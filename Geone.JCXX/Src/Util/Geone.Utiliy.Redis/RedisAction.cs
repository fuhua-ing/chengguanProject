using Geone.Utiliy.Library;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;

namespace Geone.Utiliy.Redis
{
    /// <summary>
    /// Redis数据库模型
    /// </summary>
    public class RedisAction : IRedisAction, IDisposable
    {
        public string RedisName { get; set; }

        protected IRedisConnect conn;
        protected IRedisConnection redis;
        protected ILogWriter log;

        public RedisAction(IRedisConnect RedisConn, IRedisConnection Connection, ILogWriter Log)
        {
            conn = RedisConn;
            redis = Connection;
            log = Log;

            RedisName = "Default";
        }

        //设置连接名称/模型/字符串
        public IRedisAction SetName(string name)
        {
            RedisName = name;
            return this;
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="action">操作--利用反射连接数据库</param>
        /// <returns></returns>
        public dynamic ExeRedisAction(DoRedisConection action)
        {
            using (IRedisClient db = conn.OpenConn(RedisName))
            {
                try
                {
                    var res = action(db);
                    db.SaveAsync();
                    log.WriteNoSql(RedisName, "DoRedisConection", $"操作成功，返回值为{res}");
                    return res;
                }
                catch (Exception ex)
                {
                    log.WriteNoSqlException(RedisName, "DoRedisConection", $"操作出错", ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="action">操作--利用反射连接数据库与事务</param>
        /// <returns></returns>
        public bool ExeRedisAction(DoRedisTransaction action)
        {
            using (IRedisClient db = conn.OpenConn(RedisName))
            {
                using (IRedisTransaction trans = db.CreateTransaction())
                {
                    try
                    {
                        action(db, trans);
                        trans.Commit();
                        db.SaveAsync();
                        log.WriteNoSql(RedisName, "DoRedisTransaction", $"操作成功");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        log.WriteNoSqlException(RedisName, "DoRedisTransaction", $"操作出错", ex);
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 订阅操作
        /// </summary>
        /// <param name="action">操作--利用反射连接数据库</param>
        /// <returns></returns>
        public void ExeRedisAction(DoRedisSubscription action)
        {
            using (IRedisClient db = conn.OpenConn(RedisName))
            {
                using (IRedisSubscription sub = db.CreateSubscription())
                {
                    try
                    {
                        action(sub);
                    }
                    catch (Exception ex)
                    {
                        log.WriteNoSqlException(RedisName, "DoRedisSubscription", $"操作出错", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        public void Save()
        {
            using (IRedisClient db = conn.OpenConn(RedisName))
            {
                db.Save();
            }
        }

        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            using (IRedisClient db = conn.OpenConn(RedisName))
            {
                db.SaveAsync();
            }
        }

        #region 通常

        /// <summary>
        /// 获取key的value值
        /// </summary>
        public string Get(string key)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetValue(key);
            });
        }

        /// <summary>
        /// 获取多个key的value值
        /// </summary>
        public string Get(List<string> keys)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetValues(keys);
            });
        }

        /// <summary>
        /// 设置key的value
        /// </summary>
        public bool? Set(string key, string value)
        {
            return ExeRedisAction((db) =>
            {
                return db.Set(key, value);
            });
        }

        /// <summary>
        /// 设置key的value并设置过期时间
        /// </summary>
        public bool? Set(string key, string value, DateTime dt)
        {
            return ExeRedisAction((db) =>
            {
                return db.Set(key, value, dt);
            });
        }

        /// <summary>
        /// 设置key的value并设置过期时间
        /// </summary>
        public bool? Set(string key, string value, TimeSpan sp)
        {
            return ExeRedisAction((db) =>
            {
                return db.Set(key, value, sp);
            });
        }

        /// <summary>
        /// 设置多个key/value
        /// </summary>
        public bool? Set(Dictionary<string, string> map)
        {
            return ExeRedisAction((db) =>
            {
                db.SetValues(map);
                return true;
            });
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public bool? ModifyExpire(string key, DateTime dt)
        {
            return ExeRedisAction((db) =>
            {
                return db.ExpireEntryAt(key, dt);
            });
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public bool? ModifyExpire(string key, TimeSpan tp)
        {
            return ExeRedisAction((db) =>
            {
                return db.ExpireEntryIn(key, tp);
            });
        }

        /// <summary>
        /// key重命名
        /// </summary>
        public bool? ModifyRename(string oldkey, string newkey)
        {
            return ExeRedisAction((db) =>
            {
                db.RenameKey(oldkey, newkey);
                return true;
            });
        }

        /// <summary>
        /// 移除整个key
        /// </summary>
        public bool? Remove(string key)
        {
            return ExeRedisAction((db) =>
            {
                return db.Remove(key);
            });
        }

        /// <summary>
        /// 判断是否存在key
        /// </summary>
        public bool? Contains(string key)
        {
            return ExeRedisAction((db) =>
            {
                return db.ContainsKey(key);
            });
        }

        /// <summary>
        /// 清空数据库
        /// </summary>
        public bool? FlushDb()
        {
            return ExeRedisAction((db) =>
            {
                db.FlushDb();
                return true;
            });
        }

        /// <summary>
        /// 清空数据库
        /// </summary>
        public bool? FlushAll()
        {
            return ExeRedisAction((db) =>
            {
                db.FlushAll();
                return true;
            });
        }

        #endregion 通常

        #region 哈希

        /// <summary>
        /// 获取所有hashid数据集的key/value数据集合
        /// </summary>
        public Dictionary<string, string> GetFromHash(string hashid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetAllEntriesFromHash(hashid);
            });
        }

        /// <summary>
        /// 获取hashid数据集中的数据总数
        /// </summary>
        public long? GetCountFromHash(string hashid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetHashCount(hashid);
            });
        }

        /// <summary>
        /// 获取hashid数据集中所有key的集合
        /// </summary>
        public List<string> GetKeysFromHash(string hashid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetHashKeys(hashid);
            });
        }

        /// <summary>
        /// 获取hashid数据集中的所有value集合
        /// </summary>
        public List<string> GetValuesFromHash(string hashid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetHashValues(hashid);
            });
        }

        /// <summary>
        /// 获取hashid数据集中，key的value数据
        /// </summary>
        public string GetValueFromHash(string hashid, string key)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetValueFromHash(hashid, key);
            });
        }

        /// <summary>
        /// 获取hashid数据集中，多个keys的value集合
        /// </summary>
        public List<string> GetValuesFromHash(string hashid, string[] keys)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetValuesFromHash(hashid, keys);
            });
        }

        /// <summary>
        /// 向hashid集合中添加key/value
        /// </summary>
        public bool? SetValueInHash(string hashid, string key, string value)
        {
            return ExeRedisAction((db) =>
            {
                return db.SetEntryInHash(hashid, key, value);
            });
        }

        /// <summary>
        /// 删除hashid数据集中的key数据
        /// </summary>
        public bool? RemoveValueFromHash(string hashid, string key)
        {
            return ExeRedisAction((db) =>
            {
                return db.RemoveEntryFromHash(hashid, key);
            });
        }

        /// <summary>
        /// 判断hashid数据集中是否存在key的数据
        /// </summary>
        public bool? ContainsInHash(string hashid, string key)
        {
            return ExeRedisAction((db) =>
            {
                return db.HashContainsEntry(hashid, key);
            });
        }

        #endregion 哈希

        #region 通常集合-List<T>

        /// <summary>
        /// 获取key中下标为star的值
        /// </summary>
        public string GetFromList(string listid, int star)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetItemFromList(listid, star);
            });
        }

        /// <summary>
        /// 获取key中下标为star到end的值集合
        /// </summary>
        public List<string> GetFromList(string listid, int star, int end)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetRangeFromList(listid, star, end);
            });
        }

        /// <summary>
        /// 获取key包含的所有数据集合
        /// </summary>
        public List<string> GetFromList(string listid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetAllItemsFromList(listid);
            });
        }

        /// <summary>
        /// 获取list中key包含的数据数量
        /// </summary>
        public long? GetCountFromList(string listid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetListCount(listid);
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>
        public bool? AddToList(string listid, string value)
        {
            return ExeRedisAction((db) =>
            {
                db.AddItemToList(listid, value);
                return true;
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>
        public bool? AddToList(string listid, List<string> values)
        {
            return ExeRedisAction((db) =>
            {
                db.AddRangeToList(listid, values);
                return true;
            });
        }

        /// <summary>
        /// 删除
        /// value相同则删除，返回删除量
        /// </summary>
        public long? RemoveFromList(string listid, string value)
        {
            return ExeRedisAction((db) =>
            {
                return db.RemoveItemFromList(listid, value);
            });
        }

        /// <summary>
        /// 删除List
        /// </summary>
        public bool? RemoveAllFromList(string listid)
        {
            return ExeRedisAction((db) =>
            {
                db.RemoveAllFromList(listid);
                return true;
            });
        }

        #endregion 通常集合-List<T>

        #region 唯一集合-HashSet<T>

        /// <summary>
        /// 获取所有key集合的值
        /// </summary>
        public HashSet<string> GetFromSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetAllItemsFromSet(setid);
            });
        }

        /// <summary>
        /// 随机获取key集合中的一个值
        /// </summary>
        public string GetRandomFromSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetRandomItemFromSet(setid);
            });
        }

        /// <summary>
        /// 返回keys多个集合中的并集，返还hashset
        /// </summary>
        public HashSet<string> GetFromSets(string[] setids)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetUnionFromSets(setids);
            });
        }

        /// <summary>
        /// 获取key集合值的数量
        /// </summary>
        public long? GetCountFromSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetSetCount(setid);
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>
        public bool? AddToSet(string setid, string item)
        {
            return ExeRedisAction((db) =>
            {
                db.AddItemToSet(setid, item);
                return true;
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>
        public bool? AddToSet(string setid, List<string> values)
        {
            return ExeRedisAction((db) =>
            {
                db.AddRangeToSet(setid, values);
                return true;
            });
        }

        /// <summary>
        /// 删除key集合中的value
        /// </summary>
        public bool? RemoveFromSet(string setid, string item)
        {
            return ExeRedisAction((db) =>
            {
                db.RemoveItemFromSet(setid, item);
                return true;
            });
        }

        /// <summary>
        /// 随机删除key集合中的一个值
        /// </summary>
        public string RemoveRandomFromSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.PopItemFromSet(setid);
            });
        }

        /// <summary>
        /// 从fromkey集合中移除值为value的值，并把value添加到tokey集合中
        /// </summary>
        public bool? MoveFromSetToSet(string fromsetid, string tosetid, string value)
        {
            return ExeRedisAction((db) =>
            {
                db.MoveBetweenSets(fromsetid, tosetid, value);
                return true;
            });
        }

        /// <summary>
        /// keys多个集合中的并集，放入newkey集合中
        /// </summary>
        public bool? UnionFromSets(string tosetid, string[] setids)
        {
            return ExeRedisAction((db) =>
            {
                db.StoreUnionFromSets(tosetid, setids);
                return true;
            });
        }

        /// <summary>
        /// 把fromkey集合中的数据与keys集合中的数据对比，fromkey集合中不存在keys集合中，则把这些不存在的数据放入newkey集合中
        /// </summary>
        public bool? DifferencesFromSets(string tosetid, string fromsetid, string[] setids)
        {
            return ExeRedisAction((db) =>
            {
                db.StoreDifferencesFromSet(tosetid, fromsetid, setids);
                return true;
            });
        }

        #endregion 唯一集合-HashSet<T>

        #region 排序集合-SortedSet<T>

        /// <summary>
        /// 获取key的所有集合
        /// </summary>
        public List<string> GetFromSortedSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetAllItemsFromSortedSet(setid);
            });
        }

        /// <summary>
        /// 获取key的所有集合，倒叙输出
        /// </summary>
        public List<string> GeFromSortedSetDesc(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetAllItemsFromSortedSetDesc(setid);
            });
        }

        /// <summary>
        /// 获取可以的所有集合，带分数
        /// </summary>
        public IDictionary<string, double> GetWithScoresFromSortedSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetAllWithScoresFromSortedSet(setid);
            });
        }

        /// <summary>
        /// 获取key集合从高分到低分排序数据，分数从fromscore到分数为toscore的数据
        /// </summary>
        public List<string> GetFromSortedSet(string setid, double fromscore, double toscore)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetRangeFromSortedSetByHighestScore(setid, fromscore, toscore);
            });
        }

        /// <summary>
        /// 获取key集合从低分到高分排序数据，分数从fromscore到分数为toscore的数据
        /// </summary>
        public List<string> GeFromSortedSetDesc(string setid, double fromscore, double toscore)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetRangeFromSortedSetByLowestScore(setid, fromscore, toscore);
            });
        }

        /// <summary>
        /// 获取key集合从高分到低分排序数据，分数从fromscore到分数为toscore的数据，带分数
        /// </summary>
        public IDictionary<string, double> GetWithScoresFromSortedSet(string setid, double fromscore, double toscore)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetRangeWithScoresFromSortedSetByHighestScore(setid, fromscore, toscore);
            });
        }

        /// <summary>
        ///  获取key集合从低分到高分排序数据，分数从fromscore到分数为toscore的数据，带分数
        /// </summary>
        public IDictionary<string, double> GetWithScoresFromSortedSetDesc(string setid, double fromscore, double toscore)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetRangeWithScoresFromSortedSetByLowestScore(setid, fromscore, toscore);
            });
        }

        /// <summary>
        /// 获取key所有集合的数据总数
        /// </summary>
        public long? GetCountFromSortedSet(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetSortedSetCount(setid);
            });
        }

        /// <summary>
        /// key集合数据从分数为fromscore到分数为toscore的数据总数
        /// </summary>
        public long? GetCountFromSortedSet(string setid, double fromScore, double toScore)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetSortedSetCount(setid, fromScore, toScore);
            });
        }

        /// <summary>
        /// 获取key为value的分数
        /// </summary>
        public double? GetScoreInSortedSet(string setid, string value)
        {
            return ExeRedisAction((db) =>
            {
                return db.GetItemScoreInSortedSet(setid, value);
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>
        public bool? AddToSortedSet(string setid, string value, double score)
        {
            return ExeRedisAction((db) =>
            {
                return db.AddItemToSortedSet(setid, value, score);
            });
        }

        /// <summary>
        /// 添加key/value
        /// </summary>
        public bool? AddToSortedSet(string setid, List<string> values, double score)
        {
            return ExeRedisAction((db) =>
            {
                db.AddRangeToSortedSet(setid, values, score);
                return true;
            });
        }

        /// <summary>
        /// 删除key为value的数据
        /// </summary>
        public bool? RemoveFromSortedSet(string setid, string value)
        {
            return ExeRedisAction((db) =>
            {
                return db.RemoveItemFromSortedSet(setid, value);
            });
        }

        /// <summary>
        /// 删除分数从fromscore到toscore的key集合数据
        /// </summary>
        public long? RemoveFromSortedSetByScore(string setid, double fromscore, double toscore)
        {
            return ExeRedisAction((db) =>
            {
                return db.RemoveRangeFromSortedSetByScore(setid, fromscore, toscore);
            });
        }

        /// <summary>
        /// 删除key集合中分数最大的数据
        /// </summary>
        public string RemoveFromSortedSetByMaxScore(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.PopItemWithHighestScoreFromSortedSet(setid);
            });
        }

        /// <summary>
        /// 删除key集合中分数最小的数据
        /// </summary>
        public string RemoveFromSortedSetByMinScore(string setid)
        {
            return ExeRedisAction((db) =>
            {
                return db.PopItemWithLowestScoreFromSortedSet(setid);
            });
        }

        /// <summary>
        /// 判断key集合中是否存在value数据
        /// </summary>
        public bool? ContainsInSortedSet(string setid, string item)
        {
            return ExeRedisAction((db) =>
            {
                return db.SortedSetContainsItem(setid, item);
            });
        }

        /// <summary>
        /// 获取keys多个集合的交集，并把交集添加的newkey集合中，返回交集数据的总数
        /// </summary>
        public long? IntersectFromSortedSets(string tosetid, string[] setids)
        {
            return ExeRedisAction((db) =>
            {
                return db.StoreIntersectFromSortedSets(tosetid, setids);
            });
        }

        /// <summary>
        /// 获取keys多个集合的并集，并把并集数据添加到newkey集合中，返回并集数据的总数
        /// </summary>
        public long? UnionFromSortedSets(string tosetid, string[] setids)
        {
            return ExeRedisAction((db) =>
            {
                return db.StoreUnionFromSortedSets(tosetid, setids);
            });
        }

        #endregion 排序集合-SortedSet<T>

        #region 队列

        /// <summary>
        /// 入队
        /// </summary>
        public bool? Enqueue(string listid, string value)
        {
            return ExeRedisAction((db) =>
            {
                db.EnqueueItemOnList(listid, value);
                return true;
            });
        }

        /// <summary>
        /// 出队
        /// </summary>
        public string Dequeue(string listid)
        {
            return ExeRedisAction((db) =>
            {
                return db.DequeueItemFromList(listid);
            });
        }

        /// <summary>
        /// 阻塞出队
        /// </summary>
        public string BlockingDequeue(string listid, TimeSpan? sp)
        {
            return ExeRedisAction((db) =>
            {
                return db.BlockingDequeueItemFromList(listid, sp);
            });
        }

        #endregion 队列

        #region 栈

        /// <summary>
        /// 入栈
        /// </summary>
        public bool? Push(string listid, string value)
        {
            return ExeRedisAction((db) =>
            {
                db.PushItemToList(listid, value);
                return true;
            });
        }

        /// <summary>
        /// 出栈
        /// </summary>
        public string Pop(string listid)
        {
            return ExeRedisAction((db) =>
            {
                return db.PopItemFromList(listid);
            });
        }

        /// <summary>
        /// 阻塞出栈
        /// </summary>
        public string BlockingPop(string listid, TimeSpan? sp)
        {
            return ExeRedisAction((db) =>
            {
                return db.BlockingPopItemFromList(listid, sp);
            });
        }

        /// <summary>
        /// 出栈并入栈
        /// </summary>
        public string PopAndPush(string fromlistid, string tolistid)
        {
            return ExeRedisAction((db) =>
            {
                return db.PopAndPushItemBetweenLists(fromlistid, tolistid);
            });
        }

        /// <summary>
        /// 阻塞出栈并入栈
        /// </summary>
        public string BlockingPopAndPush(string fromlistid, string tolistid, TimeSpan? sp)
        {
            return ExeRedisAction((db) =>
            {
                return db.BlockingPopAndPushItemBetweenLists(fromlistid, tolistid, sp);
            });
        }

        #endregion 栈

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DBActionHelper() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}