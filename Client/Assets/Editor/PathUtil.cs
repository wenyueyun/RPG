using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/************************* 
* 作者： wenyueyun 
* 时间： 2019/3/26 16:21:56 
* 描述： PathUtil 
*************************/
public class PathUtil
{
    /// <summary>
    /// 获取规范的路径。
    /// </summary>
    /// <param name="path">要规范的路径。</param>
    /// <returns>规范的路径。</returns>
    public static string GetRegularPath(string path)
    {
        if (path == null)
        {
            return null;
        }

        return path.Replace('\\', '/');
    }

    /// <summary>
    /// 获取连接后的路径。
    /// </summary>
    /// <param name="path">路径片段。</param>
    /// <returns>连接后的路径。</returns>
    public static string GetCombinePath(params string[] path)
    {
        if (path == null || path.Length < 1)
        {
            return null;
        }

        string combinePath = path[0];
        for (int i = 1; i < path.Length; i++)
        {
            combinePath = System.IO.Path.Combine(combinePath, path[i]);
        }

        return GetRegularPath(combinePath);
    }
}
