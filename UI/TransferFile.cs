///////////////////////////////////////////////////////////////////////////
////  TransferFile.cs - upload files in client to server repository    ////
////  Language:    C#                                                  ////
////  Writer:      Yufan Gong                                          ////
////  Class:       CSE 681 SMA                                         ////
////  Project:     project4                                            ////
///////////////////////////////////////////////////////////////////////////
/*
 * Package: Client
 * =====================
 * 
 * Class:TransferFile
 * ==================
 * 
 * This class aims to upload local files to the server repository
 * 
 * required files:
 * - CommService: ICommService, CommService
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace DocumentVault
{
    class TransferFile
    {
        MainWindow main;
        public TransferFile(MainWindow reference)
        {
            main = reference;
        }
        //--------< send file content each time 512 size >-------
        private bool SendFile(ICommService service, string file)
        {
            long blockSize = 512;
            try
            {
                string filename = Path.GetFileName(file);
                service.OpenFileForWrite(filename);
                FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read);
                int bytesRead = 0;
                while (true)
                {
                    long remainder = (int)(fs.Length - fs.Position);
                    if (remainder == 0)
                        break;
                    long size = Math.Min(blockSize, remainder);
                    byte[] block = new byte[size];
                    bytesRead = fs.Read(block, 0, block.Length);
                    service.WriteFileBlock(block);
                }
                fs.Close();
                service.CloseFile();
                return true;
            }
            catch (Exception ex)
            {
                Console.Write("\n  can't open {0} for writing - {1}", file, ex.Message);
                return false;
            }
        }
        //--------< connect with server >-------
        public void UploadToServer(string targetfile)
        {
            ICommService cs = null;
            int count = 0;
            while (true)
            {
                try
                {
                    cs = Sender.CreateProxy(main.ServerUrl);
                    break;
                }
                catch
                {
                    Console.Write("\n  connection to service failed {0} times - trying again", ++count);
                    Thread.Sleep(500);
                    continue;
                }
            }
            SendFile(cs, targetfile);
        }
    }
}