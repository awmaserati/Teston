using System;

namespace Teston.Net
{
    [Serializable]
    public class EventData
    {
        public string Type;
        public string Data;

        public EventData() { }

        public EventData(string type, string data) 
        { 
            Type = type;
            Data = data;
        }
    }
}