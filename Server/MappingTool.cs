///////////////////////////////////////////////////////////////////////////
////  MappingTool.cs - construct a map of files to store their parents ////
////                   and children relationship                       ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project4                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: MappingTool
 * ==============================
 * 
 * Class:MappingTool
 * ===============
 * 
 *  This class aims to construct a map of files to store their 
 *  parents and children relationship
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace DocumentVault
{
    class MappingTool
    {
        Dictionary<string, string> Map = new Dictionary<string, string>();
        //----< construct the map or refresh it when user require for>----------------
        public void Mapping()
        {
            string[] files = Directory.GetFiles("../../Repository");
            List<string> xmlfiles = new List<string>();
            foreach (string file in files)
            {
                try
                {
                    //IsTextFile tf = new IsTextFile();
                    if (Path.GetExtension((file)) == ".xml")
                    {
                        xmlfiles.Add(file);
                    }
                }
                catch
                {
                    Console.Write("\n Error processing {0}", file);
                }
            }
            foreach (string xmlfile in xmlfiles)
            {
                XDocument doc = XDocument.Load(xmlfile);
                var q1 = from e in
                             doc.Elements("xmlfile").Descendants()
                         select e;

                for (int i = 0; i < q1.Count(); i++)
                {
                    if (q1.ElementAt(i).Name == "child")
                    {
                        int pathPos = xmlfile.LastIndexOf("\\");
                        string name = xmlfile.Remove(0, pathPos + 1);
                        int extePos = name.LastIndexOf(".");
                        name = name.Substring(0, extePos);
                        try
                        {
                            Map.Add(q1.ElementAt(i).Value, name);
                        }
                        catch (ArgumentException)
                        {
                            Map[q1.ElementAt(i).Value] += "|" + name;
                        }
                    }
                }
            }
            foreach (var dic in Map)
            {
                Console.Write("the key is child: {0}, the value are parents: {1}\n\n", dic.Key, dic.Value);
            }
        }
        //----< when user choose a specified file returen its parents files stored in the map>----------------
        public string findParents(string FileName)
        {
            //List<string> parents = new List<string>();
            //string[] parent = null;
            int PathPos = FileName.LastIndexOf("\\");
            FileName = FileName.Remove(0, PathPos + 1);
            int pos = FileName.LastIndexOf(".");
            FileName = FileName.Substring(0, pos);
            string parentsInValue = null;
            foreach (var dic in Map)
            {
                if (dic.Key == FileName)
                {
                    parentsInValue = dic.Value;
                    //char[] separator = { '|' };
                    //parent = parentsInValue.Split(separator);
                }
            }
            return parentsInValue;
        }
#if(TEST_MAPPINGTOOL)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test MappingTool");
            Console.Write("\n===================\n\n");

            MappingTool mt = new MappingTool();
            mt.Mapping();
            string result = mt.findParents("../../Repository\\XmlCreation.cs.xml");
            Console.Write(result);
        }
#endif
    }
}
