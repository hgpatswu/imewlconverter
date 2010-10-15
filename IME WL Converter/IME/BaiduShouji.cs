﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Studyzy.IMEWLConverter
{
  public  class BaiduShouji:IWordLibraryImport,IWordLibraryExport
  {
      //public bool OnlySinglePinyin { get; set; }
        #region IWordLibraryExport 成员

        public string Export(WordLibraryList wlList)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < wlList.Count; i++)
            {
                sb.Append(wlList[i].Word);
                sb.Append(" ");
                sb.Append(wlList[i].GetPinYinString("|", BuildType.None));
                sb.Append(" 20000");
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
        public int CountWord { get; set; }
        public int CurrentStatus { get; set; }
        public Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }

        #endregion

        #region IWordLibraryImport 成员

        WordLibraryList IWordLibraryImport.Import(string str)
        {
            WordLibraryList wlList = new WordLibraryList();
            var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
               
                    string py = line.Split(' ')[1];
                    string word = line.Split(' ')[0];
                    WordLibrary wl = new WordLibrary();
                    wl.Word = word;
                    wl.Count = 1;
                    wl.PinYin = py.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    wlList.Add(wl);
              
            }
            return wlList;
        }

        #endregion
    }
}
