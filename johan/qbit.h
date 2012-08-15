// flagse
#define QBIT_COMPRESS 1

// options
#define QBIT_OPTION_NOT_ALIGNED 16
#define QBIT_OPTION_INT_ALIGNED 32
#define QBIT_OPTION_NO_COPY 512

// error codes
#define QBIT_ERROR_NULL_IMAGE 1;
#define QBIT_ERROR_OUT_OF_BUFFER_SPACE 2
#define QBIT_ERROR_UNSUPPORTED_COLOUR_DEPTH 3
#define QBIT_ERROR_UNKNOWN_FLAG 4


typedef struct tagQBITDATAHEADER
{
	unsigned long size;
	unsigned long maskcount;
}QBITDATAHEADER;


typedef unsigned long Qlong;
typedef unsigned char Qbyte;
typedef unsigned short Qint;

Qlong encodeimage(Qbyte *, Qlong, Qlong, Qlong, Qlong, Qlong, QBITDATAHEADER *, Qbyte *, Qlong);
Qlong decodeimage(Qbyte *, Qlong, Qlong, Qlong, Qlong, Qlong, QBITDATAHEADER *, Qbyte *);

long getdebugcount(void);
