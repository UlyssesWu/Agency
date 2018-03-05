﻿using System.Collections.Generic;
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
