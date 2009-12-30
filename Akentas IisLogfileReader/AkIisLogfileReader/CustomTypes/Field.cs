using System;

namespace Akentas.IisLogfileReader.CustomTypes
{
	public struct Field
	{
		#region // #### MG: Fields
		private string name;
		private string value;
		#endregion

		#region // #### MG: Properties
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}
		#endregion
	}
}