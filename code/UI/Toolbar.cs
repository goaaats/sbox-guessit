using System.Collections.Generic;
using guessit.Player;
using Sandbox;
using Sandbox.UI;

namespace guessit.UI
{
	public partial class Toolbar : Panel
	{
		public static Toolbar Instance { get; private set; }
		
		private readonly List<ToolbarIcon> slots = new();

		private int activeSlot = 0;

		public string ActiveColorName => this.slots[this.activeSlot].TargetColorName;
		
		public Toolbar()
		{
			Instance = this;
			
			this.StyleSheet.Load( "/ui/Toolbar.scss" );
			AddClass( "InventoryBar" );
			
			for ( var i = 0; i < 9; i++ )
			{
				var icon = new ToolbarIcon( i + 1, this );

				switch ( i )
				{
					case 0: 
						icon.TargetColorName = "red";
						icon.BackgroundColor = Color.Red.WithAlpha( 0.3f );;
						break;
					case 1: 
						icon.TargetColorName = "blue";
						icon.BackgroundColor = Color.Blue.WithAlpha( 0.3f );
						break;
					case 2: 
						icon.TargetColorName = "brown";
						icon.BackgroundColor = Color.Parse( "#8f4f00" ).Value.WithAlpha( 0.3f );
						break;
					case 3: 
						icon.TargetColorName = "orange";
						icon.BackgroundColor = Color.Orange.WithAlpha( 0.3f );
						break;
					case 4: 
						icon.TargetColorName = "green";
						icon.BackgroundColor = Color.Green.WithAlpha( 0.3f );
						break;
					case 5: 
						icon.TargetColorName = "purple";
						icon.BackgroundColor = Color.Parse( "#8f34eb" ).Value.WithAlpha( 0.3f );
						break;
					case 6: 
						icon.TargetColorName = "yellow";
						icon.BackgroundColor = Color.Yellow.WithAlpha( 0.3f );
						break;
					case 7: 
						icon.TargetColorName = "black";
						icon.BackgroundColor = Color.Black.WithAlpha( 0.3f );
						break;
					case 8:
						icon.TargetColorName = "clear";
						icon.BackgroundColor = Color.White.WithAlpha( 0.1f );
						break;
				}
				
				this.slots.Add( icon );
			}
		}

		public override void Tick()
		{
			base.Tick();

			for ( var i = 0; i < this.slots.Count; i++ )
			{
				UpdateIcon( this.slots[i].TargetColorName, this.slots[i], i );
			}
		}

		private void UpdateIcon( string color, ToolbarIcon inventoryIcon, int i )
		{
			/*
			if ( ent == null )
			{
				inventoryIcon.Clear();
				return;
			}
			*/

			inventoryIcon.TargetColorName = color;
			inventoryIcon.Label.Text = color.ToString();
			inventoryIcon.Style.BackgroundColor = inventoryIcon.BackgroundColor;
			inventoryIcon.Style.Dirty();
			inventoryIcon.SetClass( "active", i == this.activeSlot );
		}

		[Event( "buildinput" )]
		public void ProcessClientInput( InputBuilder input )
		{
			
			if ( input.Pressed( InputButton.Slot1 ) )
			{
				SetActiveSlot( 0 );
			}

			if ( input.Pressed( InputButton.Slot2 ) )
			{
				SetActiveSlot( 1 );
			}

			if ( input.Pressed( InputButton.Slot3 ) )
			{
				SetActiveSlot( 2 );
			}

			if ( input.Pressed( InputButton.Slot4 ) )
			{
				SetActiveSlot( 3 );
			}

			if ( input.Pressed( InputButton.Slot5 ) )
			{
				SetActiveSlot( 4 );
			}

			if ( input.Pressed( InputButton.Slot6 ) )
			{
				SetActiveSlot( 5 );
			}

			if ( input.Pressed( InputButton.Slot7 ) )
			{
				SetActiveSlot( 6 );
			}

			if ( input.Pressed( InputButton.Slot8 ) )
			{
				SetActiveSlot( 7 );
			}

			// Clears the canvas? Maybe?
			if ( input.Pressed( InputButton.Slot9 ) )
			{
				SetActiveSlot( 0 );
				GuessItGame.ClearCanvas();
				GuessItGame.Instance.AddToast( Local.Pawn as GuessPlayer, "Canvas cleared!" );
			}

			if ( input.MouseWheel != 0 )
			{
				SwitchActiveSlot( -input.MouseWheel );
			}
		}

		private void SetActiveSlot( int i )
		{
			this.activeSlot = i;
		}

		private void SwitchActiveSlot( int idelta )
		{
			var slot = this.activeSlot;
			var nextSlot = slot + idelta;

			while ( nextSlot < 0 )
			{
				nextSlot += this.slots.Count;
			}

			while ( nextSlot >= this.slots.Count )
			{
				nextSlot -= this.slots.Count;
			}

			SetActiveSlot( nextSlot );
		}
	}
}
