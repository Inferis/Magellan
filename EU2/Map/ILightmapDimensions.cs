using System;
using System.Drawing;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for ILightmapDimensions.
	/// </summary>
	public interface ILightmapDimensions
	{
		int Zoom { get; }
		Size Size { get; }
        Size SizeBlocks { get; }

		int NormalizeX( int x ); 
		int NormalizeBlockX( int x ); 
		CoordinateMapper CoordMap { get; }
	}
}
