using System;
using guessit.Player;
using guessit.Rounds;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace guessit.UI
{
	[Library]
	public partial class RoundPanel : Panel
	{
		private Label titleLabel;
		private Label timeLabel;
		
		private Panel timerProgressPanel;

		public RoundPanel()
		{
			this.StyleSheet.Load( "/ui/roundpanel.scss" );
			AddClass( "roundpanel" );

			var rbox = Add.Panel( "roundbox" );

			var rcontainer = rbox.Add.Panel("roundcontainer");

			var textcontainer = rcontainer.Add.Panel( "rcontainertext" );
			this.titleLabel = textcontainer.Add.Label("Waiting...", "title");
			this.timeLabel = textcontainer.Add.Label( "0", "time" );
			
			this.timerProgressPanel = rcontainer.Add.Panel( "timerprogress" );
		}
		
		public override void Tick()
		{
			base.Tick();
			
			var player = Local.Pawn;
			if ( player == null ) return;

			if ( GuessItGame.Instance.CurrentRound != RoundKind.InGame )
			{
				this.Style.PointerEvents = "all";
			}
			else
			{
				this.Style.PointerEvents = "none";
			}
			this.Style.Dirty();

			switch ( GuessItGame.Instance.CurrentRound )
			{
				case RoundKind.Lobby:
					this.titleLabel.Text = "Waiting for players...";
					break;
				case RoundKind.InGame:
					this.titleLabel.Text = GuessItGame.Instance.CurrentWord;
					break;
				case RoundKind.InGameAfterRound:
					this.titleLabel.Text = "Results!";
					break;
				case RoundKind.InGamePickNextWord:
					this.titleLabel.Text = GuessItGame.Instance.CurrentPlayer != null ? $"{GuessItGame.Instance.CurrentPlayer.GetClientOwner().Name} is picking the next word..." : "Waiting for host...";
					break;
				case RoundKind.Results:
					this.titleLabel.Text = "Results!";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var secondsLeft = GuessItGame.Instance.GetSecondsLeft();
			var pctComplete = (secondsLeft / (float) GuessItGame.RoundLength) * 100f;

			if ( GuessItGame.Instance.CurrentRound == RoundKind.InGame ||
			     GuessItGame.Instance.CurrentRound == RoundKind.InGameAfterRound )
			{
				this.timerProgressPanel.SetClass( "nodisplay", false );
				this.timerProgressPanel.Style.Width = Length.Percent( (float) pctComplete );
				this.timerProgressPanel.Style.Dirty();
				
				this.timeLabel.RemoveClass( "nodisplay" );
				this.timeLabel.Text = ((int)secondsLeft).ToString();
			}
			else
			{
				this.timerProgressPanel.SetClass( "nodisplay", true );
				this.timeLabel.AddClass( "nodisplay" );
			}
		}
	}
}
