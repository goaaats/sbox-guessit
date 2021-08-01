using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using guessit.Player;
using guessit.Rounds;
using guessit.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace guessit
{
	[Library( "guessit" )]
	public partial class GuessItGame : Sandbox.Game
	{
		public static GuessItGame Instance => Current as GuessItGame;

		[ServerVar( "guess_max_points", Help = "The maximum amount of points to gain each round." )]
		public static int MaxPoints { get; set; } = 1000;
		
		[ServerVar( "guess_min_players", Help = "The minimum players required to start." )]
		public static int MinPlayers { get; set; } = 2;
		public bool CanStartGame => Client.All.Count >= MinPlayers;

		[ServerVar( "guess_round_duration", Help = "The duration of a round of Guess It!" )]
		public static int RoundLength { get; set; } = 45;
		public bool HasGivenHint { get; set; }
		
		public DateTimeOffset RoundStartTime { get; private set; }
		
		public double GetSecondsLeft() => this.GetSecondsLeft(RoundLength);

		public double GetSecondsLeft( double time ) =>
			Math.Clamp( time - (DateTimeOffset.Now - RoundStartTime).TotalSeconds, 0, time );

		public List<GuessPlayer> Players { get; set; } = new List<GuessPlayer>(); 
		public List<GuessPlayer> PlayedThisPeriod { get; set; } = new List<GuessPlayer>();
		
		[Net] public GuessPlayer CurrentPlayer { get; set; }
		[Net] public int NumPeriod { get; set; }
		
		public string CurrentWord { get; set; }
		
		// TODO: Sync via [Net] instead?
		public RoundKind CurrentRound { get; set; }

		public List<string> Candidates { get; set; } = new List<string>();

		public Words Words { get; private set; }

		public GuessItGame()
		{
			if ( IsServer )
			{
				Log.Info( "My Gamemode Has Created Serverside!" );
				
				new MinimalHudEntity();
			}

			if ( IsClient )
			{
				Log.Info( "My Gamemode Has Created Clientside!" );
			}

			Words = new Words();
		}

		// In this gamemode, position doesn't matter for voice
		public override bool CanHearPlayerVoice( Client source, Client dest )
		{
			return true;
		}

		[ServerCmd]
		public static void StartGame()
		{
			Host.AssertServer();
			
			Log.Info( "Starting game..." );

			if ( !Instance.CanStartGame )
			{
				Log.Error( "Can't start game with less than two players." );
				return;
			}
			
			Instance.StartNextRound();
			
			Log.Info( "OK!" );
		}

		[ServerCmd]
		public static void PickWord( string word )
		{
			Host.AssertServer();
			
			Log.Info( $"[S] Got chosen word: {word}" );
			Instance.RoundStartTime = DateTimeOffset.Now;
			Instance.CurrentRound = RoundKind.InGame;
			Instance.CurrentWord = word;
			
			using ( Prediction.Off() )
			{
				Instance.NotifyCurrentWord( To.Single( Instance.CurrentPlayer ), word );
				Instance.NotifyCurrentWord( To.Multiple( Client.All.Where( x => x.Pawn != Instance.CurrentPlayer ) ), Words.GetWordWithPlaceholders( word ) );
				
				Instance.SendNextRound( RoundKind.InGame, null, null, null );
			}
		}

		// Update the current word, with or without hints
		[ClientRpc]
		public void NotifyCurrentWord(string word)
		{
			Host.AssertClientOrMenu();
			
			Log.Info( $"[C] Has received word update: {word}" );
			CurrentWord = word;
		}

		[ServerCmd("guess_skip_round")]
		public static void CmdSkipRound()
		{
			Instance.StartNextRound();
		}
		
		public void StartNextRound()
		{
			Host.AssertServer();
			
			var candidates = Words.GetCandidates();
			
			CurrentPlayer?.GetClientOwner().SetScore( "isdrawing", false );
			CurrentPlayer?.HideFromCanvas();
			
			CurrentPlayer = Players.FirstOrDefault(x => !PlayedThisPeriod.Contains( x ));
			if ( CurrentPlayer == null )
			{
				Log.Error( "No more players left for this round!" );

				// TODO: This is a workaround until periods are finished for real
				PlayedThisPeriod.Clear();
				StartNextRound();
				return;
			}
			
			HasGivenHint = false;

			CurrentPlayer.PlaceOnCanvas();
			
			PlayedThisPeriod.Add( CurrentPlayer );
			
			Log.Info( $"Next player is {CurrentPlayer.GetClientOwner().Name}" );
			CurrentPlayer.GetClientOwner().SetScore( "isdrawing", true );

			this.CurrentRound = RoundKind.InGamePickNextWord;

			using ( Prediction.Off() )
			{
				// Send the next words to the current player
				this.SendNextRound(To.Single( CurrentPlayer ), this.CurrentRound, candidates[0], candidates[1], candidates[2]);
				this.SendNextRound(To.Multiple( Client.All.Where( x => x.Pawn != CurrentPlayer ) ), this.CurrentRound, null, null, null);
			}
		}

		[ClientRpc]
		public void SendNextRound(RoundKind round, string word1, string word2, string word3)
		{
			Log.Info( $"[ROUNDCHANGE] Got next words! Next round is {round}" );

			if ( word1 != null )
			{
				Candidates.Clear();
				Candidates.Add( word1 );
				Candidates.Add( word2 );
				Candidates.Add( word3 );
				
				Log.Info( $"My words: '{word1}' '{word2}' '{word3}'" );
			}

			if ( round == RoundKind.InGame )
			{
				RoundStartTime = DateTimeOffset.Now;
				HasGivenHint = false;
				
				foreach ( var client in Client.All )
				{
					var player = client.Pawn as GuessPlayer;
					player.SendSound( "start" );
				}
			}

			if ( round == RoundKind.InGameAfterRound )
				RoundStartTime = DateTimeOffset.Now;
			
			CurrentRound = round;
		}

		public void RecalculateRanks()
		{
			Host.AssertServer();
			
			Log.Info( "Starting recalculate ranks..." );

			var players = Players.OrderByDescending( x => x.GetClientOwner().GetScore<int>( "points" ) ).ToArray();
			for ( var i = 1; i <= players.Length; i++ )
			{
				players[i - 1].GetClientOwner().SetScore( "rank", i );
				Log.Info( $"Player {players[i - 1].GetClientOwner().Name} is #{i}" );
			}
		}

		public void ClearSolvedState()
		{
			Host.AssertServer();

			foreach (var client in Client.All)
			{
				client.SetScore( "solved", false );
			}
		}
		
		public override void PostLevelLoaded()
		{
			_ = StartSecondTimer();

			base.PostLevelLoaded();
		}

		[ClientRpc]
		public void ClearCanvas()
		{
			Log.Info( "ClearCanvas()" );
			
			foreach (var entity in All)
			{
				entity.RemoveAllDecals();
			}
		}

		public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
		{
			Log.Info( client.Name + " left, checking minimum player count..." );

			Players.Remove( client.Pawn as GuessPlayer );

			base.ClientDisconnect( client, reason );
		}
		
		public async Task StartSecondTimer()
		{
			while (true)
			{
				await Task.DelaySeconds( 1 );
				OnSecond();
			}
		}

		private void OnSecond()
		{
			//Log.Info( $"OnSecond: {IsServer} {this.GetSecondsLeft()} {HasGivenHint}" );
			if ( CurrentRound == RoundKind.InGame)
			{
				if ( this.GetSecondsLeft() == 0 )
				{
					//CurrentRound = RoundKind.InGameAfterRound;
					// TODO Results screen etc etc
					StartNextRound();
				}
			}
		}
		
		[Event("tick")]
		private void Tick()
		{
			if ( IsServer )
			{
				if ( CurrentRound == RoundKind.InGame && this.GetSecondsLeft() < 10.0 && !HasGivenHint )
				{
					var rng = new Random();
					var hinted = Words.GetWordWithPlaceholders( CurrentWord, rng.Next(CurrentWord.Length) );
					Log.Info( $"[S] Updating hint: {hinted}" );
					
					using ( Prediction.Off() )
					{
						NotifyCurrentWord( To.Multiple( Client.All.Where( x => x.Pawn != CurrentPlayer ) ), hinted );
						
						Log.Info( "[S] Playing last10 sound..." );
					
						foreach (var guessPlayer in Players)
						{
							guessPlayer.SendSound( "last10" );
						}
					}
					
					HasGivenHint = true;
				}
			}
		}
		
		[ClientRpc]
		public void AddToast( GuessPlayer player, string text, string iconClass = "" )
		{
			if ( player == null )
			{
				Log.Warning( "Player was NULL in Game.AddToast!" );
				return;
			}

			ToastList.Current.AddItem( player, text, iconClass );
		}
		
		//TODO!!! I don't want to use decals at all for this
		[ClientRpc]
		public void PlacePaint( Vector3 position )
		{
			Host.AssertClientOrMenu();
			
			//Log.Info( $"Painting {Toolbar.Instance.ActiveColor}" );
			
			var mat = Material.Load( $"materials/paint{Toolbar.Instance.ActiveColorName}.vmat" );

			Decals.Place( mat, position, 17.0f, Rotation.Identity);
		}

		/// <summary>
		/// A client has joined the server. Make them a pawn to play with
		/// </summary>
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new GuessPlayer();
			client.Pawn = player;
			client.SetScore( "steamid", client.SteamId );

			//var rng = new Random();
			//client.SetScore( "points", rng.Next(0, 5000) );
			
			Players.Add( player );
			
			if ( IsServer )
			{
				AddToast( To.Everyone, player, $"{client.Name} joined the game!" );
				this.RecalculateRanks();
			}
			
			// TODO: Sync running game to client
			
			player.Respawn();
		}
	}

}
