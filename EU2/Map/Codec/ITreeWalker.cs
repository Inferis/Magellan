using System;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for TreeWalker.
	/// </summary>
	#region Public Enumerations
	public enum TreeWalkerMode {
		Full,
		LeftOnly,
		TopLeftOnly,
		TopOnly
	}
	#endregion
		
	public interface ITreeWalker
	{
		void WalkTree( MapBlock block );
		void WalkTree( MapBlock block, TreeWalkerMode mode );
		int StopAtLevel { get; set; }
	}
}
