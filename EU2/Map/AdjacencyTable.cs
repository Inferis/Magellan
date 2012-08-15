using System;
using System.IO;
using EU2.Data;

namespace EU2.Map
{
	/// <summary>
	/// Summary description for Adjacent.
	/// </summary>
	public class AdjacencyTable : EU2.IO.IStreamWriteable {
		private Adjacent[][] table;

		public AdjacencyTable() {
			table = new Adjacent[Province.MaxValue+1][];
		}

		public AdjacencyTable( BinaryReader reader ) : this() {
			ReadFrom( reader );
		}

		public AdjacencyTable( string path ) : this() {
			FileStream stream = null;
			try {
				stream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
				ReadFrom( new BinaryReader( stream ) );
			}
			finally {
				if ( stream != null ) stream.Close();
			}
		}

		public void Add( ushort id, Adjacent item ) {
			if ( table[id] == null ) {
				table[id] = new Adjacent[1] { item };
			}
			else {
				Adjacent[] newtable = new Adjacent[table[id].Length+1];
				table[id].CopyTo( newtable, 0 );
				newtable[table[id].Length] = item;
				table[id] = newtable;
			}

		}

		public void ReadFrom( BinaryReader reader ) {
			// Read offsets first
			int[] offsets = new int[Province.MaxValue+1];
			int realcount = Province.MaxValue+1;
			Adjacent first = null;
			for ( int i=0; i<=Province.MaxValue; ++i ) {
				offsets[i] = reader.ReadInt32();
				if ( (uint)offsets[i] == 0xCDCDCDCD ) {
					// No longer reading offsets... Stop!
					first = new Adjacent( (ushort)offsets[i-2], (AdjacencyType)offsets[i-1] );
					offsets[i] = offsets[i-1] = offsets[i-2] = 0;
					realcount = i-2;
					break;
				}
			}

			// Read adjacency lists for each province
			for ( int i=0; i<realcount-1; ++i ) {
				int length = offsets[i+1]-offsets[i];
				int a0 = 0;

				table[i] = new Adjacent[length];
				if ( first != null && length > 0 ) {
					table[i][0] = first;
					first = null;
					a0 = 1;
				}
				for ( int a=a0; a<length; ++a ) {
					table[i][a] = new Adjacent( reader );
				}

			}
		}

		public void WriteTo( BinaryWriter writer ) {
			// Write offsets
			int itemCount = 0;
			for ( int i=0; i<Province.MaxValue+1; ++i ) {
				writer.Write( itemCount );
				if ( table[i] == null ) continue;
				itemCount += table[i].Length;
			}

			// Write lists
			for ( int i=0; i<Province.MaxValue; ++i ) {
				if ( table[i] == null ) continue;
				for ( int a=0; a<table[i].Length; ++a ) {
					table[i][a].WriteTo( writer );
				}
			}
		}

		public virtual void WriteTo( Stream stream ) {
			BinaryWriter writer = new BinaryWriter( stream );
			WriteTo( writer );
		}

		public bool IsAdjacent( ushort fromId, ushort toId ) {
			return GetAdjacencyIndex( fromId, toId ) >= 0;
		}

		public int GetAdjacencyIndex( ushort fromId, ushort toId ) {
			if ( table[fromId] == null ) return -1;

			for ( int i=0; i<table[fromId].Length; ++i ) {
				if ( table[fromId][i].ID == toId ) return i;
			}

			return -1;
		}

		public Adjacent[] this[int id] {
			get {
				if ( id < 0 || id > Province.Count ) throw new ArgumentOutOfRangeException();
				return table[id];
			}
			set {
				if ( id < 0 || id > Province.Count ) throw new ArgumentOutOfRangeException();

				table[id] = new Adjacent[value.Length];
				value.CopyTo( table[id], 0 );
			}
		}

		public Adjacent GetAdjacency( ushort fromId, int index ) {
			if ( table[fromId] == null || index >= table[fromId].Length ) return new Adjacent( Province.TerraIncognitaID, AdjacencyType.Normal );

			return table[fromId][index];
		}

		public ushort[] Merge( AdjacencyTable other ) {
			int[] tagged = new int[Province.InternalCount];

			for ( int fromId=1; fromId<other.table.Length; ++fromId ) {
				if ( other.table[fromId] == null || other.table[fromId].Length == 0 ) continue;

				++tagged[fromId];
				table[fromId] = new Adjacent[other.table[fromId].Length];
				other.table[fromId].CopyTo( table[fromId], 0 );

				for ( int t=0; t<table[fromId].Length; ++t ) 
					++tagged[table[fromId][t].ID];
			}

			// Count affected ids
			int count = 0;
			for ( ushort i=0; i<tagged.Length; ++i ) {
				if ( tagged[i] > 0 ) count++;
			}

			ushort[] affected = new ushort[count];
			count = 0;
			for ( ushort i=0; i<tagged.Length; ++i ) {
				if ( tagged[i] > 0 ) affected[count++] = i;
			}

			return affected;
		}
	}

	/// <summary>
	/// Defines the type of the Adjacency item.
	/// </summary>
	public enum AdjacencyType {
		Normal = 0,
		River = 1,
	}

	public class Adjacent {
		public const int Invalid = 15;

		ushort id;
		AdjacencyType type; 

		public Adjacent( ushort id, AdjacencyType type ) {
			this.id = id;
			this.type = type;
		}

		public Adjacent( BinaryReader reader ) {
			ReadFrom( reader );
		}

		public void ReadFrom( BinaryReader reader ) {
			id = (ushort)(reader.ReadInt32());
			type = (AdjacencyType)(reader.ReadInt32());
			reader.ReadInt32(); // Unused int
		}

		public void WriteTo( BinaryWriter writer ) {
			writer.Write( (int)id );
			writer.Write( (int)type );
			writer.Write( (uint)0xCDCDCDCD );
		}

		public ushort ID {
			get { return id; }
			set { id = value; }
		}

		public AdjacencyType Type {
			get { return type; }
			set { type = value; }
		}
	}
}
