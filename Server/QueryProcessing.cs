//////////////////////////////////////////////////////////////////////////////////////////
////  QueryProcessing.cs - make query into file sets for text and xml files           ////
////  Language:    C#                                                                 ////
////  Writer:      Yufan Gong                                                         ////
////  Class:       CSE 681 SMA                                                        ////
////  Project:     project4                                                           ////
//////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package: QueryProcessing
 * ============================
 * 
 * Class:QueryProcessing
 * ==============
 *
 * 
 * This class implement function to make query into file sets in server, It will extract 
 * text strings or tags given by user and store it for search. For text file, this class
 * make query into file sets find files contain all or at least one text string. For xml
 * file, it will make query into metadata file and find files contains all tags
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;
using System.Xml.Linq;
using System.Threading;

namespace DocumentVault
{
    class QueryProcessing
    {
        List<string> foundFiles = new List<string>();
        public QueryProcessing()
        {
        }
        //----------< extract query information from message from client >----------
        public List<string> queryProcessing(ServiceMessage msg)
        {
            //List<string> foundFiles = new List<string>();
            //foundFiles.Clear();
            int i, j;
            string message = msg.Contents;

            string categories = null;
            string contents = null;
            string[] queryCategories = null;
            string[] queryContents = null;

            for (i = 0; message[i] != '&'; i++)
            {
                categories += message[i];
            }
            for (j = i + 1; j < message.Length; j++)
            {
                contents += message[j];
            }

            char[] separator = { ' ' };
            queryCategories = categories.Split(separator);
            queryContents = contents.Split(separator);

            if (msg.ResourceName == "textqueryA")
            {
                //how to send the query result to the client UI?
                TextQuery(queryCategories, queryContents, true);
            }
            else if (msg.ResourceName == "textqueryO")
            {
                TextQuery(queryCategories, queryContents, false);
            }
            else if (msg.ResourceName == "metadataquery")
            {
                XmlQuery(queryCategories, queryContents);
            }
            return foundFiles;
        }
        //----------< make query into file sets for text file >----------
        private void TextQuery(string[] categories, string[] queryContents, bool matchAll)
        {
            List<string> cateTextFiles = CateTextFiles(categories);
            //List<string> queryResult = new List<string>();
            int repetition = 0;
            int notMatch = 0;
            foreach (string textfile in cateTextFiles)
            {
                repetition = 0;
                TextReader tr = File.OpenText(textfile);
                string contents = tr.ReadToEnd();
                tr.Close();

                foreach (string text in queryContents)
                {
                    notMatch = 0;
                    if (matchAll)
                    {
                        if (contents.ToLower().IndexOf(text) >= 0)
                        {
                            continue;
                        }
                        else
                        {
                            notMatch++;
                            break;
                        }
                    }
                    else
                    {
                        if (contents.ToLower().IndexOf(text) >= 0)
                        {
                            if (repetition == 0)
                            {
                                foundFiles.Add(textfile + ".xml");
                                repetition++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (matchAll && notMatch == 0)
                {
                    foundFiles.Add(textfile + ".xml");
                }
            }
        }
        //----------< make query into file sets for xml file >----------
        private void XmlQuery(string[] categories, string[] queryContents)
        {
            List<string> cateXmlFiles = CateXmlFiles(categories);
            List<string> queryResult = new List<string>();
            int repetition = 0;
            foreach (string xmlfile in cateXmlFiles)
            {
                repetition = 0;
                //linq to xml
                XDocument doc = XDocument.Load(xmlfile);
                foreach (string tag in queryContents)
                {
                    var q1 = from e in
                                 doc.Elements("xmlfile").Descendants()
                             select e;
                    if (repetition == 0)
                    {
                        for (int i = 0; i < q1.Count(); i++)
                        {
                            if (q1.ElementAt(i).Name == tag)
                            {
                                foundFiles.Add(xmlfile);
                                repetition++;
                                break;
                            }
                        }
                    }
                    else
                        break;
                }
            }
        }
        //----------< get xml files in specified category >----------
        private List<string> CateXmlFiles(string[] categories)
        {
            List<string> xmlfiles = new List<string>();
            List<string> cateXmlFiles = new List<string>();
            string[] files = Directory.GetFiles("../../Repository");
            //xmlfiles.Clear();
            //cateXmlFiles.Clear();
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

            foreach (string category in categories)
            {
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
                            cateXmlFiles.Add(xmlfile);
                    }
                }
            }
            return cateXmlFiles;
        }
        //----------< get text files in specified category >----------
        private List<string> CateTextFiles(string[] categories)
        {
            List<string> textfiles = new List<string>();
            List<string> cateTextFiles = new List<string>();
            //textfiles.Clear();
            //cateTextFiles.Clear();
            List<string> cateXmlFiles = CateXmlFiles(categories);
            IsTextFile tf = new IsTextFile();
            string[] files = Directory.GetFiles("../../Repository");
            foreach (string file in files)
            {
                try
                {
                    if (tf.textFile(Path.GetExtension(file)))
                        textfiles.Add(file);
                }
                catch
                {
                    Console.Write("\n Error processing {0}", file);
                }
            }
            foreach (string cateXmlFile in cateXmlFiles)
            {
                foreach (string textfile in textfiles)
                {
                    if (textfile + ".xml" == cateXmlFile)
                    {
                        cateTextFiles.Add(textfile);
                    }
                }
            }
            return cateTextFiles;
        }
        //This file has already been test in project2
    }
}
