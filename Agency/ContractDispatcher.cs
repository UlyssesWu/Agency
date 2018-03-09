using System;

namespace Agency
{
    public delegate object PersonalContract(string name, object[] objs);
    public class ContractDispatcher : MarshalByRefObject
    {
        public event PersonalContract Contract;
        public object AssignContract(string name, object[] objs)
        {
            return Contract?.Invoke(name, objs);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ContractDispatcher()
        { }
    }
}
