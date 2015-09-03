using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
    [AttributeUsageAttribute(AttributeTargets.All)]
    public class StringValueAttribute : Attribute {
        private string _value;

        public StringValueAttribute(string value) {
            _value = value;
        }

        public string Value {
            get { return _value; }
        }
    }
}