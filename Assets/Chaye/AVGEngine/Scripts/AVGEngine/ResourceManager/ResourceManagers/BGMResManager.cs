using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IdlessChaye.IdleToolkit.AVGEngine {
    public class BGMResManager<TValue> : BaseResManager<TValue> where TValue : class {

        private BGMResBufferIndexer<TValue> bufferIndexer;

        public BGMResManager(int bufferMaxCount) {
            bufferIndexer = new BGMResBufferIndexer<TValue>(bufferMaxCount);
        }

 
        public override TValue Get(string subIndex) {
            string key = subIndex;
            string finalIndex = subIndex;
            if (PathDict.ContainsKey(key)) {
                finalIndex = PathDict[key];
            }
            return bufferIndexer.Get(key, finalIndex);
        }

        public override bool LoadFileInfoList(List<FileInfo> fileInfoList) {
            for (int i = 0; i < fileInfoList.Count; i++) {
                FileInfo fileInfo = fileInfoList[i];
                string name = fileInfo.Name.Split('.')[0];
                string fullPath = fileInfo.FullName;

                if (pathDict.ContainsKey(name)) {
                    pathDict[name] = fullPath;
                } else {
                    pathDict.Add(name, fullPath);
                }
            }
            return true;
        }

        public void ForceAdd(string key, TValue value) {
            bufferIndexer.ForceAdd(key, value);
        }

    }
}