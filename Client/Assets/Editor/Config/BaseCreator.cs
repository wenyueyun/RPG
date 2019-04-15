using Game.Core.Config;
using System.Collections.Generic;
using System.Data;

public abstract class BaseCreator
{
    protected DataTable table;
    protected List<string> key_list;
    protected List<string> type_list;
    protected List<List<ExcelData>> excelData;
    public BaseCreator()
    {
    }


    //读取整个配置表数据
    public virtual void ReadData(DataTable table)
    {
        this.table = table;
        key_list = new List<string>();
        type_list = new List<string>();
        excelData = new List<List<ExcelData>>();

        for (int i = 0; i < table.Rows.Count; i++)
        {
            List<ExcelData> row_list = new List<ExcelData>();

            object[] items = table.Rows[i].ItemArray;

            for (int j = 0; j < items.Length; j++)
            {
                if (i == 0)
                {
                    key_list.Add(items[j].ToString());
                }
                else if (i == 1)
                {
                    type_list.Add(items[j].ToString());
                }
                else if (i == 2)
                {
                    break;
                }
                else
                {
                    ExcelData data = new ExcelData();
                    data.type = type_list[j];
                    data.key = key_list[j];
                    data.value = items[j].ToString();
                    row_list.Add(data);
                }
            }
            if (row_list.Count > 0)
            {
                excelData.Add(row_list);
            }
        }
    }


    //根据Key和index获取对应的数据
    private ExcelData GetExcelData(string key, int index)
    {
        if (excelData.Count > index)
        {
            return excelData[index].Find(vo => vo.key == key);
        }
        return null;
    }

    //byte
    protected byte GetByte(string key, int index)
    {
        byte outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            byte.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //short
    protected short GetInt16(string key, int index)
    {
        short outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            short.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //ushort
    protected ushort GetUInt16(string key, int index)
    {
        ushort outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            ushort.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //int
    protected int GetInt32(string key, int index)
    {
        int outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            int.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //uint
    protected uint GetUInt32(string key, int index)
    {
        uint outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            uint.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //long
    protected long GetInt64(string key, int index)
    {
        long outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            long.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //ulong
    protected ulong GetUInt64(string key, int index)
    {
        ulong outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            ulong.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //float
    protected float GetSingle(string key, int index)
    {
        float outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            float.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //double
    protected double GetDouble(string key, int index)
    {
        double outValue = 0;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            double.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //bool
    protected bool GetBoolean(string key, int index)
    {
        bool outValue = false;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            bool.TryParse(data.value, out outValue);
        }
        return outValue;
    }

    //string
    protected string GetString(string key, int index)
    {
        string outValue = string.Empty;
        ExcelData data = GetExcelData(key, index);
        if (data != null)
        {
            outValue = data.value;
        }
        return outValue;
    }
}