namespace liion_battery_patcher
{
    internal class Program
    {
		static void Main(string[] args)
		{
			string inputPath = "initial.bin";
			string outputPath = "patched.bin";

			if (!File.Exists(inputPath))
			{
				Console.WriteLine("Input file not found!");
				return;
			}

			byte[] data = File.ReadAllBytes(inputPath);

			if (data.Length != 128 && data.Length != 256)
			{
				Console.WriteLine("Unsupported file size. Only 128 or 256 bytes are supported.");
				return;
			}

			Console.WriteLine($"Patching EEPROM dump ({data.Length} bytes)...");

			if (data.Length == 256)
			{
				// Patch block for S-25C020A dumps (M3(2019-) M8(2018-) X3M X5M )
				Dictionary<int, byte> patchMap256 = new()
			{
				{ 0x000C, 0x00 }, // clear error/state
                { 0x0010, 0x62 }, // safe value
                { 0x0013, 0xCB },
				{ 0x0018, 0x32 },
				{ 0x0020, 0x47 },
				{ 0x0038, 0x00 }, // reset counter
                { 0x005A, 0x30 },
				{ 0x0066, 0x6F },
				{ 0x0068, 0x6F },
				{ 0x006A, 0x6F },
				{ 0x006C, 0x97 },
				{ 0x006D, 0x5D },
				{ 0x0078, 0x02 }, // unlock flag
                { 0x008C, 0x47 },
				{ 0x00F8, 0xFD }
			};

				foreach (var (offset, value) in patchMap256)
				{
					if (offset < data.Length)
						data[offset] = value;
				}

				Console.WriteLine("Applied extended 256-byte patch block.");
			}
			else if (data.Length == 128)
			{
				// Patch block for S-25C020A dumps (M5(2019-) X5M)
				Dictionary<int, byte> patchMap128 = new()
			{
				{ 0x0000, 0x57 },
				{ 0x0003, 0x29 },
				{ 0x000C, 0x00 }, // error counter reset
                { 0x000D, 0x00 },
				{ 0x0010, 0x54 },
				{ 0x0013, 0x8D },
				{ 0x0014, 0x31 },
				{ 0x0018, 0x13 },
				{ 0x0020, 0x09 },
				{ 0x0021, 0xE2 },
				{ 0x0024, 0x00 },
				{ 0x0034, 0x00 },
				{ 0x0038, 0x00 },
				{ 0x003C, 0x33 },
				{ 0x0044, 0x00 },
				{ 0x005A, 0x00 },
				{ 0x0066, 0xB6 },
				{ 0x0067, 0x51 },
				{ 0x006C, 0xCC },
				{ 0x0078, 0x01 }
			};

				foreach (var (offset, value) in patchMap128)
				{
					if (offset < data.Length)
						data[offset] = value;
				}

				Console.WriteLine("Applied extended 128-byte patch block.");
			}

			File.WriteAllBytes(outputPath, data);
			Console.WriteLine("Patch complete. Output written to: " + outputPath);
			Console.ReadKey();
		}
	}
}
