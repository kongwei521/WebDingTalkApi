using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    /// <summary>  
    /// 简易缓存  
    /// </summary>  
    public class SimpleCacheProvider : ICacheProvider
    {
        private static SimpleCacheProvider _instance = null;
        private static readonly object _lockObj = new object();
        #region GetInstance  
        /// <summary>  
        /// 获取缓存实例  
        /// </summary>  
        /// <returns></returns>  
        public static SimpleCacheProvider GetInstance()
        {
            if (_instance == null) lock (_lockObj) { _instance = _instance ?? new SimpleCacheProvider(); };//使用单例模式以确保并发时实例始终是同一个  
            return _instance;
        }
        #endregion

        private Dictionary<string, CacheItem> _caches;

        private SimpleCacheProvider()
        {
            this._caches = new Dictionary<string, CacheItem>();
        }

        #region GetCache  
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public object GetCache(string key)
        {
            object obj = this._caches.ContainsKey(key) ? this._caches[key].Expired() ? null : this._caches[key].Value : null;
            return obj;
        }

        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public T GetCache<T>(String key)
        {
            object obj = GetCache(key);
            if (obj == null)
            {
                return default(T);
            }
            T result = (T)obj;
            return result;
        }
        #endregion

        #region SetCache  
        /// <summary>  
        /// 设置缓存  
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="value"></param>  
        /// <param name="expire"></param>  
        public void SetCache(string key, object value, int expire = 300)
        {
            this._caches[key] = new CacheItem(key, value, expire);
        }
        #endregion
    }
    /// <summary>  
    /// 缓存接口  
    /// </summary>  
    interface ICacheProvider
    {
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <param name="key">缓存key</param>  
        /// <returns>缓存对象或null,不存在或者过期返回null</returns>  
        object GetCache(string key);

        /// <summary>  
        /// 写入缓存  
        /// </summary>  
        /// <param name="key">缓存key</param>  
        /// <param name="value">缓存值</param>  
        /// <param name="expire">缓存有效期，单位为秒，默认300</param>  
        void SetCache(string key, object value, int expire = 300);
    }

    /// <summary>  
    /// 缓存项  
    /// </summary>  
    public class CacheItem
    {

        #region 属性  
        private object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        #endregion

        #region 内部变量  
        /// <summary>  
        /// 插入时间  
        /// </summary>  
        private DateTime _insertTime;
        /// <summary>  
        /// 过期时间  
        /// </summary>  
        private int _expire;
        #endregion

        #region 构造函数  
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="key">缓存的KEY</param>  
        /// <param name="value">缓存的VALUE</param>  
        /// <param name="expire">缓存的过期时间</param>  
        public CacheItem(string key, object value, int expire)
        {
            this._key = key;
            this._value = value;
            this._expire = expire;
            this._insertTime = DateTime.Now;
        }
        #endregion

        #region Expired  
        /// <summary>  
        /// 是否过期  
        /// </summary>  
        /// <returns></returns>  
        public bool Expired()
        {
            return DateTime.Now <this._insertTime.AddSeconds(_expire);
        }
        #endregion
    }
}