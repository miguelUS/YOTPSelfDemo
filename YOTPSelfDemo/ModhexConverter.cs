using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOTPSelfDemo
{
	class ModhexConverter
	{
		private const string alphabet = "cbdefghijklnrtuv";

		public string ModHexEncode(byte[] data)
		{
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				result.Append(alphabet[(data[i] >> 4) & 0xf]);
				result.Append(alphabet[data[i] & 0xf]);
			}

			return result.ToString();
		}

		public int ModHexDecode(String s)
		{
			List<byte> baos = new List<byte>();
			int len = s.Length;

			bool toggle = false;
			int keep = 0;

			for (int i = 0; i < len; i++)
			{
				char ch = s[i];
				int n = alphabet.IndexOf(ch.ToString().ToLower());
				if (n == -1)
				{
					throw new Exception(s + " is not properly encoded");
				}

				toggle = !toggle;

				if (toggle)
				{
					keep = n;
				}
				else
				{
					baos.Add((byte)((keep << 4) | n));
				}
			}

			byte[] b1 = baos.ToArray();
			Array.Reverse(b1);
			int t2 = BitConverter.ToInt32(b1, 0);
			return t2;
		}
	}
}

