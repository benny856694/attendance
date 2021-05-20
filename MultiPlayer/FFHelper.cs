﻿using System;
using System.Runtime.InteropServices;

namespace VideoHelper
{
    public enum AVCodecID
    {
        AV_CODEC_ID_NONE,

        /* video codecs */
        AV_CODEC_ID_MPEG1VIDEO,
        AV_CODEC_ID_MPEG2VIDEO, ///< preferred ID for MPEG-1/2 video decoding
        AV_CODEC_ID_H261,
        AV_CODEC_ID_H263,
        AV_CODEC_ID_RV10,
        AV_CODEC_ID_RV20,
        AV_CODEC_ID_MJPEG,
        AV_CODEC_ID_MJPEGB,
        AV_CODEC_ID_LJPEG,
        AV_CODEC_ID_SP5X,
        AV_CODEC_ID_JPEGLS,
        AV_CODEC_ID_MPEG4,
        AV_CODEC_ID_RAWVIDEO,
        AV_CODEC_ID_MSMPEG4V1,
        AV_CODEC_ID_MSMPEG4V2,
        AV_CODEC_ID_MSMPEG4V3,
        AV_CODEC_ID_WMV1,
        AV_CODEC_ID_WMV2,
        AV_CODEC_ID_H263P,
        AV_CODEC_ID_H263I,
        AV_CODEC_ID_FLV1,
        AV_CODEC_ID_SVQ1,
        AV_CODEC_ID_SVQ3,
        AV_CODEC_ID_DVVIDEO,
        AV_CODEC_ID_HUFFYUV,
        AV_CODEC_ID_CYUV,
        AV_CODEC_ID_H264,
        AV_CODEC_ID_INDEO3,
        AV_CODEC_ID_VP3,
        AV_CODEC_ID_THEORA,
        AV_CODEC_ID_ASV1,
        AV_CODEC_ID_ASV2,
        AV_CODEC_ID_FFV1,
        AV_CODEC_ID_4XM,
        AV_CODEC_ID_VCR1,
        AV_CODEC_ID_CLJR,
        AV_CODEC_ID_MDEC,
        AV_CODEC_ID_ROQ,
        AV_CODEC_ID_INTERPLAY_VIDEO,
        AV_CODEC_ID_XAN_WC3,
        AV_CODEC_ID_XAN_WC4,
        AV_CODEC_ID_RPZA,
        AV_CODEC_ID_CINEPAK,
        AV_CODEC_ID_WS_VQA,
        AV_CODEC_ID_MSRLE,
        AV_CODEC_ID_MSVIDEO1,
        AV_CODEC_ID_IDCIN,
        AV_CODEC_ID_8BPS,
        AV_CODEC_ID_SMC,
        AV_CODEC_ID_FLIC,
        AV_CODEC_ID_TRUEMOTION1,
        AV_CODEC_ID_VMDVIDEO,
        AV_CODEC_ID_MSZH,
        AV_CODEC_ID_ZLIB,
        AV_CODEC_ID_QTRLE,
        AV_CODEC_ID_TSCC,
        AV_CODEC_ID_ULTI,
        AV_CODEC_ID_QDRAW,
        AV_CODEC_ID_VIXL,
        AV_CODEC_ID_QPEG,
        AV_CODEC_ID_PNG,
        AV_CODEC_ID_PPM,
        AV_CODEC_ID_PBM,
        AV_CODEC_ID_PGM,
        AV_CODEC_ID_PGMYUV,
        AV_CODEC_ID_PAM,
        AV_CODEC_ID_FFVHUFF,
        AV_CODEC_ID_RV30,
        AV_CODEC_ID_RV40,
        AV_CODEC_ID_VC1,
        AV_CODEC_ID_WMV3,
        AV_CODEC_ID_LOCO,
        AV_CODEC_ID_WNV1,
        AV_CODEC_ID_AASC,
        AV_CODEC_ID_INDEO2,
        AV_CODEC_ID_FRAPS,
        AV_CODEC_ID_TRUEMOTION2,
        AV_CODEC_ID_BMP,
        AV_CODEC_ID_CSCD,
        AV_CODEC_ID_MMVIDEO,
        AV_CODEC_ID_ZMBV,
        AV_CODEC_ID_AVS,
        AV_CODEC_ID_SMACKVIDEO,
        AV_CODEC_ID_NUV,
        AV_CODEC_ID_KMVC,
        AV_CODEC_ID_FLASHSV,
        AV_CODEC_ID_CAVS,
        AV_CODEC_ID_JPEG2000,
        AV_CODEC_ID_VMNC,
        AV_CODEC_ID_VP5,
        AV_CODEC_ID_VP6,
        AV_CODEC_ID_VP6F,
        AV_CODEC_ID_TARGA,
        AV_CODEC_ID_DSICINVIDEO,
        AV_CODEC_ID_TIERTEXSEQVIDEO,
        AV_CODEC_ID_TIFF,
        AV_CODEC_ID_GIF,
        AV_CODEC_ID_DXA,
        AV_CODEC_ID_DNXHD,
        AV_CODEC_ID_THP,
        AV_CODEC_ID_SGI,
        AV_CODEC_ID_C93,
        AV_CODEC_ID_BETHSOFTVID,
        AV_CODEC_ID_PTX,
        AV_CODEC_ID_TXD,
        AV_CODEC_ID_VP6A,
        AV_CODEC_ID_AMV,
        AV_CODEC_ID_VB,
        AV_CODEC_ID_PCX,
        AV_CODEC_ID_SUNRAST,
        AV_CODEC_ID_INDEO4,
        AV_CODEC_ID_INDEO5,
        AV_CODEC_ID_MIMIC,
        AV_CODEC_ID_RL2,
        AV_CODEC_ID_ESCAPE124,
        AV_CODEC_ID_DIRAC,
        AV_CODEC_ID_BFI,
        AV_CODEC_ID_CMV,
        AV_CODEC_ID_MOTIONPIXELS,
        AV_CODEC_ID_TGV,
        AV_CODEC_ID_TGQ,
        AV_CODEC_ID_TQI,
        AV_CODEC_ID_AURA,
        AV_CODEC_ID_AURA2,
        AV_CODEC_ID_V210X,
        AV_CODEC_ID_TMV,
        AV_CODEC_ID_V210,
        AV_CODEC_ID_DPX,
        AV_CODEC_ID_MAD,
        AV_CODEC_ID_FRWU,
        AV_CODEC_ID_FLASHSV2,
        AV_CODEC_ID_CDGRAPHICS,
        AV_CODEC_ID_R210,
        AV_CODEC_ID_ANM,
        AV_CODEC_ID_BINKVIDEO,
        AV_CODEC_ID_IFF_ILBM,
        //#define AV_CODEC_ID_IFF_BYTERUN1 AV_CODEC_ID_IFF_ILBM
        AV_CODEC_ID_KGV1,
        AV_CODEC_ID_YOP,
        AV_CODEC_ID_VP8,
        AV_CODEC_ID_PICTOR,
        AV_CODEC_ID_ANSI,
        AV_CODEC_ID_A64_MULTI,
        AV_CODEC_ID_A64_MULTI5,
        AV_CODEC_ID_R10K,
        AV_CODEC_ID_MXPEG,
        AV_CODEC_ID_LAGARITH,
        AV_CODEC_ID_PRORES,
        AV_CODEC_ID_JV,
        AV_CODEC_ID_DFA,
        AV_CODEC_ID_WMV3IMAGE,
        AV_CODEC_ID_VC1IMAGE,
        AV_CODEC_ID_UTVIDEO,
        AV_CODEC_ID_BMV_VIDEO,
        AV_CODEC_ID_VBLE,
        AV_CODEC_ID_DXTORY,
        AV_CODEC_ID_V410,
        AV_CODEC_ID_XWD,
        AV_CODEC_ID_CDXL,
        AV_CODEC_ID_XBM,
        AV_CODEC_ID_ZEROCODEC,
        AV_CODEC_ID_MSS1,
        AV_CODEC_ID_MSA1,
        AV_CODEC_ID_TSCC2,
        AV_CODEC_ID_MTS2,
        AV_CODEC_ID_CLLC,
        AV_CODEC_ID_MSS2,
        AV_CODEC_ID_VP9,
        AV_CODEC_ID_AIC,
        AV_CODEC_ID_ESCAPE130,
        AV_CODEC_ID_G2M,
        AV_CODEC_ID_WEBP,
        AV_CODEC_ID_HNM4_VIDEO,
        AV_CODEC_ID_HEVC,
        //#define AV_CODEC_ID_H265 AV_CODEC_ID_HEVC
        AV_CODEC_ID_FIC,
        AV_CODEC_ID_ALIAS_PIX,
        AV_CODEC_ID_BRENDER_PIX,
        AV_CODEC_ID_PAF_VIDEO,
        AV_CODEC_ID_EXR,
        AV_CODEC_ID_VP7,
        AV_CODEC_ID_SANM,
        AV_CODEC_ID_SGIRLE,
        AV_CODEC_ID_MVC1,
        AV_CODEC_ID_MVC2,
        AV_CODEC_ID_HQX,
        AV_CODEC_ID_TDSC,
        AV_CODEC_ID_HQ_HQA,
        AV_CODEC_ID_HAP,
        AV_CODEC_ID_DDS,
        AV_CODEC_ID_DXV,
        AV_CODEC_ID_SCREENPRESSO,
        AV_CODEC_ID_RSCC,
        AV_CODEC_ID_AVS2,

