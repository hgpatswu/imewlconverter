﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Studyzy.IMEWLConverter
{
    /// <summary>
    /// 搜狗细胞词库
    /// </summary>
    class SougouPinyinScel : IWordLibraryImport
    {
        #region IWordLibraryImport 成员

        public bool OnlySinglePinyin { get; set; }

        public WordLibraryList Import(string str)
        {
            WordLibraryList wlList = new WordLibraryList();
            var lines = str.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.IndexOf("'") == 0)
                {
                    string py = line.Split(' ')[0];
                    string word = line.Split(' ')[1];
                    WordLibrary wl = new WordLibrary();
                    wl.Word = word;
                    wl.Count = 1;
                    wl.PinYin = new List<string>(py.Split(new char[] { '\'' }, StringSplitOptions.RemoveEmptyEntries));
                    wlList.Add(wl);
                }
            }
            return wlList;
        }

        #endregion

        public static string ReadScel(string path)
        {
            Dictionary<int, string> pyDic = new Dictionary<int, string>();
            Dictionary<string, string> pyAndWord = new Dictionary<string, string>();
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] str = new byte[128];
            byte[] outstr = new byte[128];
            byte[] num;
            //fs.Read(str, 0, 128);//\x40\x15\x00\x00\x44\x43\x53\x01
            //fs.Position = 0x130;
            //fs.Read(str, 0, 64);
            //string txt = Encoding.Unicode.GetString(str);
            ////Console.WriteLine("字库名称:" + txt);
            //fs.Position = 0x338;
            //fs.Read(str, 0, 64);
            ////Console.WriteLine("字库类别:" + Encoding.Unicode.GetString(str));

            //fs.Position = 0x540;
            //fs.Read(str, 0, 64);
            ////Console.WriteLine("字库信息:" + Encoding.Unicode.GetString(str));

            //fs.Position = 0xd40;
            //fs.Read(str, 0, 64);
            ////Console.WriteLine("字库示例:" + Encoding.Unicode.GetString(str));

            fs.Position = 0x1540;
            str = new byte[4];
            fs.Read(str, 0, 4);//\x9D\x01\x00\x00
            while (true)
            {
                num = new byte[4];
                fs.Read(num, 0, 4);
                int mark = (int)num[0] + (int)num[1] * 256;
                str = new byte[128];
                fs.Read(str, 0, (int)(num[2]));
                string py = Encoding.Unicode.GetString(str);
                py = py.Substring(0, py.IndexOf('\0'));
                pyDic.Add(mark, py);
                if (py == "zuo")//最后一个拼音
                {
                    break;
                }
            }

            fs.Position = 0x2628;
            int i = 0, count = 0, offset = 0;
            //byte[] pybuf = new byte[128];
            //byte[] hzbuf = new byte[128];
            byte[] buf = new byte[256];
            while (true)
            {
                num = new byte[4];
                fs.Read(num, 0, 4);
                count = (int)num[2] + (int)num[3] * 256;
                offset = (int)num[0] + (int)num[1] * 256 - 1;
                str = new byte[256];
                for (i = 0; i < count; i++)
                {
                    str[i] = (byte)fs.ReadByte();
                }
                string wordPY = "";
                for (i = 0; i < count / 2; i++)
                {
                    int key = str[i * 2] + str[i * 2 + 1] * 256;
                    wordPY += pyDic[key] + "'";
                }
                wordPY = wordPY.Remove(wordPY.Length - 1);//移除最后一个单引号
                num = new byte[2];
                fs.Read(num, 0, 2);
                count = num[0] + num[1] * 256;
                str = new byte[256];
                fs.Read(str, 0, count);
                string word = Encoding.Unicode.GetString(str);
                word = word.Substring(0, word.IndexOf('\0'));
                pyAndWord.Add(wordPY, word);
                //接下来这是干啥的呢？
                str = new byte[512];
                for (i = 0; i < (12 + offset * (12 + count + 2)); i++)
                {
                    str[i] = (byte)fs.ReadByte();
                }

                if (fs.Length == fs.Position)//判断文件结束
                {
                    fs.Close();
                    break;
                }

            }
            StringBuilder sb = new StringBuilder();
            foreach (string key in pyAndWord.Keys)
            {
               sb.AppendLine("'"+key + " " + pyAndWord[key]);//以搜狗文本词库的方式返回
            }
            return sb.ToString();
        }
    }
}
