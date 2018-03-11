using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Agency;
using Newtonsoft.Json;

namespace DynamiteyDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            TestUnity();
        }

        static void TestUnity()
        {
            Console.WriteLine("Input your ID:");
            var id = Console.ReadLine();
            id = id?.Replace('\r', ' ').Replace('\n', ' ').Trim() ?? "Anonymous Agent";

            dynamic agent = Agency.Agency.SpawnAgent("ICA-Meeting", new TcpHandler());

            Console.WriteLine("Connected to ICA Chatroom.");
            agent.Join(id);
            Action<string, string> Chat = (name, content) => Console.WriteLine($"{name}: {content}");

            agent.OnChatServer += Chat;

            while (true)
            {
                var content = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(content))
                {
                    continue;
                }

                if (content == "^C" || content == "\\quit" || content == "\\exit")
                {
                    agent.Leave(id);
                    break;
                }

                agent.Chat(id, content);
            }

            agent.OnChatServer -= Chat;
            Console.WriteLine("Disconnected.");
            Console.ReadLine();
        }

        static void TestJson()
        {
            RemoteRecorder r = new RemoteRecorder(new Agent47());
            RemoteRecorder rSwampMan = new RemoteRecorder();
            dynamic dr = r;
            dr.Weapon = "Kazo TRG";
            dr.PointShooting(4);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters = new List<JsonConverter>() { new RemoteInvocationConverter() };
            var j = JsonConvert.SerializeObject(r.Recording, settings);
            var l = JsonConvert.DeserializeObject<List<RemoteInvocation>>(j, settings);
            rSwampMan.Recording = l;
            var s = new Travis();
            rSwampMan.ReplayOn(s);
            var w = s.Weapon;
            var n = s.Name;
        }

        static void TestEvent()
        {
            Func<int, float, string> func = (i, f) =>
            {
                Console.WriteLine("Good job.");
                return $"Account: ${i * f}";
            };

            Func<int, float, string> func2 = (i, f) =>
            {
                Console.WriteLine("Good job.");
                return $"Account: ${i * f}";
            };
            bool b = func == func2;

        }

        static void TestBinary()
        {
            RemoteRecorder r = new RemoteRecorder(new Agent47());
            RemoteRecorder rSwampMan = new RemoteRecorder();
            dynamic dr = r;
            dr.Weapon = "Kazo TRG";
            dr.PointShooting(4);
            using (var ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, r.Recording);
                ms.Seek(0, SeekOrigin.Begin);
                rSwampMan.Recording = bf.Deserialize(ms) as List<RemoteInvocation>;
            }
            var s = new Travis();
            rSwampMan.ReplayOn(s);
            var w = s.Weapon;
            var n = s.Name;
        }
    }
}
