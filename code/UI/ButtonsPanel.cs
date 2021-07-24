using System;
using guessit.Rounds;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace guessit.UI
{
	[Library]
	public partial class ButtonsPanel : Panel
	{
		private Panel btnContainer;
		
		private Button button1;
		private Button button2;
		private Button button3;

		private Label explainerLabel;

		public ButtonsPanel()
		{
			this.StyleSheet.Load( "/ui/ButtonsPanel.scss" );
			AddClass( "btnpanel" );

			this.btnContainer = Add.Panel( "flexcontainer" );

			var textContainer = this.btnContainer.Add.Panel( "textcontainer" );

			this.explainerLabel = textContainer.Add.Label( "Start the game?" );
			
			var buttonsContainer = this.btnContainer.Add.Panel( "btncontainer" );
			
			this.button1 = buttonsContainer.Add.Button( "Button 1", "button" );
			this.button2 = buttonsContainer.Add.Button( "Button 2", "button" );
			this.button3 = buttonsContainer.Add.Button( "Button 3", "button" );
			
			this.button1.AddEventListener( "onclick", Button1Click );
			this.button2.AddEventListener( "onclick", Button2Click );
			this.button3.AddEventListener( "onclick", Button3Click );
			
			Add.Label( "Hold Space to draw!", "alert" );
		}

		public override void Tick()
		{
			base.Tick();
			
			var player = Local.Pawn;
			if ( player == null ) return;

			if ( GuessItGame.Instance.CurrentRound == RoundKind.Lobby && Local.Client.HasPermission( "admin" ) )
			{
				this.btnContainer.RemoveClass( "nodisplay" );
				
				this.explainerLabel.Text = "Everyone ready?";
				
				this.button1.AddClass( "nodisplay" );
				this.button2.RemoveClass( "nodisplay" );
				this.button3.AddClass( "nodisplay" );

				this.button2.Text = "Start!";
			}
			else if ( GuessItGame.Instance.CurrentRound == RoundKind.InGamePickNextWord &&
			          GuessItGame.Instance.CurrentPlayer == player )
			{
				this.btnContainer.RemoveClass( "nodisplay" );
				
				this.explainerLabel.Text = "It's your turn - pick a word!";
				
				this.button1.RemoveClass( "nodisplay" );
				this.button2.RemoveClass( "nodisplay" );
				this.button3.RemoveClass( "nodisplay" );
				
				this.button1.Text = GuessItGame.Instance.Candidates[0];
				this.button2.Text = GuessItGame.Instance.Candidates[1];
				this.button3.Text = GuessItGame.Instance.Candidates[2];
			}
			else if ( GuessItGame.Instance.CurrentRound == RoundKind.InGamePickNextWord && GuessItGame.Instance.CurrentPlayer != player )
			{
				this.btnContainer.AddClass( "nodisplay" );
			}
			else if ( GuessItGame.Instance.CurrentRound == RoundKind.InGame )
			{
				this.btnContainer.AddClass( "nodisplay" );
			}
			else
			{
				this.btnContainer.RemoveClass( "nodisplay" );
				
				this.explainerLabel.Text = "Waiting for host to start...";
				
				this.button1.AddClass( "nodisplay" );
				this.button2.AddClass( "nodisplay" );
				this.button3.AddClass( "nodisplay" );
			}
			
			

			//TitleText = player.GetClientOwner().SteamId.ToString();
		}
		
		public void Update()
		{
			
		}

		private void Button1Click( PanelEvent obj )
		{
			if ( GuessItGame.Instance.CurrentRound == RoundKind.InGamePickNextWord )
			{
				GuessItGame.PickWord( GuessItGame.Instance.Candidates[0] );
			}
		}
		
		private void Button2Click( PanelEvent obj )
		{
			if ( GuessItGame.Instance.CurrentRound == RoundKind.Lobby )
			{
				GuessItGame.StartGame();
			}
			else if ( GuessItGame.Instance.CurrentRound == RoundKind.InGamePickNextWord )
			{
				GuessItGame.PickWord( GuessItGame.Instance.Candidates[1] );
			}
		}
		
		private void Button3Click( PanelEvent obj )
		{
			if ( GuessItGame.Instance.CurrentRound == RoundKind.InGamePickNextWord )
			{
				GuessItGame.PickWord( GuessItGame.Instance.Candidates[2] );
			}
		}
	}
}
