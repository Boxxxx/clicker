using System;
using System.Collections;
using System.Collections.Generic;

namespace Utils {
    public class VarPool : IEnumerable, ICloneable {
        private Dictionary<string, object> _variables = new Dictionary<string, object>();

        public object this[string key] {
            set {
                _variables[key] = value;
            }
            get {
                return _variables[key];
            }
        }
        public string[] Keys {
            get { return _variables.Keys.ToArray(); }
        }
        public object[] Values {
            get { return _variables.Values.ToArray(); }
        }

        public IEnumerator GetEnumerator() {
            return _variables.GetEnumerator();
        }

        public VarPool() { }
        public VarPool(VarPool other) {
            _variables = new Dictionary<string, object>(other._variables);
        }

        public int Count {
            get { return _variables.Count; }
        }
        public KeyValuePair<string, object>[] ToArray() {
            return _variables.ToArray();
        }
        public object Clone() {
            return new VarPool(this);
        }
        public bool Contains(string key) {
            return _variables.ContainsKey(key);
        }
        public bool Remove(string key) {
            return _variables.Remove(key);
        }
        public void LeftJoin(VarPool varPool) {
            foreach (var key in varPool.Keys) {
                if (!_variables.ContainsKey(key)) {
                    _variables[key] = varPool[key];
                }
            }
        }
        public void RightRemove(VarPool varPool) {
            foreach (var key in varPool.Keys) {
                _variables.Remove(key);
            }
        }

        public bool HasInt(string key) {
            return Contains(key) && (_variables[key] is int || _variables[key] is Int64 || _variables[key] is uint || _variables[key] is UInt64);
        }
        public bool HasFloat(string key) {
            return Contains(key) && (_variables[key] is float || _variables[key] is double || HasInt(key));
        }
        public bool HasString(string key) {
            return Contains(key) && _variables[key] is string;
        }
        public bool HasBool(string key) {
            return Contains(key) && _variables[key] is bool;
        }

        public int GetInt(string key) {
            return Convert.ToInt32(_variables[key]);
        }
        public float GetFloat(string key) {
            return Convert.ToSingle(_variables[key]);
        }
        public string GetString(string key) {
            return _variables[key].ToString();
        }
        public bool GetBool(string key) {
            return Convert.ToBoolean(_variables[key]);
        }

        public void AddInt(string key, int value) {
            _variables[key] = value;
        }
        public void AddFloat(string key, float value) {
            _variables[key] = value;
        }
        public void AddString(string key, string value) {
            _variables[key] = value;
        }
        public void AddBool(string key, bool value) {
            _variables[key] = value;
        }

        public static VarPool CreateFromList(params object[] args) {
            Asserts.Assert((args.Length & 1) == 0);

            VarPool varPool = new VarPool();
            for (var i = 0; i < args.Length; i += 2) {
                varPool[args[i].ToString()] = args[i + 1];
            }
            return varPool;
        }
    }
}