        AV_CODEC_ID_Y41P = 0x8000,
        AV_CODEC_ID_AVRP,
        AV_CODEC_ID_012V,
        AV_CODEC_ID_AVUI,
        AV_CODEC_ID_AYUV,
        AV_CODEC_ID_TARGA_Y216,
        AV_CODEC_ID_V308,
        AV_CODEC_ID_V408,
        AV_CODEC_ID_YUV4,
        AV_CODEC_ID_AVRN,
        AV_CODEC_ID_CPIA,
        AV_CODEC_ID_XFACE,
        AV_CODEC_ID_SNOW,
        AV_CODEC_ID_SMVJPEG,
        AV_CODEC_ID_APNG,
        AV_CODEC_ID_DAALA,
        AV_CODEC_ID_CFHD,
        AV_CODEC_ID_TRUEMOTION2RT,
        AV_CODEC_ID_M101,
        AV_CODEC_ID_MAGICYUV,
        AV_CODEC_ID_SHEERVIDEO,
        AV_CODEC_ID_YLC,
        AV_CODEC_ID_PSD,
        AV_CODEC_ID_PIXLET,
        AV_CODEC_ID_SPEEDHQ,
        AV_CODEC_ID_FMVC,
        AV_CODEC_ID_SCPR,
        AV_CODEC_ID_CLEARVIDEO,
        AV_CODEC_ID_XPM,
        AV_CODEC_ID_AV1,
        AV_CODEC_ID_BITPACKED,
        AV_CODEC_ID_MSCC,
        AV_CODEC_ID_SRGC,
        AV_CODEC_ID_SVG,
        AV_CODEC_ID_GDV,
        AV_CODEC_ID_FITS,
        AV_CODEC_ID_IMM4,
        AV_CODEC_ID_PROSUMER,
        AV_CODEC_ID_MWSC,
        AV_CODEC_ID_WCMV,
        AV_CODEC_ID_RASC,
        AV_CODEC_ID_HYMT,
        AV_CODEC_ID_ARBC,
        AV_CODEC_ID_AGM,
        AV_CODEC_ID_LSCR,
        AV_CODEC_ID_VP4,

        /* various PCM "codecs" */
        AV_CODEC_ID_FIRST_AUDIO = 0x10000,     ///< A dummy id pointing at the start of audio codecs
        AV_CODEC_ID_PCM_S16LE = 0x10000,
        AV_CODEC_ID_PCM_S16BE,
        AV_CODEC_ID_PCM_U16LE,
        AV_CODEC_ID_PCM_U16BE,
        AV_CODEC_ID_PCM_S8,
        AV_CODEC_ID_PCM_U8,
        AV_CODEC_ID_PCM_MULAW,
        AV_CODEC_ID_PCM_ALAW,
        AV_CODEC_ID_PCM_S32LE,
        AV_CODEC_ID_PCM_S32BE,
        AV_CODEC_ID_PCM_U32LE,
        AV_CODEC_ID_PCM_U32BE,
        AV_CODEC_ID_PCM_S24LE,
        AV_CODEC_ID_PCM_S24BE,
        AV_CODEC_ID_PCM_U24LE,
        AV_CODEC_ID_PCM_U24BE,
        AV_CODEC_ID_PCM_S24DAUD,
        AV_CODEC_ID_PCM_ZORK,
        AV_CODEC_ID_PCM_S16LE_PLANAR,
        AV_CODEC_ID_PCM_DVD,
        AV_CODEC_ID_PCM_F32BE,
        AV_CODEC_ID_PCM_F32LE,
        AV_CODEC_ID_PCM_F64BE,
        AV_CODEC_ID_PCM_F64LE,
        AV_CODEC_ID_PCM_BLURAY,
        AV_CODEC_ID_PCM_LXF,
        AV_CODEC_ID_S302M,
        AV_CODEC_ID_PCM_S8_PLANAR,
        AV_CODEC_ID_PCM_S24LE_PLANAR,
        AV_CODEC_ID_PCM_S32LE_PLANAR,
        AV_CODEC_ID_PCM_S16BE_PLANAR,

