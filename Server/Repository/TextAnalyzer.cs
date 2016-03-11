//////////////////////////////////////////////////////////////////////////////////////////
////  TextAnalyzer.cs - the entry of program, executive part                          ////
////  Language:    C#                                                                 ////
////  Writer:      Yufan Gong                                                         ////
////  Class:       CSE 681 SMA                                                        ////
////  Project:     project2                                                           ////
//////////////////////////////////////////////////////////////////////////////////////////
/*
 * Package: TextAnlyzer
 * ====================
 * 
 * This Package acts as a executive part. Controller of the program.
 * 
 */
using System;

namespace project2_YufanGong
{
    class TextAnalyzer
    {
        //-----< executive function >---
        static void Main(string[] args)
        {
            Display dp = new Display();
            dp.displayCommandLineArgs(args);
            CommandLinePa myProject2 = new CommandLinePa();
            myProject2.parseCommandLineArgs(args);
        }
    }
}
