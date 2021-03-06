﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices;
using CCnv = System.Runtime.InteropServices.CallingConvention;

namespace Libraria {
	namespace Reflection {
		public static class ReflectionRuntime {
			public const BindingFlags DefaultFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
			internal static AssemblyBuilder AB;
			internal static ModuleBuilder DefMod;
			internal static Dictionary<string, Type> DelegateTypes;

			static ReflectionRuntime() {
				AB = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Libraria.Test.dll"),
						AssemblyBuilderAccess.RunAndSave);
				DefMod = AB.DefineDynamicModule("Libraria.DefMod");

				DelegateTypes = new Dictionary<string, Type>();
			}

			public static string CreateDelegateName(MethodInfo MI) {
				return CreateDelegateName(MI.ReturnType, MI.GetParamTypes());
			}

			public static string CreateDelegateName(Type ReturnType, Type[] Args) {
				string Name = "Delegate?" + ReturnType.FullName + "??";

				for (int i = 0; i < Args.Length; i++) {
					Name += Args[i].FullName;
					if (i + 1 < Args.Length)
						Name += "?";
				}

				return Name;
			}

			public static string CreateMethodName(MethodInfo MI) {
				string Name = "Method?" + MI.ReturnType.FullName + "??" + MI.Name + "??";

				Type[] Args = MI.GetParamTypes();
				for (int i = 0; i < Args.Length; i++) {
					Name += Args[i].FullName;
					if (i + 1 < Args.Length)
						Name += "?";
				}

				return Name;
			}

			public static Type CreateDelegateType(this Delegate D, CCnv CConv = CCnv.Cdecl) {
				return CreateDelegateType(D.Method.ReturnType, D.Method.GetParamTypes(), CConv);
			}
			
			public static Type CreateDelegateType(Type ReturnType, Type[] Args, CCnv CConv = CCnv.Cdecl) {
				string DelegateName = CreateDelegateName(ReturnType, Args);
				if (DelegateTypes.ContainsKey(DelegateName))
					return DelegateTypes[DelegateName];

				TypeBuilder TB = DefMod.DefineType(DelegateName, TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
					TypeAttributes.Sealed);
				TB.SetParent(typeof(MulticastDelegate));
				TB.SetCustomAttribute(new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor
					(new[] { typeof(CCnv) }),
					new object[] { CConv }));

				TB.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName |
					MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { typeof(Object), typeof(IntPtr) })
					.SetImplementationFlags(MethodImplAttributes.Runtime);

				TB.DefineMethod("BeginInvoke", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.NewSlot |
					MethodAttributes.Virtual, CallingConventions.HasThis, typeof(IAsyncResult),
					Args.Concat<Type>(new[] { typeof(AsyncCallback), typeof(object) }).ToArray())
					.SetImplementationFlags(MethodImplAttributes.Runtime);

				TB.DefineMethod("EndInvoke", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.NewSlot |
					MethodAttributes.Virtual, CallingConventions.HasThis, ReturnType, new[] { typeof(AsyncCallback) })
					.SetImplementationFlags(MethodImplAttributes.Runtime);

				TB.DefineMethod("Invoke", MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.NewSlot |
					MethodAttributes.Virtual, CallingConventions.HasThis, ReturnType, Args)
					.SetImplementationFlags(MethodImplAttributes.Runtime);

				Type T = TB.CreateType();
				DelegateTypes.Add(DelegateName, T);
				return T;
			}
		}
	}
}