using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Inspector.Engine
{
	public class AssemblyLoader
	{
		private readonly HashSet<string> _filePaths = new HashSet<string>();

		public Assembly LoadAssembly(string assemblyNameOrPath)
		{
			Assembly assembly;
			if (File.Exists(assemblyNameOrPath))
			{
				assembly = TryLoadAssemblyFromFile(assemblyNameOrPath);
			}
			else
			{
				assembly = TryLoadAssembly(assemblyNameOrPath);
			}

			if (assembly != null)
			{
				string assemblyFilePath = Path.GetDirectoryName(GetAssemblyLocalPath(assembly));
				if (assemblyFilePath != null)
				{
					_filePaths.Add(assemblyFilePath);
				}
			}

			return assembly;
		}

		public static bool IsSystemAssembly(string name)
		{
			return
				name.StartsWith("System") ||
				name.StartsWith("Microsoft") ||
				name.StartsWith("mscorlib");
		}

		public static bool IsSystemAssembly(Assembly assembly)
		{
			return IsSystemAssembly(assembly.GetName().Name);
		}

		public static string GetAssemblyLocalPath(Assembly assembly)
		{
			return new Uri(assembly.CodeBase).LocalPath;
		}

		private Assembly TryLoadAssembly(string assemblyFullName)
		{
			try
			{
				return Assembly.ReflectionOnlyLoad(assemblyFullName);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Could not load assembly " + assemblyFullName);
				Debug.WriteLine(ex);
			}

			return TryLoadAssemblyUsingProbingPaths(assemblyFullName);
		}

		private static Assembly TryLoadAssemblyFromFile(string filePath)
		{
			if (filePath == null || !File.Exists(filePath))
			{
				return null;
			}

			try
			{
				return Assembly.ReflectionOnlyLoadFrom(filePath);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Could not load assembly from file " + filePath);
				Debug.WriteLine(ex);
				return null;
			}
		}

		private Assembly TryLoadAssemblyUsingProbingPaths(string assemblyName)
		{
			foreach (string path in _filePaths)
			{
				string fullPath = Path.Combine(path, assemblyName + ".dll");
				Assembly assembly = TryLoadAssemblyFromFile(fullPath);
				if (assembly == null)
				{
					fullPath = Path.Combine(path, assemblyName + ".exe");
					assembly = TryLoadAssemblyFromFile(fullPath);
				}

				if (assembly != null)
				{
					return assembly;
				}
			}
			return null;			
		}
	}
}
