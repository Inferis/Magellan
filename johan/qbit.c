// quad tree bit plane compression
// Doug Houghton July 2, 2000
// revised April 28, 2002
// doug_houghton@sympatico.ca
// ---------------------------------------------------
// THIS SOFTWARE IS FREE AND PROVIDED AS IS WITH NO GAURANTEE OF ANY KIND FOR 
// IT'S  QUALITY, ACCURACY OR RELIABILITY.  THE AUTHOR IS IN NO WAY LIABLE FOR 
// ANY DAMAGES RESULTING FROM OR RELATED TO THE USE OF THIS SOFTWARE. USE AT 
// OWN RISK.
// ---------------------------------------------------
#include <stdlib.h>
#include <string.h>
#include "qbit.h"

#define COLLAPSE_LEAF 16
#define COLLAPSE_CACHE_MAX 65535

typedef struct tagMETRICS
{
	Qbyte *startstream;
	Qlong width;
	Qlong height;
	Qlong bitdepth;
	Qlong bytedepth;
	Qlong nextmask;
	Qlong shifty;
	Qlong scanpad;
	Qlong bitlu[256];
	Qlong weight[256];
	Qlong unweight0[256];
	Qlong unweight1[256];
	Qlong unweight2[256];
	Qlong collapsebits;
	Qlong maxbit;
	Qlong allbits;
	Qlong leadingbits;
	Qlong *dhash;
	Qint *masks;
} METRICS;


typedef struct tagSTREAM
{
	Qbyte *start;
	Qbyte *current;
	Qbyte low;
}STREAM;


typedef struct tagREGISTERS
{
	Qlong init;
	Qlong current;
	Qlong lastreg;
	Qlong masks[12];
	Qlong registers[12];
	Qlong shifts[12];
}REGISTERS;

typedef struct tagCACHE
{
	Qlong colour;
	Qlong index;
}CACHE;


#define LOCATION(x,y,metrics) metrics->startstream + (y * metrics->shifty) + (x * metrics->bytedepth)

#define PUT_COLOUR_TRUE_ADVANCE(source, colour) {source[0] = (colour >> 16) & 255; source[1] = (colour >> 8) & 255; source[2] = colour & 255; source += 3;}
#define PUT_COLOUR_HIGH_ADVANCE(source, colour) {source[0] = (colour >> 8) & 255; source[1] = colour & 255; source += 2;}
#define PUT_COLOUR_DWORD_ADVANCE(source, colour) {source[0] = (colour >> 24) & 255; source[1] = (colour >> 16) & 255; source[2] = (colour >> 8) & 255; source[3] = colour & 255; source += 4;}

#define GET_COLOUR_TRUE(source) (Qlong)(source[0] << 16) | (source[1] << 8) | source[2]
#define GET_COLOUR_HIGH(source)	(Qlong)(source[0] << 8) | source[1]
#define GET_COLOUR_DWORD(source)	(Qlong)(source[0] << 24) | (source[1] << 16) | (source[2] << 8) | source[3]

#define UNMAP_TRUE(metrics, colour) (Qlong)metrics->unweight0[(colour >> 16) & 255] | metrics->unweight1[(colour >> 8) & 255] | metrics->unweight2[colour & 255]
#define UNMAP_HIGH(metrics, colour) (Qlong)metrics->unweight0[(colour >> 8) & 255] | metrics->unweight1[colour & 255]

#define MAPWEIGHT_TRUE(metrics, source) (Qlong)metrics->weight[source[0]] | (metrics->weight[source[1]] >> 1) | (metrics->weight[source[2]] >> 2)
#define MAPWEIGHT_HIGH(metrics, source) (Qlong)metrics->weight[source[0]] | (metrics->weight[source[1]] >> 1)

//#define READ_QUAD(buffer) buffer->low? *(buffer->current++) & 15:*(buffer->current) >> 4; {buffer->low = !buffer->low;}
#define WRITE_QUAD(buffer, quad) if (buffer->low){*(buffer->current) = (quad & 15); buffer->low = 0;} else { *(buffer->current) |= (quad << 4); buffer->current--; buffer->low = 255;}


// encode
int encodetile(Qlong, Qlong, Qlong, METRICS *, STREAM *, Qbyte *, Qlong);
void encodeblock(Qlong, Qlong, METRICS *, Qbyte *);
void getquad(Qlong *, Qlong, Qlong, METRICS *);
Qlong collapseall(METRICS *, STREAM *);
Qlong bin(Qlong, Qlong, Qlong *, Qlong);
Qlong collapse(Qlong, Qlong, METRICS *, STREAM *, Qlong, Qlong);
void regout(REGISTERS *, STREAM *, Qlong);


// decode
void decodetile(Qlong, Qlong, METRICS *, Qlong, Qlong, Qlong, Qlong, STREAM *);
void decodeblock(Qlong, Qlong, METRICS *, Qlong, Qlong);
void uncollapse(Qlong, Qlong, METRICS *, STREAM *, Qlong);
Qlong *readpal(Qlong *, Qlong, STREAM *, Qlong);
Qbyte readquad(STREAM *);


