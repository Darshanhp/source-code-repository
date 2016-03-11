///////////////////////////////////////////////////////////////////////////
////  TextFile.cs - make sure a file is a text file pattern            ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project2                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: TextQueryProcessing
 * ==============================
 * 
 * Class:TextFile
 * ===============
 * 
 * This class aim to judge is this file a text file.
 * 
 */

using System;

namespace project2_YufanGong
{
    class TextFile
    {
        //-------< is this extension of a text file >--------
        public bool textFile(string ext)
        {
            switch (ext)
            {
                case ".cs": return true;
                case ".csproj": return true;
                case ".config": return true;
                case ".txt": return true;
                case ".bat": return true;
                default: return false;
            }
        }
    }
}
