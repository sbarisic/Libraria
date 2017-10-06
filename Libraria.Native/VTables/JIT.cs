using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Runtime.InteropServices;

namespace Libraria.Native {
	public unsafe class NativeClassImpl {
		public IntPtr InstancePointer;
		public NativeClassInfo NativeInfo;

		public IntPtr GetMethodPointer(int VTableOffset, int MethodIndex) {
			IntPtr* VTable = *(IntPtr**)InstancePointer + VTableOffset;
			IntPtr Method = VTable[MethodIndex];
			return Method;
		}

		public IntPtr GetVariablePointer(int Offset) {
			return InstancePointer + Offset;
		}

		public IntPtr OffsetThisPtr(int Offset) {
			return InstancePointer + Offset;
		}

		public NativeTypeInfo GetTypeInfo() {
			return NativeClass.GetTypeInfo(InstancePointer);
		}
	}

	public static class JIT {
		static AssemblyBuilder AsmBuilder;
		static ModuleBuilder ModBuilder;
		static Dictionary<string, Type> NativeClassWrappers = new Dictionary<string, Type>();

		public static AssemblyBuilder GetAssemblyBuilder() {
			if (AsmBuilder != null)
				return AsmBuilder;

			AssemblyName AName = new AssemblyName(nameof(JIT));
			AppDomain AppDomain = Thread.GetDomain();
			return AsmBuilder = AppDomain.DefineDynamicAssembly(AName, AssemblyBuilderAccess.Run);
		}

		public static ModuleBuilder GetModuleBuilder() {
			if (ModBuilder != null)
				return ModBuilder;

			AssemblyBuilder AsmBuilder = GetAssemblyBuilder();
			return ModBuilder = AsmBuilder.DefineDynamicModule(AsmBuilder.GetName().Name);
		}

		public static Type CreateType(string Name, Action<TypeBuilder> Create) {
			ModuleBuilder ModBuiler = GetModuleBuilder();
			TypeBuilder TB = ModBuilder.DefineType(Name, TypeAttributes.Public | TypeAttributes.Class);
			Create(TB);
			return TB.CreateType();
		}

		public static Type CreateInterfaceImpl<T>(string Name, Action<TypeBuilder> Create) where T : class {
			return CreateType(Name, (TB) => {
				TB.SetParent(typeof(NativeClassImpl));
				TB.AddInterfaceImplementation(typeof(T));
				Create(TB);
			});
		}

		public static void CreateMethodImpl(TypeBuilder TB, MethodInfo Template, Action<MethodBuilder, ILGenerator, Type[]> Create) {
			MethodBuilder MB = TB.DefineMethod(Template.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual);
			Type[] ParamTypes = Template.GetParameters().Select((PI) => PI.ParameterType).ToArray();
			MB.SetParameters(ParamTypes);
			MB.SetReturnType(Template.ReturnType);
			Create(MB, MB.GetILGenerator(), ParamTypes);
		}

		public static void CreateVariableImpl(TypeBuilder TB, PropertyInfo Template, int InstanceOffset) {
			MethodAttributes Attribs = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.SpecialName;
			PropertyBuilder PB = TB.DefineProperty(Template.Name, PropertyAttributes.None, Template.PropertyType, Type.EmptyTypes);

			MethodBuilder GetMethod = TB.DefineMethod("get_" + Template.Name, Attribs);
			GetMethod.SetReturnType(Template.PropertyType);
			ILGenerator GetGen = GetMethod.GetILGenerator();
			{
				GetGen.Emit(OpCodes.Ldarg_0);
				GetGen.Emit(OpCodes.Ldc_I4, InstanceOffset);
				GetGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.OffsetThisPtr)), null);
				// ptr to field on stack ^

