using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agency
{
    /// <summary>
    /// <para>How mission is accounted</para>
    /// </summary>
    /// <typeparam name="T">serialized content</typeparam>
    /// <typeparam name="K">object</typeparam>
    public interface IContract<T, K>
    {
        T Serialize(K target);
        K Deserialize(T target);
    }
}
