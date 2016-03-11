///////////////////////////////////////////////////////////////////////////
////  ExtractFile.cs - extract file content and send to client         ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project4                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: Server
 * ==============================
 * 
 * Class:ExtractFile
 * ===============
 * 
 * This class aims to extract file content and send to client.
 * So that client-side can show the file content.
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
    class ExtractFile
    {
        //----< extract text file content>----------------
        public string TextFile(string filename)
        {
            string name = null;
            string contents = null;
            int pos = filename.LastIndexOf('.');
            name = filename.Substring(0, pos);
            int pos2 = name.LastIndexOf("\\");
            name = name.Remove(0, pos2 + 1);
            string[] files = Directory.GetFiles("../../Repository");
            foreach (string file in files)
            {
                int pos3 = file.LastIndexOf("\\");
                string shortname = file.Remove(0, pos3 + 1);
                if (shortname == name)
                {
                    TextReader tr = File.OpenText(file);
                    contents = tr.ReadToEnd();
                }
            }
            return contents;
        }
        //----< extract xml file content>----------------
        public string XmlFile(string filename)
        {
            int pos = filename.LastIndexOf("\\");
            if (pos > -1)
            {
                filename = filename.Remove(0, pos+1);
            }
            string contents = null;
            string[] files = Directory.GetFiles("../../Repository");
            foreach (string file in files)
            {
                int pos2 = file.LastIndexOf("\\");
                string shortfilename = file.Remove(0, pos2+1);
                if (shortfilename == filename)
                {
                    XDocument doc = XDocument.Load(file);
                    contents = doc.ToString();
                }
            }
            return contents;
        }
#if(TEST_EXTRACTFILE)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test ExtractFile");
            Console.Write("\n===================\n\n");
            ExtractFile ef = new ExtractFile();
            string filename = "../../Repository\\XmlCreation.cs.xml";
            string content1 = ef.TextFile(filename);
            string content2 = ef.XmlFile(filename);
            Console.Write("{0}\n\n{1}", content1, content2);
        }
#endif
    }
}
