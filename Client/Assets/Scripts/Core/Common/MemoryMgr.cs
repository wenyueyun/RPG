
using UnityEngine;
using Game.Core.Common;
using UnityEngine.Profiling;

namespace Qarth
{
    public class MemoryMgr :Singleton<MemoryMgr>
    {
        [SerializeField]
        private int m_MaxMemoryUse = 170;
        [SerializeField]
        private int m_MaxHeapMemoryUse = 50;
        [SerializeField]
        private bool m_DisplayOnGUI = true;

        public void Init()
        {
        }

        void Update()
        {
        //内存管理
        //MonitorMemorySize();
        }

        void OnGUI()
        {
            if (m_DisplayOnGUI)
            {
                float totalAllocatedMemory = ByteToM(Profiler.GetTotalAllocatedMemoryLong());
                float reservedMemory = ByteToM(Profiler.GetTotalReservedMemoryLong());

                float monoUsedSize = ByteToM(Profiler.GetMonoUsedSizeLong());
                float monoHeapSize = ByteToM(Profiler.GetMonoHeapSizeLong());
                
                GUILayout.TextField(string.Format("Reserved：{0}M, TotalAllocated:{1}M", reservedMemory, totalAllocatedMemory));
                GUILayout.TextField(string.Format("Heap：{0}M, HeadUsed:{1}M", monoHeapSize, monoUsedSize));
                //GUILayout.TextField(string.Format("ResLoader:{0}, Res:{1}, AB:{2}", ResLoader.activeResLoaderCount, ResMgr.S.totalResCount, AssetBundleRes.s_ActiveCount));
            }
        }

        #region 内存监控

        // 字节到兆
        //const float ByteToM = 0.000001f;

        static bool s_isFreeMemory = false;
        static bool s_isFreeMemory2 = false;

        static bool s_isFreeHeapMemory = false;
        static bool s_isFreeHeapMemory2 = false;

        /// <summary>
        /// 用于监控内存
        /// </summary>
        /// <param name="tag"></param>
        void MonitorMemorySize()
        {
            if (ByteToM(Profiler.GetTotalReservedMemoryLong()) > m_MaxMemoryUse * 0.7f)
            {
                if (!s_isFreeMemory)
                {
                    s_isFreeMemory = true;
                }

                if (ByteToM(Profiler.GetMonoHeapSizeLong()) > m_MaxMemoryUse)
                {
                    if (!s_isFreeMemory2)
                    {
                        s_isFreeMemory2 = true;
                        Debug.LogError("总内存超标告警 ！当前总内存使用量： " + ByteToM(Profiler.GetTotalAllocatedMemoryLong()) + "M");
                    }
                }
                else
                {
                    s_isFreeMemory2 = false;
                }
            }
            else
            {
                s_isFreeMemory = false;
            }

            if (ByteToM(Profiler.GetMonoUsedSize()) > m_MaxHeapMemoryUse * 0.7f)
            {
                if (!s_isFreeHeapMemory)
                {
                    s_isFreeHeapMemory = true;
                }

                if (ByteToM(Profiler.GetMonoUsedSize()) > m_MaxHeapMemoryUse)
                {
                    if (!s_isFreeHeapMemory2)
                    {
                        s_isFreeHeapMemory2 = true;
                        Debug.LogError("堆内存超标告警 ！当前堆内存使用量： " + ByteToM(Profiler.GetMonoUsedSize()) + "M");
                    }
                }
                else
                {
                    s_isFreeHeapMemory2 = false;
                }
            }
            else
            {
                s_isFreeHeapMemory = false;
            }
        }

        #endregion

        float ByteToM(long byteCount)
        {
            return (float)(byteCount / (1024.0f * 1024.0f));
        }
    }
}
