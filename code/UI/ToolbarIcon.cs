using Sandbox.UI;
using Sandbox.UI.Construct;

namespace guessit.UI
{
	public class ToolbarIcon : Panel
	{
		public string TargetColor;
		public Label Label;
		public Label Number;

		public ToolbarIcon( int i, Panel parent )
		{
			AddClass( "InventoryIcon" );
			
			Parent = parent;
			Label = Add.Label( "empty", "item-name" );
			Number = Add.Label( $"{i}", "slot-number" );
		}

		public void Clear()
		{
			Label.Text = "";
			SetClass( "active", false );
		}
	}
}