//
void getmetrics(METRICS *, Qlong, Qlong, Qlong, Qlong, Qlong, Qlong *);


// debug
long debugcount;
long getdebugcount(void) {return(debugcount);}



// ================================================
// this function is called by both decoding
// and encoding processes.  i set's up the
// metrics struct with lookup tables, image dimentions,
// data pointers etc...
// ---------------------------------------
void getmetrics(
	METRICS *metrics,
	Qlong width,
	Qlong height,
	Qlong depth,
	Qlong flags,
	Qlong options,
	Qlong *span)
{

	long i, j, align;
	Qlong bit, shift;

	if (options & QBIT_OPTION_INT_ALIGNED)
		align = 2;
	else if (options & QBIT_OPTION_NOT_ALIGNED)
		align = 0;
	else
		align = 4;

	metrics->width = width;
	metrics->height = height;
	metrics->bitdepth = depth;
	metrics->bytedepth = depth / 8;
	metrics->shifty = width * metrics->bytedepth;

	if (align)
		metrics->scanpad = (metrics->shifty % align)? align - (metrics->shifty % align): 0;
	else
		metrics->scanpad = 0;

	metrics->shifty += metrics->scanpad;


	metrics->maxbit = 128;
	metrics->allbits = 255;
	for (i = 1; i < metrics->bytedepth; i++)
	{
		metrics->maxbit *= 256;
		metrics->allbits *= 256;
		metrics->allbits |= 255;
	}

	metrics->leadingbits = 8;
	for (i = 1; i < 8; i++)
	{
		metrics->leadingbits *= 16;
		metrics->leadingbits |= 8;
	}

	// generate a look up table for getting coverage masks
	for (i = 0; i < 256; i++)
	{
		metrics->bitlu[i] = 0;
		for(bit = 128, j = 24; bit; bit >>= 1, j -= 3)
			metrics->bitlu[i] |= (i & bit) << j;
	}

	// square the image
	*span = 4;
	while ((*span < metrics->width) || (*span < metrics->height))
		*span <<= 1;

	metrics->collapsebits = 0;

	if (flags & QBIT_COMPRESS)
	{
		// process every second level down to the last
		// make sure to skip the first tilesize
		for(bit = COLLAPSE_LEAF; bit < (*span >> 1) ; bit <<= 2)
			metrics->collapsebits |= bit;

		for (i = 0; i < 256; i++)
		{
			metrics->weight[i] = 0;
			switch(depth)
			{
			case 24:

				for(bit = 128, shift = 16; bit; shift -= 2, bit >>= 1)
					metrics->weight[i] |= (i & bit) << shift;

				// mapping to undo the weighting
				metrics->unweight0[i] =
					  ((i & 128) << 16)	// 1 of 0
					| ((i & 64) << 9) 	// 1 of 1
					| ((i & 32) << 2) 	// 1 of 2
					| ((i & 16) << 18) 	// 2 of 0
					| ((i & 8) << 11) 	// 2 of 1
					| ((i & 4) << 4) 	// 2 of 2
					| ((i & 2) << 20) 	// 3 of 0
					| ((i & 1) << 13); 	// 3 of 1

				metrics->unweight1[i] =
					  ((i & 128) >> 2)	// 3 of 2
					| ((i & 64) << 14)  // 4 of 0
					| ((i & 32) << 7) 	// 4 of 1
					| (i & 16) 			// 4 of 2
					| ((i & 8) << 16)  	// 5 of 0
					| ((i & 4) << 9) 	// 5 of 1
					| ((i & 2) << 2)  	// 5 of 2
					| ((i & 1) << 18); 	// 6 of 0

				metrics->unweight2[i] =
					  ((i & 128) << 3)	// 6 of 1
					| ((i & 64) >> 4)  	// 6 of 2
					| ((i & 32) << 12) 	// 7 of 0
					| ((i & 16) << 5)	// 7 of 1
					| ((i & 8) >> 2)  	// 7 of 2
					| ((i & 4) << 14) 	// 8 of 0
					| ((i & 2) << 7)  	// 8 of 1
					| (i & 1); 	// 8 of 2
				break;
			case 16:


				for(bit = 128, shift = 8; bit; shift -= 1, bit >>= 1)
					metrics->weight[i] |= (i & bit) << shift;

				// mapping to undo the weighting
				metrics->unweight0[i] =
					  ((i & 128) << 8)	// 1 of 0
					| ((i & 64) << 1) 	// 1 of 1
					| ((i & 32) << 9) 	// 2 of 0
					| ((i & 16) << 2) 	// 2 of 1
					| ((i & 8) << 10) 	// 3 of 0
					| ((i & 4) << 3) 	// 3 of 1
					| ((i & 2) << 11) 	// 4 of 0
					| ((i & 1) << 4); 	// 4 of 1

				metrics->unweight1[i] =
					  ((i & 128) << 4)	// 5 of 0
					| ((i & 64) >> 3)  	// 5 of 1
					| ((i & 32) << 5)	// 6 of 0
					| ((i & 16) >> 2)	// 6 of 1
					| ((i & 8) << 6)  	// 7 of 0
					| ((i & 4) >> 1) 	// 7 of 1
					| ((i & 2) << 7)  	// 8 of 0
					| (i & 1); 			// 8 of 1
				break;
			}
		}
	}
}



