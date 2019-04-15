
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
/// <summary>
/// 自动生成，禁止修改
/// </summary>

namespace Game.Core.Config
{
    [Serializable]
    public class ConfigModule : ConfigBase, ISerializable
    {
        
		/// <summary>
		///唯一标识
		/// </summary>
		public uint bid;
		/// <summary>
		///名字
		/// </summary>
		public string name;
		/// <summary>
		///模块
		/// </summary>
		public string module;
		/// <summary>
		///等级
		/// </summary>
		public uint level;


        public ConfigModule()
        {
        }

        protected ConfigModule(SerializationInfo info, StreamingContext context)
        {
            //read
			bid = info.GetUInt32("bid");
			name = info.GetString("name");
			module = info.GetString("module");
			level = info.GetUInt32("level");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //write
			info.AddValue("bid", bid);
			info.AddValue("name", name);
			info.AddValue("module", module);
			info.AddValue("level", level);
  
        }
    }
}