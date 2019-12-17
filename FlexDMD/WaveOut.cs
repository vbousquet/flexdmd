using System;
using System.Runtime.InteropServices;


// Sourcecode from https://www.codeproject.com/Articles/866347/Streaming-Audio-to-the-WaveOut-Device
// FIXME add the https://www.codeproject.com/info/cpol10.aspx license
namespace FlexDMD
{
    //----------------------------------------------------------------------------------------------------
    // THIS CLASS WRAPS UP A SIMPLE INTERFACE TO THE WAVE OUT DEVICE
    //----------------------------------------------------------------------------------------------------
    // Note, because there are some unmanaged resources required in this interface, the class is declared as IDisposable; in consequence there is a Dispose method included to delete
    // them when finished.
    public class woLib : IDisposable
    {
        //----------------------------------------------------------------------------------------------------
        // this structure is used at the head of each block of raw data
        [StructLayout(LayoutKind.Sequential)]
        public struct WaveHdr
        {
            public IntPtr lpData;                                                       // pointer to locked data buffer
            public int dwBufferLength;                                                  // length of data buffer
            public int dwBytesRecorded;                                                 // used for input only
            public IntPtr dwUser;                                                       // for client's use
            public int dwFlags;                                                         // assorted flags (see defines)
            public int dwLoops;                                                         // loop control counter
            public IntPtr lpNext;                                                       // PWaveHdr, reserved for driver
            public int reserved;                                                        // reserved for driver
        }

        //----------------------------------------------------------------------------------------------------
        // these identify the expected nature of the raw audio data. Typically it will be PCM or G711
        public enum WaveFormats
        {
            Unknown = 0,
            Pcm = 1,
            Adpcm = 2,
            Float = 3,
            alaw = 6,
            mulaw = 7
        }

        //----------------------------------------------------------------------------------------------------
        // this structure is used to initialise the WaveOut device into the required mode (sample rate, channels, &etc)
        [StructLayout(LayoutKind.Sequential)]
        public class WaveFormat
        {
            public short wFormatTag;
            public short nChannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public short nBlockAlign;
            public short wBitsPerSample;
            public short cbSize;
        }
        public const int CALLBACK_FUNCTION = 0x00030000;                                // flag used if we require a callback when audio frames are completed
        public const int CALLBACK_NULL = 0x00000000;                                    // flag used if no callback is required
        public const int BUFFER_DONE = 0x3BD;                                           // flag used in callback to identify the reason for the callback
        public delegate void WaveDelegate(IntPtr dev, int uMsg, int dwUser, int dwParam1, int dwParam2);

        //----------------------------------------------------------------------------------------------------
        // here we import the WaveOut interface components from an external dll

