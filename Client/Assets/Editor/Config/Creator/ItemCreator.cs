
using Game.Core.Config;
using System.Collections.Generic;
using System.Data;
/// <summary>
/// 自动生成，禁止修改
/// </summary>
public class ItemCreator :BaseCreator, ICreator<ConfigItem>
{
    public ItemCreator()
    {
    }

    public List<ConfigItem> GetData()
    {
        List<ConfigItem> list = new List<ConfigItem>();
        for (int i = 0; i < excelData.Count; i++)
        {
            ConfigItem c = new ConfigItem();
            
			c.bid = GetUInt32("bid", i);
			c.name = GetString("name", i);
			c.quality = GetUInt32("quality", i);
			c.icon = GetUInt32("icon", i);
			c.desc = GetString("desc", i);

            list.Add(c);
        }
        return list;
    }

    public override void ReadData(DataTable table)
    {
        base.ReadData(table);
    }
}
