///////////////////////////////////////////////////////////////////////////////
// Server.cs - Document Vault Server prototype                               //
//                                                                           //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2013           //
//                                                                           //
// Changed by Yufan Gong                                                     //
// Nov 19, 2013                                                              //
// project 4                                                                 //
///////////////////////////////////////////////////////////////////////////////
/*
 *  Package Contents:
 *  -----------------
 *  This package defines four classes:
 *  Server:
 *    Provides prototype behavior for the DocumentVault server.
 *  EchoCommunicator:
 *    Simply diplays its messages on the server Console.
 *  QueryCommunicator:
 *    Serves as a placeholder for query processing.  You should be able to
 *    invoke your Project #2 query processing from the ProcessMessages function.
 *  NavigationCommunicator:
 *    Serves as a placeholder for navigation processing.  You should be able to
 *    invoke your navigation processing from the ProcessMessages function.
 * 
 *  Required Files:
 *  - Server:      Server.cs, Sender.cs, Receiver.cs
 *  - Components:  ICommLib, AbstractCommunicator, BlockingQueue
 *  - CommService: ICommService, CommService
 *
 *  Required References:
 *  - System.ServiceModel
 *  - System.RuntimeSerialization
 *
 *  Build Command:  devenv Project4HelpF13.sln /rebuild debug
 *
 *  Maintenace History:
 *  ver 2.2 : Nov 19, 2013
 *  - Change the messages for DocumentVault
 *  ver 2.1 : Nov 7, 2013
 *  - replaced ServerSender with a merged Sender class
 *  ver 2.0 : Nov 5, 2013
 *  - fixed bugs in the message routing process
 *  ver 1.0 : Oct 29, 2013
 *  - first release
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
    // Echo Communicator

    class EchoCommunicator : AbstractCommunicator
    {
        string ClientURL = null;
        public EchoCommunicator()
        {
        }

        //----< Get the client url send from client-side >----------------
        public string GetClientUrl()
        {
            return ClientURL;
        }

        //----< get and process message from client>----------------
        protected override void ProcessMessages()
        {
            while (true)
            {
                ShowFiles sf = new ShowFiles();
                ServiceMessage msg = bq.deQ();
                Console.Write("\n  {0} Recieved Message:\n", msg.TargetCommunicator);
                Console.Write("\n  Echo processing completed\n");
                if (msg.Contents == "connect to server")
                {
                    ClientURL = msg.ResourceName;
                }
                ////////////////////////////////////////////////////////////////////////////////////////////
                // client ask for category root in repositpry, find categories repository files belong to 
                // and send the msg back to client, msg contain categories
                if (msg.ResourceName == "category")
                {
                    List<string> Categories = new List<string>();
                    Categories = sf.FindCategories();
                    foreach (string cate in Categories)
                    {
                        ServiceMessage reply = ServiceMessage.MakeMessage("client-echo", "ServiceServer", cate, "Categories");
                        reply.TargetUrl = msg.SourceUrl;
                        reply.SourceUrl = msg.TargetUrl;
                        AbstractMessageDispatcher dispatcher = AbstractMessageDispatcher.GetInstance();
                        dispatcher.PostMessage(reply);
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                // client require for files in the specified category
                // server find files and send back
                if (msg.ResourceName == "files in this category")
                {
                    List<string> filesInCate = new List<string>();
                    filesInCate = sf.FindFiles(msg.Contents);
                    foreach (string file in filesInCate)
                    {
                        ServiceMessage reply = ServiceMessage.MakeMessage("client-echo", "ServiceServer", file, "files found in category");
                        reply.TargetUrl = msg.SourceUrl;
                        reply.SourceUrl = msg.TargetUrl;
                        AbstractMessageDispatcher dispatcher = AbstractMessageDispatcher.GetInstance();
                        dispatcher.PostMessage(reply);
                    }
                }
                ///////////////////////////////////////////////////////////////////////////////////////////
                //send the xml and text file content back
                if (msg.ResourceName == "extract file")
                {
                    ExtractFile ef = new ExtractFile();
                    string textfile = ef.TextFile(msg.Contents);
                    string xmlfile = ef.XmlFile(msg.Contents);

                    ServiceMessage reply1 = ServiceMessage.MakeMessage("client-echo", "ServiceServer", textfile, "textfile");
                    reply1.TargetUrl = msg.SourceUrl;
                    reply1.SourceUrl = msg.TargetUrl;
                    AbstractMessageDispatcher dispatcher1 = AbstractMessageDispatcher.GetInstance();
                    dispatcher1.PostMessage(reply1);

                    ServiceMessage reply2 = ServiceMessage.MakeMessage("client-echo", "ServiceServer", xmlfile, "xmlfile");
                    reply2.TargetUrl = msg.SourceUrl;
                    reply2.SourceUrl = msg.TargetUrl;
                    AbstractMessageDispatcher dispatcher2 = AbstractMessageDispatcher.GetInstance();
                    dispatcher2.PostMessage(reply2);
                }
                if (msg.Contents == "quit")
                    break;
            }
        }
    }
    // Query Communicator

    class QueryCommunicator : AbstractCommunicator
    {
        //----< get and process message from client>----------------
        // find text files and xml files which contain tags or strings
        // send xml file content back
        protected override void ProcessMessages()
        {
            while (true)
            {
                List<string> foundFiles = new List<string>();
                ServiceMessage msg = bq.deQ();
                Console.Write("\n  {0} Recieved Message:\n", msg.TargetCommunicator);
                QueryProcessing qpro = new QueryProcessing();
                foundFiles = qpro.queryProcessing(msg);

                //Console.Write("\n  Query processing is an exercise for students\n");
                if (msg.Contents == "quit")
                    break;
                foreach (string file in foundFiles)
                {
                    XDocument doc = XDocument.Load(file);

                    string fileContent = doc.ToString();

                    ServiceMessage reply = ServiceMessage.MakeMessage("client-query", "ServiceServer", fileContent);
                    reply.TargetUrl = msg.SourceUrl;
                    reply.SourceUrl = msg.TargetUrl;
                    AbstractMessageDispatcher dispatcher = AbstractMessageDispatcher.GetInstance();
                    dispatcher.PostMessage(reply);
                }
            }
        }
    }

    // Navigate Communicator

    class NavigationCommunicator : AbstractCommunicator
    {
        //----< get and process message from client>----------------
        protected override void ProcessMessages()
        {
            MappingTool mt = new MappingTool();
            while (true)
            {
                ServiceMessage msg = bq.deQ();
                Console.Write("\n  {0} Recieved Message:\n", msg.TargetCommunicator);
                //msg.ShowMessage();
                //Console.Write("\n  Navigation processing is an exercise for students\n");
                if (msg.Contents == "quit")
                    break;
                ///////////////////////////////////////////////////////////////////////////////////////
                // find files relationship and construct parents and child map when client
                // connect with server
                if (msg.Contents == "mapping")
                {
                    mt.Mapping();
                    ServiceMessage reply1 = ServiceMessage.MakeMessage("client-nav", "nav", "mapping finished", "mapping finished");
                    reply1.TargetUrl = msg.SourceUrl;
                    reply1.SourceUrl = msg.TargetUrl;
                    AbstractMessageDispatcher dispatcher = AbstractMessageDispatcher.GetInstance();
                    dispatcher.PostMessage(reply1);
                }
                //////////////////////////////////////////////////////////////////////////////////////
                // client send current file name to server, server find the parents and children 
                // then reply
                if (msg.ResourceName == "file relationship")
                {
                    string parents = null;
                    parents = mt.findParents(msg.Contents);
                    ServiceMessage reply2 = ServiceMessage.MakeMessage("client-nav", "nav", parents, "found parents");
                    reply2.TargetUrl = msg.SourceUrl;
                    reply2.SourceUrl = msg.TargetUrl;
                    AbstractMessageDispatcher dispatcher = AbstractMessageDispatcher.GetInstance();
                    dispatcher.PostMessage(reply2);
                }
            }
        }
    }
    // Server

    class Server
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Starting CommService");
            Console.Write("\n ======================\n");

            string ServerUrl = "http://localhost:8000/CommService";
            Receiver receiver = new Receiver(ServerUrl);

            EchoCommunicator echo = new EchoCommunicator();
            echo.Name = "echo";
            receiver.Register(echo);
            echo.Start();

            string ClientUrl = echo.GetClientUrl();
            Sender sender = new Sender();
            sender.Name = "sender";
            sender.Connect(ClientUrl);
            receiver.Register(sender);
            sender.Start();
            

            QueryCommunicator query = new QueryCommunicator();
            query.Name = "query";
            receiver.Register(query);
            query.Start();

            // parent/child relationships

            NavigationCommunicator nav = new NavigationCommunicator();
            nav.Name = "nav";
            receiver.Register(nav);
            nav.Start();

            Console.Write("\n  Started CommService - Press key to exit:\n ");
            Console.ReadKey();
        }
    }
}
