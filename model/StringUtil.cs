using System;
using System.Text;

namespace SchemaZen.Library {
	public class StringUtil {
		/// <summary>
		///     Adds a space to the beginning of a string.
		///     If the string is null or empty it's returned as-is
		/// </summary>
		public static string AddSpaceIfNotEmpty(string val) {
			if (string.IsNullOrEmpty(val)) return val;
			return $" {val}";
		}

		public static byte[] StringToByteArray(string hex)
		{
			if (hex.Length % 2 == 1)
				throw new Exception("The binary key cannot have an odd number of digits");

			byte[] arr = new byte[hex.Length >> 1];

			for (int i = 0; i < hex.Length >> 1; ++i)
			{
				arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
			}

			return arr;
		}

		public static int GetHexVal(char hex)
		{
			int val = (int)hex;
			//For uppercase A-F letters:
			//return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
			//Or the two combined, but a bit slower:
			return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
		}

		public static string ByteArrayToHexString(byte[] Bytes)
		{
			StringBuilder Result = new StringBuilder(Bytes.Length * 2);
			string HexAlphabet = "0123456789ABCDEF";

			foreach (byte B in Bytes)
			{
				Result.Append(HexAlphabet[(int)(B >> 4)]);
				Result.Append(HexAlphabet[(int)(B & 0xF)]);
			}

			return Result.ToString();
		}

		public static byte[] HexStringToByteArray(string Hex)
		{
			byte[] Bytes = new byte[Hex.Length / 2];
			int[] HexValue = new int[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05,
				   0x06, 0x07, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
				   0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

			for (int x = 0, i = 0; i < Hex.Length; i += 2, x += 1)
			{
				Bytes[x] = (byte)(HexValue[Char.ToUpper(Hex[i + 0]) - '0'] << 4 |
								  HexValue[Char.ToUpper(Hex[i + 1]) - '0']);
			}

			return Bytes;
		}

	}

	namespace Extensions {
		/// <summary>
		///     Extension methods to make sql script generators more readable.
		/// </summary>
		public static class Strings {
			public static string Space(this String val) {
				return StringUtil.AddSpaceIfNotEmpty(val);
			}
		}
	}
}
