using System.IO;
using System.Windows;

namespace DigitalMusicAnalysis
{
    public class wavefile
    {
        public float[] wave;
        public byte[] data;
        public char[] ChunkID = new char[4];
	    public int ChunkSize;
	    public char[] Format = new char[4];
	    public char[] Subchunk1ID = new char[4];
	    public int Subchunk1Size;
	    public short AudioFormat;
	    public short NumChannels;
	    public int SampleRate;
	    public int ByteRate;
	    public short BlockAlign;
	    public short BitsPerSample;
        
	    public char[] Subchunk2ID = new char[4];
	    public int Subchunk2Size;

        public wavefile(FileStream file)
        {
            BinaryReader binRead = new BinaryReader(file);

            ChunkID =  binRead.ReadChars(4);
            ChunkSize = binRead.ReadInt32();
            Format = binRead.ReadChars(4);
            Subchunk1ID = binRead.ReadChars(4);
            Subchunk1Size = binRead.ReadInt32();
            AudioFormat = binRead.ReadInt16();
            NumChannels = binRead.ReadInt16();
            SampleRate = binRead.ReadInt32();
            ByteRate = binRead.ReadInt32();
            BlockAlign = binRead.ReadInt16();
            BitsPerSample = binRead.ReadInt16();
            
            char testValue = 'd';
            while (true) {
                if (binRead.ReadChar() == testValue) {
                    binRead.BaseStream.Seek(-1, SeekOrigin.Current);
                    Subchunk2ID = binRead.ReadChars(4);
                    break;
                }
            }
            
            Subchunk2Size = binRead.ReadInt32();

            int numSamples = Subchunk2Size / (BitsPerSample / 8);
            
            data = new byte[numSamples];
            wave = new float[numSamples];
            
            data = binRead.ReadBytes(numSamples);
        /*    MessageBox.Show("ChunkID " + new string(ChunkID) + "\n" +
                            "ChunkSize " + ChunkSize + "\n" +
                            "Format " + new string(Format) + "\n" +
                            "Subchunk1ID " + new string(Subchunk1ID) + "\n" +
                            "Subchunk1Size " + Subchunk1Size + "\n" +
                            "AudioFormat " + AudioFormat + "\n" +
                            "NumChannels " + NumChannels + "\n" +
                            "SampleRate " + SampleRate + "\n" +
                            "ByteRate " + ByteRate + "\n" +
                            "BlockAlign " + BlockAlign + "\n" +
                            "BitsPerSample " + BitsPerSample + "\n" +
                            "Subchunk2ID " + new string(Subchunk2ID).ToString() + "\n" +
                            "Subchunk2Size " + Subchunk2Size + "\n" +
                            "numSamples " + numSamples + "\n" +
                            "data " + data);*/

            for (int i = 0; i < numSamples; i++)
            {
                wave[i] = ((float)data[i] - 128) / 128;
            }

        }
    }
}
