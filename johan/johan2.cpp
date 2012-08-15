void CGameMapCodec::BlockDraw(int x, int y, int sx, int sy, int level) {
	int d = 1<<level;
	int l, id;

	int node = BlockCompressed.Nodes[BlockCompressed.NodeIndex] & BlockCompressed.NodeMask;

	__asm 
	{
		rol BlockCompressed.NodeMask, 1;
		adc BlockCompressed.NodeIndex, 0
	}

	if (node && level > 1)
	{
		BlockDraw(x+(d>>1), y+(d>>1), sx+(d>>1), sy+(d>>1), level-1);
		BlockDraw(x, y+(d>>1), sx, sy+(d>>1),  level-1);
		BlockDraw(x+(d>>1), y, sx+(d>>1), sy,  level-1);
		BlockDraw(x, y, sx, sy, level-1);
	} else {
		int a, b;
		int topleft, bottomleft, topright, bottomright;
		int left, right;
		int xstep, light;
		int xmax = d, ymax = d;

		if (node)
		{
			id = BlockDecompressed.IdTable[BlockDecompressed.Ids[BlockDecompressed.IdIndex++]];
			topleft = (BlockDecompressed.Leafs[BlockDecompressed.LeafIndex++]);
			ShadowBuffer[(x+1) + (y+1) * 64] = topleft;
			
			l = (RangeCheck[topleft - ExtraBuffer[(x+1) +(y+1) * 32] + 32+128-Province[id]._nLight+LIGHT_50_PERCENT]<<6) | Province[id]._nTexture;
			MapAddr[(x+1)*2 + (y+1) * MapPitch] = ColorShades32[l] & 255;
			MapAddr[(x+1)*2+1 + (y+1) * MapPitch] = ColorShades32[l] >> 8;

			id = BlockDecompressed.IdTable[BlockDecompressed.Ids[BlockDecompressed.IdIndex++]];
			topleft = BlockDecompressed.Leafs[BlockDecompressed.LeafIndex++];
			ShadowBuffer[x + (y+1) * 64] = topleft;
			
			l = (RangeCheck[topleft - ExtraBuffer[(x) + (y+1) * 32] + 32+128-Province[id]._nLight+LIGHT_50_PERCENT]<<6) | Province[id]._nTexture;
			MapAddr[(x)*2 + (y+1) * MapPitch] = ColorShades32[l] & 255;
			MapAddr[(x)*2+1 + (y+1) * MapPitch] = ColorShades32[l] >> 8;

			id = BlockDecompressed.IdTable[BlockDecompressed.Ids[BlockDecompressed.IdIndex++]];
			topleft = (BlockDecompressed.Leafs[BlockDecompressed.LeafIndex++]);
			ShadowBuffer[(x+1) + y * 64] = topleft;

			l = (RangeCheck[topleft - ExtraBuffer[(x+1) + (y) * 32] + 32+128-Province[id]._nLight+LIGHT_50_PERCENT]<<6) | Province[id]._nTexture;
			MapAddr[(x+1)*2 + (y) * MapPitch] = ColorShades32[l] & 255;
			MapAddr[(x+1)*2+1 + (y) * MapPitch] = ColorShades32[l] >> 8;
			
			id = BlockDecompressed.IdTable[BlockDecompressed.Ids[BlockDecompressed.IdIndex++]];
			topleft = BlockDecompressed.Leafs[BlockDecompressed.LeafIndex++];
			ShadowBuffer[x + y * 64] = topleft;
			
			l = (RangeCheck[topleft - ExtraBuffer[(x) + (y) * 32] + 32+128-Province[id]._nLight+LIGHT_50_PERCENT]<<6) | Province[id]._nTexture;
			MapAddr[(x)*2 + (y) * MapPitch] = ColorShades32[l] & 255;
			MapAddr[(x)*2+1 + (y) * MapPitch] = ColorShades32[l] >> 8;

		} else {
			topleft = BlockDecompressed.Leafs[BlockDecompressed.LeafIndex++];
			id = BlockDecompressed.IdTable[BlockDecompressed.Ids[BlockDecompressed.IdIndex++]];

			// The bottom-right value is always known
			bottomright = ShadowBuffer[x + d + (y+d) * 64];
			topright = ShadowBuffer[x + d + y * 64]; // Value already present
			bottomleft = ShadowBuffer[x + (y + d) * 64]; // Value already present

			topleft <<= 16;
			topright <<= 16;
			bottomleft <<= 16;
			bottomright <<= 16;

			int leftstep = (bottomleft - topleft) >> level;
			int rightstep = (bottomright - topright) >> level;
			
			left = topleft;
			right = topright;
			
			int prlight = (Province[id]._nLight);//<<16;
			int prtex = Province[id]._nTexture*2;

			// Do the interpolation
			xstep = (right - left) >> level;
			light = left;

			for(a = 0; a < xmax; a++) // Loop horizontally
			{
				ShadowBuffer[(x+a) + y * 64] = light>>16;
				light += xstep;
			}
	
			unsigned char * addr = &MapAddr[x*2+y*MapPitch];
			unsigned char * terra = &ExtraBuffer[x + y * 32];

			for(b = 0; b < ymax; b++) // Loop vertically
			{
				xstep = (left - right) >> level;
				light = right;

				ShadowBuffer[x + (y+b) * 64] = left>>16;
  
				for(a = xmax-1; a >= 0; a-=1) // Loop horizontally
				{
					l = (RangeCheck[((light+0x8000)>>16) + 128 + 32 - terra[a] - prlight+LIGHT_50_PERCENT]<<6) | (prtex/2);

					addr[a*2] = ColorShades32[l] & 255;
					addr[a*2+1] = ColorShades32[l] >> 8;

					light += xstep;
				}


				addr += MapPitch;
				terra += 32;

				left += leftstep;
				right += rightstep;
			}		
		}
	}
}




