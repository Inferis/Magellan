#define dfast

using System;
using System.Runtime.InteropServices;
using EU2.Map;
using System.Diagnostics;

namespace EU2.Map.Codec
{
	/// <summary>
	/// Summary description for Pixel.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct Pixel
	{
		public static Pixel Empty = new Pixel( 0, 0, 0, 0 );

#if fast 
		[FieldOffset(0)] public byte Color;
		[FieldOffset(1)] public byte Border;
		[FieldOffset(2)] public ushort ID;
		//[FieldOffset(4)] public ushort Full;
#else
		[FieldOffset(0)]
		private uint data;
#endif
		public Pixel( byte color, ushort id ) : this( color, id, 0, 0 ) {
		}

		public Pixel( byte color, ushort id, ushort riverid, byte border ) {
#if !fast 
			Debug.Assert( id <= 4095, "Invalid ctor ID " + id + " for pixel!" );
			Debug.Assert( riverid <= 4095, "Invalid ctor river ID " + riverid + " for pixel!" );
			this.data = 0;
			
			ID = id;
			RiverID = riverid;
			Border = border;
			Color = color;
#else
			this.Color = color;
			this.ID = id;
			this.Border = border;
#endif
			//this.Full = 0;
		}

#if !fast
		public byte Color {
			get {
				return (byte)(data & 0x3F);
			}
			set {
				data = (uint)value | (data & 0xFFFFFFC0);
			}
		}

		public byte Border {
			get {
				return (byte)((data >> 6) & 0x3);
			}
			set {
				data = ((uint)value << 6) | (data & 0xFFFFFF3F);
			}
		}

		public ushort RiverID {
			get {
				return (ushort)((data >> 8) & 0xFFF);
			}
			set {
				data = ((uint)value << 8) | (data & 0xFFF000FF);
			}
		}

		public ushort ID {
			get {
				return (ushort)((data >> 20) & 0xFFF);
			}
			set {
				Debug.Assert( value >= 0 && value <= EU2.Data.Province.MaxValue, "Invalid Province ID " + value + " for pixel!" );
				data = ((uint)value << 20) | (data & 0x000FFFFF);
			}
		}
#endif

		public bool IsBorder() {
			return Border > 0;
		}

		public override bool Equals( System.Object other ) {
			return other is Pixel && this.Color == ((Pixel)other).Color && this.ID == ((Pixel)other).ID && this.Border == ((Pixel)other).Border;
		}

		public static bool operator == ( Pixel one, Pixel other ) {
			return one.Equals( other );
		}

		public static bool operator != ( Pixel one, Pixel other ) {
			return !one.Equals( other );
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}


		/*
		public Pixel( NodeData data ) {
			this.color = data.Color ;
			this.id = data.Owner.ProvinceId;
			this.border = data.Owner.NeighbourId > 0;
		}
		*/
	}
}
