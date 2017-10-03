using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Libraria.Native {
	public class Tree<T> {
		public class TreeNode {
			public List<TreeNode> Children = new List<TreeNode>();
			public TreeNode Parent;

			public T Userdata;

			public bool HasChildren {
				get {
					return Children.Count > 0;
				}
			}

			public TreeNode(T Userdata = default(T)) {
				this.Userdata = Userdata;
			}

			public TreeNode(IEnumerable<TreeNode> Children) {
				Add(Children);
			}

			public void Add(TreeNode Node) {
				Children.Add(Node);
				Node.Parent = this;
			}

			public void Add(IEnumerable<TreeNode> Nodes) {
				foreach (var N in Nodes)
					Add(N);
			}

			public TreeNode[] PathToParent() {
				List<TreeNode> Nodes = new List<TreeNode>();
				Nodes.Add(this);

				TreeNode Last = null;
				while ((Last = Nodes.Last()).Parent != null)
					Nodes.Add(Last.Parent);

				return Nodes.ToArray();
			}

			public override string ToString() {
				if (Userdata == null)
					return "null";
				return Userdata.ToString();
			}
		}

		public TreeNode Root;

		public Tree(T RootUserdata, IEnumerable<TreeNode> Nodes) {
			Root = new TreeNode();
			Root.Userdata = RootUserdata;
			Root.Add(Nodes);
		}

		public TreeNode[] GetLeaves() {
			List<TreeNode> Leaves = new List<TreeNode>();
			Leaves.Add(Root);

		Repeat:
			for (int i = 0; i < Leaves.Count; i++) {
				if (Leaves[i].HasChildren) {
					Leaves.Replace(Leaves[i], Leaves[i].Children);
					goto Repeat;
				}
			}

			return Leaves.ToArray();
		}
	}
	
	public class NativeMethodInfo {
		public bool DoesOverride;
		public int VTableOffset;
		public int MethodIndex;

		public MethodInfo Method;
		public MethodInfo BaseMethod;

		public T GetDelegate<T>(IntPtr Instance) where T : class {
			return (T)GetDelegate(typeof(T), Instance);
		}

		public object GetDelegate(Type DelegateType, IntPtr Instance) {
			IntPtr VTablePtr = Marshal.ReadIntPtr(Instance, VTableOffset);
			IntPtr F = ReadVTableSlot(VTablePtr, MethodIndex);
			return Marshal.GetDelegateForFunctionPointer(F, DelegateType);
		}

		/*public static IntPtr GetVTablePtr(IntPtr Instance, int VTableIdx = 0) {
			return Marshal.ReadIntPtr(Instance, IntPtr.Size * VTableIdx);
		}*/

		public static IntPtr ReadVTableSlot(IntPtr VTable, int Slot) {
			return Marshal.ReadIntPtr(VTable, IntPtr.Size * Slot);
		}
	}

	public class NativeVariableInfo {
		public int Offset;
		public string Name;
		public Type VariableType;
		public PropertyInfo PropertyInfo;

		public IntPtr GetAddress(IntPtr Instance) {
			return Instance + Offset;
		}
	}

	public class NativeClassInfo {
		public List<NativeMethodInfo> MethodInfo;
		public List<NativeVariableInfo> VariableInfo;

		public NativeClassInfo() {
			MethodInfo = new List<NativeMethodInfo>();
			VariableInfo = new List<NativeVariableInfo>();
		}

		public NativeMethodInfo Find(MethodInfo Info, bool SkipOverrides = false) {
			NativeMethodInfo Ret = MethodInfo.FirstOrDefault((VMI) => VMI.Method == Info && (SkipOverrides ? VMI.DoesOverride != true : true));
			if (Ret != null && Ret.DoesOverride)
				return Find(Ret.BaseMethod, true);
			return Ret;
		}

		public NativeVariableInfo FindVariable(PropertyInfo Prop) {
			foreach (var VI in VariableInfo) {
				if (VI.PropertyInfo == Prop)
					return VI;
			}
			return null;
		}

		/*public void Invoke(IntPtr Instance, MethodInfo Info) {
			Find(Info).Invoke(Instance);
		}*/
	}

	public unsafe class VTable {
		public static VTable GetVTable(object Interface) {
			Type ObjType = Interface.GetType();
			FieldInfo VTableField = ObjType.GetField(nameof(VTable));

			if (VTableField == null || VTableField.FieldType != typeof(VTable))
				throw new Exception(Interface.ToString() + " not a valid interface type");

			VTable VT = (VTable)VTableField.GetValue(Interface);
			if (VT == null)
				VTableField.SetValue(Interface, VT = new VTable(Interface));

			return VT;
		}

		protected object Object;
		protected Type ObjectType;
		protected Type InterfaceType;

		protected IntPtr ObjectAddress;
		protected IntPtr* VTbl;

		protected Dictionary<int, IntPtr> Originals;
		protected List<GCHandle> NewHookHandles;

		private VTable(object Object) {
			Originals = new Dictionary<int, IntPtr>();
			NewHookHandles = new List<GCHandle>();

			this.Object = Object;
			ObjectType = Object.GetType();
			InterfaceType = ObjectType.GetInterfaces().First();

			ObjectAddress = (IntPtr)ObjectType.GetField("ObjectAddress").GetValue(Object);
			VTbl = *(IntPtr**)ObjectAddress;
		}

		public int GetIdxForName(string Name) {
			MethodInfo MethInfo = InterfaceType.GetMethod(Name);
			return MethInfo.GetCustomAttribute<VTableSlotAttribute>().Slot;
		}

		public IntPtr Hook(int Idx, IntPtr New) {
			if (!Originals.ContainsKey(Idx))
				Originals.Add(Idx, VTbl[Idx]);

			MemProtection P = MemProtection.ReadWrite;
			Kernel32.VirtualProtect((IntPtr)(&VTbl[Idx]), IntPtr.Size, P, out P);

			IntPtr Old = VTbl[Idx];
			VTbl[Idx] = New;
			return Old;
		}

		public IntPtr Hook(string Name, IntPtr New) {
			return Hook(GetIdxForName(Name), New);
		}

		public T Hook<T>(int Idx, T New) where T : class {
			NewHookHandles.Add(GCHandle.Alloc(New));
			return Marshal.GetDelegateForFunctionPointer(Hook(Idx, Marshal.GetFunctionPointerForDelegate(New)), typeof(T)) as T;
		}

		public T Hook<T>(string Name, T New) where T : class {
			return Hook<T>(GetIdxForName(Name), New);
		}

		public void Unhook(int Idx) {
			if (Originals.ContainsKey(Idx)) {
				IntPtr Old = Originals[Idx];
				Originals.Remove(Idx);
				VTbl[Idx] = Old;
			}
		}

		public void UnhookAll() {
			int[] Hooked = Originals.Keys.ToArray();
			for (int i = 0; i < Hooked.Length; i++)
				Unhook(Hooked[i]);

			foreach (var Handle in NewHookHandles)
				Handle.Free();
			NewHookHandles.Clear();
		}

		// VTable crap
		static IEnumerable<Tree<Type>.TreeNode> GetNodes(Type T) {
			Type[] Interfaces = T.GetInterfaces();
			Type[] Types = Interfaces.Except(Interfaces.SelectMany(Typ => Typ.GetInterfaces())).ToArray();

			for (int i = 0; i < Types.Length; i++) {
				Tree<Type>.TreeNode N = new Tree<Type>.TreeNode(GetNodes(Types[i]));
				N.Userdata = Types[i];
				yield return N;
			}
		}

		static bool BaseContains(MethodInfo MethodInfo, out MethodInfo BaseMethodInfo) {
			Type IntType = MethodInfo.DeclaringType;

			Type[] AllInterfaces = IntType.GetInterfaces();
			for (int i = 0; i < AllInterfaces.Length; i++) {
				MethodInfo[] AllMethods = AllInterfaces[i].GetMethods();

				for (int j = 0; j < AllMethods.Length; j++) {
					MethodInfo BaseM = AllMethods[j];
					if (MethodInfo.Name == BaseM.Name && MethodInfo.ReturnType == BaseM.ReturnType) {
						ParameterInfo[] MInfParams = MethodInfo.GetParameters();
						ParameterInfo[] BaseParams = BaseM.GetParameters();

						if (MInfParams.Length != BaseParams.Length)
							continue;

						bool Continue = false;
						for (int k = 0; k < MInfParams.Length; k++)
							if (MInfParams[k].ParameterType != BaseParams[k].ParameterType) {
								Continue = true;
								break;
							}

						if (Continue)
							continue;

						BaseMethodInfo = BaseM;
						return true;
					}
				}
			}

			BaseMethodInfo = null;
			return false;
		}

		static bool IsPropertyMethod(MethodInfo MI) {
			return GetPropertyForMethod(MI) != null;
			//return MI.DeclaringType.GetProperties().Any((P) => P.GetSetMethod() == MI || P.GetGetMethod() == MI);
		}

		static PropertyInfo GetPropertyForMethod(MethodInfo MI) {
			PropertyInfo[] Props = MI.DeclaringType.GetProperties();

			for (int i = 0; i < Props.Length; i++)
				if (Props[i].GetGetMethod() == MI || Props[i].GetSetMethod() == MI)
					return Props[i];

			return null;
		}

		public static int SizeOfVariables(Type T) {
			return T.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Select((PI) => Marshal.SizeOf(PI.PropertyType)).Sum();
		}

		public static int VTablePointerSize = IntPtr.Size;

		static void CalculateVTableLayout(Type T, Action<int, Type> OnTypeResolve) {
			Tree<Type> InterfaceTree = new Tree<Type>(T, GetNodes(T));
			Tree<Type>.TreeNode[] Leaves = InterfaceTree.GetLeaves();

			List<Type> Interfaces = new List<Type>();
			int VTableOffset = 0;

			for (int i = 0; i < Leaves.Length; i++) {
				Tree<Type>.TreeNode[] PTP = Leaves[i].PathToParent();

				int SizeOffset = 0;

				for (int j = 0; j < PTP.Length; j++) {
					if (!Interfaces.Contains(PTP[j].Userdata)) {
						Interfaces.Add(PTP[j].Userdata);

						SizeOffset += SizeOfVariables(PTP[j].Userdata);
						OnTypeResolve(VTableOffset, PTP[j].Userdata);
					}
				}

				VTableOffset += VTablePointerSize + SizeOffset;
			}
		}

		public static NativeClassInfo CalculateClassLayout(Type T) {
			List<NativeMethodInfo> MethodInfos = new List<NativeMethodInfo>();
			List<NativeVariableInfo> VariableInfos = new List<NativeVariableInfo>();

			Dictionary<int, int> MethodCounter = new Dictionary<int, int>();
			Dictionary<int, int> VariableOffsets = new Dictionary<int, int>();

			CalculateVTableLayout(T, (VTableOffset, Interface) => {
				MethodInfo[] Methods = Interface.GetMethods();

				if (!MethodCounter.ContainsKey(VTableOffset)) {
					MethodCounter.Add(VTableOffset, 0);
					VariableOffsets.Add(VTableOffset, VTablePointerSize);
				}

				for (int i = 0; i < Methods.Length; i++) {
					PropertyInfo Prop;
					if ((Prop = GetPropertyForMethod(Methods[i])) != null) {
						if (Prop.GetSetMethod() == Methods[i])
							continue;

						NativeVariableInfo VarInfo = new NativeVariableInfo();
						VarInfo.Offset = VariableOffsets[VTableOffset];

						VarInfo.PropertyInfo = Prop;
						VarInfo.VariableType = VarInfo.PropertyInfo.PropertyType;
						VarInfo.Name = VarInfo.PropertyInfo.Name;

						VariableOffsets[VTableOffset] += Marshal.SizeOf(VarInfo.VariableType);
						VariableInfos.Add(VarInfo);
						continue;
					}

					NativeMethodInfo VMethInfo = new NativeMethodInfo();
					VMethInfo.Method = Methods[i];

					MethodInfo BMI;
					if (BaseContains(Methods[i], out BMI)) {
						VMethInfo.BaseMethod = BMI;
						VMethInfo.DoesOverride = true;
					} else {
						VMethInfo.BaseMethod = null;
						VMethInfo.MethodIndex = MethodCounter[VTableOffset]++;
						VMethInfo.VTableOffset = VTableOffset;
						VMethInfo.DoesOverride = false;
					}

					MethodInfos.Add(VMethInfo);
				}
			});

			NativeClassInfo Ret = new NativeClassInfo();
			Ret.MethodInfo.AddRange(MethodInfos);
			Ret.VariableInfo.AddRange(VariableInfos);
			return Ret;
		}
	}
}
