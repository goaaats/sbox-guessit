using System.Diagnostics;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace guessit.UI
{
	[Library]
	public partial class GuessScoreboardEntry : Panel
	{
		private PlayerScore.Entry entry;
		
		private Label name;
		private Label points;
		private Label rank;
		private Image avatar;
		private Label drawingIcon;

		private Panel crownPanel;
		
		public GuessScoreboardEntry()
		{
			AddClass( "playerentry" );

			var container = Add.Panel("container");
			var left = container.Add.Panel("left");
			var right = container.Add.Panel("right");

			var avatarContainer = left.Add.Panel( "avatarcontainer" );
			this.crownPanel = avatarContainer.Add.Panel( "avatar-crown" );
			this.avatar = avatarContainer.Add.Image(null, "avatar");

			var panel = left.Add.Panel("namepts");
			this.name = panel.Add.Label("Name", "name");
			this.points = panel.Add.Label("100", "points");

			this.rank = right.Add.Label("#1", "rank");
			
			this.drawingIcon = container.Add.Label( "edit", "editicon" );
		}

		public virtual void UpdateFrom( PlayerScore.Entry entry )
		{
			var rank = entry.Get("rank", 1);
			var points = entry.Get( "points", 0 );

			this.name.Text = entry.GetString( "name" );
			this.points.Text = points.ToString();
			this.rank.Text = "#" + rank;
			this.avatar.SetTexture( $"avatar:{entry.Get<ulong>( "steamid", 0 )}" );

			var drawing = entry.Get( "isdrawing", false );
			this.drawingIcon.SetClass( "nodisplay", !drawing );
			this.rank.SetClass( "nodisplay", drawing );
			
			this.crownPanel.SetClass( "avatar-crown-nodisplay", !(rank == 1 && points != 0) );
			
			SetClass( "drawing", drawing );
			SetClass( "solved", entry.Get<bool>( "solved" ) );
		}
	}
}
