using System;

namespace Agency.Contracts
{
    [Serializable]
    public class EventContract
    {
        public string Description => $"[{Event}] {MethodName}";
        public bool IsAdd { get; set; }
        public string Event { get; set; }
        public string MethodName { get; set; }

        public EventContract(string @event, string method, bool isAdd = true)
        {
            Event = @event;
            MethodName = method;
            IsAdd = isAdd;
        }

        public override string ToString()
        {
            return $"{Agency.AgencyEventToken}{Event} {(IsAdd?"+=":"-=")} {MethodName}";
        }
    }
}
