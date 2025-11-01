using System;
using System.Collections.Generic;

namespace QuestNav.Config
{
    [Serializable]
    public class ConfigData
    {
        public Dictionary<string, object> values = new Dictionary<string, object>();
        public string version = "1.0";
        public long lastModified;
    }

    [Serializable]
    public class ConfigFieldSchema
    {
        public string path;
        public string displayName;
        public string description;
        public string category;
        public string type;
        public string controlType;
        public object min;
        public object max;
        public object step;
        public object defaultValue;
        public object currentValue;
        public bool requiresRestart;
        public int order;
        public string[] options;
    }

    [Serializable]
    public class ConfigSchema
    {
        public List<ConfigFieldSchema> fields = new List<ConfigFieldSchema>();
        public Dictionary<string, List<ConfigFieldSchema>> categories =
            new Dictionary<string, List<ConfigFieldSchema>>();
        public string version = "1.0";
    }

    [Serializable]
    public class ConfigUpdateRequest
    {
        public string path;
        public object value;
    }

    [Serializable]
    public class ConfigUpdateResponse
    {
        public bool success;
        public string message;
        public object oldValue;
        public object newValue;
    }

    [Serializable]
    public class AuthToken
    {
        public string token;
        public long createdAt;
        public string deviceId;
    }
}
