using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;
using Libraria.IO;
using Libraria.Net;

namespace Test {
	class Program {
		static void Main(string[] Args) {
			string Src = File.ReadAllText("nanovg.h");
			List<string> Lines = new List<string>();

			string EXP = "NANOVG_EXPORT";
			while (Src.Contains(EXP)) {
				int Start = Src.IndexOf(EXP);
				int End = Src.IndexOf(';', Start) + 1;
				string L = Src.Substring(Start, End - Start).Replace("\n", " ").Replace("\r\n", " ").Replace("\r", " ");
				Src = Src.Remove(Start, End - Start);

				string Func = L.Replace("NANOVG_EXPORT", "").Trim();
				string FuncName = Func.Split(' ')[1].Split('(')[0];
				Func = Func.Replace(FuncName, FuncName.Substring(3));

				Func = Func.Replace("string", "Str").Replace("const char*", "string");
				Func = Func.Replace("const unsigned char*", "byte[]");
				Func = Func.Replace("unsigned char*", "byte[]");
				Func = Func.Replace("unsigned char", "byte");

				Func = Func.Replace("const float*", "float[]");
				Func = Func.Replace("float*", "float[]");

				Func = Func.Replace("const int*", "int[]");
				Func = Func.Replace("int*", "int[]");

				//Func = Func.Replace("const", "");

				Lines.Add(string.Format("[DllImport(DllName, EntryPoint = \"{0}\", CallingConvention = CConv, CharSet = CSet)]", FuncName));
				Lines.Add("public static extern " + Func);
				Lines.Add("// " + L);
				Lines.Add("");
			}

			File.WriteAllText("Exported.cs", string.Join("\n", Lines.ToArray()));

			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}