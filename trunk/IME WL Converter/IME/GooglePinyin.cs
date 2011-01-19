﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Studyzy.IMEWLConverter
{
   public class GooglePinyin:IWordLibraryExport,IWordLibraryImport
   {
       public int CountWord { get; set; }
       public int CurrentStatus { get; set; }
        #region IWordLibraryExport 成员

        public string Export(WordLibraryList wlList)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < wlList.Count; i++)
            {
                sb.Append(ExportLine(wlList[i]));
                sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        #endregion

        #region IWordLibraryImport 成员

        public WordLibraryList Import(string str)
        {
            WordLibraryList wlList = new WordLibraryList();
            var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                wlList.AddWordLibraryList(ImportLine(line));
            }
            return wlList;
        }

        #endregion


        public string ExportLine(WordLibrary wl)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(wl.Word);
            sb.Append("\t");
            sb.Append(wl.Count);
            sb.Append("\t");
            sb.Append(wl.GetPinYinString(" ", BuildType.None));
            

            return sb.ToString();
        }


       public WordLibraryList ImportLine(string line)
        {
            var c = line.Split('\t');
            WordLibrary wl = new WordLibrary();
            wl.Word = c[0];
            wl.Count = Convert.ToInt32(c[1]);
            wl.PinYin = c[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            WordLibraryList wll = new WordLibraryList();
            wll.Add(wl);
            return wll;
        }
   }
}