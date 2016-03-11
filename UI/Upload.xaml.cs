///////////////////////////////////////////////////////////////////////////////
// Upload.xaml.cs - Document Vault client                                    //
//                                                                           //
// Yufan Gong, CSE681 - Software Modeling and Analysis, Fall 2013            //
//                                                                           //
// Nov 19, 2013                                                              //
///////////////////////////////////////////////////////////////////////////////
/*
 *  Package: Client
 *  ===============
 *  
 *  Class: Upload
 *  =============
 *  
 *  upload view to transfer file from local to server, which require a associate 
 *  metadata file as well.
 *  if user enter this view by click edit metadata button, the text file is already
 *  in the server repository, and the xml file will given by client, user can only 
 *  upload this xml file.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Windows.Forms;
using System.Windows.Threading;

namespace DocumentVault
{
    public partial class Upload : Window
    {
        MainWindow main;
        public Dispatcher dispatcher;
        string filename = null;
        string xmlname = null;
        Upload upload;
        OnlyTextFile otf = new OnlyTextFile();


        public Upload(MainWindow reference)
        {
            InitializeComponent();
            dispatcher = Dispatcher.CurrentDispatcher;
            main = reference;
            upload = this;
        }
        //----< browse local file set and choose one to upload>----------------
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            full_file_name.Text = null;
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                filename = dlg.FileName;
                string extension = Path.GetExtension((filename));
                if (otf.textFile(extension))
                {
                    full_file_name.Text = filename;
                    int pos = filename.LastIndexOf("\\");
                    string shortfilename = filename.Remove(0, pos + 1);
                    Metadata_name.Text = shortfilename + ".xml";
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Choose a text file");
                }
            }
        }
        //----< upload text file local with metadata or metadata edited from server>----------------
        private void upload_file_Click(object sender, RoutedEventArgs e)
        {
            if (xmlname == null)
            {
                System.Windows.Forms.MessageBox.Show("Must upload text file with metadatafile");
            }
            else
            {
                TransferFile tf = new TransferFile(main);
                dispatcher.Invoke(
                     new Action<string>(tf.UploadToServer),
                     System.Windows.Threading.DispatcherPriority.Background,
                     new string[] { filename }
                     );
                dispatcher.Invoke(
                     new Action<string>(tf.UploadToServer),
                     System.Windows.Threading.DispatcherPriority.Background,
                     new string[] { xmlname }
                     );
            }
        }
        //----< provide categories already in repository>----------------
        private void Upload_Cate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Categories.Text += Upload_Cate.SelectedItem + " ";
        }
        //----< provide files already in repository, user can choose them as children>----------------
        private void Upload_Child_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int pos = Upload_Child.SelectedItem.ToString().LastIndexOf("\\");
            string child = Upload_Child.SelectedItem.ToString().Remove(0, pos + 1);
            int pos2 = child.LastIndexOf(".");
            child = child.Substring(0, pos2);
            Children.Text += child + " ";
        }
        //----< create matadata file >----------------
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            MetadataTool mt = new MetadataTool(upload);
            if (filename == null)
            {
                filename = Metadata_name.Text;
            }
            mt.xmlCreation(filename);
            xmlname = mt.getXml();
        }

    }
}
