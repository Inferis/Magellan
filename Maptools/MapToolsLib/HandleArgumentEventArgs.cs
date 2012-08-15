using System;

namespace MapToolsLib
{
	public enum ArgumentType {
		Parameter,
		Option
	}

	/// <summary>
	/// Summary description for HandleArgumentEventArgs.
	/// </summary>
	public class ProcessArgumentEventArgs
	{
		public ProcessArgumentEventArgs( ArgumentType type, string value, string data ) {
			this.type = type;
			this.value = value;
			this.data = data;
		}

		public ProcessArgumentEventArgs( ArgumentType type, string value ) : this ( type, value, "" ) {
		}

		public ArgumentType Type {
			get { return type; }
		}

		public string Value {
			get { return value; }
		}

		public string Data {
			get { return data; }
		}
		
		public bool IsOption() {
			return type == ArgumentType.Option;
		}

		public bool IsParameter() {
			return type == ArgumentType.Parameter;
		}

		private ArgumentType type;
		private string value;
		private string data;
	}
}
