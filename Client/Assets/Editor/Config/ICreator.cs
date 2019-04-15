using Game.Core.Config;
using System.Collections.Generic;
using System.Data;


/************************* 
* 作者： wenyueyun 
* 时间： 2019/3/27 10:57:44 
* 描述： ICreator 
*************************/
public interface ICreator<T> where T : ConfigBase
{
    void ReadData(DataTable table);
    List<T> GetData();
}
