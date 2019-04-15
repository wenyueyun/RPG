using Excel;
using Game.Core.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEditor;
using UnityEngine;

/************************* 
* 作者： wenyueyun 
* 时间： 2019/3/26 17:09:59 
* 描述： ExcelPack 
*************************/
public class ExcelPack
{

    [MenuItem("工具/配置表/构建配置表")]
    public static void testc()
    {
        ExcelPack pack = new ExcelPack();
        pack.PackAll();
    }

    private readonly string excelPath = PathUtil.GetCombinePath(Application.dataPath, "../../Config/");
    private readonly string dataPath = Path.Combine(Application.dataPath, "Res/Config/");
    private List<List<ExcelData>> excelData = new List<List<ExcelData>>();

    /// <summary>
    /// 打包所有配置文件
    /// </summary>
    public void PackAll()
    {
        Debug.Log("准备打包所有配置表");
        DirectoryInfo info = new DirectoryInfo(excelPath);
        FileInfo[] files = info.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
        Assembly a = Assembly.Load("Assembly-CSharp");
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];
            using (FileStream stream = File.Open(file.FullName, FileMode.Open, FileAccess.Read))
            {
                //获取配置表名字
                string name = file.Name.Substring(0, file.Name.Length - 5);
                //读取配置表数据
                IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                DataSet result = excelReader.AsDataSet();
                DataTableCollection tables = result.Tables;
                for (int j = 0; j < tables.Count; j++)
                {
                    DataTable table = tables[j];
                    if (table.TableName == "data")
                    {
                        string creatorName = string.Format("{0}Creator", ExeclUtil.FirstCharToUpper(name));
                        string configName = string.Format("Config{0}", ExeclUtil.FirstCharToUpper(name));
                        string configPath = "Game.Core.Config." + configName;

                        MethodInfo mi = this.GetType().GetMethod("Pack");
                        MethodInfo miGen = mi.MakeGenericMethod(new Type[] { a.GetType(configPath, true, true) });
                        miGen.Invoke(this, new object[] { table, creatorName, configName });
                        break;
                    }
                }
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("所有配置表打包成功");
    }

    /// <summary>
    /// 打包
    /// </summary>
    public void Pack<T>(DataTable table, string creatorName, string configName) where T : ConfigBase, new()
    {
        Debug.LogFormat("{0} ----->开始打包", configName);
        Type type = Type.GetType(creatorName, true, true);
        ICreator<T> creator = Activator.CreateInstance(type) as ICreator<T>;
        creator.ReadData(table);
        List<T> list = creator.GetData();
        if (list.Count > 0)
        {
            using (FileStream fs = new FileStream(dataPath + configName + ".bytes", FileMode.Create))
            {
                BinaryFormatter binFormat = new BinaryFormatter();
                binFormat.Serialize(fs, list);
                Debug.LogFormat("{0}----->打包成功", configName);
            }
        }
    }
}
