///////////////////////////////////////////////////////////////////////////
////  TextQuery.cs - make query into file sets for text file           ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project2                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: TextQueryProcessing
 * ==============================
 * 
 * Class:TextQuery
 * ===============
 * 
 * This class implement function to make query into file sets in current 
 * directory or all subdirectories. It will extract text strings given by
 * user and store it for search for, and query into file sets find files 
 * contain all or at least one text string.
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace project2_YufanGong
{
    class TextQuery
    {
        List<string> queryResult = new List<string>();
        List<string> queryTexts = new List<string>();

        //----------<extract text string in arguments >--------
        private void getQueryTexts(string[] args)
        {
            foreach (string arg in args)
                if (arg.Contains("/T"))
                {
                    queryTexts.Add(arg.Substring(2).ToLower());
                }
        }
        //----------< make query into in current file set for text file >----------
        private void textQuery(string[] args, string path, bool matchAll)
        {
            int notMatch = 0;
            int repetition = 0;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                TextFile tf = new TextFile();
                if (tf.textFile(Path.GetExtension(file)))
                {
                    repetition = 0;
                    TextReader tr = File.OpenText(file);
                    string contents = tr.ReadToEnd();
                    tr.Close();

                    foreach (string text in queryTexts)
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
                                //if one text string given not match with text file 
                                notMatch++;
                                break;
                            }
                        }
                        else
                        {
                            if (contents.ToLower().IndexOf(text) >= 0)
                            {
                                //avoid add repeat file name 
                                if (repetition == 0)
                                {
                                    queryResult.Add(file);
                                    repetition++;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //make sure situation text file match all text string
                    if (matchAll && notMatch == 0)
                    {
                        queryResult.Add(file);
                    }
                }
            }
        }
        //---------< make query into all subdirectories >-------------
        public void textQueryR(bool recursion, string[] args, bool matchAll)
        {
            string path = args[0];
            getQueryTexts(args);
            Recursion rec = new Recursion();
            if (recursion)
            {
                Console.Write("\n\n Implement Requirement 5.\n");
                rec.newDir += new Recursion.newDirHandler(textQuery);
                rec.go(args,path, matchAll);
                if (matchAll)
                {
                    Display dp = new Display();
                    dp.displayText("Text query with recursion", "Query files match all texts", path, queryResult);
                }
                else
                {
                    Display dp = new Display();
                    dp.displayText("Text query with recursion", "Query files match with at least one text", path, queryResult);
                }
            }
            else
            {
                Console.Write("\n\n Implement Requirement 3.\n");
                textQuery(args, path, matchAll);
                if (matchAll)
                {
                    Display dp = new Display();
                    dp.displayText("Text query", "Query files match with all texts", path, queryResult);
                }
                else
                {
                    Display dp = new Display();
                    dp.displayText("Text query", "Query files match with at least one text", path, queryResult);
                }
            }
        }
#if(TEST_TEXTQUERY)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test TextQuery");
            Console.Write("\n================\n");

            TextQuery tq = new TextQuery();
            
            string path = args[0];
            bool matchAll = true;
            bool recursion = true;
            tq.textQueryR(recursion,args,matchAll);
        }
#endif
    }
}
