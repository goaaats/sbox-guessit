namespace guessit.Player.Camera
{
	class StaticCamera : Sandbox.Camera
	{
		public override void Activated()
		{
			Pos = new Vector3( -608, -2286, 400 );
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
