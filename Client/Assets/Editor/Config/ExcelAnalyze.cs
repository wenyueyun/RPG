using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System.Text;

public class ExcelAnalyze
{
    private static readonly string class_path = Path.Combine(Application.dataPath, "Scripts/Core/Config/");
    private static readonly string creator_path = Path.Combine(Application.dataPath, "Editor/Config/Creator/");
    private static List<ExcelData> data_list = new List<ExcelData>();
    [MenuItem("工具/配置表/解析配置表")]
    public static void GenerateCode()
    {
        string path = EditorUtility.OpenFilePanel("Select xlsx file", "", "xlsx");
        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            Debug.Log("解析配置表---->" + path);

            //根据路径获取配置表名字
            int startIdx = path.LastIndexOf('/');
            int endIdx = path.IndexOf(".xlsx");
            string name = path.Substring(startIdx + 1, endIdx - startIdx - 1);

            //读取配置表数据
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            DataTableCollection tables = result.Tables;
            for (int i = tables.Count - 1; i >= 0; i--)
            {
                DataTable table = tables[i];
                if (table.TableName == "format")
                {
                    ReadData(name, table);
                    GenerateClassCode(name, table);
                    GenerateCreatorCode(name, table);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 读取配置表数据
    /// </summary>
    private static void ReadData(string name, DataTable table)
    {
        //读取数据
        data_list.Clear();
        for (int i = 1; i < table.Rows.Count; i++)
        {
            object[] items = table.Rows[i].ItemArray;
            ExcelData data = new ExcelData();
            data.key = items[0].ToString().Trim();
            data.type = items[1].ToString().Trim();
            data.keyBit = items[2].ToString().Trim();
            data.isClient = items[3].ToString().Trim().Contains("c");
            data.desc = items[4].ToString().Trim();

            if (data.isClient)
            {
                data_list.Add(data);
            }
        }
    }

    /// <summary>
    /// 生成CS类
    /// </summary>
    private static void GenerateClassCode(string name, DataTable table)
    {
        if (data_list.Count <= 0)
        {
            Debug.LogError(string.Format("{0}----------->没读到数据", name));
            return;
        }

        //生成文件
        string className = string.Format("Config{0}", ExeclUtil.FirstCharToUpper(name));
        StringBuilder classsb = new StringBuilder();
        classsb.Append(classTemplate);

        StringBuilder property = new StringBuilder();
        property.Append("\n");
        StringBuilder read = new StringBuilder();
        read.Append("//read");
        read.Append("\n");
        StringBuilder write = new StringBuilder();
        write.Append("//write");
        write.Append("\n");

        for (int i = 0; i < data_list.Count; i++)
        {
            property.Append("\t\t");
            property.Append("/// <summary>\n");
            property.Append("\t\t");
            property.AppendFormat("///{0}\n", data_list[i].desc);
            property.Append("\t\t");
            property.Append("/// </summary>\n");
            property.Append("\t\t");
            property.AppendFormat("public {0} {1};\n", data_list[i].type, data_list[i].key);

            read.Append("\t\t\t");
            read.AppendFormat("{0} = info.{1}(\"{2}\");\n", data_list[i].key, ExeclUtil.GetMethod(data_list[i].type), data_list[i].key);

            write.Append("\t\t\t");
            write.AppendFormat("info.AddValue(\"{0}\", {1});\n", data_list[i].key, data_list[i].key);
        };

        classsb.Replace("%CLASSNAME%", className);
        classsb.Replace("%PROPERTY&", property.ToString());
        classsb.Replace("%READ%", read.ToString());
        classsb.Replace("%WRITE%", write.ToString());

        string file_path = PathUtil.GetRegularPath(string.Format("{0}{1}.cs", class_path, className));
        if (File.Exists(file_path))
        {
            File.Delete(file_path);
        }
        File.WriteAllText(file_path, classsb.ToString(), Encoding.UTF8);

        Debug.Log("生成代码文件 ----->" + file_path);
    }


    /// <summary>
    /// 生成打包代码
    /// </summary>
    private static void GenerateCreatorCode(string name, DataTable table)
    {
        if (data_list.Count <= 0)
        {
            Debug.LogError(string.Format("{0}----------->没读到数据", name));
            return;
        }

        //生成文件
        string className = string.Format("{0}Creator", ExeclUtil.FirstCharToUpper(name));
        string configName = string.Format("Config{0}", ExeclUtil.FirstCharToUpper(name));
        StringBuilder classsb = new StringBuilder();
        classsb.Append(creatorTemplate);

        StringBuilder read = new StringBuilder();
        read.Append("\n");

        for (int i = 0; i < data_list.Count; i++)
        {
            ExcelData data = data_list[i];
            read.Append("\t\t\t");
            read.AppendFormat("c.{0} = {1}(\"{2}\", i);\n", data.key, ExeclUtil.GetMethod(data_list[i].type), data.key);
        }

        classsb.Replace("%READ%", read.ToString());
        classsb.Replace("%CLASSNAME%", className);
        classsb.Replace("%CONFIGNAME%", configName);

        string file_path = PathUtil.GetRegularPath(string.Format("{0}{1}.cs", creator_path, className));
        if (File.Exists(file_path))
        {
            File.Delete(file_path);
        }
        File.WriteAllText(file_path, classsb.ToString(), Encoding.UTF8);

        Debug.Log("生成打包文件 ----->" + file_path);
    }


    #region classTemplate
    private static string classTemplate =
  @"
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
    public class %CLASSNAME% : ConfigBase, ISerializable
    {
        %PROPERTY&

        public %CLASSNAME%()
        {
        }

        protected %CLASSNAME%(SerializationInfo info, StreamingContext context)
        {
            %READ%
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            %WRITE%  
        }
    }
}";
    #endregion

    #region creatorTemplate
    private static string creatorTemplate =
  @"
using Game.Core.Config;
using System.Collections.Generic;
using System.Data;
/// <summary>
/// 自动生成，禁止修改
/// </summary>
public class %CLASSNAME% :BaseCreator, ICreator<%CONFIGNAME%>
{
    public %CLASSNAME%()
    {
    }

    public List<%CONFIGNAME%> GetData()
    {
        List<%CONFIGNAME%> list = new List<%CONFIGNAME%>();
        for (int i = 0; i < excelData.Count; i++)
        {
            %CONFIGNAME% c = new %CONFIGNAME%();
            %READ%
            list.Add(c);
        }
        return list;
    }

    public override void ReadData(DataTable table)
    {
        base.ReadData(table);
    }
}
";
    #endregion
}


