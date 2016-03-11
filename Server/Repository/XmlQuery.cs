//////////////////////////////////////////////////////////////////////////////////////////
////  XmlQuery.cs - make query into file sets for xml file                            ////
////  Language:    C#                                                                 ////
////  Writer:      Yufan Gong                                                         ////
////  Class:       CSE 681 SMA                                                        ////
////  Project:     project2                                                           ////
//////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package: XmlQueryProcessing
 * ============================
 * 
 * Class:XmlQuery
 * ==============
 * 
 * This class aim to query into file sets and find is there all text files have associate
 * metadata files, and send error message if one text file does't have metadata file.
 * This class also make query into every file's associate xml file to find which xml contains
 * same tag name with what user input by calling LinqToXml class.
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace project2_YufanGong
{
    class XmlQuery
    {
        List<string> errfiles = new List<string>();
       //------< make query into current file set >-----------
        private void xmlQuery(string[] args, string path, bool matchAll)
        {
            bool exist;
            //judge is that every text file has a associate matadata file
            List<string> textfiles = new List<string>();
            List<string> xmlfiles = new List<string>();
            string[] files = Directory.GetFiles(path);
            textfiles.Clear();//clear container for recursion function
            xmlfiles.Clear();
            foreach (string file in files)
            {
                try
                {
                    TextFile tf = new TextFile();
                    if (tf.textFile(Path.GetExtension(file)))
                    {
                        textfiles.Add(file);
                    }
                    else if (Path.GetExtension((file)) == ".xml")
                    {
                        xmlfiles.Add(file);
                    }
                }
                catch
                {
                    Console.Write("\n Error processing {0}", file);
                }
            }
            //find associate xml file for each text file by compare name
                foreach (string textfile in textfiles)
                {
                    exist = false;
                    foreach (string xmlfile in xmlfiles)
                    {
                        if (xmlfile == textfile + ".xml")
                        {
                            exist = true;
                            LinqToXml ltx = new LinqToXml();
                            ltx.linqToXml(args,xmlfile);
                            break;
                        }
                        else
                            continue;
                    }
                    //text file doesn't have a associate xml file
                    if (!exist)
                        errfiles.Add(textfile);
                }
        }
      
        //----------< make query into all subdirectories file sets >--------
        public void xmlQueryR(bool recursion, string[] args, bool matchAll)
        {
            string path = args[0];
            Recursion rec = new Recursion();
            if (recursion)
            {
                Console.Write("\n Implement Requirement 5.\n");
                Display dp = new Display();
                dp.displayXml("Metadata query with recursion", "Query process including all subdirectories");
                rec.newDir += new Recursion.newDirHandler(xmlQuery);
                rec.go(args, path, matchAll);
                if (errfiles != null)
                {
                    foreach (string errfile in errfiles)
                        Console.Write("\nERROR! This file doesn't have a associate Metadata file: \n {0} \n", errfile);
                }

            }
            else
            {
                Console.Write("\n Implement Requirement 4.\n");
                Display dp = new Display();
                dp.displayXml("Metadata query", "Query process in current directory");
                xmlQuery(args, path, matchAll);
                if (errfiles != null)
                {
                    foreach (string errfile in errfiles)
                        Console.Write("\nERROR! This file doesn't have a associate Metadata file: \n {0} \n", errfile);
                }
            }
        }
#if(TEST_XMLQUERY)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test XmlQuery");
            Console.Write("\n==============\n");

            bool recursion = true;
            bool matchAll = true;
            XmlQuery xq = new XmlQuery();
            xq.xmlQueryR(recursion,args,matchAll);
        }
#endif
    }
}