        AV_CODEC_ID_PCM_S64LE = 0x10800,
        AV_CODEC_ID_PCM_S64BE,
        AV_CODEC_ID_PCM_F16LE,
        AV_CODEC_ID_PCM_F24LE,
        AV_CODEC_ID_PCM_VIDC,

        /* various ADPCM codecs */
        AV_CODEC_ID_ADPCM_IMA_QT = 0x11000,
        AV_CODEC_ID_ADPCM_IMA_WAV,
        AV_CODEC_ID_ADPCM_IMA_DK3,
        AV_CODEC_ID_ADPCM_IMA_DK4,
        AV_CODEC_ID_ADPCM_IMA_WS,
        AV_CODEC_ID_ADPCM_IMA_SMJPEG,
        AV_CODEC_ID_ADPCM_MS,
        AV_CODEC_ID_ADPCM_4XM,
        AV_CODEC_ID_ADPCM_XA,
        AV_CODEC_ID_ADPCM_ADX,
        AV_CODEC_ID_ADPCM_EA,
        AV_CODEC_ID_ADPCM_G726,
        AV_CODEC_ID_ADPCM_CT,
        AV_CODEC_ID_ADPCM_SWF,
        AV_CODEC_ID_ADPCM_YAMAHA,
        AV_CODEC_ID_ADPCM_SBPRO_4,
        AV_CODEC_ID_ADPCM_SBPRO_3,
        AV_CODEC_ID_ADPCM_SBPRO_2,
        AV_CODEC_ID_ADPCM_THP,
        AV_CODEC_ID_ADPCM_IMA_AMV,
        AV_CODEC_ID_ADPCM_EA_R1,
        AV_CODEC_ID_ADPCM_EA_R3,
        AV_CODEC_ID_ADPCM_EA_R2,
        AV_CODEC_ID_ADPCM_IMA_EA_SEAD,
        AV_CODEC_ID_ADPCM_IMA_EA_EACS,
        AV_CODEC_ID_ADPCM_EA_XAS,
        AV_CODEC_ID_ADPCM_EA_MAXIS_XA,
        AV_CODEC_ID_ADPCM_IMA_ISS,
        AV_CODEC_ID_ADPCM_G722,
        AV_CODEC_ID_ADPCM_IMA_APC,
        AV_CODEC_ID_ADPCM_VIMA,

        AV_CODEC_ID_ADPCM_AFC = 0x11800,
        AV_CODEC_ID_ADPCM_IMA_OKI,
        AV_CODEC_ID_ADPCM_DTK,
        AV_CODEC_ID_ADPCM_IMA_RAD,
        AV_CODEC_ID_ADPCM_G726LE,
        AV_CODEC_ID_ADPCM_THP_LE,
        AV_CODEC_ID_ADPCM_PSX,
        AV_CODEC_ID_ADPCM_AICA,
        AV_CODEC_ID_ADPCM_IMA_DAT4,
        AV_CODEC_ID_ADPCM_MTAF,
        AV_CODEC_ID_ADPCM_AGM,

        /* AMR */
        AV_CODEC_ID_AMR_NB = 0x12000,
        AV_CODEC_ID_AMR_WB,

        /* RealAudio codecs*/
        AV_CODEC_ID_RA_144 = 0x13000,
        AV_CODEC_ID_RA_288,

        /* various DPCM codecs */
        AV_CODEC_ID_ROQ_DPCM = 0x14000,
        AV_CODEC_ID_INTERPLAY_DPCM,
        AV_CODEC_ID_XAN_DPCM,
        AV_CODEC_ID_SOL_DPCM,

        AV_CODEC_ID_SDX2_DPCM = 0x14800,
        AV_CODEC_ID_GREMLIN_DPCM,

        /* audio codecs */
        AV_CODEC_ID_MP2 = 0x15000,
        AV_CODEC_ID_MP3, ///< preferred ID for decoding MPEG audio layer 1, 2 or 3
        AV_CODEC_ID_AAC,
        AV_CODEC_ID_AC3,
        AV_CODEC_ID_DTS,
        AV_CODEC_ID_VORBIS,
        AV_CODEC_ID_DVAUDIO,
        AV_CODEC_ID_WMAV1,
        AV_CODEC_ID_WMAV2,
        AV_CODEC_ID_MACE3,
        AV_CODEC_ID_MACE6,
        AV_CODEC_ID_VMDAUDIO,
        AV_CODEC_ID_FLAC,
        AV_CODEC_ID_MP3ADU,
        AV_CODEC_ID_MP3ON4,
        AV_CODEC_ID_SHORTEN,
        AV_CODEC_ID_ALAC,
        AV_CODEC_ID_WESTWOOD_SND1,
        AV_CODEC_ID_GSM, ///< as in Berlin toast format
        AV_CODEC_ID_QDM2,
        AV_CODEC_ID_COOK,
        AV_CODEC_ID_TRUESPEECH,
        AV_CODEC_ID_TTA,
        AV_CODEC_ID_SMACKAUDIO,
        AV_CODEC_ID_QCELP,
        AV_CODEC_ID_WAVPACK,
        AV_CODEC_ID_DSICINAUDIO,
        AV_CODEC_ID_IMC,
        AV_CODEC_ID_MUSEPACK7,
        AV_CODEC_ID_MLP,
        AV_CODEC_ID_GSM_MS, /* as found in WAV */
        AV_CODEC_ID_ATRAC3,
        AV_CODEC_ID_APE,
        AV_CODEC_ID_NELLYMOSER,
        AV_CODEC_ID_MUSEPACK8,
        AV_CODEC_ID_SPEEX,
        AV_CODEC_ID_WMAVOICE,
        AV_CODEC_ID_WMAPRO,
        AV_CODEC_ID_WMALOSSLESS,
        AV_CODEC_ID_ATRAC3P,
        AV_CODEC_ID_EAC3,
        AV_CODEC_ID_SIPR,
        AV_CODEC_ID_MP1,
        AV_CODEC_ID_TWINVQ,
        AV_CODEC_ID_TRUEHD,
        AV_CODEC_ID_MP4ALS,
        AV_CODEC_ID_ATRAC1,
        AV_CODEC_ID_BINKAUDIO_RDFT,
        AV_CODEC_ID_BINKAUDIO_DCT,
        AV_CODEC_ID_AAC_LATM,
        AV_CODEC_ID_QDMC,
        AV_CODEC_ID_CELT,
        AV_CODEC_ID_G723_1,
        AV_CODEC_ID_G729,
        AV_CODEC_ID_8SVX_EXP,
        AV_CODEC_ID_8SVX_FIB,
        AV_CODEC_ID_BMV_AUDIO,
        AV_CODEC_ID_RALF,
        AV_CODEC_ID_IAC,
        AV_CODEC_ID_ILBC,
        AV_CODEC_ID_OPUS,
        AV_CODEC_ID_COMFORT_NOISE,
        AV_CODEC_ID_TAK,
        AV_CODEC_ID_METASOUND,
        AV_CODEC_ID_PAF_AUDIO,
        AV_CODEC_ID_ON2AVC,
        AV_CODEC_ID_DSS_SP,
        AV_CODEC_ID_CODEC2,

