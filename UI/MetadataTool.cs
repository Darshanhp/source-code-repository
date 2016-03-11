///////////////////////////////////////////////////////////////////////////
////  MetadataTool.cs - create xml file for specified text file        ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project4                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: Client
 * =====================
 * 
 * Class: MetadataTool
 * ====================
 * 
 * This class implement creation function to create xml files.
 * User will specify a text file in client or server repository, 
 * MetadataTool will create a xml file, show xml file contents.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace DocumentVault
{
    class MetadataTool
    {
        Upload upload;
        string XmlName;
        public MetadataTool(Upload reference)
        {
            upload = reference;
        }
        
        //-----------< create xml file and write info >--------------
        public void xmlCreation(string filename)
        {
            string desInput = upload.Description.Text;
            string depInput = upload.Dependency.Text;
            string keyInput = upload.Keyword.Text;
            string catInput = upload.Categories.Text;
            string chiInput = upload.Children.Text;

            string[] description = null;
            string[] dependency = null;
            string[] keyword = null;
            string[] category = null;
            string[] children = null;
            char[] separator = { ' ' };
            description = desInput.Split(separator);
            dependency = depInput.Split(separator);
            keyword = keyInput.Split(separator);
            if (catInput != "")
            {
                catInput = catInput.Substring(0, catInput.Length - 1);
            }
            category = catInput.Split(separator);
            if (chiInput != "")
            {
                chiInput = chiInput.Substring(0, chiInput.Length - 1);
            }
            children = chiInput.Split(separator);

            string path;
            int pos = filename.LastIndexOf("\\");
            if (pos > -1)
            {
                path = filename.Substring(0, pos + 1);
                string shortfilename = filename.Remove(0, pos + 1);

                DirectoryInfo dit = new DirectoryInfo(path);
                FileInfo[] filesinfo = dit.GetFiles();
                foreach (FileInfo targetfile in filesinfo)
                {
                    if (targetfile.Name == shortfilename)
                    {

                        XmlTextWriter tw = null;
                        XmlName = targetfile + ".xml";
                        tw = new XmlTextWriter(XmlName, null);

                        tw.Formatting = Formatting.Indented;
                        tw.WriteStartDocument();
                        tw.WriteStartElement("xmlfile");
                        tw.WriteElementString("name", targetfile.ToString());
                        foreach (string cat in category)
                            tw.WriteElementString("category", cat);
                        foreach (string chi in children)
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
                        ShowXml(XmlName);
                    }
                }
              
            }
            else
            {
                XmlName = filename;
                int ext = filename.LastIndexOf(".");
                string targetfile = filename.Substring(0, ext);
                XmlTextWriter tw = null;
                tw = new XmlTextWriter(XmlName, null);

                tw.Formatting = Formatting.Indented;
                tw.WriteStartDocument();
                tw.WriteStartElement("xmlfile");
                tw.WriteElementString("name", targetfile.ToString());
                foreach (string cat in category)
                    tw.WriteElementString("category", cat);
                foreach (string chi in children)
                    tw.WriteElementString("child", chi);
                foreach (string des in description)
                    tw.WriteElementString("description", des);
                foreach (string dep in dependency)
                    tw.WriteElementString("dependency", dep);
                foreach (string key in keyword)
                    tw.WriteElementString("keyword", key);
                tw.WriteEndElement();
                tw.WriteEndDocument();
                tw.Flush();
                tw.Close();
                ShowXml(XmlName);
            }
        }
        //--------< show content of a xml file >-------
        private void ShowXml(string name)
        {
            XDocument doc = XDocument.Load(@name);
            upload.xml_view.Text = doc.ToString();
        }
        public string getXml()
        {
            return XmlName;
        }
        //tested in project2
    }
}
