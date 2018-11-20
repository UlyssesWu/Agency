using System;

namespace Agency
{
    /// <summary>
    /// <para>How Contract is passed</para>
    /// </summary>
    public interface IHandler : IDisposable
    {
        void Host(string address, object obj);
        Agent Connect(string address);
    }
}