        [DllImport("winmm.dll")]
        public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, IntPtr dwInstance, int dwFlags);
        [DllImport("winmm.dll")]
        public static extern int waveOutReset(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutRestart(IntPtr hWaveOut);
        [DllImport("winmm.dll")]
        public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutWrite(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll")]
        public static extern int waveOutClose(IntPtr hWaveOut);

        //----------------------------------------------------------------------------------------------------
        // these variables are used to manage the persistent audio output buffer which must be maintained while the waveout device is outputting.
        private UInt32 pFrames = 0;                                                     // counts audio frames delivered to WaveOut device
        private IntPtr rFrames = (IntPtr)0;                                             // pointer to UNMANAGED UInt32; used by callback to count completed buffers
        private IntPtr hwo = (IntPtr)0;                                                 // wave output device
        private const int audioRawSize = 10000000;                                      // size of raw audio buffer used with hwo device
        private IntPtr audioRawP = (IntPtr)0;                                           // this will be an audio buffer for the wave out device but it must be in UNMANAGED memory
        private UInt32 audioRawIndex = 0;                                               // this is used to point to the current end of data in the audio buffer
        private WaveDelegate woDone = new WaveDelegate(WaveOutDone);                    // delegate for wave out done callback
                                                                                        //----------------------------------------------------------------------------------------------------
                                                                                        // this callback is called when the waveout device has finished outputting a buffer. If we choose to use callbacks then this allows us to estimate current latency since the
                                                                                        // difference in number between sent and completed frames (queued) tells us how much data are outstanding. Clearly we need to know how much data are in each buffer plus the sample
                                                                                        // rates and so forth. Queued frames can be used to pace audio when playing data from a file. In streaming applications we either start to dump frames or reset the buffer if
                                                                                        // the number of queued frames gets too large.

        unsafe static void WaveOutDone(IntPtr dev, int uMsg, int dwUser, int dwParam1, int dwParam2)
        {
            if ((uMsg == BUFFER_DONE) && (dwUser != 0))
            {
                try
                {
                    (*(UInt32*)dwUser)++;                                               // increment the integer at the pointer dwUser. This must be unmanaged memory otherwise the

                }                                                                       // callback may well try to increment a variable that is no longer there and probably, in
                catch                                                                   // fact, something else. This integer is held at "rFrames".
                {
                }
            }
        }

        //----------------------------------------------------------------------------------------------------
        // this routine creates a wave out device based on a set of input parameters. sf - sample frequency, chans - number of audio channels, bps - bits per (mono) sample, G711 - true
        // if the data are G711 mulaw. The default audio device is always used (WAVE_MAPPER) or -1 is specified in waveOutOpen. If you want to choose the wave out device, -1 can be
        // replace by an initialisation parameter. Import and call "int waveOutGetNumDevs()" if you want to know how many valid audio devices your system has.
        public void InitWODevice(UInt32 sf, UInt32 chans, UInt32 bps, bool G711)
        {
            //.................................................................................................................................................................................
            // we may call this more than once if different sample configurations must be played so check if a player has already been created and, if so, close it
            if (hwo != (IntPtr)0) CloseWODevice();
            //.................................................................................................................................................................................
            // check if an unmanaged memory pool has been created yet. If not, create it
            if (audioRawP == (IntPtr)0)                                                 // see if the audio buffer has been created yet
            {                                                                           // if not, create it. Because data must remain live while being played out, this memory must
                audioRawP = Marshal.AllocHGlobal(audioRawSize);                         // stay at a fixed location and, therefore, be unmanaged.
                rFrames = Marshal.AllocHGlobal(4);                                      // similarly, create a fixed memory pool for the callback "frames-rendered" counter
            }
            //.................................................................................................................................................................................
            // set up the WaveFormat for this initialisation

            WaveFormat wFmt = new WaveFormat();                                         // create new wave format object
            wFmt.nAvgBytesPerSec = (int)(sf * (bps / 8) * chans);
            wFmt.nBlockAlign = (short)(chans * (bps / 8));
            wFmt.nChannels = (short)chans;
            wFmt.nSamplesPerSec = (int)sf;
            wFmt.wBitsPerSample = (short)bps;
            if (G711) wFmt.wFormatTag = (short)WaveFormats.mulaw; else wFmt.wFormatTag = (short)WaveFormats.Pcm;
            wFmt.cbSize = 0;
            //.................................................................................................................................................................................
            // open the WaveOut device

            waveOutOpen(out hwo, -1, wFmt, woDone, rFrames, CALLBACK_FUNCTION);         // use this if you want the callback for frame counting
                                                                                        //        waveOutOpen(out hwo, -1, wFmt, woDone, (IntPtr) 0, CALLBACK_NULL);        // use this, otherwise
            ResetWODevice();
        }

        //----------------------------------------------------------------------------------------------------
        // reset the wave out device
        public void ResetWODevice()
        {
            if (hwo != (IntPtr)0) waveOutRestart(hwo);                                  // restart the audio device - flusing any unfinished buffers
            pFrames = 0;                                                                // clear the played frames (i.e. delivered) counter
            if (rFrames != (IntPtr)0)                                                   // clear the rendered frames (i.e. completed) counter
                unsafe
                {
                    UInt32* i = (UInt32*)rFrames.ToPointer();                           // note that this is in unmanaged memory
                    *i = 0;
                }
            audioRawIndex = 0;                                                          // reset the raw buffer index to start
        }

        //----------------------------------------------------------------------------------------------------
        // indicates that the wave out device has been inititialised
        public bool IsInit()
        {
            return (hwo != (IntPtr)0);
        }

        //----------------------------------------------------------------------------------------------------
        // returns the number of frames submitted to the wave out device
        public UInt32 GetFrames()
        {
            return pFrames;
        }

        //----------------------------------------------------------------------------------------------------
        // returns the number of queued frames that are pending within the wave out device
        public UInt32 GetQueued()
        {
            if (rFrames != (IntPtr)0)
                unsafe
                {
                    UInt32* i = (UInt32*)rFrames.ToPointer();
                    if (pFrames > *i)
                        return (pFrames - *i);
                    else
                        return 0;
                }
            else return 0;
        }

        //----------------------------------------------------------------------------------------------------
        // close the wave out device in the event that we need to change its settings (sample rate, for example)
        public void CloseWODevice()
        {
            ResetWODevice();
            if (hwo != (IntPtr)0) waveOutClose(hwo);
            hwo = (IntPtr)0;
        }

        //----------------------------------------------------------------------------------------------------
        // generic interface to deliver raw data to a wave out device - uses a raw audio buffer; data must remain valid until playout is complete so we copy them to a buffer prior to
        // delivery. This is done in two parts. First a wave header is created on the buffer, then the raw data are copied immediately after the header. The header is then filled in and
        // delivered to the specified wave out device. Currently the size of the raw audio buffer is fixed but it could be made easily programmable. Bear in mind that the raw audio buffer
        // must be large enough to accommodate the latency (queued frames) in the play out device as data must remain alive until completed. This is not checked here. If in doubt, increase
        // the allocated buffer size. For multi-byte samples (more than one byte per sample or more than one channel) byte ordering must be correct. If the sound comes out harsh and very
        // distorted, you've probably got the byte order swapped.
        public void SendWODevice(IntPtr data, UInt32 bytes)
        {
            if ((hwo != (IntPtr)0) && (audioRawP != (IntPtr)0))                           // check that a device has been initialised
                unsafe
                {
                    UInt32 pwhs = (UInt32)sizeof(WaveHdr);                             // get the size of the wave header class
                    if ((bytes + audioRawIndex + pwhs) > audioRawSize)
                        audioRawIndex = 0;                                              // reset the index to 0 if there's not enough room on the buffer for header and data*

                    //.................................................................................................................................................................................
                    if ((bytes + pwhs) < audioRawSize)                                  // check that we actually have enough room on the buffer to accommodate this request
                    {
                        //.................................................................................................................................................................................
                        // create wave header on static buffer

                        byte* bpi = (byte*)audioRawP;                                   // point to buffer start
                        byte* bpw = (byte*)&bpi[audioRawIndex];                         // create a wave header at the next free space on the buffer
                        byte* bpr = (byte*)&bpi[audioRawIndex + pwhs];                  // create a pointer for the raw data that we're adding to the buffer
                        WaveHdr* pwh = (WaveHdr*)bpw;                                   // type cast to wave header structure that will be placed next in the raw buffer
                        pwh->lpData = (IntPtr)bpr;                                      // point the wave header data pointer to the new audio data
                        pwh->dwBufferLength = (int)bytes;                               // set the wave buffer length, flags, and so forth
                        pwh->dwUser = (IntPtr)0;
                        pwh->dwFlags = 0;
                        pwh->dwLoops = 0;
                        //.................................................................................................................................................................................
                        // copy raw data to static buffer
                        byte[] raw = new byte[bytes];
                        Marshal.Copy(data, raw, 0, (int)bytes);
                        IntPtr bpd = (IntPtr)bpr;
                        Marshal.Copy(raw, 0, bpd, (int)bytes);
                        //.................................................................................................................................................................................
                        // update the buffer index
                        audioRawIndex += (pwhs + bytes);                                // advance audio buffer index by the size of the wave header class
                                                                                        //.................................................................................................................................................................................
                                                                                        // deliver header to the wave out device
                        waveOutPrepareHeader(hwo, ref *pwh, (int)sizeof(WaveHdr));      // prepeare the wave header
                        waveOutWrite(hwo, ref *pwh, (int)sizeof(WaveHdr));              // send the data
                        pFrames++;                                                      // indicate played frames have increased
                    }
                }
        }

        // * note, by examining the amount of queued frames, if we use constant data sizes when calling SendWODevice, we can determine whether or not there will be enough room for this
        // call on the buffer. Essentially, if the queued frames * size (in bytes) per frame (including WaveHeader) is approaching audioRawSize then we should call ResetWODevice().
        // responsibility for this is left with the user - this is only a simple interface for demonstration purposes
        //----------------------------------------------------------------------------------------------------
        // this is included in order for the unmanaged resources to be deleted by the application when the class is cleaned up.
        public void Dispose()
        {
            CloseWODevice();                                                            // close the wave out device
            if (audioRawP == (IntPtr)0)                                                 // ensure that memory was allocated
            {
                Marshal.FreeHGlobal(audioRawP);                                         // free it up
                Marshal.FreeHGlobal(rFrames);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------
    // CLASS END CLASS END CLASS END CLASS END CLASS END CLASS ENDCLASS END CLASS END CLASS ENDCLASS END CLASS END CLASS END CLASS END CLASS END CLASS END CLASS END CLASS END CLASS EN
    //----------------------------------------------------------------------------------------------------
}