        AV_CODEC_ID_FFWAVESYNTH = 0x15800,
        AV_CODEC_ID_SONIC,
        AV_CODEC_ID_SONIC_LS,
        AV_CODEC_ID_EVRC,
        AV_CODEC_ID_SMV,
        AV_CODEC_ID_DSD_LSBF,
        AV_CODEC_ID_DSD_MSBF,
        AV_CODEC_ID_DSD_LSBF_PLANAR,
        AV_CODEC_ID_DSD_MSBF_PLANAR,
        AV_CODEC_ID_4GV,
        AV_CODEC_ID_INTERPLAY_ACM,
        AV_CODEC_ID_XMA1,
        AV_CODEC_ID_XMA2,
        AV_CODEC_ID_DST,
        AV_CODEC_ID_ATRAC3AL,
        AV_CODEC_ID_ATRAC3PAL,
        AV_CODEC_ID_DOLBY_E,
        AV_CODEC_ID_APTX,
        AV_CODEC_ID_APTX_HD,
        AV_CODEC_ID_SBC,
        AV_CODEC_ID_ATRAC9,
        AV_CODEC_ID_HCOM,

        /* subtitle codecs */
        AV_CODEC_ID_FIRST_SUBTITLE = 0x17000,          ///< A dummy ID pointing at the start of subtitle codecs.
        AV_CODEC_ID_DVD_SUBTITLE = 0x17000,
        AV_CODEC_ID_DVB_SUBTITLE,
        AV_CODEC_ID_TEXT,  ///< raw UTF-8 text
        AV_CODEC_ID_XSUB,
        AV_CODEC_ID_SSA,
        AV_CODEC_ID_MOV_TEXT,
        AV_CODEC_ID_HDMV_PGS_SUBTITLE,
        AV_CODEC_ID_DVB_TELETEXT,
        AV_CODEC_ID_SRT,

        AV_CODEC_ID_MICRODVD = 0x17800,
        AV_CODEC_ID_EIA_608,
        AV_CODEC_ID_JACOSUB,
        AV_CODEC_ID_SAMI,
        AV_CODEC_ID_REALTEXT,
        AV_CODEC_ID_STL,
        AV_CODEC_ID_SUBVIEWER1,
        AV_CODEC_ID_SUBVIEWER,
        AV_CODEC_ID_SUBRIP,
        AV_CODEC_ID_WEBVTT,
        AV_CODEC_ID_MPL2,
        AV_CODEC_ID_VPLAYER,
        AV_CODEC_ID_PJS,
        AV_CODEC_ID_ASS,
        AV_CODEC_ID_HDMV_TEXT_SUBTITLE,
        AV_CODEC_ID_TTML,
        AV_CODEC_ID_ARIB_CAPTION,

        /* other specific kind of codecs (generally used for attachments) */
        AV_CODEC_ID_FIRST_UNKNOWN = 0x18000,           ///< A dummy ID pointing at the start of various fake codecs.
        AV_CODEC_ID_TTF = 0x18000,

        AV_CODEC_ID_SCTE_35, ///< Contain timestamp estimated through PCR of program stream.
        AV_CODEC_ID_BINTEXT = 0x18800,
        AV_CODEC_ID_XBIN,
        AV_CODEC_ID_IDF,
        AV_CODEC_ID_OTF,
        AV_CODEC_ID_SMPTE_KLV,
        AV_CODEC_ID_DVD_NAV,
        AV_CODEC_ID_TIMED_ID3,
        AV_CODEC_ID_BIN_DATA,


        AV_CODEC_ID_PROBE = 0x19000, ///< codec_id is not known (like AV_CODEC_ID_NONE) but lavf should attempt to identify it

        AV_CODEC_ID_MPEG2TS = 0x20000, /**< _FAKE_ codec to indicate a raw MPEG-2 TS
                                * stream (only used by libavformat) */
        AV_CODEC_ID_MPEG4SYSTEMS = 0x20001, /**< _FAKE_ codec to indicate a MPEG-4 Systems
                                * stream (only used by libavformat) */
        AV_CODEC_ID_FFMETADATA = 0x21000,   ///< Dummy codec for streams containing only metadata information.
        AV_CODEC_ID_WRAPPED_AVFRAME = 0x21001, ///< Passthrough codec, AVFrames wrapped in AVPacket
    }

    public enum AVHWDeviceType
    {
        AV_HWDEVICE_TYPE_NONE,
        AV_HWDEVICE_TYPE_VDPAU,
        AV_HWDEVICE_TYPE_CUDA,
        AV_HWDEVICE_TYPE_VAAPI,
        AV_HWDEVICE_TYPE_DXVA2,
        AV_HWDEVICE_TYPE_QSV,
        AV_HWDEVICE_TYPE_VIDEOTOOLBOX,
        AV_HWDEVICE_TYPE_D3D11VA,
        AV_HWDEVICE_TYPE_DRM,
        AV_HWDEVICE_TYPE_OPENCL,
        AV_HWDEVICE_TYPE_MEDIACODEC,
    }

