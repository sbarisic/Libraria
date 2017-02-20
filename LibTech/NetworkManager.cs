﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Libraria.Native;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LibTech {
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	public sealed class NetworkedAttribute : Attribute {
	}

	public class PropertyOverride {
		public object Origin;
		public PropertyInfo Property;
		public HookHandle Hook;

		public PropertyOverride(HookHandle Hook, PropertyInfo Property, object Origin) {
			this.Property = Property;
			this.Hook = Hook;
			this.Origin = Origin;
		}

		public void Set(object Value) {
			Hook.Unhook();
			Property.SetValue(Origin, Value);
			Hook.Hook();
		}

		public object Get() {
			return Property.GetValue(Origin);
		}
	}

	public static class PropertyHooks {
		public delegate void SetOverrideAction(object Origin, object Value, PropertyOverride Override);

		public static List<PropertyOverride> Overrides;
		static ModuleBuilder ModBuilder;
		static Random Rnd;

		static PropertyHooks() {
			Overrides = new List<PropertyOverride>();
			Rnd = new Random();

			AppDomain CurDoman = AppDomain.CurrentDomain;
			AssemblyName AsmName = new AssemblyName();
			AsmName.Name = nameof(PropertyHooks);
			AssemblyBuilder AsmBuilder = CurDoman.DefineDynamicAssembly(AsmName, AssemblyBuilderAccess.Run);
			ModBuilder = AsmBuilder.DefineDynamicModule(nameof(PropertyHooks) + "Mod");
		}

		public static void Hook(object Obj, SetOverrideAction SetOverride) {
			PropertyInfo[] Props = Obj.GetType().GetProperties().Where((P) => P.GetCustomAttribute(typeof(NetworkedAttribute)) != null).ToArray();

			TypeBuilder TypeBuilder = ModBuilder.DefineType("Overrides" + Rnd.Next().ToString() + Rnd.Next().ToString(), TypeAttributes.Public);
			string[] PropOverrideNames = new string[Props.Length];

			for (int i = 0; i < Props.Length; i++) {
				PropOverrideNames[i] = "_" + Rnd.Next().ToString() + Rnd.Next().ToString();

				MethodBuilder MethBuilder = TypeBuilder.DefineMethod(PropOverrideNames[i], MethodAttributes.Public | MethodAttributes.Static,
					typeof(void), new Type[] { typeof(object), Props[i].PropertyType });

				FieldInfo OverridesField = typeof(PropertyHooks).GetField(nameof(Overrides));
				PropertyInfo OverridesFieldIndexer = OverridesField.FieldType.GetProperties().Where((P) => P.GetIndexParameters().Length > 0).FirstOrDefault();
				MemberExpression OverrideMember = Expression.MakeMemberAccess(null, OverridesField);
				IndexExpression OverrideVal = Expression.MakeIndex(OverrideMember, OverridesFieldIndexer, new Expression[] { Expression.Constant(Overrides.Count + i) });

				ParameterExpression This = Expression.Parameter(typeof(object), "This");
				ParameterExpression Val = Expression.Parameter(Props[i].PropertyType, "Val");
				UnaryExpression ValAsObject = Expression.Convert(Val, typeof(object));

				MethodCallExpression SetOverrideCall = Expression.Call(SetOverride.GetMethodInfo(), This, ValAsObject, OverrideVal);
				Expression.Lambda(SetOverrideCall, This, Val).CompileToMethod(MethBuilder);
			}

			Type OverridesType = TypeBuilder.CreateType();

			for (int i = 0; i < Props.Length; i++) {
				MethodInfo SetMethodInfo = Props[i].GetSetMethod(true);
				Overrides.Add(new PropertyOverride(HookHandle.CreateHook(SetMethodInfo, OverridesType.GetMethod(PropOverrideNames[i])).Hook(), Props[i], Obj));
			}
		}
	}

	public static class NetworkManager {
		public static void SetOverride(object Origin, object Value, PropertyOverride Override) {
			Console.WriteLine(Console.Magenta + "{0}->{1} = {2}", Origin.GetType(), Override.Property.Name, Value);
			Override.Set(Value);
		}

		public static void Network(object Obj) {
			Console.WriteLine(Console.Magenta + "Type created: `{0}`", Obj);
			PropertyHooks.Hook(Obj, SetOverride);
		}
	}
}
