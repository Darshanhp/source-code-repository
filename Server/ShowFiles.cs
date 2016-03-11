///////////////////////////////////////////////////////////////////////////
////  ShowFile.cs - get all categories and files in each category      ////
////                in the server repository                           ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project4                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: Server
 * ==============================
 * 
 * Class:ShowFiles
 * ===============
 * 
 * This class aims to get all categories and files in each category and send
 * the information to the client.
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
    class ShowFiles
    {
        public ShowFiles()
        {
        }
        //----------< get categories in repository >----------
        public List<string> FindCategories()
        {
            string[] files = Directory.GetFiles("../../Repository");
            List<string> xmlfiles = new List<string>();
            List<string> categories = new List<string>();
            bool newcategory = true;
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
                             doc.Elements("xmlfile").Elements("category")
                         select e;
                for (int i = 0; i < q1.Count(); i++)
                {
                    newcategory = true;
                    foreach (string cate in categories)
                    {
                        if (q1.ElementAt(i).Value == cate)
                        {
                            newcategory = false;
                        }
                    }
                    if (newcategory)
                    {
                        categories.Add(q1.ElementAt(i).Value.ToString());
                    }
                }
            }
            return categories;
        }
        //----------< get files in each category in repository >----------
        public List<string> FindFiles(string category)
        {
            List<string> filesInCate = new List<string>();
            List<string> xmlfiles = new List<string>();
            string[] files = Directory.GetFiles("../../Repository");
            foreach (string file in files)
            {
                try
                {
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

            //search for category in repository
            foreach (string xmlfile in xmlfiles)
            {
                XDocument doc = XDocument.Load(xmlfile);

                var q1 = from e in
                             doc.Elements("xmlfile").Elements("category")
                         select e;

                //files = new string[q1.Count()];
                for (int i = 0; i < q1.Count(); i++)
                {
                    if (q1.ElementAt(i).Value == category)
                        filesInCate.Add(xmlfile);
                }
            }
            return filesInCate;
        }
#if(TEST_SHOWFILES)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test ShowFiles");
            Console.Write("\n===================\n\n");

            List<string> a1 = new List<string>();
            List<string> a2 = new List<string>();
            ShowFiles sf = new ShowFiles();
            a1 = sf.FindCategories();
            a2 = sf.FindFiles("code");
            foreach (string a in a1)
            {
                Console.Write(a1);
                Console.Write("\n");
            }
            foreach (string a in a2)
            {
                Console.Write(a1);
                Console.Write("\n");
            }
        }
#endif
    }
}

