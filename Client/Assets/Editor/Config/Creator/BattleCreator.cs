
using Game.Core.Config;
using System.Collections.Generic;
using System.Data;
/// <summary>
/// 自动生成，禁止修改
/// </summary>
public class BattleCreator :BaseCreator, ICreator<ConfigBattle>
{
    public BattleCreator()
    {
    }

    public List<ConfigBattle> GetData()
    {
        List<ConfigBattle> list = new List<ConfigBattle>();
        for (int i = 0; i < excelData.Count; i++)
        {
            ConfigBattle c = new ConfigBattle();
            
			c.bid = GetUInt32("bid", i);
			c.level = GetUInt32("level", i);
			c.name = GetString("name", i);
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
