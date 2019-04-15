
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
    public class ConfigBattle : ConfigBase, ISerializable
    {
        
		/// <summary>
		///唯一标识
		/// </summary>
		public uint bid;
		/// <summary>
		///等级
		/// </summary>
		public uint level;
		/// <summary>
		///名字
		/// </summary>
		public string name;
		/// <summary>
		///描述
		/// </summary>
		public string desc;


        public ConfigBattle()
        {
        }

        protected ConfigBattle(SerializationInfo info, StreamingContext context)
        {
            //read
			bid = info.GetUInt32("bid");
			level = info.GetUInt32("level");
			name = info.GetString("name");
			desc = info.GetString("desc");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //write
			info.AddValue("bid", bid);
			info.AddValue("level", level);
			info.AddValue("name", name);
			info.AddValue("desc", desc);
  
        }
    }
}