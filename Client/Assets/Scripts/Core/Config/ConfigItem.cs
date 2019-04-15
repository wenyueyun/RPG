
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
    public class ConfigItem : ConfigBase, ISerializable
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
		///品质
		/// </summary>
		public uint quality;
		/// <summary>
		///图标
		/// </summary>
		public uint icon;
		/// <summary>
		///描述
		/// </summary>
		public string desc;


        public ConfigItem()
        {
        }

        protected ConfigItem(SerializationInfo info, StreamingContext context)
        {
            //read
			bid = info.GetUInt32("bid");
			name = info.GetString("name");
			quality = info.GetUInt32("quality");
			icon = info.GetUInt32("icon");
			desc = info.GetString("desc");

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //write
			info.AddValue("bid", bid);
			info.AddValue("name", name);
			info.AddValue("quality", quality);
			info.AddValue("icon", icon);
			info.AddValue("desc", desc);
  
        }
    }
}