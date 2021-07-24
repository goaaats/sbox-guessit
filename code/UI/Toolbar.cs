using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace guessit.UI
{
	public partial class Toolbar : Panel
	{
		public static Toolbar Instance { get; private set; }
		
		private readonly List<ToolbarIcon> slots = new();

		private int activeSlot = 0;

		public string ActiveColor => this.slots[this.activeSlot].TargetColor;
		
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
						icon.TargetColor = "red";
						break;
					case 1: 
						icon.TargetColor = "blue";
						break;
					case 2: 
						icon.TargetColor = "brown";
						break;
					case 3: 
						icon.TargetColor = "orange";
						break;
					case 4: 
						icon.TargetColor = "green";
						break;
					case 5: 
						icon.TargetColor = "purple";
						break;
					case 6: 
						icon.TargetColor = "red";
						break;
					case 7: 
						icon.TargetColor = "red";
						break;
					case 8: 
						icon.TargetColor = "clear";
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
				UpdateIcon( this.slots[i].TargetColor, this.slots[i], i );
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

			inventoryIcon.TargetColor = color;
			inventoryIcon.Label.Text = color.ToString();
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

			if ( input.Pressed( InputButton.Slot9 ) )
			{
				SetActiveSlot( 8 );
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