// ===================================
// encoder entry point
// ===================================
Qlong encodeimage(
	Qbyte *image,
	Qlong width,
	Qlong height,
	Qlong depth,
	Qlong flags,
	Qlong options,
	QBITDATAHEADER *dataheader,
	Qbyte *buffer,
	Qlong buffersize)
{

	METRICS metrics;
	STREAM buf;
	Qbyte  *tmpdata;
	Qlong span, maxcol, tmpdatasize;
	long i;

	// this will be set with the total data size and the number of
	// 16 bit masks at the start of the quad stream
	dataheader->size = 0;
	dataheader->maskcount = 0;

	if (!image)
		return QBIT_ERROR_NULL_IMAGE;

	// sanity check
	if ((depth != 16) && (depth != 24) && (depth != 32))
		return QBIT_ERROR_UNSUPPORTED_COLOUR_DEPTH;;

	if (buffersize < depth)
		return QBIT_ERROR_OUT_OF_BUFFER_SPACE;;


	getmetrics(&metrics, width, height, depth, flags, options, &span);


	// output buffer uses half bytes
	buf.start = buffer;
	buf.current = (buffer + (buffersize - 1));
	buf.low = 1;


	if (flags & QBIT_COMPRESS)
	{
		// compression may run against a copy of the image
		// and calls the first colour collapse function
		if (options & QBIT_OPTION_NO_COPY)
			metrics.startstream = image;
		else
		{
			metrics.startstream = (Qbyte *)malloc(height * metrics.shifty);
			memcpy(metrics.startstream, image, height * metrics.shifty);
		}

		maxcol = collapseall(&metrics, &buf);
	}
	else
		metrics.startstream = image;


	// set up a temporary buffer
	tmpdatasize = 0;
	for(i = (long)span; i >= 2; i >>= 1)
		tmpdatasize += depth;

	tmpdata = (Qbyte *)malloc(tmpdatasize);

	metrics.masks = (Qint *)buffer;
	metrics.nextmask = 0;


	// recursively encode the image
	encodetile(0, 0, span >> 1, &metrics, &buf, tmpdata, maxcol);
	dataheader->maskcount = metrics.nextmask;


	// pack the first header onto end of compressed data
	for (i = metrics.bitdepth - 1; i >= 0; i--)
	{
		if (buf.low)
		{
			*(buf.current) = tmpdata[i];
			buf.current--;
		}
		else
		{
			*(buf.current) |= (tmpdata[i] & 240);
			buf.current--;
			*(buf.current) = (tmpdata[i] & 15);
		}
	}


	// fre hte tmp buffer used to pass
	// information up the tree
	free(tmpdata);


	if (flags & QBIT_COMPRESS)
	{
		// write colour count tp "start" of quad stream
		for(i = 0; i <= 28; i += 4)
			WRITE_QUAD ((&buf), ((maxcol & (15 << i)) >> i));

		// free palette table used by collapse preprocess
		if (maxcol)
			free(metrics.dhash);

		// compression may have setup a copy of the image
		if ((options & QBIT_OPTION_NO_COPY) == 0)
			free(metrics.startstream);
	}


	// set the last of the last byte (first read) true
	// if the most leftmost quad in the data stream is used
	if (buf.low)
	{
		*(buf.current) = 255;
	}
	else
	{
		buf.current--;
		*(buf.current) = 0;
	}


	// the control stream was written to the end of bufffer,
	// move it so it follows the 16 bit masks
	buffer += (dataheader->maskcount << 1);

	if (buf.current < buffer)
		return QBIT_ERROR_OUT_OF_BUFFER_SPACE;;

	dataheader->size = buffersize - (buf.current - buf.start);
	memmove(buffer, buf.current, dataheader->size);

	//
	dataheader->size += (dataheader->maskcount << 1);

	return (0);
}



