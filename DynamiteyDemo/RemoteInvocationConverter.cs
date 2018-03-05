using System;
using System.Collections.Generic;
using Agency;
using Dynamitey;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynamiteyDemo
{
    class RemoteInvocationConverter : JsonConverter<RemoteInvocation>
    {
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, RemoteInvocation value, JsonSerializer serializer)
        {
            return; //Won't be called
        }

        public override RemoteInvocation ReadJson(JsonReader reader, Type objectType, RemoteInvocation existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject obj = (JObject)serializer.Deserialize(reader);
            List<object> args = new List<object>();
            foreach (var jToken in obj["Args"].Values())
            {
                switch (jToken.Type)
                {
                    case JTokenType.Boolean:
                        args.Add(jToken.Value<bool>());
                        break;
                    case JTokenType.Integer:
                        args.Add(jToken.Value<int>());
                        break;
                    case JTokenType.Float:
                        args.Add(jToken.Value<float>());
                        break;
                    case JTokenType.String:
                        args.Add(jToken.Value<string>());
                        break;
                    case JTokenType.Date:
                        args.Add(jToken.Value<DateTime>());
                        break;
                }
            }

            return new RemoteInvocation((InvocationKind)obj["Kind"].Value<int>(), (obj["Name"]).Value<string>(), args.ToArray());
        }
    }


}