    public enum AVPixelFormat
    {
        AV_PIX_FMT_NONE = -1,
        AV_PIX_FMT_YUV420P,   ///< planar YUV 4:2:0, 12bpp, (1 Cr & Cb sample per 2x2 Y samples)
        AV_PIX_FMT_YUYV422,   ///< packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr
        AV_PIX_FMT_RGB24,     ///< packed RGB 8:8:8, 24bpp, RGBRGB...
        AV_PIX_FMT_BGR24,     ///< packed RGB 8:8:8, 24bpp, BGRBGR...
        AV_PIX_FMT_YUV422P,   ///< planar YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
        AV_PIX_FMT_YUV444P,   ///< planar YUV 4:4:4, 24bpp, (1 Cr & Cb sample per 1x1 Y samples)
        AV_PIX_FMT_YUV410P,   ///< planar YUV 4:1:0,  9bpp, (1 Cr & Cb sample per 4x4 Y samples)
        AV_PIX_FMT_YUV411P,   ///< planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples)
        AV_PIX_FMT_GRAY8,     ///<        Y        ,  8bpp
        AV_PIX_FMT_MONOWHITE, ///<        Y        ,  1bpp, 0 is white, 1 is black, in each byte pixels are ordered from the msb to the lsb
        AV_PIX_FMT_MONOBLACK, ///<        Y        ,  1bpp, 0 is black, 1 is white, in each byte pixels are ordered from the msb to the lsb
        AV_PIX_FMT_PAL8,      ///< 8 bits with AV_PIX_FMT_RGB32 palette
        AV_PIX_FMT_YUVJ420P,  ///< planar YUV 4:2:0, 12bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV420P and setting color_range
        AV_PIX_FMT_YUVJ422P,  ///< planar YUV 4:2:2, 16bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV422P and setting color_range
        AV_PIX_FMT_YUVJ444P,  ///< planar YUV 4:4:4, 24bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV444P and setting color_range
        AV_PIX_FMT_UYVY422,   ///< packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1
        AV_PIX_FMT_UYYVYY411, ///< packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3
        AV_PIX_FMT_BGR8,      ///< packed RGB 3:3:2,  8bpp, (msb)2B 3G 3R(lsb)
        AV_PIX_FMT_BGR4,      ///< packed RGB 1:2:1 bitstream,  4bpp, (msb)1B 2G 1R(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
        AV_PIX_FMT_BGR4_BYTE, ///< packed RGB 1:2:1,  8bpp, (msb)1B 2G 1R(lsb)
        AV_PIX_FMT_RGB8,      ///< packed RGB 3:3:2,  8bpp, (msb)2R 3G 3B(lsb)
        AV_PIX_FMT_RGB4,      ///< packed RGB 1:2:1 bitstream,  4bpp, (msb)1R 2G 1B(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
        AV_PIX_FMT_RGB4_BYTE, ///< packed RGB 1:2:1,  8bpp, (msb)1R 2G 1B(lsb)
        AV_PIX_FMT_NV12,      ///< planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)
        AV_PIX_FMT_NV21,      ///< as above, but U and V bytes are swapped

        AV_PIX_FMT_ARGB,      ///< packed ARGB 8:8:8:8, 32bpp, ARGBARGB...
        AV_PIX_FMT_RGBA,      ///< packed RGBA 8:8:8:8, 32bpp, RGBARGBA...
        AV_PIX_FMT_ABGR,      ///< packed ABGR 8:8:8:8, 32bpp, ABGRABGR...
        AV_PIX_FMT_BGRA,      ///< packed BGRA 8:8:8:8, 32bpp, BGRABGRA...

        AV_PIX_FMT_GRAY16BE,  ///<        Y        , 16bpp, big-endian
        AV_PIX_FMT_GRAY16LE,  ///<        Y        , 16bpp, little-endian
        AV_PIX_FMT_YUV440P,   ///< planar YUV 4:4:0 (1 Cr & Cb sample per 1x2 Y samples)
        AV_PIX_FMT_YUVJ440P,  ///< planar YUV 4:4:0 full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV440P and setting color_range
        AV_PIX_FMT_YUVA420P,  ///< planar YUV 4:2:0, 20bpp, (1 Cr & Cb sample per 2x2 Y & A samples)
        AV_PIX_FMT_RGB48BE,   ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as big-endian
        AV_PIX_FMT_RGB48LE,   ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as little-endian

