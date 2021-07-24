using Sandbox;
using guessit.Player.Camera;

namespace guessit.Player
{
	[Library]
	public partial class GuessPlayer : Sandbox.Player
	{
		public GuessPlayer()
		{
			Transmit = TransmitType.Always;
		}
		
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new TopDownWalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			//
			// Use ThirdPersonCamera (you can make your own Camera for 100% control)
			//
			//Camera = new ThirdPersonCamera();
			Camera = new StaticCamera();

			EnableAllCollisions = false;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		public override void Spawn()
		{
			base.Spawn();
			
		}
		
		public void PlaceOnCanvas()
		{
			EnableShadowCasting = true;
			Position = new Vector3( -608, -2286, 64 );
		}

		// TODO this is terrible
		public void HideFromCanvas()
		{
			EnableShadowCasting = false;
			Position = new Vector3( 10000, 10000, 10000 );
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );

			//
			// If we're running serverside and Attack1 was just pressed, spawn a ragdoll
			//
			if ( IsServer && Input.Down( InputButton.Attack1 ) || Input.Down( InputButton.Jump ) )
			{
				//if (this.cntPlaced % 1 != 0)
				//	return;
				
				/*
				var ragdoll = new ModelEntity();
				ragdoll.SetModel( "models/citizen/citizen.vmdl" );  
				ragdoll.Position = EyePos + EyeRot.Forward * 40;
				ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
				ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
				*/
				//var mat = Material.Load( "materials/decals/decalgraffiti001b.vmat" );
				
				GuessItGame.Instance.PlacePaint( Position );
			}
			
			//DebugOverlay.ScreenText( new Vector2( 30, 40 ), Position.ToString() );
			
			if ( IsServer && Input.Pressed( InputButton.Attack2 ) )
			{
				/*
				var ragdoll = new ModelEntity();
				ragdoll.SetModel( "models/citizen/citizen.vmdl" );  
				ragdoll.Position = EyePos + EyeRot.Forward * 40;
				ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
				ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
				ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
				*/

				GuessItGame.Instance.ClearCanvas();
				Log.Info( "Removing all decals..." );
			}
		}

		[ClientRpc]
		public void SendSound( string soundName )
		{
			PlaySound( soundName );
		}
		
		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