// ==================================
// node encoder
// ------------------------------
int encodetile(
	Qlong x,
	Qlong y,
	Qlong tilespan,
	METRICS *metrics,
	STREAM *buf,
	Qbyte *data,
	Qlong maxcount)
{

	Qbyte blockbit, statebit, fakebit, test;
	Qlong mymax;
	long i;

	fakebit = 0;

	memset(data, 0, metrics->bitdepth);


	//  iterate the quadrants backwards
	for (blockbit = 1, statebit = 16; blockbit <= 8; blockbit <<= 1, statebit <<= 1)
	{
		// tile into the image recursively backwards
		switch (statebit)
		{
		case 32: case 128: x -= tilespan; break;
		case 64: x += tilespan; y -= tilespan; break;
		case 16: y += tilespan; x += tilespan; break;
		}

		data += metrics->bitdepth;
		if ((x < metrics->width) && (y < metrics->height))
		{
			// compression preprocess
			if (tilespan & metrics->collapsebits)
				mymax = collapse(x, y, metrics, buf, tilespan, maxcount);
			else
				mymax = maxcount;

			// sub call out and advance pointer to one past the end of the sub block of control bytes
			if (tilespan > 4)
				encodetile(x, y, tilespan >> 1, metrics, buf, data, mymax);
			else
				encodeblock(x, y, metrics, data);
		}
		else
		{
			// simulate no coverage for area outside actual image
			memset(data, 15, metrics->bitdepth);
			fakebit |= statebit;
		}

		// analyse the control bits passed into sub call and populate
		// the appropriate bits in quadmask
		for (i = metrics->bitdepth; i; i--)
		{
			data--;

			// get this sub quadrants data bits
			test = *(data + metrics->bitdepth);

			switch (test)
			{
			case 15:

				// a block of no bits, set the blockbit clear the statebit
				*data |= blockbit;
				break;
			case 255:

				// block of bits, set the blockbit and the statebit
				*data |= blockbit;
				*data |= statebit;
				break;
			default:

				// there are no partial block bits for the 16 pixel block
				if (tilespan > 4)
				{
	 				if (test & 15)
					{
						*data |= statebit;

						if (buf->low)
						{
							*(buf->current) = test;
							buf->current--;
						}
						else
						{
							*(buf->current) |= test & 240;
							buf->current--;
							*(buf->current) = test & 15;
						}
					}
					else
					{
						// there are no statebits without blockbits from the tilespan==4 block
						if (tilespan > 8)
						{
							if (buf->low)
							{
								*(buf->current) = test >> 4;
								buf->low = 0;
							}
							else
							{
								*(buf->current) |= (test & 240);
								buf->current--;
								buf->low = 1;
							}
						}
					}
				}
				break;
			}
		}
	}


	// update the fake bit control bits
	if (fakebit)
	{
		test = (128 | 8);
		for (i = 1; i <= metrics->bitdepth; i++, data++)
			if ((*data & test) == test)
				*data |= fakebit;
	}

	return (1);
}



// ==================================
// leaf encoder
// where a bit in all 16 pixels has the same value
// it is encoded into the control stream (data), otherwise
// it ends up written out as a 16 bit mask to raw
// ------------------------------
void encodeblock(
	Qlong x,
	Qlong y,
	METRICS *metrics,
	Qbyte *data)
{
	Qlong masks[32];
	Qlong *coverage;
	Qlong *mask;
	Qlong testbits;
	Qbyte blockbit, shift, inventbits;
	long j, i;

	memset (masks, 0, 128);

	inventbits = 0;

	coverage = data;

	for (blockbit = 8, shift = 12; blockbit; blockbit >>= 1, shift -= 4)
	{
		switch (blockbit)
		{
			case 4: case 1:
				x += 2;
				break;
			case 2:
				y += 2;
				x -= 2;
				break;
		}

		if ((y < metrics->height) && (x < metrics->width))
			getquad(coverage, x, y, metrics);
		else
			inventbits |= (15 << shift);

		mask = masks;
		for ( i = 0; i < metrics->bytedepth; i++)
			for (testbits = (15 << 28), j = 28; testbits; testbits >>= 4, j -= 4, mask++)
				*mask |= (testbits & coverage[i]) >> j << shift;
	}


	mask = masks;
	for (i = 0; i < metrics->bitdepth; i++, mask++, data++)
	{
		// or the invented bits onto the mask if
		// the mask has any coverage at all
		if (*mask)
			*mask |= inventbits;

		switch(*mask)
		{
		case 0:

			*data = 15;
			break;
		case 65535:

			*data = 255;
			break;
		default:

			*data = 0;
			metrics->masks[metrics->nextmask++] = (Qint)*mask;
			break;
		}
	}
}


// ========================================
// get coverage mask of four pixels
// ----------------------------------
void getquad(
	Qlong *coverage,
	const Qlong x,
	const Qlong y,
	METRICS *metrics)
{

	Qbyte *image;
	Qlong done;
	long i;

	image = LOCATION(x, y, metrics);

	done = 8;
	for (i = 0; i < metrics->bytedepth; i++)
		coverage[i] = metrics->bitlu[image[i]];

	image += metrics->bytedepth;

	if ((x + 1) < metrics->width)
	{
		done |= 4;
		for (i = 0; i < metrics->bytedepth; i++)
			coverage[i] |= (metrics->bitlu[image[i]] >> 1);
	}


	if ((y + 1) < metrics->height)
	{
		done |= 2;
		image = LOCATION(x, (y + 1), metrics);
		for (i = 0; i < metrics->bytedepth; i++)
			coverage[i] |= (metrics->bitlu[image[i]] >> 2);

		image += metrics->bytedepth;

		if ((x + 1) < metrics->width)
		{
			done |= 1;
			for (i = 0; i < metrics->bytedepth; i++)
				coverage[i] |= (metrics->bitlu[image[i]] >> 3);
		}
	}

	// set coverage outside the image to be the same as the control pixel
	if (done != 15)
	{
		for (i = 0; i < metrics->bytedepth; i++)
		{
			if ((done & 1) == 0) coverage[i] |= ((*coverage & metrics->leadingbits) >> 3);
			if ((done & 2) == 0) coverage[i] |= ((*coverage & metrics->leadingbits) >> 2);
			if ((done & 4) == 0) coverage[i] |= ((*coverage & metrics->leadingbits) >> 1);
		}
	}
}


