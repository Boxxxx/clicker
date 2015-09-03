using System;
using System.Linq;
using System.Reflection;

namespace FullInspector.Internal {
    /// <summary>
    /// Implements ICustomAttributeProvider with the given set of attribute objects.
    /// </summary>
    public class fiAttributeProvider : MemberInfo {
        private readonly object[] _attributes;

        public fiAttributeProvider(params object[] attributes) {
            _attributes = attributes;
        }

        public override Type DeclaringType {
            get {
                throw new NotSupportedException();
            }
        }

        public override MemberTypes MemberType {
            get {
                throw new NotSupportedException();
            }
        }

        public override string Name {
            get {
                throw new NotSupportedException();
            }
        }

        public override Type ReflectedType {
            get {
                throw new NotSupportedException();
            }
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return _attributes;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return
                (from attr in _attributes
                 where attr.GetType().IsAssignableFrom(attributeType)
                 select attr).ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return
                (from attr in _attributes
                 where attr.GetType().IsAssignableFrom(attributeType)
                 select attr).Any();
        }
    }
}