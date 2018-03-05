using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Agency
{
    class BinaryContract : IContract<byte[], object>
    {
        BinaryFormatter _formatter = new BinaryFormatter();

        public byte[] Serialize(object target)
        {
            using (var ms = new MemoryStream())
            {
                _formatter.Serialize(ms, target);
                return ms.ToArray();
            }
        }

        public object Deserialize(byte[] target)
        {
            using (var ms = new MemoryStream(target))
            {
                return _formatter.Deserialize(ms);
            }
        }
    }
}