// =================================
// compression pre process,
// removes unused colour value and
// writes reconstruct data to buf
// -----------------------------
Qlong collapseall(
	METRICS *metrics,
	STREAM *buf)
{

	CACHE cache[COLLAPSE_CACHE_MAX + 1];
	REGISTERS reg;
	Qlong *pal;
	Qbyte *image;
	Qlong col, count, idx, testbit, bit, colbase;
	long i, j;


	pal = (Qlong *)malloc(((metrics->allbits >> 5) + 1) * 4);
	memset(pal, 0, ((metrics->allbits >> 5) + 1) * 4);


	count = 0;
	image = metrics->startstream;
	for (i = 0; i < metrics->height; i++, image += metrics->scanpad)
	{
		for (j = 0; j < metrics->width; j++)
		{
			switch(metrics->bytedepth)
			{
			case 3:

				col = MAPWEIGHT_TRUE(metrics, image);
				image += 3;
				break;
			case 2:

				col = MAPWEIGHT_HIGH(metrics, image);
				image += 2;
				break;
			case 4:

				col = GET_COLOUR_DWORD(image);
				image += 4;
 				break;
			case 1:

				col = (Qlong)*image++;
				break;
			}

			bit = 1 << (col & 31);

			if ((pal[col >> 5] & bit) == 0)
			{
				count++;
			    pal[col >> 5] |= bit;
			}
		}
	}

	metrics->dhash = (Qlong *)malloc(count * 4);
	memset(metrics->dhash, 0, count * 4);

	// this stores tempory lookups into the palette in
	// order skip the binary search
	memset(cache, 0, (COLLAPSE_CACHE_MAX + 1) * 8);

	// write out the palette map
	// and load the palette array
	reg.init = 0;
	idx = count - 1;
	for(i = (metrics->allbits >> 5); i >= 0; i--)
		if (pal[i])
		{
			colbase = i << 5;
			for(j = 31, testbit = (1 << 31); j >= 0; j--, testbit >>= 1)
				if (pal[i] & testbit)
				{
					col = colbase | j;

					cache[col & COLLAPSE_CACHE_MAX].colour = col;
					cache[col & COLLAPSE_CACHE_MAX].index = idx;

					metrics->dhash[idx--] = col;
					regout(&reg, buf, col);
				}
		}


	// write the last colour out
	for(i = 0; i <= reg.lastreg; i++)
		WRITE_QUAD(buf, reg.registers[i]);


	free(pal);


	// update the image at metrics->startstream
  	image = metrics->startstream;
	for (i = 0; i < metrics->height; i++, image += metrics->scanpad)
	{
		for (j = 0; j < metrics->width; j++)
		{
			switch(metrics->bytedepth)
			{
			case 3:

				col = MAPWEIGHT_TRUE(metrics, image);
				break;
			case 2:

 				col = MAPWEIGHT_HIGH(metrics, image);
				break;
			case 4:

				col = GET_COLOUR_DWORD(image);
				break;
			case 1:

				col = (Qlong)*image;
				break;
			}

			if (cache[col & COLLAPSE_CACHE_MAX].colour == col)
				idx = cache[col & COLLAPSE_CACHE_MAX].index;
			else
			{
				idx = bin(0, count - 1, metrics->dhash, col);
				cache[col & COLLAPSE_CACHE_MAX].colour = col;
				cache[col & COLLAPSE_CACHE_MAX].index = idx;
			}

			switch(metrics->bytedepth)
			{
			case 3:

				PUT_COLOUR_TRUE_ADVANCE(image, idx);
				break;
			case 2:

				PUT_COLOUR_HIGH_ADVANCE(image, idx);
				break;
			case 4:

				PUT_COLOUR_DWORD_ADVANCE(image, idx);
				break;
			case 1:

				*image++ = idx & 255;
				break;
			}
		}
	}

	return (count);
}



// ===================================
// binary search
// -------------------------------
Qlong bin(const Qlong lbnd, const Qlong ubnd, Qlong *pal, const Qlong test)
{
	Qlong mid;

	mid = (ubnd + lbnd) >> 1;
	if (pal[mid] == test)
		return mid;
	else
		if (pal[mid] > test)
			return bin (lbnd, mid - 1, pal, test);
		else
			return bin (mid + 1, ubnd, pal, test);
}



