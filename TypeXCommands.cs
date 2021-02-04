using System;
using System.Windows.Input;
namespace CustomCommands
{
	public static class TypeXCommands
	{
		public TypeXCommands()
		{
		}
		public static readonly RoutedUICommand AutoSaveCmd = new RoutedUICommand("AutoSaveCmd", "AutoSaveCmd", typeof(TypeXCommands),
				new InputGestureCollection()
				{
					new KeyGesture(Key.T,ModifierKeys.Control)
				});
	}
}
