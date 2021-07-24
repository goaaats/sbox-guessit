using guessit.UI;
using Sandbox;
using Sandbox.UI;

namespace guessit.UI
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	[Library]
	public partial class MinimalHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public static MinimalHudEntity Current { get; private set; }
		
		public RoundPanel RoundPanel { get; private set; }
		public GuessScoreboard<GuessScoreboardEntry> PlayerPanel { get; private set; }
		public ButtonsPanel ButtonsPanel { get; private set; }
		public ToastList ToastList { get; private set; }
		
		public MinimalHudEntity()
		{
			Current = this;
			
			if ( IsClient )
			{
				PlayerPanel = RootPanel.AddChild<GuessScoreboard<GuessScoreboardEntry>>();
				RoundPanel = RootPanel.AddChild<RoundPanel>();
				ButtonsPanel = RootPanel.AddChild<ButtonsPanel>();
				ToastList = RootPanel.AddChild<ToastList>();
				RootPanel.AddChild<Toolbar>();

				var chat = RootPanel.AddChild<GuessChatBox>();
			}
		}
	}

}
