using System.Reflection;

namespace Inspector.Engine
{
	public class AssemblyNode
	{
		public Assembly Assembly { get; private set; }

		public AssemblyNode(Assembly assembly)
		{
			Assembly = assembly;
		}

		public bool Equals(AssemblyNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Assembly.GetName().Name, Assembly.GetName().Name);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (AssemblyNode)) return false;
			return Equals((AssemblyNode) obj);
		}

		public override int GetHashCode()
		{
			return Assembly.GetName().Name.GetHashCode();
		}
	}
}