        AV_PIX_FMT_RGB565BE,  ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), big-endian
        AV_PIX_FMT_RGB565LE,  ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), little-endian
        AV_PIX_FMT_RGB555BE,  ///< packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), big-endian   , X=unused/undefined
        AV_PIX_FMT_RGB555LE,  ///< packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), little-endian, X=unused/undefined

        AV_PIX_FMT_BGR565BE,  ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), big-endian
        AV_PIX_FMT_BGR565LE,  ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), little-endian
        AV_PIX_FMT_BGR555BE,  ///< packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), big-endian   , X=unused/undefined
        AV_PIX_FMT_BGR555LE,  ///< packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), little-endian, X=unused/undefined

        /** @name Deprecated pixel formats */
        /**@{*/
        AV_PIX_FMT_VAAPI_MOCO, ///< HW acceleration through VA API at motion compensation entry-point, Picture.data[3] contains a vaapi_render_state struct which contains macroblocks as well as various fields extracted from headers
        AV_PIX_FMT_VAAPI_IDCT, ///< HW acceleration through VA API at IDCT entry-point, Picture.data[3] contains a vaapi_render_state struct which contains fields extracted from headers
        AV_PIX_FMT_VAAPI_VLD,  ///< HW decoding through VA API, Picture.data[3] contains a VASurfaceID
        /**@}*/
        AV_PIX_FMT_VAAPI = AV_PIX_FMT_VAAPI_VLD,

        AV_PIX_FMT_YUV420P16LE,  ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV420P16BE,  ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV422P16LE,  ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV422P16BE,  ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P16LE,  ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P16BE,  ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_DXVA2_VLD,    ///< HW decoding through DXVA2, Picture.data[3] contains a LPDIRECT3DSURFACE9 pointer

        AV_PIX_FMT_RGB444LE,  ///< packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), little-endian, X=unused/undefined
        AV_PIX_FMT_RGB444BE,  ///< packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), big-endian,    X=unused/undefined
        AV_PIX_FMT_BGR444LE,  ///< packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), little-endian, X=unused/undefined
        AV_PIX_FMT_BGR444BE,  ///< packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), big-endian,    X=unused/undefined
        AV_PIX_FMT_YA8,       ///< 8 bits gray, 8 bits alpha

        AV_PIX_FMT_Y400A = AV_PIX_FMT_YA8, ///< alias for AV_PIX_FMT_YA8
        AV_PIX_FMT_GRAY8A = AV_PIX_FMT_YA8, ///< alias for AV_PIX_FMT_YA8

        AV_PIX_FMT_BGR48BE,   ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as big-endian
        AV_PIX_FMT_BGR48LE,   ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as little-endian

        /**
         * The following 12 formats have the disadvantage of needing 1 format for each bit depth.
         * Notice that each 9/10 bits sample is stored in 16 bits with extra padding.
         * If you want to support multiple bit depths, then using AV_PIX_FMT_YUV420P16* with the bpp stored separately is better.
         */
        AV_PIX_FMT_YUV420P9BE, ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P9LE, ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV420P10BE,///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P10LE,///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV422P10BE,///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P10LE,///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P9BE, ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P9LE, ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P10BE,///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P10LE,///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV422P9BE, ///< planar YUV 4:2:2, 18bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P9LE, ///< planar YUV 4:2:2, 18bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_GBRP,      ///< planar GBR 4:4:4 24bpp
        AV_PIX_FMT_GBR24P = AV_PIX_FMT_GBRP, // alias for #AV_PIX_FMT_GBRP
        AV_PIX_FMT_GBRP9BE,   ///< planar GBR 4:4:4 27bpp, big-endian
        AV_PIX_FMT_GBRP9LE,   ///< planar GBR 4:4:4 27bpp, little-endian
        AV_PIX_FMT_GBRP10BE,  ///< planar GBR 4:4:4 30bpp, big-endian
        AV_PIX_FMT_GBRP10LE,  ///< planar GBR 4:4:4 30bpp, little-endian
        AV_PIX_FMT_GBRP16BE,  ///< planar GBR 4:4:4 48bpp, big-endian
        AV_PIX_FMT_GBRP16LE,  ///< planar GBR 4:4:4 48bpp, little-endian
        AV_PIX_FMT_YUVA422P,  ///< planar YUV 4:2:2 24bpp, (1 Cr & Cb sample per 2x1 Y & A samples)
        AV_PIX_FMT_YUVA444P,  ///< planar YUV 4:4:4 32bpp, (1 Cr & Cb sample per 1x1 Y & A samples)
        AV_PIX_FMT_YUVA420P9BE,  ///< planar YUV 4:2:0 22.5bpp, (1 Cr & Cb sample per 2x2 Y & A samples), big-endian
        AV_PIX_FMT_YUVA420P9LE,  ///< planar YUV 4:2:0 22.5bpp, (1 Cr & Cb sample per 2x2 Y & A samples), little-endian
        AV_PIX_FMT_YUVA422P9BE,  ///< planar YUV 4:2:2 27bpp, (1 Cr & Cb sample per 2x1 Y & A samples), big-endian
        AV_PIX_FMT_YUVA422P9LE,  ///< planar YUV 4:2:2 27bpp, (1 Cr & Cb sample per 2x1 Y & A samples), little-endian
        AV_PIX_FMT_YUVA444P9BE,  ///< planar YUV 4:4:4 36bpp, (1 Cr & Cb sample per 1x1 Y & A samples), big-endian
        AV_PIX_FMT_YUVA444P9LE,  ///< planar YUV 4:4:4 36bpp, (1 Cr & Cb sample per 1x1 Y & A samples), little-endian
        AV_PIX_FMT_YUVA420P10BE, ///< planar YUV 4:2:0 25bpp, (1 Cr & Cb sample per 2x2 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA420P10LE, ///< planar YUV 4:2:0 25bpp, (1 Cr & Cb sample per 2x2 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA422P10BE, ///< planar YUV 4:2:2 30bpp, (1 Cr & Cb sample per 2x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA422P10LE, ///< planar YUV 4:2:2 30bpp, (1 Cr & Cb sample per 2x1 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA444P10BE, ///< planar YUV 4:4:4 40bpp, (1 Cr & Cb sample per 1x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA444P10LE, ///< planar YUV 4:4:4 40bpp, (1 Cr & Cb sample per 1x1 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA420P16BE, ///< planar YUV 4:2:0 40bpp, (1 Cr & Cb sample per 2x2 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA420P16LE, ///< planar YUV 4:2:0 40bpp, (1 Cr & Cb sample per 2x2 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA422P16BE, ///< planar YUV 4:2:2 48bpp, (1 Cr & Cb sample per 2x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA422P16LE, ///< planar YUV 4:2:2 48bpp, (1 Cr & Cb sample per 2x1 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA444P16BE, ///< planar YUV 4:4:4 64bpp, (1 Cr & Cb sample per 1x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA444P16LE, ///< planar YUV 4:4:4 64bpp, (1 Cr & Cb sample per 1x1 Y & A samples, little-endian)

        AV_PIX_FMT_VDPAU,     ///< HW acceleration through VDPAU, Picture.data[3] contains a VdpVideoSurface

        AV_PIX_FMT_XYZ12LE,      ///< packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as little-endian, the 4 lower bits are set to 0
        AV_PIX_FMT_XYZ12BE,      ///< packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as big-endian, the 4 lower bits are set to 0
        AV_PIX_FMT_NV16,         ///< interleaved chroma YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
        AV_PIX_FMT_NV20LE,       ///< interleaved chroma YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_NV20BE,       ///< interleaved chroma YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian

        AV_PIX_FMT_RGBA64BE,     ///< packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
        AV_PIX_FMT_RGBA64LE,     ///< packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian
        AV_PIX_FMT_BGRA64BE,     ///< packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
        AV_PIX_FMT_BGRA64LE,     ///< packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian

        AV_PIX_FMT_YVYU422,   ///< packed YUV 4:2:2, 16bpp, Y0 Cr Y1 Cb

        AV_PIX_FMT_YA16BE,       ///< 16 bits gray, 16 bits alpha (big-endian)
        AV_PIX_FMT_YA16LE,       ///< 16 bits gray, 16 bits alpha (little-endian)

        AV_PIX_FMT_GBRAP,        ///< planar GBRA 4:4:4:4 32bpp
        AV_PIX_FMT_GBRAP16BE,    ///< planar GBRA 4:4:4:4 64bpp, big-endian
        AV_PIX_FMT_GBRAP16LE,    ///< planar GBRA 4:4:4:4 64bpp, little-endian
        /**
         *  HW acceleration through QSV, data[3] contains a pointer to the
         *  mfxFrameSurface1 structure.
         */
        AV_PIX_FMT_QSV,
        /**
         * HW acceleration though MMAL, data[3] contains a pointer to the
         * MMAL_BUFFER_HEADER_T structure.
         */
        AV_PIX_FMT_MMAL,

        AV_PIX_FMT_D3D11VA_VLD,  ///< HW decoding through Direct3D11 via old API, Picture.data[3] contains a ID3D11VideoDecoderOutputView pointer

        /**
         * HW acceleration through CUDA. data[i] contain CUdeviceptr pointers
         * exactly as for system memory frames.
         */
        AV_PIX_FMT_CUDA,

        AV_PIX_FMT_0RGB,        ///< packed RGB 8:8:8, 32bpp, XRGBXRGB...   X=unused/undefined
        AV_PIX_FMT_RGB0,        ///< packed RGB 8:8:8, 32bpp, RGBXRGBX...   X=unused/undefined
        AV_PIX_FMT_0BGR,        ///< packed BGR 8:8:8, 32bpp, XBGRXBGR...   X=unused/undefined
        AV_PIX_FMT_BGR0,        ///< packed BGR 8:8:8, 32bpp, BGRXBGRX...   X=unused/undefined

        AV_PIX_FMT_YUV420P12BE, ///< planar YUV 4:2:0,18bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P12LE, ///< planar YUV 4:2:0,18bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV420P14BE, ///< planar YUV 4:2:0,21bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P14LE, ///< planar YUV 4:2:0,21bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV422P12BE, ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P12LE, ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV422P14BE, ///< planar YUV 4:2:2,28bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P14LE, ///< planar YUV 4:2:2,28bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P12BE, ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P12LE, ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P14BE, ///< planar YUV 4:4:4,42bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P14LE, ///< planar YUV 4:4:4,42bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_GBRP12BE,    ///< planar GBR 4:4:4 36bpp, big-endian
        AV_PIX_FMT_GBRP12LE,    ///< planar GBR 4:4:4 36bpp, little-endian
        AV_PIX_FMT_GBRP14BE,    ///< planar GBR 4:4:4 42bpp, big-endian
        AV_PIX_FMT_GBRP14LE,    ///< planar GBR 4:4:4 42bpp, little-endian
        AV_PIX_FMT_YUVJ411P,    ///< planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples) full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV411P and setting color_range

        AV_PIX_FMT_BAYER_BGGR8,    ///< bayer, BGBG..(odd line), GRGR..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_RGGB8,    ///< bayer, RGRG..(odd line), GBGB..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_GBRG8,    ///< bayer, GBGB..(odd line), RGRG..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_GRBG8,    ///< bayer, GRGR..(odd line), BGBG..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_BGGR16LE, ///< bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_BGGR16BE, ///< bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, big-endian */
        AV_PIX_FMT_BAYER_RGGB16LE, ///< bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_RGGB16BE, ///< bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, big-endian */
        AV_PIX_FMT_BAYER_GBRG16LE, ///< bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_GBRG16BE, ///< bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, big-endian */
        AV_PIX_FMT_BAYER_GRBG16LE, ///< bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_GRBG16BE, ///< bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, big-endian */

        AV_PIX_FMT_XVMC,///< XVideo Motion Acceleration via common packet passing

        AV_PIX_FMT_YUV440P10LE, ///< planar YUV 4:4:0,20bpp, (1 Cr & Cb sample per 1x2 Y samples), little-endian
        AV_PIX_FMT_YUV440P10BE, ///< planar YUV 4:4:0,20bpp, (1 Cr & Cb sample per 1x2 Y samples), big-endian
        AV_PIX_FMT_YUV440P12LE, ///< planar YUV 4:4:0,24bpp, (1 Cr & Cb sample per 1x2 Y samples), little-endian
        AV_PIX_FMT_YUV440P12BE, ///< planar YUV 4:4:0,24bpp, (1 Cr & Cb sample per 1x2 Y samples), big-endian
        AV_PIX_FMT_AYUV64LE,    ///< packed AYUV 4:4:4,64bpp (1 Cr & Cb sample per 1x1 Y & A samples), little-endian
        AV_PIX_FMT_AYUV64BE,    ///< packed AYUV 4:4:4,64bpp (1 Cr & Cb sample per 1x1 Y & A samples), big-endian

        AV_PIX_FMT_VIDEOTOOLBOX, ///< hardware decoding through Videotoolbox

        AV_PIX_FMT_P010LE, ///< like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, little-endian
        AV_PIX_FMT_P010BE, ///< like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, big-endian

        AV_PIX_FMT_GBRAP12BE,  ///< planar GBR 4:4:4:4 48bpp, big-endian
        AV_PIX_FMT_GBRAP12LE,  ///< planar GBR 4:4:4:4 48bpp, little-endian

        AV_PIX_FMT_GBRAP10BE,  ///< planar GBR 4:4:4:4 40bpp, big-endian
        AV_PIX_FMT_GBRAP10LE,  ///< planar GBR 4:4:4:4 40bpp, little-endian

        AV_PIX_FMT_MEDIACODEC, ///< hardware decoding through MediaCodec

        AV_PIX_FMT_GRAY12BE,   ///<        Y        , 12bpp, big-endian
        AV_PIX_FMT_GRAY12LE,   ///<        Y        , 12bpp, little-endian
        AV_PIX_FMT_GRAY10BE,   ///<        Y        , 10bpp, big-endian
        AV_PIX_FMT_GRAY10LE,   ///<        Y        , 10bpp, little-endian

        AV_PIX_FMT_P016LE, ///< like NV12, with 16bpp per component, little-endian
        AV_PIX_FMT_P016BE, ///< like NV12, with 16bpp per component, big-endian

        /**
         * Hardware surfaces for Direct3D11.
         *
         * This is preferred over the legacy AV_PIX_FMT_D3D11VA_VLD. The new D3D11
         * hwaccel API and filtering support AV_PIX_FMT_D3D11 only.
         *
         * data[0] contains a ID3D11Texture2D pointer, and data[1] contains the
         * texture array index of the frame as intptr_t if the ID3D11Texture2D is
         * an array texture (or always 0 if it's a normal texture).
         */
        AV_PIX_FMT_D3D11,

        AV_PIX_FMT_GRAY9BE,   ///<        Y        , 9bpp, big-endian
        AV_PIX_FMT_GRAY9LE,   ///<        Y        , 9bpp, little-endian

        AV_PIX_FMT_GBRPF32BE,  ///< IEEE-754 single precision planar GBR 4:4:4,     96bpp, big-endian
        AV_PIX_FMT_GBRPF32LE,  ///< IEEE-754 single precision planar GBR 4:4:4,     96bpp, little-endian
        AV_PIX_FMT_GBRAPF32BE, ///< IEEE-754 single precision planar GBRA 4:4:4:4, 128bpp, big-endian
        AV_PIX_FMT_GBRAPF32LE, ///< IEEE-754 single precision planar GBRA 4:4:4:4, 128bpp, little-endian

        /**
         * DRM-managed buffers exposed through PRIME buffer sharing.
         *
         * data[0] points to an AVDRMFrameDescriptor.
         */
        AV_PIX_FMT_DRM_PRIME,
        /**
         * Hardware surfaces for OpenCL.
         *
         * data[i] contain 2D image objects (typed in C as cl_mem, used
         * in OpenCL as image2d_t) for each plane of the surface.
         */
        AV_PIX_FMT_OPENCL,

        AV_PIX_FMT_GRAY14BE,   ///<        Y        , 14bpp, big-endian
        AV_PIX_FMT_GRAY14LE,   ///<        Y        , 14bpp, little-endian

        AV_PIX_FMT_GRAYF32BE,  ///< IEEE-754 single precision Y, 32bpp, big-endian
        AV_PIX_FMT_GRAYF32LE,  ///< IEEE-754 single precision Y, 32bpp, little-endian

        AV_PIX_FMT_YUVA422P12BE, ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), 12b alpha, big-endian
        AV_PIX_FMT_YUVA422P12LE, ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), 12b alpha, little-endian
        AV_PIX_FMT_YUVA444P12BE, ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), 12b alpha, big-endian
        AV_PIX_FMT_YUVA444P12LE, ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), 12b alpha, little-endian

        AV_PIX_FMT_NV24,      ///< planar YUV 4:4:4, 24bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)
        AV_PIX_FMT_NV42,      ///< as above, but U and V bytes are swapped

        AV_PIX_FMT_NB         ///< number of pixel formats, DO NOT USE THIS if you want to link with shared libav* because the number of formats might differ between versions
    }

    /// <summary>
    /// ffmpeg中AVFrame结构体的前半部分，因为它太长了我不需要完全移植过来
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 408)]
    public struct AVFrame
    {
        //#define AV_NUM_DATA_POINTERS 8
        //        uint8_t* data[AV_NUM_DATA_POINTERS];
        public IntPtr data1;// 一般是y分量
        public IntPtr data2;// 一般是v分量
        public IntPtr data3;// 一般是u分量
        public IntPtr data4;// 一般是surface(dxva2硬解时)
        public IntPtr data5;
        public IntPtr data6;
        public IntPtr data7;
        public IntPtr data8;
        public int linesize1;// y分量每行长度（stride)
        public int linesize2;// v分量每行长度（stride)
        public int linesize3;// u分量每行长度（stride)
        public int linesize4;
        public int linesize5;
        public int linesize6;
        public int linesize7;
        public int linesize8;
        //uint8_t **extended_data;
        IntPtr extended_data;
        public int width;
        public int height;
        public int nb_samples;
        public AVPixelFormat format;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 128)]
    public struct AVCodec { }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 72)]
    public unsafe struct AVPacket
    {
        fixed byte frontUnused[24]; // 前部无关数据
        public void* data;
        public int size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    public struct AVBufferRef { }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 904)]
    public unsafe struct AVCodecContext
    {
        fixed byte frontUnused[880]; // 前部无关数据
        public AVBufferRef* hw_device_ctx;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AVDictionary { }

    public unsafe static class FFHelper
    {
        const string avcodec = "avcodec-58";
        const string avutil = "avutil-56";
        const CallingConvention callingConvention = CallingConvention.Cdecl;

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static void avcodec_register_all();

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static AVCodec* avcodec_find_decoder(AVCodecID id);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static AVPacket* av_packet_alloc();

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static void av_init_packet(AVPacket* pkt);

        //[DllImport(avcodec, CallingConvention = callingConvention)]
        //public extern static void av_packet_unref(AVPacket* pkt);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static void av_packet_free(AVPacket** pkt);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static AVCodecContext* avcodec_alloc_context3(AVCodec* codec);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static int avcodec_open2(AVCodecContext* avctx, AVCodec* codec, AVDictionary** options);

        //[DllImport(avcodec, CallingConvention = callingConvention)]
        //public extern static int avcodec_decode_video2(IntPtr avctx, IntPtr picture, ref int got_picture_ptr, IntPtr avpkt);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static void avcodec_free_context(AVCodecContext** avctx);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static int avcodec_send_packet(AVCodecContext* avctx, AVPacket* pkt);

        [DllImport(avcodec, CallingConvention = callingConvention)]
        public extern static int avcodec_receive_frame(AVCodecContext* avctx, AVFrame* frame);




        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static int av_hwdevice_ctx_create(AVBufferRef** device_ctx, AVHWDeviceType type, string device, AVDictionary* opts, int flags);

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static AVBufferRef* av_buffer_ref(AVBufferRef* buf);

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static void av_buffer_unref(AVBufferRef** buf);

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static AVFrame* av_frame_alloc();

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static void av_frame_free(AVFrame** frame);

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static void av_log_set_level(int level);

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static int av_dict_set_int(AVDictionary** pm, string key, long value, int flags);

        [DllImport(avutil, CallingConvention = callingConvention)]
        public extern static void av_dict_free(AVDictionary** m);
    }
}
