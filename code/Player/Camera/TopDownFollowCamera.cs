using System;
using Sandbox;

namespace guessit.Player.Camera
{
	class TopDownFollowCamera : Sandbox.Camera
	{
		public float Height { get; set; } = 200;

		public override void Activated()
		{
			//if ( Local.Pawn is MinimalPlayer player )
			//{
			//	Pos = player.Position;
			//	Rot = player.Rotation;
			//}

			Pos = new Vector3( 0, 0, 1000 );
			Rot = Rotation.LookAt( Vector3.Down );

			base.Activated();
		}

		public override void Update()
		{
			if ( Local.Pawn is GuessPlayer player )
			{
				FieldOfView = 70f;
				
				var center = player.Position + Vector3.Up * 64;
				//Pos = Pos.LerpTo( new Vector3( player.Position.x, player.Position.y, 1000 ), Time.Delta );
				//Rot = player.Rotation;
				
				Pos = center;
				Rot = Rotation.From( 90, 360, 0 );

				float distance = 130.0f * player.Scale;
				var targetPos = Pos;
				targetPos += Vector3.Up * Height;
				Pos = targetPos;
			}
			
			DebugOverlay.ScreenText( new Vector2( 20, 20 ), Pos.ToString() );

			Viewer = null;
		}
	}
}
