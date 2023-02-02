
using System;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace SpiExamples
{
	public class Program
	{
		public static void Main()
		{
			//configure SPI bus-1 HSPI
			//Configuration.SetPinFunction(14, DeviceFunction.SPI1_CLOCK);//14
			//Configuration.SetPinFunction(12, DeviceFunction.SPI1_MISO);//12
			//Configuration.SetPinFunction(13, DeviceFunction.SPI1_MOSI);//13---CS15

			//configure SPI bus-2 VSPI this one is mandatory!
			//Configuration.SetPinFunction(18, DeviceFunction.SPI2_CLOCK);//18
			//Configuration.SetPinFunction(19, DeviceFunction.SPI2_MISO);//19
			//Configuration.SetPinFunction(23, DeviceFunction.SPI2_MOSI);//23---CS5

			//device A
			SpiDevice spiDevice_A;
			SpiConnectionSettings connectionSettings_A;
			connectionSettings_A = new SpiConnectionSettings(1, 5);
			connectionSettings_A.ClockFrequency = 100000;
			connectionSettings_A.Mode = SpiMode.Mode3;
			spiDevice_A = SpiDevice.Create(connectionSettings_A);
			//..................

			//device B
			SpiDevice spiDevice_B;
			SpiConnectionSettings connectionSettings_B;
			connectionSettings_B = new SpiConnectionSettings(1, 4);
			connectionSettings_B.ClockFrequency = 100000;
			connectionSettings_B.Mode = SpiMode.Mode3;
			spiDevice_B = SpiDevice.Create(connectionSettings_B);
			//..................

			//device C
			SpiDevice spiDevice_C;
			SpiConnectionSettings connectionSettings_C;
			connectionSettings_C = new SpiConnectionSettings(1, 15);
			connectionSettings_C.ClockFrequency = 100000;
			connectionSettings_C.Mode = SpiMode.Mode3;
			spiDevice_C = SpiDevice.Create(connectionSettings_C);

			//...................
			// short SpanByte
			SpanByte shortSpanByte = new byte[2] { 42, 24 };
			//medium length transmit and recive buffers
			Byte[] Sendlength8 = new Byte[8];
			Byte[] Receivelength8 = new Byte[8];
			//longer length send and recive buffers
			Byte[] Sendlength16 = new Byte[16];
			Byte[] Receivelength16 = new Byte[16];
			//..................


			//fill buffers in order to read reconigsable data
			for (int i = 0; i < Sendlength16.Length; i++)
			{
				if (i< Sendlength8.Length)
				{
					Sendlength8[i]= (byte)i;
					Receivelength8[i] = 0;
				}
				Sendlength16[i] = (byte)i;
				Receivelength16[i] = 0;			
			}

			//loop just to create SPI signals on the hardware
			//
			//spiDevice_A should produce a short CLK burst and respective Cs
			//spiDevice_B should produce a longer CLK burst and respective Cs
			//spiDevice_C should produce the longer CLK burst and respective Cs
			//they are perfectly diferenciated on a regular scope without bus debugging.
			for (int i = 0; i < 1000000; i++)
			{
				spiDevice_A.Write(shortSpanByte);//produces the shortest Cs-pin signal
				//NOTE: in order to receive, a wire must connect MOSI and MISO
				spiDevice_B.TransferFullDuplex(Sendlength8, Receivelength8);//produces the "medium" length Cs-pin signal
				//NOTE: in order to receive, a wire must connect MOSI and MISO															
				spiDevice_C.TransferFullDuplex(Sendlength16, Receivelength16);//produces the "largest" length Cs-pin signal
																			  
				Debug.WriteLine("running loop step= " + i.ToString());
			}
			Debug.WriteLine("LOOP FINISHED");

			Thread.Sleep(Timeout.Infinite);
		}
	}
}
