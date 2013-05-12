
using System;
using System.Web;
using System.Web.Caching;
using UptimeMonitoring.Controllers;

namespace UptimeMonitoring
{
    public class JobsConfig
    {
        public static void JobStart()
        {
            JobsConfig jc = new JobsConfig();
            jc.AddTask("5MinCheck", 300);
            jc.AddTask("10MinCheck", 600);
            jc.AddTask("15MinCheck", 600);
        }

        private static CacheItemRemovedCallback OnCacheRemove;

        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            if (k == "5MinCheck")
            {
                SiteCheck check = new SiteCheck();
                check.CheckSite(5);
            }
            if (k == "10MinCheck")
            {
                SiteCheck check = new SiteCheck();
                check.CheckSite(10);
            }
            if (k == "15MinCheck")
            {
                SiteCheck check = new SiteCheck();
                check.CheckSite(15);
            }
            AddTask(k, Convert.ToInt32(v));
        }
    }
}