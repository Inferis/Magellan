void CGameMapCodec::DecompressBlock(int wx, int wy, int zoom)
{
#ifndef _BUILDMAP
	int a, b = 4711;

	if (wx < 0)
		wx += MAP_WIDTH;
	if (wx >= MAP_WIDTH)
		wx -= MAP_WIDTH;
	
	ASSERT(wy >= 0 && wy < MAP_HEIGHT);
	
	wx >>= zoom;
	
	wy >>= zoom;

				int mappitch = MAP_WIDTH >> zoom;
				int index = MapTrees[zoom].Offsets[(((wx)>>5) + ((wy)>>5) * ((mappitch>>5)))*3] + (MapTrees[zoom].Offsets[(((wx)>>5) + ((wy)>>5) * ((mappitch>>5)))*3+1]<<8) + (MapTrees[zoom].Offsets[(((wx)>>5) + ((wy)>>5) * ((mappitch>>5)))*3+2]<<16);
				int size = MapTrees[zoom].Offsets[(((wx)>>5) + ((wy)>>5) * ((mappitch>>5)))*3+3] + (MapTrees[zoom].Offsets[(((wx)>>5) + ((wy)>>5) * ((mappitch>>5)))*3+4]<<8) + (MapTrees[zoom].Offsets[(((wx)>>5) + ((wy)>>5) * ((mappitch>>5)))*3+5]<<16) - index;
				
				// The id list is the first thing in the compressed block
				BlockCompressed.IdTable = &MapTrees[zoom].Trees[index];
				
				// Build an id table out of the bit7-terminated list
				BlockDecompressed.NumOfIds = -1;
				do
				{
					BlockDecompressed.NumOfIds++;
					BlockDecompressed.IdTable[BlockDecompressed.NumOfIds] = BlockCompressed.IdTable[(BlockDecompressed.NumOfIds<<1)] | ((BlockCompressed.IdTable[(BlockDecompressed.NumOfIds<<1)+1]&127)<<8);
					if (BlockDecompressed.IdTable[BlockDecompressed.NumOfIds]>MAX_PROVINCE)
						int r = 0;
					index += 2;
				}
				while(BlockCompressed.IdTable[(BlockDecompressed.NumOfIds<<1)+1] < TERMINATOR);
				BlockDecompressed.NumOfIds++;

				for(a = 0; a < BlockDecompressed.NumOfIds; a++)
				{
					if (BlockDecompressed.IdTable[a] > MAX_PROVINCE)
					{
						int color = BlockDecompressed.IdTable[a] & 1;
						int id1 = BlockDecompressed.IdTable[((BlockDecompressed.IdTable[a]>>9)&63)-4];
						int id2 = Province[id1].GetNeighbor((BlockDecompressed.IdTable[a]>>1)&15);
						int river = (BlockDecompressed.IdTable[a]>>5)&15;
						if ((!(gpMap->MapMode._DrawFlags & PROVINCE_DRAW_BORDERS)) || Province[id1].GetBorderStatus() == Province[id2].GetBorderStatus())
						{
							if (river != INVALID_ADJ)
								id1 = Province[id1].GetNeighbor(river);
							BlockDecompressed.IdTable[a] = id1;
						}
						else
						{
							if (Province[id1].IsViewable())
								BlockDecompressed.IdTable[a] = MAX_PROVINCE;//+color;
							else
								BlockDecompressed.IdTable[a] = MAX_PROVINCE+2;//color;//+2;
						}
					}
				}
				
				// The tree structure pointer
				BlockCompressed.Nodes = &MapTrees[zoom].Trees[index];
				
				BlockDecompressed.NumOfLeafs = 0;
				BlockDecompressed.SizeOfLeafs = 0;
				BlockCompressed.NodeIndex = 0;
				BlockCompressed.NodeMask = 1;
				TreeInfo(5);
				
				index += BlockCompressed.NodeIndex;
				if (BlockCompressed.NodeMask > 1)
					index++;

				
				// The id information pointer
				BlockCompressed.Ids = &MapTrees[zoom].Trees[index];

				// Read the id stream diffrently depending on bit depth
				if (BlockDecompressed.NumOfIds == 1)
				{
					b = 0;
					for (a = 0; a < BlockDecompressed.NumOfLeafs; a++)
						BlockDecompressed.Ids[a] = 0;//BlockDecompressed.IdTable[0];
				}
				else if (BlockDecompressed.NumOfIds == 2)
				{
					a = b = 0;
					while (a < BlockDecompressed.NumOfLeafs)
					{
						BlockDecompressed.Ids[a++] = BlockCompressed.Ids[b] & 1;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 2)>>1;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 4)>>2;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 8)>>3;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 16)>>4;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 32)>>5;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 64)>>6;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 128)>>7;
						b++;
					}
				}
				else if (BlockDecompressed.NumOfIds > 2 && BlockDecompressed.NumOfIds <= 4)
				{
					a = b = 0;
					while (a < BlockDecompressed.NumOfLeafs)
					{
						BlockDecompressed.Ids[a++] = BlockCompressed.Ids[b] & 3;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 12)>>2;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 48)>>4;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 192)>>6;
						b++;
					}
				}
				else if (BlockDecompressed.NumOfIds > 4 && BlockDecompressed.NumOfIds <= 16)
				{
					a = b = 0;
					while (a < BlockDecompressed.NumOfLeafs)
					{
						BlockDecompressed.Ids[a++] = BlockCompressed.Ids[b] & 15;
						BlockDecompressed.Ids[a++] = (BlockCompressed.Ids[b] & 240)>>4;
						b++;
					}
				}				
				else if (BlockDecompressed.NumOfIds > 16)
				{
					a = b = 0;
					while (a < BlockDecompressed.NumOfLeafs)
					{
						BlockDecompressed.Ids[a++] = BlockCompressed.Ids[b];
						b++;
					}
				}				

				index += b;

				
				// The leafs pointer
				BlockCompressed.Leafs = &MapTrees[zoom].Trees[index];

				// Read the leaf stream
				a = b = 0;
				while(a < BlockDecompressed.SizeOfLeafs)
				{
					BlockDecompressed.Leafs[a++] = BlockCompressed.Leafs[b] & 63;
					BlockDecompressed.Leafs[a++] = (BlockCompressed.Leafs[b] >> 6 | BlockCompressed.Leafs[b+1] << 2) & 63;
					BlockDecompressed.Leafs[a++] = (BlockCompressed.Leafs[b+1] >> 4 | BlockCompressed.Leafs[b+2] << 4) & 63;
					BlockDecompressed.Leafs[a++] = BlockCompressed.Leafs[b+2] >> 2;
					b += 3;
				}

				// Initialize the tree bitstream counters
				BlockCompressed.NodeIndex = 0;
				BlockCompressed.NodeMask = 1;

				BlockDecompressed.LeafIndex = 0;
				BlockDecompressed.IdIndex = 0;
#endif
}
