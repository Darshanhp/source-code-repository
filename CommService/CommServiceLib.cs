///////////////////////////////////////////////////////////////////////////////
// CommServiceLib.cs - Document Vault communication service prototype        //
//                                                                           //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2013           //
// Changed by Yufan Gong                                                     //
// Nov 19, 2013                                                              //
///////////////////////////////////////////////////////////////////////////////
/*
 * Required Files:
 * - ICommLib.cs, AbstractCommunicator.cs, BlockingQueue   Defines AbstractMessageDispatcher
 * - ICommServiceLib.cs, CommServiceLib.cs                 Defines message-passing service
 *
 * Required References:
 * - System.ServiceModel
 * - System.Runtime.Serialization
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;

namespace DocumentVault
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
  public class CommService : ICommService  //: IComm
  {
    AbstractMessageDispatcher dispatcher = null;

    public string Name { get; set; }

    //----< Create service and get reference to dispatcher >---------

    public CommService()
    {
      Name = "CommService";
      dispatcher = AbstractMessageDispatcher.GetInstance();
      /*
       *  A class that derives from AbstractMessageDispatcher must
       *  be created before calling GetInstance().  The service
       *  Host must do that.
       */
    }
    //----< Post a message to the MessageDispatcher >----------------

    public void PostMessage(ServiceMessage servMsg)
    {
      //Console.Write("\n  CommService.PostMessage called with Message:");
      //msg.ShowMessage();
      dispatcher.PostMessage(servMsg);
    }
    string filePath = "../../Repository";
    string fileSpec = "";
    FileStream fs = null;  // remove static for WSHttpBinding


    //----< Set server repository path >----------------

    public void SetServerFilePath(string path)
    {
        filePath = path;
    }

    //----< open file need to upload and write >----------------

    public bool OpenFileForWrite(string name)
    {
        if (!Directory.Exists(filePath))
            Directory.CreateDirectory(filePath);

        fileSpec = filePath + "\\" + name;
        try
        {
            fs = File.Open(fileSpec, FileMode.Create, FileAccess.Write);
            Console.Write("\n  {0} opened", fileSpec);
            return true;
        }
        catch
        {
            Console.Write("\n  {0} filed to open", fileSpec);
            return false;
        }
    }

    //----< file chunk >----------------
    public bool WriteFileBlock(byte[] block)
    {
        try
        {
            Console.Write("\n  writing block with {0} bytes", block.Length);
            fs.Write(block, 0, block.Length);
            fs.Flush();
            return true;
        }
        catch { return false; }
    }
    //----< close file >----------------
    public bool CloseFile()
    {
        try
        {
            fs.Close();
            Console.Write("\n  {0} closed", fileSpec);
            return true;
        }
        catch { return false; }
    }
  }
  //----< create ServiceHost and return to caller >------------------

  public class Host
  {
    public static ServiceHost CreateChannel(string url)
    {
      WSHttpBinding binding = new WSHttpBinding();
      Uri address = new Uri(url);
      Type service = typeof(CommService);
      ServiceHost host = new ServiceHost(service, address);
      host.AddServiceEndpoint(typeof(ICommService), binding, address);
      return host;
    }
  }
}
