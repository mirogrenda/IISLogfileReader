<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<script runat="server">
    void Page_Load(object sender, EventArgs e)
    {
        // #### MG: Read the logfiles on page load
        ReadLogfiles();
    }

    void ReadLogfiles()
    {
        try
        {
            // #### MG: Tell LogfileReader to process all logfiles with relevant date/filename
            Akentas.IisLogfileReader.W3C.Logfiles logfiles = new Akentas.IisLogfileReader.W3C.Logfiles(Server.MapPath("~/Logfiles"), new DateTime(2009, 4, 8), new DateTime(2009, 4, 12));

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
                    this.LiteralOutput.Text += string.Format("<li>Logfile <strong>'{0}'</strong>:<ol>", System.IO.Path.GetFileName(logfiles.Logfile.Filepath));

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
                                this.LiteralOutput.Text += string.Format("<li>Line <strong>#{0}</strong><ol>", logfiles.Logfile.TotalLines);
                                
                                foreach (Akentas.IisLogfileReader.CustomTypes.Field field in fields)
                                {
                                    this.LiteralOutput.Text += string.Format("<li><strong>{0}:</strong> {1}</li>", field.Name, HttpUtility.HtmlEncode(field.Value));
                                }

                                this.LiteralOutput.Text += "</ol></li>";
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
                            this.LiteralOutput.Text += string.Format("<li class=\"Error\">Line <strong>#{0}</strong> - !ERROR!<ol><li>{1}</li></ol></li>", logfiles.Logfile.TotalLines, HttpUtility.HtmlEncode(logfiles.Logfile.LastError));
                        }
                    }

                    // #### MG: Close the logfile
                    logfiles.Logfile.Close();

                    // #### MG: Close HTML Output
                    this.LiteralOutput.Text += "</ol></li>";
                }
                else if (logfiles.FileType == Akentas.IisLogfileReader.W3C.FileType.Error)
                {
                    this.LiteralOutput.Text += string.Format("<li class=\"Error\">Logfile <strong>'{0}'</strong> - !ERROR!<ol><li>{1}</li></ol></li>", System.IO.Path.GetFileName(logfiles.LastFilepath), HttpUtility.HtmlEncode(logfiles.LastError));
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }        
    }
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Akentas IIS Logfile Reader Sample</title>
    <link rel="Stylesheet" type="text/css" href="Default.aspx.css" />
</head>
<body>
    <form id="form1" runat="server">
        <img id="Logo" runat="server" src="~/Logo.png" alt="Akentas Logo" />
        <h1>Akentas IIS Logfile Reader Sample</h1>
        <fieldset id="Output">
            <legend>Output</legend>
            <ol id="OutputList">
                <asp:Literal ID="LiteralOutput" runat="server" />
            </ol>
        </fieldset>
    </form>
</body>
</html>