// =================================
// compression pre process,
// removes unused colour value and
// writes recontruct data to buf
// -----------------------------
Qlong collapse(
	Qlong x,
	Qlong y,
	METRICS *metrics,
	STREAM *buf,
	Qlong tilespan,
	Qlong count)
{

	REGISTERS reg;
	Qbyte *image;
	Qlong height, width, idx, col, mycount, wrap;
	long i, j;


	if (count == 0)
	{
		WRITE_QUAD(buf, 15);
		return 0;
	}


	// get the dimensions of the operation
	width = (x + tilespan) <= metrics->width? tilespan: metrics->width - x;
	height = (y + tilespan) <= metrics->height? tilespan: metrics->height - y;
	wrap = metrics->shifty - (width * metrics->bytedepth);

	memset(metrics->dhash, 0, count * 4);

	mycount = 0;

	// mark the hash table with used colours
	image = LOCATION(x, y, metrics);
	for (i = 0; i < height; i++, image += wrap)
		for (j = 0; j < width; j++)
		{
			switch(metrics->bytedepth)
			{
			case 3:
				col = GET_COLOUR_TRUE(image);
				image += 3;
				break;
			case 2:

				col = GET_COLOUR_HIGH(image);
				image += 2;
				break;
			case 4:

				col = GET_COLOUR_DWORD(image);
				image += 4;
				break;
			case 1:

				col = *image++;
				break;
			}

			if (!metrics->dhash[col])
			{
				metrics->dhash[col] = 1;
				mycount++;
			}
		}



	// conditions where we should skip the palette
	// write all bits meaning no palette
	if ((mycount * 3) > (width * height * 2))
	{
		WRITE_QUAD(buf, 15);
		return 0;
	}

	if ((mycount * 3) > (count * 2))
	{
		WRITE_QUAD(buf, 15);
		return 0;
	}


	// output the palette
	reg.init = 0;
	idx = count;
	for(i = (count - 1); i >= 0; i--)
		if (metrics->dhash[i])
		{
			metrics->dhash[i] = --idx;
			regout(&reg, buf, i);
		}


	// write the last colour out
	for(i = 0; i <= reg.lastreg; i++)
		WRITE_QUAD(buf, reg.registers[i]);


	// write the palette index back to the image
 	image = LOCATION(x, y, metrics);
	for (i = 0; i < height; i++, image += wrap)
		for (j = 0; j < width; j++)
			switch(metrics->bytedepth)
			{
			case 3:

				col = metrics->dhash[GET_COLOUR_TRUE(image)];
				PUT_COLOUR_TRUE_ADVANCE(image, (col - idx));
				break;
			case 2:

				col = metrics->dhash[GET_COLOUR_HIGH(image)];
				PUT_COLOUR_HIGH_ADVANCE(image, (col - idx));
				break;
			case 4:

				col = metrics->dhash[GET_COLOUR_DWORD(image)];
				PUT_COLOUR_DWORD_ADVANCE(image, (col - idx));
				break;
			case 1:

				col = metrics->dhash[*image];
				*image++ = col & 255;
				break;
			}


	// write the index of the highest register needed to
	// encode the palette
	WRITE_QUAD (buf, reg.lastreg);


	//return the number of colours in this palette pass
	return (count - idx);
}


// ======================================
// dump registers for stuff that has changed
// -----------------------------------
void regout(REGISTERS *reg, STREAM *buf, Qlong const col)
{
	Qlong mask, tmpcol, shift;
	long i, maxreg;

	if (reg->init == 0)
	{
		reg->masks[0] = 3;
		reg->shifts[0] = 0;
		for(i = 1, mask = (reg->masks[0] << 2), shift = 2; i < 12; i++, mask <<= 2, shift += 2)
		{
			reg->masks[i] = mask;
			reg->shifts[i] = shift;
		}

		tmpcol = 0;
		for(i = 0; i < 12; i++)
		{
			tmpcol |= reg->masks[i];
			if (tmpcol >= col)
				break;
		}

		reg->lastreg = i;
		for(i = 0; i <= reg->lastreg; i++)
			reg->registers[i] = (1 << ((col & reg->masks[i]) >> reg->shifts[i]));

		reg->init = 1;
	}
	else
	{
		// get the highest 2 bits that match the last colour
		tmpcol = col ^ reg->current;
		for(maxreg = reg->lastreg; maxreg; maxreg--)
		{
			if (reg->masks[maxreg] & tmpcol)
				break;
		}


		// write out the number from the bottom up

		for(i = 0; i < maxreg; i++)
		{
			WRITE_QUAD(buf, reg->registers[i]);
			reg->registers[i] = 1 << ((col & reg->masks[i]) >> reg->shifts[i]);
		}

		reg->registers[maxreg] |= 1 << ((col & reg->masks[maxreg]) >> reg->shifts[maxreg]);
	}

	reg->current = col;
}




