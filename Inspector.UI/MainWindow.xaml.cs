using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphSharp.Controls;
using Inspector.Engine;
using Microsoft.Win32;
using QuickGraph;

namespace Inspector.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly DependencyGraphBuilder _graphBuilder = new DependencyGraphBuilder();

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		private void OpenAssemblyButtonClick(object sender, RoutedEventArgs e)
		{
			try
			{
				AddAssembly();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void AddAssembly()
		{
			var openFileDialog = new OpenFileDialog();
			openFileDialog.CheckFileExists = true;
			openFileDialog.Title = "Select an MS Project file";
			if (openFileDialog.ShowDialog(this).Value)
			{
				_graphBuilder.AddAssemblyAndAllDependencies(openFileDialog.FileName);
				BindGraph(_graphBuilder.BuildAssemblyDependencyGraph().ToBidirectionalGraph());
			}
		}

		public void BindGraph(BidirectionalGraph<object, IEdge<object>> graph)
		{
			Graph = graph;

			var bindingExpression = _graphLayout.GetBindingExpression(GraphLayout.GraphProperty);
			if (bindingExpression != null)
			{
				bindingExpression.UpdateTarget();
			}
		}

		public BidirectionalGraph<object, IEdge<object>> Graph { get; set; }
	}
}
