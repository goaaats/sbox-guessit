namespace guessit.Player.Camera
{
	class StaticCamera : Sandbox.Camera
	{
		public override void Activated()
		{
			Pos = new Vector3( -990, -1210, 470 );
			Rot = Rotation.LookAt( Vector3.Down );

			base.Activated();
		}

		public override void Update()
		{
			FieldOfView = 70f;

			Viewer = null;
		}
	}
}
