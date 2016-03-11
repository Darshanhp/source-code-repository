////////////////////////////////////////////////////////////////////////////////
////  LinqToXml.cs - linq to a specified xml file and find match elements   ////
////  Language:    C#                                                       ////
////  Writer:      Yufan Gong                                               ////
////  Class:       CSE 681 SMA                                              ////
////  Project:     project2                                                 ////
////////////////////////////////////////////////////////////////////////////////
/*
 * Package: XmlQueryProcessing
 * ============================
 * 
 * Class:LinqToQuery
 * ==================
 *
 * This class will implement searching in a specified xml file and extract the element name
 * will be searched given by arguments. If this xml file contains same element names, this
 * function will input the element names and its values.
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;


namespace project2_YufanGong
{
    class LinqToXml
    {
        //-------------< extract tags names given by arguments >-------
        private void getTags(string[] args,out List<string> tags)
        {        
            tags = new List<string>();
            //find tag name position
            int pos = 0;
            for (int strNum = 0; strNum < args.Length; strNum++)
                if (args[strNum].Contains("/M"))
                    pos = strNum;
            for (int i = pos + 1; i < args.Length; i++)
                tags.Add(args[i]);
        }
        //-------< linq in sepecified xml file and find element name match with given tags >---
        public void linqToXml(string[] args, string xmlfile)
        {
            List<string> tags;
            getTags(args, out tags);
            XDocument doc = XDocument.Load(xmlfile);
            //linq in this xml file to find all tags
            foreach (string tag in tags)
            {
                var q = from x in
                            doc.Descendants()
                        where (x.Name == tag)
                        select x;
                foreach (var elem in q)
                    Console.Write("\n {0, -12} {1}", elem.Name, elem.Value);
            }
            Console.Write("\n\n");
        }
#if(TEST_LINQTOXML)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test LinqToXml");
            Console.Write("\n================\n");

            List<string> xmlfiles = new List<string>();
            LinqToXml ltx = new LinqToXml();
            string path = args[0];
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (Path.GetExtension((file)) == ".xml")
                    xmlfiles.Add(file);
            }
            foreach (string xmlfile in xmlfiles)
                Console.Write("\n{0}\n", xmlfile);

            ltx.linqToXml(args, xmlfiles);

        }
#endif
    }
}
