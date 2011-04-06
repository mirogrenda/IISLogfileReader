using System;
using System.IO;
using System.Text;

namespace Akentas.IisLogfileReader.W3C
{
	#region // #### Enums
	public enum LineType
	{
		Header,
		Data,
		Ignore,
		Error
	}
	#endregion

	public class Logfile
	{
		#region // #### MG: Fields
		private string filepath;
		private bool endOfFile;
		private LineType linetype;
		private StreamReader sr;
		private CustomTypes.Field[] field;
		private string lastLine;
		private string lastError;
		private int totalLines;
		private int totalErrors;
		#endregion

		#region // #### MG: Constructors
		public Logfile(string filepath)
		{
			this.filepath = filepath;
			this.endOfFile = true;
		}
		#endregion

		#region // #### MG: Properties
		public string Filepath
		{
			get
			{
				return this.filepath;
			}
		}

		public bool EndOfFile
		{
			get
			{
				return this.endOfFile;
			}
		}

		public LineType LineType
		{
			get
			{
				return this.linetype;
			}
		}

		public CustomTypes.Field[] Field
		{
			get
			{
				return this.field;
			}
		}

		public string LastLine
		{
			get
			{
				return this.lastLine;
			}
		}

		public string LastError
		{
			get
			{
				return this.lastError;
			}
		}
		
		public int TotalLines
		{
			get
			{
				return this.totalLines;
			}
		}

		public int TotalErrors
		{
			get
			{
				return this.totalErrors;
			}
		}
		#endregion

		#region // #### MG: Methods
		public void Open()
		{
			this.sr = new StreamReader(this.filepath, Encoding.Default);
			this.endOfFile = false;
		}

		public void Read()
		{
			this.lastLine = this.sr.ReadLine();
			char[] separator = " ".ToCharArray();
			this.endOfFile = (this.sr.Peek() == -1);
			this.totalLines++;

			if(this.lastLine.StartsWith("#")) // #### MG: Line defines something
			{
				if(this.lastLine.StartsWith("#Fields:"))
				{
					this.linetype = LineType.Header;
					string[] values = this.lastLine.Remove(0, 9).TrimEnd(separator).Split(separator);
					this.field = new CustomTypes.Field[values.Length];

					for(int i = 0; i < values.Length; i++)
					{
						this.field[i].Name = values[i];
					}
				}
				else
				{
					this.linetype = LineType.Ignore;
					return;
				}
			}
			else
			{
				if(this.field == null)
				{
					this.linetype = LineType.Error;
					this.lastError = string.Format("Fields definitions missing, while reading file {0} in line '{1}'", this.filepath, this.lastLine);
					this.totalErrors++;
					return;
				}
				
				string[] values = this.lastLine.Split(separator);

				if(values.Length == this.field.Length)
				{
					this.linetype = LineType.Data;
					
					for(int i = 0; i < values.Length; i++)
					{
						this.field[i].Value = values[i];
					}	
				}
				else
				{
					this.linetype = LineType.Error;
					this.lastError = string.Format("Fields in definition and fields in line don't match {0}:{1}, while reading file '{2}' in line '{3}'", this.field.Length, values.Length, this.filepath, this.lastLine);
					this.totalErrors++;
					return;					
				}				
			}
		}

		public void Close()
		{
			this.sr.Close();
		}
		#endregion
	}
}