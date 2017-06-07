using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Libraria.Rendering;
using OpenTK.Input;

namespace LibTech {
	public enum ModuleEvent {
		NONE = 0,
		UPDATE, RENDER
	}

	/*public interface IModule {
		void Open(IModule Client, IModule Server, IModule UI);
		void Close();

		bool OnMouseMove(int X, int Y, int RelativeX, int RelativeY);
		bool OnMouseButton(int Clicks, int Button, int X, int Y, bool Pressed);
		bool OnMouseWheel(int X, int Y);
		bool OnKey(int Repeat, Scancodes Scancode, int Keycode, int Mod, bool Pressed);
		bool OnTextInput(string Txt);

		void Update(float Dt);
		void Render(float Dt);
	}*/

	public delegate void OnMessageAction(string MessageName, params object[] Args);

	public class ModuleBase {
		public event OnMessageAction OnMessage;

		public virtual void Message(string MessageName, params object[] Args) {
			OnMessage?.Invoke(MessageName, Args);
		}

		public virtual void Open() {
		}

		public virtual void Close() {
		}

		public virtual bool OnKey(KeyboardKeyEventArgs E, bool Pressed) {
			return false;
		}

		public virtual bool OnMouseButton(MouseButtonEventArgs E, bool Pressed) {
			return false;
		}

		public virtual bool OnMouseMove(MouseMoveEventArgs E) {
			return false;
		}

		public virtual bool OnMouseWheel(MouseWheelEventArgs E) {
			return false;
		}

		public virtual bool OnTextInput(string Txt) {
			return false;
		}

		public virtual void Update(float Dt) {
		}

		public virtual void Render(float Dt) {
		}
	}

	public class ModuleLoader {
		public static ModuleBase LoadModule(string Pth) {
			Pth = Files.GetFullPath(Pth);
			if (!Pth.ToLower().EndsWith(".dll"))
				Pth += ".dll";
			Pth = Files.GetFilePath(Pth);

			if (!File.Exists(Pth))
				throw new FileNotFoundException("Could not find module", Pth);
			Console.WriteLine("Loading '{0}'", Files.GetRelativePath(Pth, false));

			Assembly Asm = Assembly.LoadFile(Pth);
			Type[] Types = Asm.GetExportedTypes();

			//Types = Types.Where(T => T.GetInterfaces().Contains(typeof(IModule))).ToArray();
			Types = Types.Where(T => T.BaseType == typeof(ModuleBase)).ToArray();

			if (Types.Length != 1)
				throw new Exception("Assembly must implement exactly one IModule");

			ModuleBase Mod = (ModuleBase)Activator.CreateInstance(Types[0]);
			return Mod;
		}

		public static ModuleBase LoadModule(params string[] Pth) {
			return LoadModule(Path.Combine(Pth));
		}

		public static void UnloadModule(ModuleBase Mod) {
			if (Mod == null)
				return;
			Mod.Close();
		}
	}
}