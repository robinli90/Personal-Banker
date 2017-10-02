using System;
using System.Collections;

namespace Opulos.Core.Utils {

public class MultiKey {

	public StringComparer Comparer { get; private set; }
	private ArrayList keys = new ArrayList();

	public MultiKey(IEnumerable e) { //ICollection col) {
		var e2 = e.GetEnumerator();
		while (e2.MoveNext())
			keys.Add(e2.Current);
		//keys.AddRange(col);
	}

	public MultiKey(Object key1, Object key2, params Object[] keys) {
		this.keys.Add(key1);
		this.keys.Add(key2);
		this.keys.AddRange(keys);
	}

	public MultiKey(StringComparer comparer, params Object[] keys) {
		this.Comparer = comparer;
		this.keys.AddRange(keys);
	}

	public Object[] Keys {
		get {
			return keys.ToArray();
		}
	}

	public override bool Equals(Object obj) {
		if (!(obj is MultiKey))
			return false;

		MultiKey mk = (MultiKey) obj;
		if (mk.keys.Count != keys.Count)
			return false;

		var c = Comparer;
		if (c != null) {
			for (int i = 0; i < mk.keys.Count; i++) {
				Object o1 = mk.keys[i];
				Object o2 = keys[i];
				if (o1 == null) {
					if (o2 == null)
						continue;
					return false;
				}

				if (o1 is String && o2 is String) {
					if (c.Compare((String) o1, (String) o2) != 0)
						return false;
				}
				else if (!o1.Equals(o2))
					return false;
			}
		}
		else {
			for (int i = 0; i < mk.keys.Count; i++) {
				Object o1 = mk.keys[i];
				Object o2 = keys[i];
				if (o1 == null) {
					if (o2 == null)
						continue;
					return false;
				}
				if (!o1.Equals(o2))
					return false;
			}
		}
		return true;
	}

	public override int GetHashCode() {
		unchecked { // Overflow is fine, just wrap
			int hash = 17;
			var c = Comparer;
			if (c != null) {
				foreach (Object o in keys) {
					if (o == null)
						continue;

					int hc;
					if (o is String)
						hc = c.GetHashCode((String) o);
					else
						hc = o.GetHashCode();

					hash = hash * 29 + hc;
				}
			}
			else {
				foreach (Object o in keys) {
					if (o == null)
						continue;
					hash = hash * 29 + o.GetHashCode();
				}
			}

			return hash;
		}
	}
}
}