				GetGen.Emit(OpCodes.Ldobj, Template.PropertyType);
				GetGen.Emit(OpCodes.Ret);
			}
			PB.SetGetMethod(GetMethod);

			MethodBuilder SetMethod = TB.DefineMethod("set_" + Template.Name, Attribs);
			SetMethod.SetReturnType(typeof(void));
			SetMethod.SetParameters(Template.PropertyType);
			ILGenerator SetGen = SetMethod.GetILGenerator();
			{
				SetGen.Emit(OpCodes.Ldarg_0);
				SetGen.Emit(OpCodes.Ldc_I4, InstanceOffset);
				SetGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.OffsetThisPtr)), null);
				// ptr to field on stack ^

				SetGen.Emit(OpCodes.Ldarg_1);
				SetGen.Emit(OpCodes.Stobj, Template.PropertyType);
				SetGen.Emit(OpCodes.Ret);
			}
			PB.SetSetMethod(SetMethod);
		}

		public static Type CreateWrapper<T>(IntPtr Instance, out NativeClassInfo NativeCInfo) where T : class {
			NativeCInfo = null;

			string WrapperName = typeof(T).Name + "_impl";
			if (NativeClassWrappers.ContainsKey(WrapperName))
				return NativeClassWrappers[WrapperName];

			NativeClassInfo NativeInfo = NativeCInfo = NativeClass.CalculateClassLayout(typeof(T));

			Type TypeWrapper = CreateInterfaceImpl<T>(WrapperName, (TB) => {
				foreach (var VarInf in NativeInfo.VariableInfo)
					CreateVariableImpl(TB, VarInf.PropertyInfo, VarInf.Offset);

				foreach (var MetInf in NativeInfo.MethodInfo) {
					if (MetInf.DoesOverride)
						continue;

					CreateMethodImpl(TB, MetInf.Method, (MB, ILGen, ParamTypes) => {
						ILGen.Emit(OpCodes.Ldarg_0);
						ILGen.Emit(OpCodes.Ldc_I4, MetInf.ThisOffset);
						ILGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.OffsetThisPtr)), null);

						for (int i = 0; i < ParamTypes.Length; i++)
							ILGen.Emit(OpCodes.Ldarg, i + 1);

						ILGen.Emit(OpCodes.Ldarg_0);
						ILGen.Emit(OpCodes.Ldc_I4, MetInf.VTableOffset);
						ILGen.Emit(OpCodes.Ldc_I4, MetInf.MethodIndex);
						ILGen.EmitCall(OpCodes.Call, TB.BaseType.GetMethod(nameof(NativeClassImpl.GetMethodPointer)), null);

						ILGen.EmitCalli(OpCodes.Calli, CallingConvention.ThisCall, MetInf.Method.ReturnType, new Type[] { typeof(IntPtr) }.Append(ParamTypes));
						ILGen.Emit(OpCodes.Ret);
					});
				}
			});


			NativeClassWrappers.Add(WrapperName, TypeWrapper);
			return TypeWrapper;
		}

		public static T ConvertInstance<T>(IntPtr Instance) where T : class {
			NativeClassInfo NativeInfo;
			Type WrapperType = CreateWrapper<T>(Instance, out NativeInfo);

			NativeClassImpl NativeClass = (NativeClassImpl)Activator.CreateInstance(WrapperType);
			NativeClass.InstancePointer = Instance;
			NativeClass.NativeInfo = NativeInfo;

			return (T)(object)NativeClass;
		}
	}

	public class NativeClassMarshal<T> : ICustomMarshaler where T : class {
		public void CleanUpManagedData(object ManagedObj) {
		}

		public void CleanUpNativeData(IntPtr pNativeData) {
		}

		public int GetNativeDataSize() {
			return IntPtr.Size;
		}

		public IntPtr MarshalManagedToNative(object ManagedObj) {
			return IntPtr.Zero;
		}

		public object MarshalNativeToManaged(IntPtr pNativeData) {
			return JIT.ConvertInstance<T>(pNativeData);
		}

		public static ICustomMarshaler GetInstance(string Cookie) {
			return new NativeClassMarshal<T>();
		}
	}
}