// =======================================
// decoder entry point
// ------------------------------------
Qlong decodeimage(
	Qbyte *buffer,
	Qlong width,
	Qlong height,
	Qlong depth,
	Qlong flags,
	Qlong options,
	QBITDATAHEADER *dataheader,
	Qbyte *data)
{

	METRICS metrics;
	STREAM buf;
	Qlong span;
	long i, maxpalette;

	if (flags)
	{
		if (flags != QBIT_COMPRESS)
			return QBIT_ERROR_UNKNOWN_FLAG;
	}

	getmetrics(&metrics, width, height, depth, flags, options, &span);
	metrics.startstream = buffer;


	// 16 bit masks start at the end of the masks and are
	// read backwards, nextmask is an array index
	metrics.masks = (Qint *)data;
	metrics.nextmask =  dataheader->maskcount - 1;;


 	// offset is the start of the encoded data
	buf.start = data + (dataheader->maskcount << 1);
	buf.current = buf.start;

	// determine if the first quad is the top or bottom
	// of the first byte
	buf.low = *(buf.current)? 0: 255;
	buf.current++;

	if (flags & QBIT_COMPRESS)
	{
		// read the size of the colour palette
		// and set up memory buffer for look ups
		maxpalette = 0;
		for(i = 28; i >= 0; i -= 4)
			maxpalette |= ((Qlong)readquad(&buf) << i);

		if (maxpalette)
			metrics.dhash = (Qlong *)malloc(maxpalette * 4);
	}


	decodetile(0, 0, &metrics, span >> 1, 0, 0, metrics.allbits, &buf);

	if ((flags & QBIT_COMPRESS) && (maxpalette))
	{
		// run final decompression pass and free
		// look palette
		uncollapse(0, 0, &metrics, &buf, 0);
		free(metrics.dhash);
	}

	return(0);
}



// =============================
// node decoder
// ------------------------
void decodetile(
	Qlong x,
	Qlong y,
	METRICS *metrics,
	Qlong segment,
	Qlong setbits,
	Qlong nobits,
	Qlong controlbits,
	STREAM *buf)
{

	Qlong masks[32];
	Qlong testbit, subcontrolbits, subsetbits, subnobits, getcontrolbits;
	Qbyte statebit, blockbit;
	long idx;

	getcontrolbits = metrics->allbits - (setbits | nobits);

	// read in the masks
	memset (masks, 0, 128);
	for (idx = 0, testbit = metrics->maxbit; testbit; testbit >>= 1, idx++)
		if (testbit & getcontrolbits)
			if (controlbits & testbit)
			{
				if (buf->low)
					masks[idx] = buf->current[0] & 15 | buf->current[1] & 240;
				else
					masks[idx] = *buf->current;

				buf->current++;
			}
			else
			{
				if (segment > 4)
				{
					// read the quad into the high bits
					if (buf->low)
 					{
						masks[idx] = (*buf->current << 4) & 240;
						buf->current++;
						buf->low = 0;
					}
					else
					{
						masks[idx] = *buf->current & 240;
						buf->low = 1;
					}
				}
			}


	// interpret the masks
	for (blockbit = 8, statebit = 128; blockbit; blockbit >>= 1, statebit >>= 1)
	{
		subnobits = nobits;
		subsetbits = setbits;
		subcontrolbits = 0;

		for (idx = 0, testbit = metrics->maxbit; testbit; testbit >>= 1, idx++)
			if (masks[idx] & blockbit)
			{
				if (masks[idx] & statebit)
					subsetbits |= testbit;
				else
				    subnobits |= testbit;
			}
			else
			    if (masks[idx] & statebit)
					subcontrolbits |= testbit;


		// rotate around the sub tiles top left -> right, bottom left -> right
		switch (blockbit)
		{
		case 4: case 1:

			x += segment;
			break;
		case 2:

			y += segment;
			x -= segment;
			break;
		}

		// recurse into this tile's subtiles
		if ((x < metrics->width) && (y < metrics->height))
			if (segment == 4)
				decodeblock(x,  y, metrics, subsetbits, subnobits);
			else
				decodetile(x,  y, metrics, segment >> 1, subsetbits, subnobits, subcontrolbits, buf);

	}

	if ((segment << 1) & metrics->collapsebits)
		uncollapse(x - segment, y - segment, metrics, buf, segment << 1);
}



