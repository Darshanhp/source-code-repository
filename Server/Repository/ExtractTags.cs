////////////////////////////////////////////////////////////////////////////////
////  ExtractTags.cs - extraxt tag information given by Commend Line Args   ////
////  Language:    C#                                                       ////
////  Writer:      Yufan Gong                                               ////
////  Class:       CSE 681 SMA                                              ////
////  Project:     project2                                                 ////
////////////////////////////////////////////////////////////////////////////////
/*
 * Package: MetadataTool
 * ======================
 * 
 * This class implement extracting element values given by Command Line Arguments 
 * and store in some containers, these containers will be used by XmlCreation.
 * 
 * 
 */
using System;
using System.Collections.Generic;

namespace project2_YufanGong
{
    class ExtractTags
    {   
        //----------------< get element value given by Command Line Argument>---------------
        public void getElement(string[] args,out List<string> category, out List<string>child, out List<string> description, out List<string> dependency, out List<string> keyword)
        {
            category = new List<string>();
            description = new List<string>();
            dependency = new List<string>();
            keyword = new List<string>();
            child = new List<string>();

            // find the positions of each tag name
            int strNum;
            int posC, posCH, posT, posD, posK;
            posC = posCH = posT = posD = posK = 0;
            for (strNum = 0; strNum < args.Length; strNum++)
            {
                if (args[strNum].StartsWith("/C"))
                    posC = strNum;
                if (args[strNum].StartsWith("/H"))
                    posCH = strNum;
                if (args[strNum].StartsWith("/T"))
                    posT = strNum;
                if (args[strNum].StartsWith("/D"))
                    posD = strNum;
                if (args[strNum].StartsWith("/K"))
                    posK = strNum;
            }
            // store tag names in containers
            for (int i = posC + 1; i < posCH; i++)
                category.Add(args[i]);
            for (int i = posCH + 1; i < posT; i++)
                child.Add(args[i]);
            for (int i = posT + 1; i < posD; i++)
                description.Add(args[i]);
            for (int i = posD + 1; i < posK; i++)
                dependency.Add(args[i]);
            for (int i = posK + 1; i < args.Length; i++)
                keyword.Add(args[i]);
        }
#if(TEST_EXTRACTTAGS)
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("\n Test ExtractTags");
            Console.Write("\n==================\n");

            List<string> description;
            List<string> dependency;
            List<string> keyword;

            ExtractTags et = new ExtractTags();
            et.getElement(args, out description, out dependency, out keyword);

            Console.Write("\n Description:\n");
            foreach (string des in description)
                Console.Write("{0} \t",des);
            Console.Write("\n Dependency:\n");
            foreach (string dep in dependency)
                Console.Write("{0} \t",dep);
            Console.Write("\n Keyword:\n");
            foreach (string key in keyword)
                Console.Write("{0} \t",key);
        }
#endif
    }
}
