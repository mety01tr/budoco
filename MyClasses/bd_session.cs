using System;
using System.Collections.Generic;

namespace budoco
{
    public static class bd_session
    {
        /*
        
            Because we can't store objects in HttpContext.Session, which is
            just memory, I said, what the heck, let me create a simple 
            place to store things that persist across requests.

            So far (nov 2020), not using it though.

        */

        static Dictionary<string, dynamic> cache = new Dictionary<string, dynamic>();
        static object my_lock = new object();

        public static void Set(string key, dynamic obj)
        {
            lock (my_lock)
            {
                cache[key] = obj;
            }
        }

        public static dynamic Get(string key)
        {
            lock (my_lock)
            {
                if (cache.ContainsKey(key))
                    return cache[key];
                else
                    return null;
            }
        }
    }
}
