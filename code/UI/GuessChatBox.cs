using Sandbox;
using Sandbox.Hooks;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using guessit.Player;
using guessit.Rounds;

namespace guessit.UI
{
	public partial class GuessChatBox : Panel
	{
		static GuessChatBox Current;

		public Panel Canvas { get; protected set; }
		public TextEntry Input { get; protected set; }

		public GuessChatBox()
		{
			Current = this;
			
			AddClass( "chatbox" );

			this.StyleSheet.Load( "/ui/GuessChatBox.scss" );

			Canvas = Add.Panel( "chat_canvas" );

			Input = Add.TextEntry( "" );
			Input.AddEventListener( "onsubmit", () => Submit() );
			Input.AddEventListener( "onblur", () => Close() );
			Input.AcceptsFocus = true;
			Input.AllowEmojiReplace = true;

			Chat.OnOpenChat += Open;
		}

		void Open()
		{
			AddClass( "open" );
			Input.Focus();
		}

		void Close()
		{
			RemoveClass( "open" );
			Input.Blur();
		}

		void Submit()
		{
			Close();

			var msg = Input.Text.Trim();
			Input.Text = "";

			if ( string.IsNullOrWhiteSpace( msg ) )
				return;

			Say( msg );
		}

		public void AddEntry( string name, string message, string avatar )
		{
			var e = Canvas.AddChild<GuessChatEntry>();
			//e.SetFirstSibling();
			e.Message.Text = message;
			e.NameLabel.Text = name;
			e.Avatar.SetTexture( avatar );

			e.SetClass( "noname", string.IsNullOrEmpty( name ) );
			e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );
		}


		[ClientCmd( "guess_chat_add", CanBeCalledFromServer = true )]
		public static void AddChatEntry( string name, string message, string avatar = null )
		{
			Current?.AddEntry( name, message, avatar );

			// Only log clientside if we're not the listen server host
			if ( !Global.IsListenServer )
			{
				Log.Info( $"{name}: {message}" ); 
			}
		}

		[ClientCmd( "guess_chat_addinfo", CanBeCalledFromServer = true )]
		public static void AddInformation( string message, string avatar = null )
		{
			Current?.AddEntry( null, message, avatar );
		}

		[ServerCmd( "guess_say" )]
		public static void Say( string message )
		{
			Assert.NotNull( ConsoleSystem.Caller );

			// todo - reject more stuff
			if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
				return;

			if ( GuessItGame.Instance.CurrentRound == RoundKind.InGame &&
			     message.ToLowerInvariant().Contains( GuessItGame.Instance.CurrentWord ))
			{
				if ( ConsoleSystem.Caller.Pawn == GuessItGame.Instance.CurrentPlayer )
				{
					AddChatEntry( To.Single( ConsoleSystem.Caller ), "System", "You can't solve your own word!", $"avatar:{ConsoleSystem.Caller.SteamId}" );
					return;
				}
				
				foreach ( var client in Client.All )
				{
					var player = client.Pawn as GuessPlayer;
					player.SendSound( "solve" );
				}
				
				var pctComplete = (GuessItGame.Instance.GetSecondsLeft() / (float) GuessItGame.RoundLength);
				var score = ConsoleSystem.Caller.GetScore( "points", 0 );

				score += (int) (GuessItGame.MaxPoints * pctComplete);

				ConsoleSystem.Caller.SetScore( "points", score );
				ConsoleSystem.Caller.SetScore( "solved", true );
			
				AddChatEntry( To.Everyone, "System", $"{ConsoleSystem.Caller.Name} solved!", $"avatar:{ConsoleSystem.Caller.SteamId}" );
				return;
			}
			
			// TODO: "You are close!"

			Log.Info( $"{ConsoleSystem.Caller}: {message}" );
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.SteamId}" );
		}

	}
}