// =================================
// leaf decoder
// --------------------------------
void decodeblock(
	const Qlong x,
	const Qlong y,
	METRICS *metrics,
	const Qlong setbits,
	const Qlong nobits)
{

	Qlong pixes[16];
	Qlong testbit, checkbits;
	Qint quad;
	Qlong *curpix;
	Qbyte *image;
	Qlong wrap;
	long i, j, xcount, ycount;


	memset(pixes, 0, 64);

	checkbits = ~(setbits | nobits);
	checkbits &= metrics->allbits;

	for (testbit = 1; testbit <= checkbits; testbit <<= 1)
		if (testbit & checkbits)
		{
			quad = metrics->masks[metrics->nextmask--];
			if (quad & 32768) pixes[0] |= testbit;
			if (quad & 16384) pixes[1] |= testbit;

			if (quad & 2048) pixes[2] |= testbit;
			if (quad & 1024) pixes[3] |= testbit;

			if (quad & 8192) pixes[4] |= testbit;
			if (quad & 4096) pixes[5] |= testbit;

			if (quad & 512) pixes[6] |= testbit;
			if (quad & 256) pixes[7] |= testbit;

			if (quad & 128) pixes[8] |= testbit;
			if (quad & 64) pixes[9] |= testbit;

			if (quad & 8) pixes[10] |= testbit;
			if (quad & 4) pixes[11] |= testbit;

			if (quad & 32) pixes[12] |= testbit;
			if (quad & 16) pixes[13] |= testbit;

 			if (quad & 2) pixes[14] |= testbit;
			if (quad & 1) pixes[15] |= testbit;
		}


	xcount = (x + 3) < metrics->width?  4: metrics->width - x;
	ycount = (y + 3) < metrics->height?  4: metrics->height - y;
	wrap = metrics->shifty - (xcount * metrics->bytedepth);

	curpix = pixes;
	image = LOCATION(x, y, metrics);
	for(i = 0; i < ycount; i++)
	{
		for(j = 0; j < xcount; j++, curpix++)
			switch(metrics->bytedepth)
			{
			case 3:

				PUT_COLOUR_TRUE_ADVANCE(image, (*curpix | setbits));
				break;
			case 2:

				PUT_COLOUR_HIGH_ADVANCE(image, (*curpix | setbits));
				break;
			case 4:

				PUT_COLOUR_DWORD_ADVANCE(image, (*curpix | setbits));
				break;
			case 1:

				*image++ = (*curpix | setbits) & 255;
				break;
			}

		curpix += (4 - xcount);
		image += wrap;
	}
}



// =============================
// uncollapse
// read the palette in from the data stream
// and assign colour values
// ---------------------------
void uncollapse(
	const Qlong x,
	const Qlong y,
	METRICS *metrics,
	STREAM *buf,
	const Qlong tilespan)
{

	Qbyte *image;
	Qlong height, width, col, wrap, level;
	long i, j;


	if (tilespan)
	{
		width = (x + tilespan) <= metrics->width? tilespan: metrics->width - x;
		height = (y + tilespan) <= metrics->height? tilespan: metrics->height - y;

		// read in the size of the palette
		// in register elements used
		level = readquad(buf);
	}
	else
	{
		// this was the first run
		width = metrics->width;
		height = metrics->height;
		level = (metrics->bitdepth >> 1) - 1;
	}


	// if this quadrant isn't palettized at this
	// level the quad will have all bits set
	if (level == 15)
		return;


	wrap = metrics->shifty - (width * metrics->bytedepth);

	// read in the palette
	readpal(metrics->dhash, 0, buf, level * 2);

	// write the sort back to the image
	// the first thing the compressor does is transform the colour
	// into a weighted value.  if this is the last decompression
	// pass, reditribute the bits
	image = LOCATION(x, y, metrics);
	for (i = 0; i < height; i++, image += wrap)
		for (j = 0; j < width; j++)
			switch(metrics->bytedepth)
			{
			case 3:

				col = metrics->dhash[GET_COLOUR_TRUE(image)];
				if (!tilespan) col = UNMAP_TRUE(metrics, col);
				PUT_COLOUR_TRUE_ADVANCE(image, col);
				break;
			case 2:

				col = metrics->dhash[GET_COLOUR_HIGH(image)];
				if (!tilespan) col = UNMAP_HIGH(metrics, col);
				PUT_COLOUR_HIGH_ADVANCE(image, col);
				break;
			case 4:

				col = metrics->dhash[GET_COLOUR_DWORD(image)];
				if (!tilespan) col = col;
				PUT_COLOUR_DWORD_ADVANCE(image, col);
				break;
			default:

				col = metrics->dhash[*image];
				if (!tilespan) col = col;
				*image = col & 255;
				image ++;
				break;
			}
}



// =======================================
// this is a recursive call to read the
// palette in from the "quad" stream
// -----------------------------------
Qlong *readpal(
	Qlong *pal,
	const Qlong mask,
	STREAM *buf,
	const Qlong shift)
{

	Qlong testmask;

	testmask = readquad(buf);

	if (shift)
	{
		if (testmask & 1)
			pal = readpal(pal, mask, buf, shift - 2);

		if (testmask & 2)
			pal = readpal(pal, mask | (1 << shift), buf, shift - 2);

		if (testmask & 4)
			pal = readpal(pal, mask | (2 << shift), buf, shift - 2);

		if (testmask & 8)
			pal = readpal(pal, mask | (3 << shift), buf, shift - 2);
	}
	else
	{
		if (testmask & 1)
			*pal++ = mask;

		if (testmask & 2)
			*pal++ = mask | 1;

		if (testmask & 4)
			*pal++ = mask | 2;

		if (testmask & 8)
			*pal++ = mask | 3;
	}

	return pal;
}



// ================================
// read and write one quads/bytes
//  to the stream
// --------------------------
Qbyte readquad(STREAM *buf)
{
	Qbyte tmp;

	if (buf->low)
	{
		tmp = *(buf->current) & 15;
		buf->current++;
		buf->low = 0;
	}
	else
	{
		tmp = *(buf->current) >> 4;
		buf->low = 1;
	}

	return tmp;
}

