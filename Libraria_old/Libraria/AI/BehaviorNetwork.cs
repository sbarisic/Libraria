using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkAction = System.Action<Libraria.AI.BehaviorNetwork>;
using DecisionSortFunc = System.Func<Libraria.AI.BehaviorNetwork, int>;
using ActionTuple = System.Tuple<System.Action<Libraria.AI.BehaviorNetwork>, System.Func<Libraria.AI.BehaviorNetwork, int>>;

namespace Libraria.AI {
	public class BehaviorNetwork {
		Dictionary<string, object> Properties;
		HashSet<ActionTuple> Actions;

		public float Sanity;

		public BehaviorNetwork() {
			Properties = new Dictionary<string, object>();
			Actions = new HashSet<Tuple<Action<BehaviorNetwork>, Func<BehaviorNetwork, int>>>();
			Sanity = 0.998f;
		}

		public object this[string Name]
		{
			get
			{
				return Properties[Name];
			}
			set
			{
				Properties[Name] = value;
			}
		}

		public void Add(NetworkAction Action, DecisionSortFunc Weight) {
			Actions.Add(new ActionTuple(Action, Weight));
		}

		public void Decide() {
			ActionTuple[] OrderedActions = Actions.OrderBy((K) => K.Item2(this)).Reverse().ToArray();
			for (int i = 0; i < OrderedActions.Length; i++)
				if (Sane()) {
					OrderedActions[i].Item1(this);
					break;
				}
		}

		bool Sane() {
			return Rand.Chance() <= Sanity;
		}
	}
}