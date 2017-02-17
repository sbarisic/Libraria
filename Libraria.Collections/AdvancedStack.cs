using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Libraria.Collections {
	[DebuggerDisplay("Count = {Count}")]
	public class AdvancedStack<T> : IEnumerable {
		T[] Values;
		int Top;

		public int Count {
			get {
				return Top;
			}
		}

		public AdvancedStack(int Count) {
			Values = new T[Count];
			Top = 0;
		}

		public AdvancedStack() : this(16) {
		}

		public IEnumerator GetEnumerator() {
			return Values.GetEnumerator();
		}

		public T this[int Idx] {
			get {
				if (Idx < 0 || Idx >= Top)
					return default(T);
				return Values[Idx];
			}

			set {
				if (Idx < 0 || Idx >= Top)
					return;
				Values[Idx] = value;
			}
		}

		public void Push(T Val) {
			if (Top >= Values.Length)
				Upsize();
			Values[Top++] = Val;
		}

		public void Push(params T[] Vals) {
			for (int i = 0; i < Vals.Length; i++)
				Push(Vals[i]);
		}

		public T Pop() {
			if (Top > 0) {
				T Ret = Values[--Top];
				Values[Top] = default(T);
				return Ret;
			}

			return default(T);
		}

		public T PopBottom() {
			if (Top > 0) {
				T Ret = Values[0];
				for (int i = 1; i < Top; i++)
					Values[i - 1] = Values[i];
				Values[--Top] = default(T);
				return Ret;
			}

			return default(T);
		}

		public T[] Pop(int Num) {
			T[] Arr = new T[Num];
			for (int i = 0; i < Num; i++)
				Arr[i] = Pop();
			return Arr;
		}

		public T Peek() {
			if (Top > 0)
				return Values[Top - 1];
			return default(T);
		}

		public void Reverse() {
			Array.Reverse(Values, 0, Top);
		}

		public T[] ToArray() {
			T[] Arr = new T[Top];
			for (int i = 0; i < Top; i++)
				Arr[i] = Values[i];
			return Arr;
		}

		public void Clear() {
			for (int i = 0; i < Values.Length; i++)
				Values[i] = default(T);
			Top = 0;
		}

		void Upsize() {
			const int LinearAfter = 65536;
			const int LinearAmount = 1024;

			int NewLen = 0;
			if (Values.Length >= LinearAfter)
				NewLen = Values.Length + LinearAmount;
			else
				NewLen = Values.Length * 2;

			Array.Resize(ref Values, NewLen);
		}
	}
}