using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;

namespace Libraria.Runtime {
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class DllExportAttribute : Attribute {
		public DllExportAttribute() {
		}

		public static void DoCrap() {
		}
	}

	public static class DllExporter {
		public static void Export(string In, string Out = null) {
			if (string.IsNullOrEmpty(Out))
				Out = "Exported." + In;
			Console.WriteLine("Exporting '{0}' to '{1}'", In, Out);

			AssemblyDefinition AsmDef = AssemblyDefinition.ReadAssembly(In);
			ModuleDefinition MainModule = AsmDef.MainModule;
			MethodDefinition Voide = MainModule.GetType("LibTests.Program").GetMethods().ToArray()[1];
			MethodBody VoideBody = Voide.Body;

			AsmDef.Write(Out);
		}
	}
}
