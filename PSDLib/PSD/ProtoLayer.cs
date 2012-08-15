using System;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PSD
{
	/// <summary>
	/// Summary description for Layer.
	/// </summary>
	public class ProtoLayer
	{
		public ProtoLayer() {
		}

		#region IO
		public void ReadDefinitionFrom( BinaryReader reader ) {
			// Read bounds
			int top = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			int left = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			int bottom = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			int right = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			bounds = Rectangle.FromLTRB( left, top, right, bottom );
			maskbounds = bounds;
			hasmask = false;

			channels = new Channels( reader );

			// Blend info
			Utils.CheckSignature( reader, "8BIM", new InvalidBlendSignature() );
			blendkey = new string( reader.ReadChars( 4 ) );
			opacity = reader.ReadByte();
			clipping = reader.ReadByte() == 0 ? LayerClipping.Base : LayerClipping.NonBase;

			// Flags
			byte flags = reader.ReadByte();
			protecttrans = (flags & 1) != 0;
			visible = (flags & 2) == 0;

			reader.ReadByte(); // filler

			// extra data (?)
			int extradatasize = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( extradatasize > 0 ) extradatasize += (int)reader.BaseStream.Position;

			// Layer mask data
			int datasize = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( datasize > 0 ) {
				top = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
				left = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
				bottom = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
				right = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
				maskbounds = Rectangle.FromLTRB( left, top, right, bottom );
				hasmask = true;

				// layer mask data
				datasize = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			}

			// Range data
			datasize = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( datasize > 0 ) {
				reader.ReadBytes( datasize ); // throw away
			}

			name = Utils.ReadPascalString( reader, 4 );

			// Read rest
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			while ( (int)reader.BaseStream.Position < extradatasize ) {
				LayerAdjustment adj = LayerAdjustment.Construct( reader ); 
				if ( adj == null ) break;

				list.Add( adj );
			}
			if ( list.Count > 0 ) {
				adjustments = new LayerAdjustment[list.Count];
				list.CopyTo( adjustments, 0 );
			}
			else
				adjustments = new LayerAdjustment[0];
			
			if ( channels.Mask != null ) channels.Mask.Size = maskbounds.Size;
			if ( channels.Alpha != null ) channels.Alpha.Size = bounds.Size;
			if ( channels.Red != null ) {
				// Let's assume that a valid Red channel also means valid Green and Blue channels
				channels.Red.Size = bounds.Size;
				channels.Green.Size = bounds.Size;
				channels.Blue.Size = bounds.Size;
			}
		}

		public void ReadPixelDataFrom( BinaryReader reader ) {
			channels.ReadPixelDataFrom( reader );
		}

		public void WriteDefinitionTo( BinaryWriter writer ) {
			writer.Write( IPAddress.HostToNetworkOrder( bounds.Top ) );
			writer.Write( IPAddress.HostToNetworkOrder( bounds.Left ) );
			writer.Write( IPAddress.HostToNetworkOrder( bounds.Bottom ) );
			writer.Write( IPAddress.HostToNetworkOrder( bounds.Right ) );

			channels.WriteTo( writer );

			// Blend info
			writer.Write( new char[4] { '8', 'B', 'I', 'M' } );
			writer.Write( blendkey.ToCharArray(0,4) );

			writer.Write( (byte)opacity );
			writer.Write( (byte)(clipping == LayerClipping.Base ? 0 : 1) );
			byte flags = (byte)((protecttrans ? 1 : 0) | (visible ? 0 : 2));
			writer.Write( flags );
			writer.Write( (byte)0 );

			// Extra data size: None
			writer.Write( IPAddress.HostToNetworkOrder( (int)(8+Utils.GetPascalLength( name, 4 )) ) );
			writer.Write( (int)0 );		// layermask data size: None
			writer.Write( (int)0 );		// range data size: None

			Utils.WritePascalString( writer, name, 4 );
		}

		public void WritePixelDataTo( BinaryWriter writer ) {
			channels.WritePixelDataTo( writer );
		}

		#endregion

		#region Public Properties
		public string Name {
			get { return name; }
			set { name = value; }
		}

		public Rectangle Bounds {
			get { return bounds; }
			set { bounds = value; }
		}

		public Rectangle MaskBounds {
			get { return maskbounds; }
			set { maskbounds = value; }
		}

		public int Opacity {
			get { return opacity; }
			set { opacity = value; if ( opacity < 0 ) opacity = 0; if ( opacity > 255 ) opacity = 255; }
		}

		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}

		public bool ProtectTransparancy {
			get { return protecttrans; }
			set { protecttrans = value; }
		}

		public string Blendkey {
			get { return blendkey; }
			set { blendkey = value; }
		}

		public LayerClipping Clipping {
			get { return clipping; }
			set { clipping = value; }
		}

		public LayerAdjustment[] Adjustments {
			get { return adjustments; }
			set { adjustments = value; }
		}

		#endregion

		public Bitmap BuildImage() {
			return channels.Combine( false );
		}

		public LayerMask BuildMask() {
			if ( hasmask ) {
				Bitmap maskimage = channels.Combine( true );
				if ( maskimage != null ) return new LayerMask( maskbounds, maskimage );
			}

			return null;
		}

		public static ProtoLayer FromLayer( Layer source ) {
			ProtoLayer result = new ProtoLayer();

			result.name = source.Name;
			result.bounds = source.Bounds;
			result.opacity = source.Opacity;
			result.visible = source.Visible;
			result.protecttrans = source.ProtectTransparancy;
			result.clipping = source.Clipping;
			result.blendkey = source.Mode.Key;
			result.channels = Channels.FromImage( source.Image );

			return result;
		}

		#region Private Fields
		string name;
		private Channels channels;
		private Rectangle bounds;
		private Rectangle maskbounds;
		private string blendkey;
		private int opacity;
		private LayerClipping clipping;
		private bool protecttrans;
		private bool visible;
		private bool hasmask;
		private LayerAdjustment[] adjustments;
		#endregion
	}

	public class InvalidBlendSignature : Exception {
		public InvalidBlendSignature() : base( "The layer blend signature is incorrect." ) {}
	}

}
