using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkNET.Models
{
    public static class Permissions
    {

        public const string NOTIFY = "notify";
        public const string FRIENDS = "friends";
        public const string PHOTOS = "photos";
        public const string AUDIO = "audio";
        public const string VIDEO = "video";
        public const string DOCS = "docs";
        public const string NOTES = "notes";
        public const string PAGES = "pages";
        public const string STATUS = "status";
        public const string QUESTIONS = "questions";
        public const string WALL = "wall";
        public const string GROUPS = "groups";
        public const string MESSAGES = "messages";
        public const string EMAIL = "email";
        public const string NOTIFICATIONS = "notifications";
        public const string STATS = "stats";
        public const string ADS = "ads";
        public const string OFFLINE = "offline";
        public const string NOHTTPS = "nohttps";

        public static string Merge(params string[] permissions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in permissions)
            {
                sb.Append(permissions);
                sb.Append(",");
            }
            return sb.ToString();
        }
    }
}
