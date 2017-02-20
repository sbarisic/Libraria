using System;
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
		public NetworkedAttribute() {
		}
	}

	public static class NetworkManager {
		public class PropertyOverride {
			public static void SetOverride(object Origin, object Value, int Idx) {
				PropertyOverride Override = Overrides[Idx];

				Console.WriteLine(Console.Magenta + "{0}->{1} = {2}", Origin.GetType(), Override.Prop.Name, Value);
			}

			public PropertyInfo Prop;
			public HookHandle Hook;

			public PropertyOverride(HookHandle Hook, PropertyInfo Prop) {
				this.Prop = Prop;
				this.Hook = Hook;
			}
		}

		delegate void IntSetter(object This, int Val);
		static List<PropertyOverride> Overrides;
		static ModuleBuilder ModBuilder;

		static NetworkManager() {
			Overrides = new List<PropertyOverride>();

			AppDomain CurDoman = AppDomain.CurrentDomain;
			AssemblyName AsmName = new AssemblyName();
			AsmName.Name = "DynAsm";
			AssemblyBuilder AsmBuilder = CurDoman.DefineDynamicAssembly(AsmName, AssemblyBuilderAccess.RunAndSave);
			ModBuilder = AsmBuilder.DefineDynamicModule("DynAsmMod");
		}

		public static void Network(object Obj) {
			Console.WriteLine(Console.Magenta + "Type created: `{0}`", Obj);
			PropertyInfo[] Props = Obj.GetType().GetProperties().Where((P) => P.GetCustomAttribute(typeof(NetworkedAttribute)) != null).ToArray();

			Random Rnd = new Random();
			TypeBuilder TypeBuilder = ModBuilder.DefineType("Overrides" + Rnd.Next().ToString() + Rnd.Next().ToString(), TypeAttributes.Public);

			string[] PropOverrideNames = new string[Props.Length];
			for (int i = 0; i < Props.Length; i++) {
				PropOverrideNames[i] = "_" + Rnd.Next().ToString() + Rnd.Next().ToString();
				MethodBuilder MethBuilder = TypeBuilder.DefineMethod(PropOverrideNames[i], MethodAttributes.Public | MethodAttributes.Static,
					typeof(void), new Type[] { typeof(object), Props[i].PropertyType });

				ParameterExpression This = Expression.Parameter(typeof(object), "This"), Val = Expression.Parameter(Props[i].PropertyType, "Val");
				Expression.Lambda(Expression.Call(typeof(PropertyOverride).GetMethod("SetOverride"), This, Expression.Convert(Val, typeof(object)),
					Expression.Constant(Overrides.Count + i)),
							This, Val).CompileToMethod(MethBuilder);
			}

			Type OverridesType = TypeBuilder.CreateType();

			for (int i = 0; i < Props.Length; i++) {
				MethodInfo SetMethodInfo = Props[i].GetSetMethod(true);
				Overrides.Add(new PropertyOverride(HookHandle.CreateHook(SetMethodInfo, OverridesType.GetMethod(PropOverrideNames[i])).Hook(), Props[i]));
			}
		}
	}
}
