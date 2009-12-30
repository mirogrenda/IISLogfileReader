using System;
using System.IO;

namespace Akentas.IisLogfileReader.W3C
{
	#region // #### MG: Enums
	public enum FileType
	{
		Data,
		Error
	}
	#endregion

	public class Logfiles
	{
		#region // #### MG: Fields
		private string pathLogfiles;
		private DateTime firstFileDate;
		private DateTime lastFileDate;
		private DateTime actualFileDate;
		private int days;
		private Logfile logfile;
		private FileType filetype;
		private bool endOfFiles;
		private string lastFilepath;
		private string lastError;
		private int totalFiles;
		private int totalErrors;
		#endregion

		#region // #### MG: Constructors
		public Logfiles(string pathLogfiles, DateTime firstFileDate, DateTime lastFileDate)
		{
			this.pathLogfiles = pathLogfiles;
			this.firstFileDate = firstFileDate;
			this.lastFileDate = lastFileDate;
			this.actualFileDate = this.firstFileDate;
			this.endOfFiles = false;
			this.days = this.lastFileDate.Day - this.firstFileDate.Day;

			if(firstFileDate > lastFileDate)
			{
				throw(new Exception("[E00004] StartDate > StopDate"));
			}
		}
		#endregion

		#region // #### MG: Properties
		public string LastFilepath
		{
			get
			{
				return this.lastFilepath;
			}
		}

		public string LastError
		{
			get
			{
				return this.lastError;
			}
		}
				
		public Logfile Logfile
		{
			get
			{
				return this.logfile;
			}
		}

		public FileType FileType
		{
			get
			{
				return this.filetype;
			}
		}

		public bool EndOfFiles
		{
			get
			{
				return this.endOfFiles;
			}
		}

		public int TotalFiles
		{
			get
			{
				return this.totalFiles;
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
		public void Read()
		{
			// #### MG: Schema of logfile name = exyymmdd.log;
            this.lastFilepath = string.Format("{0}{1}ex{2}.log", this.pathLogfiles, Path.DirectorySeparatorChar, this.actualFileDate.ToString("yyMMdd"));
			FileInfo fi = new FileInfo(this.lastFilepath);
			this.totalFiles++;

			if(fi.Exists)
			{
				this.logfile = new Logfile(fi.FullName);
				this.filetype = FileType.Data;				
			}
			else
			{
				this.logfile = null;
				this.filetype = FileType.Error;
				this.lastError = string.Format("Error (File missing) while reading file '{0}'", this.lastFilepath);
				this.totalErrors++;
			}

			this.endOfFiles = (this.actualFileDate == this.lastFileDate);
			this.actualFileDate = this.actualFileDate.AddDays(1);
		}
		#endregion
	}
}