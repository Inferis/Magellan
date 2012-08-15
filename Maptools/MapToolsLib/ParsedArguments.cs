using System;
using System.Collections;

namespace MapToolsLib
{
	/// <summary>
	/// Summary description for ParsedArguments.
	/// </summary>
	public abstract class ParsedArguments {
		public delegate void ProcessArgumentHandler( ParsedArguments sender, ProcessArgumentEventArgs e );
		public event ProcessArgumentHandler ProcessParameter;
		public event ProcessArgumentHandler ProcessOption;

		protected ParsedArguments( ) {
			// Attach help handler
			ProcessOption += new ProcessArgumentHandler( ParsedArguments_ProcessOption );
		}

		public bool Help {
			get { return help; }
		}

		protected virtual void OnProcessArgument( ArgumentType type, string value, string data ) {
			ProcessArgumentHandler handler = null;

			if ( type == ArgumentType.Parameter ) 
				handler = ProcessParameter;
			else
				handler = ProcessOption;

			if ( handler != null ) handler( this, new ProcessArgumentEventArgs( type, value, data ) );
		}

		protected virtual void OnProcessArgument( ArgumentType type, string value ) {
			ProcessArgumentHandler handler = null;

			if ( type == ArgumentType.Parameter ) 
				handler = ProcessParameter;
			else
				handler = ProcessOption;

			if ( handler != null ) handler( this, new ProcessArgumentEventArgs( type, value ) );
		}

		protected void Parse( string[] args ) {
			for ( int i=0; i<args.Length; ++i ) {
				if ( args[i].StartsWith("/") || args[i].StartsWith("-") ) {
					int idx = args[i].IndexOf( ":" );
					if ( idx >= 0 ) {
						OnProcessArgument( ArgumentType.Option, args[i].Substring( 1, idx-1 ), args[i].Substring( idx+1, args[i].Length - idx - 1 ) );
						continue;
					}
					else {
						OnProcessArgument( ArgumentType.Option, args[i].Substring( 1, args[i].Length-1 ) );
						continue;
					}
				}
				
				OnProcessArgument( ArgumentType.Parameter, args[i] );
			}
		}

		private void ParsedArguments_ProcessOption( ParsedArguments sender, ProcessArgumentEventArgs e ) {
			if ( e.Type != ArgumentType.Option ) return;
			
			string option = e.Value.ToLower();
			if ( option == "h" || option == "?" || option == "help" ) help = true;
		}

		private bool help = false;
	}
}
