/**************************************************
*文件名：KeyConsumer
*描   述：
*创建者：lrh
*时间：2018-6-5 17:56:30
*
****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol.Util
{
    public class KeyConsumer<T>
    {
        private List<ParallelDataConsumer<T>> consumerDict = new List<ParallelDataConsumer<T>>();

        public KeyConsumer(Action<T> processAction,int concurrentLevel)
        {
            for(var i = 0; i < concurrentLevel; i++)
            {
                consumerDict.Add(new ParallelDataConsumer<T>(1, processAction));
            }
        }

        public void Produce(string key, T t) {
            int index = Hash(key);
            consumerDict[index].Produce(t);
        }

        public void Start()
        {
            for (var i = 0; i < consumerDict.Count; i++)
            {
                consumerDict[i].Start();
            }
        }

        public void Stop()
        {
            for (var i = 0; i < consumerDict.Count; i++)
            {
                consumerDict[i].Stop();
            }
        }

        private int Hash(string key)
        {
            return key.ToCharArray().Sum(c => (int)c) % consumerDict.Count;
        }
    }
}
