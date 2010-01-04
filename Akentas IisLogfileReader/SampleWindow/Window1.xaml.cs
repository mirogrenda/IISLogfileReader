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
using System.Windows.Shapes;
using System.Diagnostics;

namespace SampleWindow
{
    public partial class Window1 : Window
    {
        #region // #### MG: Constructor
        public Window1()
        {
            InitializeComponent();

            // #### MG: Read the logfiles
            ReadLogfiles();
        }
        #endregion

        #region // #### MG: Methods
        void ReadLogfiles()
        {
            try
            {
                // #### MG: Get the logfiles folder
                string logfilesFolder = string.Format("{0}{1}..{1}..{1}Logfiles", System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), System.IO.Path.DirectorySeparatorChar);

                if (!System.IO.Directory.Exists(logfilesFolder))
                {
                    this.richTextBox1.AppendText(string.Format("ERROR - Sorry, logfiles missing in {0}", logfilesFolder));

                    logfilesFolder = string.Format("{0}{1}Logfiles", System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), System.IO.Path.DirectorySeparatorChar);

                    if (!System.IO.Directory.Exists(logfilesFolder))
                    {
                        this.richTextBox1.AppendText(string.Format("ERROR - Sorry, logfiles missing in {0}", logfilesFolder));
                        return;
                    }
                }

                // #### MG: Tell LogfileReader to process all logfiles with relevant date/filename
                Akentas.IisLogfileReader.W3C.Logfiles logfiles = new Akentas.IisLogfileReader.W3C.Logfiles(logfilesFolder, new DateTime(2009, 4, 8), new DateTime(2009, 4, 12));

                // #### MG: Start processing logfiles
                while (!logfiles.EndOfFiles)
                {
                    // #### MG: Get next logfile
                    logfiles.Read();

                    // #### MG: If logfile is of type Data then go on
                    if (logfiles.FileType == Akentas.IisLogfileReader.W3C.FileType.Data)
                    {
                        // #### MG: Open the logfile to read data
                        logfiles.Logfile.Open();

                        // #### MG: Write some logfile info to output
                        this.richTextBox1.AppendText(string.Format("#### Logfile '{0}':{1}", System.IO.Path.GetFileName(logfiles.Logfile.Filepath), Environment.NewLine));

                        // #### MG: Start processing the logfile
                        while (!logfiles.Logfile.EndOfFile)
                        {
                            // #### MG: Get next line
                            logfiles.Logfile.Read();

                            // #### MG: If line contains data then get the field names and values
                            if (logfiles.Logfile.LineType == Akentas.IisLogfileReader.W3C.LineType.Data)
                            {
                                // #### MG: Fill the fields
                                Akentas.IisLogfileReader.CustomTypes.Field[] fields = logfiles.Logfile.Field;

                                // #### MG: Write all fields (name + value) to output
                                if (fields != null)
                                {
                                    this.richTextBox1.AppendText(string.Format(" - Line #{0}{1}", logfiles.Logfile.TotalLines, Environment.NewLine));

                                    foreach (Akentas.IisLogfileReader.CustomTypes.Field field in fields)
                                    {
                                        this.richTextBox1.AppendText(string.Format("    - {0}: {1}{2}", field.Name, field.Value, Environment.NewLine));
                                    }
                                }
                            }
                            else if (logfiles.Logfile.LineType == Akentas.IisLogfileReader.W3C.LineType.Ignore)
                            {
                                // #### MG: This type of line contains no relevant data for us
                            }
                            else if (logfiles.Logfile.LineType == Akentas.IisLogfileReader.W3C.LineType.Header)
                            {
                                // #### MG: This type of line contains important data which is used internal by IisLogfileReader
                            }
                            else if (logfiles.Logfile.LineType == Akentas.IisLogfileReader.W3C.LineType.Error)
                            {
                                // #### MG: Write error to output
                                this.richTextBox1.AppendText(string.Format(" - Line #{0} - !ERROR!: {1}{2}", logfiles.Logfile.TotalLines, logfiles.Logfile.LastError, Environment.NewLine));
                            }
                        }

                        // #### MG: Close the logfile
                        logfiles.Logfile.Close();
                    }
                    else if (logfiles.FileType == Akentas.IisLogfileReader.W3C.FileType.Error)
                    {
                        this.richTextBox1.AppendText(string.Format("#### Logfile '{0}' - !ERROR!: {1}{2}", System.IO.Path.GetFileName(logfiles.LastFilepath), logfiles.LastError, Environment.NewLine));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
