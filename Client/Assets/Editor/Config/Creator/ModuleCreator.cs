
using Game.Core.Config;
using System.Collections.Generic;
using System.Data;
/// <summary>
/// 自动生成，禁止修改
/// </summary>
public class ModuleCreator :BaseCreator, ICreator<ConfigModule>
{
    public ModuleCreator()
    {
    }

    public List<ConfigModule> GetData()
    {
        List<ConfigModule> list = new List<ConfigModule>();
        for (int i = 0; i < excelData.Count; i++)
        {
            ConfigModule c = new ConfigModule();
            
			c.bid = GetUInt32("bid", i);
			c.name = GetString("name", i);
			c.module = GetString("module", i);
			c.level = GetUInt32("level", i);

            list.Add(c);
        }
        return list;
    }

    public override void ReadData(DataTable table)
    {
        base.ReadData(table);
    }
}
