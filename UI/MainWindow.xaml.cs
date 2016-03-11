///////////////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - Document Vault client                                //
//                                                                           //
// Yufan Gong, CSE681 - Software Modeling and Analysis, Fall 2013            //
//                                                                           //
// Nov 19, 2013                                                              //
///////////////////////////////////////////////////////////////////////////////
/*
 *  Package Contents:
 *  -----------------
 *  This package defines two classes:
 *  Mainwindow
 *    Defines the behavior of a DocumentVault client.
 *  EchoCommunicator
 *    Defines prototype behavior for processing client received messages.
 *    In this demo it simply displays the message contents on the Console.
 *  
 * Required Files:
 * - Client:      Client.cs, Sender.cs, Receiver.cs
 * - Components:  ICommLib, AbstractCommunicator, BlockingQueue
 * - CommService: ICommService, CommService
 *
 *  Required References:
 *  - System.ServiceModel
 *  - System.RuntimeSerialization
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Linq;

namespace DocumentVault
{
    //echo communicator
    class EchoCommunicator : AbstractCommunicator
    {
        MainWindow mainWin;
        public EchoCommunicator(MainWindow reference)
        {
            mainWin = reference;
        }
        //----< get and process message from server>----------------
        protected override void ProcessMessages()
        {
            while (true)
            {
                ServiceMessage msg = bq.deQ();
                //////////////////////////////////////////////////////////////////////////////////
                // receive message which contains category information
                // then show in UI
                if (msg.ResourceName == "Categories")
                {
                    mainWin.dispatcher.Invoke(
                       new Action<string>(mainWin.AddCategory),
                       System.Windows.Threading.DispatcherPriority.Background,
                       new string[] { msg.Contents }
                       );
                    string message = "Connection successful!";
                    mainWin.dispatcher.Invoke(
                               new Action<string>(mainWin.connected),
                               System.Windows.Threading.DispatcherPriority.Background,
                               new string[] { message }
                               );
                }
                ///////////////////////////////////////////////////////////////////////////////////
                // receive message which contains files information in specified category
                // then show in UI
                if (msg.ResourceName == "files found in category")
                {
                    mainWin.dispatcher.Invoke(
                      new Action<string>(mainWin.AddFile),
                      System.Windows.Threading.DispatcherPriority.Background,
                      new string[] { msg.Contents }
                      );

                }
                ////////////////////////////////////////////////////////////////////////////////////
                // receive message which contains textfile content
                // then show in UI
                if (msg.ResourceName == "textfile")
                {
                    mainWin.dispatcher.Invoke(
                        new Action<string>(mainWin.showText),
                        System.Windows.Threading.DispatcherPriority.Background,
                        new string[] { msg.Contents }
                        );
                }
                ////////////////////////////////////////////////////////////////////////////////////
                // receive message which contains xmlfile content
                // then show in UI
                if (msg.ResourceName == "xmlfile")
                {
                    mainWin.dispatcher.Invoke(
                        new Action<string>(mainWin.showXml),
                        System.Windows.Threading.DispatcherPriority.Background,
                        new string[] { msg.Contents }
                        );
                    mainWin.dispatcher.Invoke(
                       new Action<string>(mainWin.showChildren),
                       System.Windows.Threading.DispatcherPriority.Background,
                       new string[] { msg.Contents }
                       );

                }
                ////////////////////////////////////////////////////////////////////////////////////
                if (msg.Contents == "quit")
                {
                    msg.TargetCommunicator = "dispatcher";
                    AbstractMessageDispatcher.GetInstance().PostMessage(msg);
                    break;
                }
                ////////////////////////////////////////////////////////////////////////////////////
            }
        }
    }
    class QueryCommunicator : AbstractCommunicator
    {

        MainWindow mainWin;
        public QueryCommunicator(MainWindow reference)
        {
            mainWin = reference;
        }
        //----< get and process message from server>----------------
        protected override void ProcessMessages()
        {
            while (true)
            {
                ServiceMessage msg = bq.deQ();
                /////////////////////////////////////////////////////////////////
                // receive query results and show in UI
                mainWin.dispatcher.Invoke(
                   new Action<string>(mainWin.AddQueryResult),
                   System.Windows.Threading.DispatcherPriority.Background,
                   new string[] { msg.Contents }
                 );

                if (msg.Contents == "quit")
                {
                    msg.TargetCommunicator = "dispatcher";
                    AbstractMessageDispatcher.GetInstance().PostMessage(msg);
                    break;
                }
            }
        }
    }
    class NaviCommnuicator : AbstractCommunicator
    {

        MainWindow mainWin;
        public NaviCommnuicator(MainWindow reference)
        {
            mainWin = reference;
        }
        //----< get and process message from Server>----------------
        protected override void ProcessMessages()
        {
            while (true)
            {
                ServiceMessage msg = bq.deQ();

                if (msg.Contents == "quit")
                {
                    msg.TargetCommunicator = "dispatcher";
                    AbstractMessageDispatcher.GetInstance().PostMessage(msg);
                    break;
                }
                //////////////////////////////////////////////////////////////
                // show message that server finished the mapping process
                if (msg.ResourceName == "mapping finished")
                {
                    MessageBox.Show("Mapping fishined!");
                }
                //////////////////////////////////////////////////////////////
                // receive message that contains the parents of current file
                if (msg.ResourceName == "found parents")
                {
                    mainWin.dispatcher.Invoke(
                        new Action<string>(mainWin.showParents),
                        System.Windows.Threading.DispatcherPriority.Background,
                        new string[] { msg.Contents }
                        );
                }
            }
        }
    }
    // MainWindow
    public partial class MainWindow : Window
    {
        public string ServerUrl;
        public string ClientUrl;
        public Sender sender;
        public Receiver receiver;

        MainWindow reference;
        public Dispatcher dispatcher;
        List<string> CateList = new List<string>();
        List<string> fileList = new List<string>();
        public delegate void invoker();

        public MainWindow()
        {
            InitializeComponent();
            reference = this;
            dispatcher = Dispatcher.CurrentDispatcher;
        }
        //----< register with Server>----------------
        public void ClientStart()
        {
            ServerUrl = "http://localhost:8000/CommService";
            sender = null;
            sender = new Sender();
            sender.Connect(ServerUrl);
            sender.Start();

            reference.dispatcher.BeginInvoke(new invoker(GetClientUrl),
                System.Windows.Threading.DispatcherPriority.Send);
            receiver = new Receiver(ClientUrl);

            EchoCommunicator echo = new EchoCommunicator(reference);
            echo.Name = "client-echo";
            receiver.Register(echo);
            echo.Start();

            NaviCommnuicator nav = new NaviCommnuicator(reference);
            nav.Name = "client-nav";
            receiver.Register(nav);
            nav.Start();
            ServiceMessage msg =
              ServiceMessage.MakeMessage("echo", "ServiceClient", "connect to server", ClientUrl);
            msg.SourceUrl = ClientUrl;
            msg.TargetUrl = ServerUrl;
            sender.PostMessage(msg);
        }
        //----< get categories and files in Server>----------------
        public void GetContent()
        {

            ServiceMessage msg1 =
              ServiceMessage.MakeMessage("echo", "ServiceClient", "get category", "category");
            msg1.SourceUrl = ClientUrl;
            msg1.TargetUrl = ServerUrl;
            sender.PostMessage(msg1);

            ServiceMessage msg2 =
              ServiceMessage.MakeMessage("nav", "ServiceClient", "mapping", "get pc relationship");
            msg2.SourceUrl = ClientUrl;
            msg2.TargetUrl = ServerUrl;
            sender.PostMessage(msg2);

        }
        //----< get client url>----------------
        public void GetClientUrl()
        {
            ClientUrl = clienturl.Text;
        }
        //----< show process msg in UI >----------------
        public void connected(string message)
        {
            reference.ShowMsg.Text = message;
        }
        //----< add categories in UI >----------------
        public void AddCategory(string catename)
        {
            Cate.Items.Add(catename);
            CateList.Add(catename);
        }
        //----< add files in each category in UI >----------------
        public void AddFile(string filename)
        {
            Files.Items.Add(filename);
            fileList.Add(filename);
            int pos = filename.LastIndexOf(".");
            string textname = filename.Substring(0, pos);
            Files.Items.Add(textname);
        }
        //----< show text file content in file information view >----------------
        public void showText(string contents)
        {
            Text_File.Text = contents;
        }
        //----< show xml file content in file information view >----------------
        public void showXml(string contents)
        {
            Metadata_FIle.Text = contents;
        }
        //----< show parent files in file navigation view >----------------
        public void showParents(string parentsInDic)
        {
            if (parentsInDic == null)
            {
                ParentsList.Items.Add(Cate.SelectedItem);
            }
            else
            {
                string[] parents = null;
                char[] separator = { '|' };
                parents = parentsInDic.Split(separator);
                foreach (string parent in parents)
                {
                    ParentsList.Items.Add(parent);
                }
            }
        }
        //----< show children files in file navigation view >----------------
        public void showChildren(string xmlfile)
        {
            if (xmlfile == null)
            {
                ChildrenList.Items.Add("null");
            }
            else
            {
                XDocument doc = XDocument.Parse(xmlfile);

                var q = from e in
                            doc.Elements("xmlfile").Descendants()
                        select e;
                for (int i = 0; i < q.Count(); i++)
                {
                    if (q.ElementAt(i).Name == "child")
                    {
                        ChildrenList.Items.Add(q.ElementAt(i).Value);
                    }
                }
            }
        }
        //----< register button >----------------
        private void Connect_Click(object _sender, RoutedEventArgs e)
        {

            Cate.Items.Clear();
            try
            {
                Thread t1 = new Thread(ClientStart);
                t1.Start();
            }
            catch
            {
                ShowMsg.Text = "Connect failed.";
            }
            finally
            {
                ShowMsg.Text = "Waiting for register. Then click Connect button.";
            }
        }
        //----< refresh button >----------------
        private void Refresh_Click(object sender_, RoutedEventArgs e)
        {
            ServiceMessage msg3 =
              ServiceMessage.MakeMessage("nav", "ServiceClient", "mapping", "refresh pc relationship");
            msg3.SourceUrl = ClientUrl;
            msg3.TargetUrl = ServerUrl;

            sender.PostMessage(msg3);
        }
        //----< upload button to open upload view >----------------
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            if (ServerUrl == null)
            {
                MessageBox.Show("Please connect with Server first!");
            }
            else
            {
                Upload up = new Upload(reference);
                up.Show();
                foreach (string cate in CateList)
                {
                    up.Upload_Cate.Items.Add(cate);
                }
                foreach (string file in fileList)
                {
                    up.Upload_Child.Items.Add(file);
                }
            }
        }
        //----< show query results in query view >----------------
        public void AddQueryResult(string fileContent)
        {
            string cat = null;
            XDocument doc = XDocument.Parse(fileContent);

            var q1 = from e in
                         doc.Elements("xmlfile").Elements("category")
                     select e;
            foreach (var elem in q1)
            {
                cat += elem.Value.ToString() + " ";
            }
            Categories1.Items.Add(cat.ToString());


            var q2 = from e in
                         doc.Elements("xmlfile").Elements("name")
                     select e;

            string filename = "no file";
            foreach (var elem in q2)
            {
                filename = elem.Value.ToString();
            }
            Result.Items.Add(filename);

            if (Metadata.IsChecked == true)
            {
                string elementInfo = "";
                char[] separator = { ' ' };
                string[] tags = QueryContent.Text.Split(separator);
                foreach (string tagname in tags)
                {
                    var q3 = from e in
                                 doc.Elements("xmlfile").Descendants()
                             select e;
                    
                    for (int i = 0; i < q3.Count(); i++)
                    {
                        if (q3.ElementAt(i).Name == tagname)
                        {
                            elementInfo += tagname + ":  " + q3.ElementAt(i).Value + "  ";
                        }
                    }
                }
                show_elements.Items.Add(elementInfo);
            }
        }
        //----< query button to send query message with query requirement >----------------
        private void Query1_Click(object sender_, RoutedEventArgs e)
        {
            if (ServerUrl == null)
            {
                MessageBox.Show("Please register with Server first!");
            }
            else
            {
                QueryCommunicator query = new QueryCommunicator(reference);
                query.Name = "client-query";
                receiver.Register(query);
                query.Start();

                Result.Items.Clear();
                Categories1.Items.Clear();
                show_elements.Items.Clear();
                string queryContents = Categories2.Text + "&" + QueryContent.Text;
                ServiceMessage msg2 = null;

                if (Text.IsChecked == true && all_strings.IsChecked == true)
                {
                    msg2 = ServiceMessage.MakeMessage("query", "ServiceClient", queryContents, "textqueryA");
                    msg2.SourceUrl = ClientUrl;
                    msg2.TargetUrl = ServerUrl;
                    sender.PostMessage(msg2);
                }
                else if (Text.IsChecked == true && all_strings.IsChecked != true)
                {
                    msg2 = ServiceMessage.MakeMessage("query", "ServiceClient", queryContents, "textqueryO");
                    msg2.SourceUrl = ClientUrl;
                    msg2.TargetUrl = ServerUrl;
                    sender.PostMessage(msg2);
                }
                else if (Metadata.IsChecked == true)
                {
                    msg2 = ServiceMessage.MakeMessage("query", "ServiceClient", queryContents, "metadataquery");
                    msg2.SourceUrl = ClientUrl;
                    msg2.TargetUrl = ServerUrl;
                    sender.PostMessage(msg2);
                }
                else
                {
                    MessageBox.Show("Choose file pattern!");

                }
            }
        }
        //----< edit metadata button to open upload view >----------------
        private void Edit_Metadata_Click(object sender, RoutedEventArgs e)
        {
            if (Files.SelectedItem == null)
            {
                MessageBox.Show("Choose a metadata file to edit!");
            }
            else
            {
                Upload up = new Upload(reference);
                up.Show();
                string editMetadata = null;
                string targetxml = Files.SelectedItem.ToString();
                if (!Files.SelectedItem.ToString().Contains(".xml"))
                {
                    editMetadata = Files.SelectedItem.ToString() + ".xml";
                }
                else
                {
                    editMetadata = Files.SelectedItem.ToString();
                }
                int pos = editMetadata.LastIndexOf("\\");
                editMetadata = editMetadata.Remove(0, pos + 1);
                up.Metadata_name.Text = editMetadata;
                up.targetxml.Text = targetxml;
            }
        }
        //----< diconnect button >----------------
        private void Disconnect_Click(object sender_, RoutedEventArgs e)
        {
            sender.Stop();
            receiver.Close();
            ShowMsg.Text = "Disconnect with Server!";
        }
        //----< show files in specified category when category change >----------------
        private void Cate_SelectionChanged(object sender_, SelectionChangedEventArgs e)
        {
            Files.Items.Clear();
            string CateName = Cate.SelectedItem.ToString();
            ServiceMessage msg2 =
              ServiceMessage.MakeMessage("echo", "ServiceClient", CateName, "files in this category");
            msg2.SourceUrl = ClientUrl;
            msg2.TargetUrl = ServerUrl;
            sender.PostMessage(msg2);
        }
        //----< change current file when click >----------------
        private void Files_SelectionChanged(object sender_, SelectionChangedEventArgs e)
        {
            if (Files.SelectedItem == null)
            {
                Thread.Sleep(500);
            }
            else
            {
                SetCurrentFile(Files.SelectedItem.ToString());
            }
        }
        //----< set current file and show its information and relationship in each view >----------------
        public void SetCurrentFile(string filename)
        {
            Text_File.Text = null;
            Metadata_FIle.Text = null;
            ParentsList.Items.Clear();
            ChildrenList.Items.Clear();

            if (!filename.Contains(".xml"))
            {
                filename = filename + ".xml";
            }
            ServiceMessage msg3 =
              ServiceMessage.MakeMessage("echo", "ServiceClient", filename, "extract file");
            msg3.SourceUrl = ClientUrl;
            msg3.TargetUrl = ServerUrl;
            sender.PostMessage(msg3);

            ServiceMessage msg4 =
                ServiceMessage.MakeMessage("nav", "ServiceClient", filename, "file relationship");
            msg4.SourceUrl = ClientUrl;
            msg4.TargetUrl = ServerUrl;
            sender.PostMessage(msg4);
        }
        //----< change current file when click >----------------
        private void ParentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ParentsList.SelectedItem == null)
            {
                Thread.Sleep(500);
            }
            else
            {
                string xmlname = ParentsList.SelectedItem.ToString() + ".xml";
                SetCurrentFile(xmlname);
            }

        }
        //----< change current file when click >----------------
        private void ChildrenList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChildrenList.SelectedItem == null)
            {
                Thread.Sleep(500);
            }
            else
            {
                string xmlname = ChildrenList.SelectedItem.ToString() + ".xml";
                SetCurrentFile(xmlname);
            }
        }
        //----< connect button: get category information and ask server to construct relationship >----------------
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ServerUrl == null)
            {
                MessageBox.Show("Please register with Server first!");
            }
            else
            {
                GetContent();
            }
        }
        //----< change current file when click >----------------
        private void Result_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (Result.SelectedItem == null)
            {
                Thread.Sleep(500);
            }
            else
            {
                string xmlname = Result.SelectedItem.ToString() + ".xml";
                SetCurrentFile(xmlname);
            }
        }
    }
}
