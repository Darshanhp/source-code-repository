///////////////////////////////////////////////////////////////////////////
////  MetadataTool.cs - create xml file for specified text file        ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project2                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: MetadataTool
 * =====================
 * 
 * Class:XmlCreation
 * ==================
 * 
 * This package implement creation function to create xml files.
 * User will specify a text file, MetadataTool should first judge is that exsit, 
 * if it is, then create a xml file, input element extracted and contained by
 * ExtractTags class, show xml file contents.
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;



namespace project2_YufanGong
{
    class XmlCreation
    {
        //-----------< create xml file and write info >--------------
        public void xmlCreation(string[] args)
        {
            List<string> description;
            List<string> dependency;
            List<string> keyword;
            List<string> category;
            List<string> child;
            //get tag names stored by calling getElement function
            ExtractTags et = new ExtractTags();
            et.getElement(args,out category,out child, out description,out dependency,out keyword);
            FileInfo targetfile;
            bool exsit;
            TargetFile tf = new TargetFile();
            tf.getFile(args,out targetfile,out exsit);
            if (exsit)
            {
                XmlTextWriter tw = null;
                string name = targetfile + ".xml";
                tw = new XmlTextWriter("090", null);

                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument();
                tw.WriteStartElement("xmlfile");
                tw.WriteElementString("name", targetfile.ToString());
                foreach (string cat in category)
                    tw.WriteElementString("category", cat);
                foreach (string chi in child)
                    tw.WriteElementString("child", chi);
                foreach (string des in description)
                    tw.WriteElementString("description", des);
                foreach (string dep in dependency)
                    tw.WriteElementString("dependency", dep);
                foreach (string key in keyword)
                    tw.WriteElementString("keyword", key);
                tw.WriteElementString("size", targetfile.Length.ToString());
                tw.WriteElementString("time-date", targetfile.LastWriteTime.ToString());
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
                ShowXml(name);
            }
            else
                Console.Write("\n\n ERROR! {0} can not be found!\n\n", args[0]);
        }
        //--------< show content of a xml file >-------
        private void ShowXml(string name)
        {
            XDocument doc = XDocument.Load(@name);
            Console.Write(doc.ToString());
        }
#if(TEST_METADATATOOL)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test MetadataTool");
            Console.Write("\n===================\n\n");

            MetadataTool mt = new MetadataTool();
            mt.metadataTool(args);
        }
#endif
    }
}
