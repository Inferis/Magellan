using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace PSD
{
	/// <summary>
	/// Summary description for Resources.
	/// </summary>
	public class Layers : IEnumerable
	{
		public Layers( File file ) {
			this.file = file;
			items = new Layer[0];
			absolutealpha = false;
		}

		public Layers( File file, BinaryReader reader ) : this( file ) {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			int datalen = IPAddress.NetworkToHostOrder( reader.ReadInt32() );
			if ( datalen  == 0 ) {
				items = new Layer[0];
				return;
			}
			int startoffset = (int)reader.BaseStream.Position;

			int sectionlength = IPAddress.NetworkToHostOrder( reader.ReadInt32() ); // section length, not used
			ReadLayers( reader );

			startoffset = (int)reader.BaseStream.Position-startoffset;
			if ( startoffset < datalen ) {
				int diff = datalen-startoffset;
				//if ( diff != 4 && diff != 24 ) Console.WriteLine( "woops!" );
				reader.ReadBytes( diff ); // bogus padding?
			}
		}

		public void WriteTo( BinaryWriter writer ) {
			MemoryStream mem = new MemoryStream( 4096 );
			
			WriteLayers( new BinaryWriter( mem ) );
			int datalen = (int)mem.Length + 4;

			// Datalength
			writer.Write( IPAddress.HostToNetworkOrder( datalen ) );

			// SectionLength
			datalen -= 4;
			writer.Write( IPAddress.HostToNetworkOrder( datalen ) );

			writer.Write( mem.ToArray() );
			mem.Close();
		}

		public Layer Add( Layer layer ) {
			Layer[] newitems = new Layer[items.Length+1];
			items.CopyTo( newitems, 0 );
			newitems[newitems.Length-1] = layer;
			layer.File = file;
			items = newitems;

			return layer;
		}

		public Layer Add( Layer layer, int where ) {
			if ( where < 0 || where > items.Length ) return Add( layer );

			layer.File = file;
			Layer[] newitems = new Layer[items.Length+1];
			if ( where == 0 ) {
				Array.Copy( items, 0, newitems, 1, items.Length  );
			}
			else {
				Array.Copy( items, 0, newitems, 0, where );
				Array.Copy( items, where, newitems, where+1, items.Length-where );
			}
			newitems[where] = layer;
			items = newitems;

			return layer;
		}

		private void ReadLayers( BinaryReader reader ) {
			int layercount = IPAddress.NetworkToHostOrder( reader.ReadInt16() );

			if ( layercount < 0 ) {
				layercount = -layercount;
				absolutealpha = true;
			}
			else {
				absolutealpha = false;
			}

			ProtoLayer[] protolayers = new ProtoLayer[layercount];
			for ( int i=0; i<layercount; ++i ) {
				protolayers[i] = new ProtoLayer();
				protolayers[i].ReadDefinitionFrom( reader );
			}

			// Read layer data nadProtolayers to real layers
			items = new Layer[protolayers.Length];
			for ( int i=0; i<protolayers.Length; ++i ) {
				protolayers[i].ReadPixelDataFrom( reader );
				items[i] = Layer.FromProtoLayer( protolayers[i], file );
			}
		}

		private void WriteLayers( BinaryWriter writer ) {
			int layercount = items.Length;
			writer.Write( IPAddress.HostToNetworkOrder( (short)(absolutealpha ? -layercount : layercount) ) );

			ProtoLayer[] protolayers = new ProtoLayer[layercount];
			for ( int i=0; i<layercount; ++i ) {
				protolayers[i] = ProtoLayer.FromLayer( items[i] );
				protolayers[i].WriteDefinitionTo( writer );
			}

			for ( int i=0; i<layercount; ++i ) {
				protolayers[i].WritePixelDataTo( writer );
			}
		}

		protected int GetIndexByName( string name ) {
			for ( int i=0; i<items.Length; ++i ) {
				if ( items[i].Name == name ) return i;
			}

			return -1;
		}

		public IEnumerator GetEnumerator() {
			return items.GetEnumerator();
		}
		
		public void ShowAll() {
			for ( int i=0; i<items.Length; ++i ) {
				items[i].Visible = true;
			}
		}

		public void HideAll() {
			for ( int i=0; i<items.Length; ++i ) {
				items[i].Visible = false;
			}
		}

		public Bitmap CreateFlattenedImage() {
			return CreateFlattenedImage( Color.Transparent );
		}
			
		public Bitmap CreateFlattenedImage( Color backgroundColor ) {
			Bitmap result = new Bitmap( file.ImageSize.Width, file.ImageSize.Height, PixelFormat.Format32bppArgb );
			Graphics g = Graphics.FromImage( result );

			if ( backgroundColor != Color.Transparent ) 
				using ( Brush b = new SolidBrush( backgroundColor ) ) 
					g.FillRectangle( b, 0, 0, file.ImageSize.Width, file.ImageSize.Height );

			for ( int i=0; i<items.Length; ++i ) {
				Layer layer = items[i];
				if ( !layer.Visible ) continue;
				Bitmap layerimg = layer.Image;

				int[] resultdata;
				if ( layer.Mask != null ) { 
					resultdata = Utils.GetBitmapData( result, layer.Mask.Bounds );
					int[] maskdata = Utils.GetBitmapData( layer.Mask.Image, Utils.LayerAdjustedBounds( result, layer.Mask.Bounds ) );
					LayerMode.SoftLight.Blend( resultdata, maskdata, 0.8F );
					Utils.SetBitmapData( result, resultdata, layer.Mask.Bounds );
				}

				if ( layerimg != null ) {
					resultdata = Utils.GetBitmapData( result, layer.Bounds );
					int[] layerdata = Utils.GetBitmapData( layerimg, Utils.LayerAdjustedBounds( result, layer.Bounds ) );
					layer.Mode.Blend( resultdata, layerdata, layer.OpacityF );
					Utils.SetBitmapData( result, resultdata, layer.Bounds );
				}
			}

			return result;
		}

		
		#region Public Properties
		public int Length {
			get { return items.Length; }
		}

		public Layer this[int index] {
			get { return index >= 0 && index < items.Length ? items[index] : null; }
			set { items[index] = value; }
		}

		public Layer this[string name] {
			get { int idx = GetIndexByName( name ); return idx >= 0 ? items[idx] : null; }
			set {
				int index = GetIndexByName( value.Name );
				if ( index < 0 ) {
					Layer[] newitems = new Layer[items.Length+1];
					items.CopyTo( newitems, 0 );
					index = items.Length;
					items = newitems;
				}
				items[index] = value;
			}
		}

		public bool Contains( string name ) {
			return GetIndexByName( name ) >= 0;
		}

		public File File {
			get { return file; }
			set { 
				file = value;
				for ( int i=0; i<items.Length; ++i ) {
					items[i].File = value;
				}
			}
		}

		#endregion

		#region Private Fields
		private File file;
		private Layer[] items;
		private bool absolutealpha;
		#endregion
	}
}
