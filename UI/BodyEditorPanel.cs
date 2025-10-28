using Gum.Forms.Controls;
using Gum.Forms.DefaultVisuals;
using Microsoft.Xna.Framework;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Newtonsoft.Json;
using nkast.Aether.Physics2D.Dynamics;
using Platformer.Component;
using System;
using System.Diagnostics;

namespace Platformer.UI
{
    class BodyEditorPanel : Panel
    {
        public BodyEditorPanel(Body body)
        {
            Visual.ClipsChildren = false;
            Visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            Visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            Width = 20;
            Height = 20;

            var background = new NineSliceRuntime();
            AddChild(background);
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = Styling.ActiveStyle.SpriteSheet;
            background.ApplyState(Styling.ActiveStyle.NineSlice.Panel);

            InnerStackPanel = new StackPanel();
            AddChild(InnerStackPanel);
            InnerStackPanel.Spacing = 10;
            InnerStackPanel.Anchor(Gum.Wireframe.Anchor.Center);

            var headerPanel = new Panel();
            InnerStackPanel.AddChild(headerPanel);
            headerPanel.Visual.WidthUnits = Gum.DataTypes.DimensionUnitType.PercentageOfParent;
            headerPanel.Visual.Width = 100;
            headerPanel.Visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToChildren;
            headerPanel.Visual.Dock(Gum.Wireframe.Dock.Top);

            var headerLabel = new Label() { Text = $"Entity: {body.Tag}" };
            headerPanel.AddChild(headerLabel);
            headerLabel.Anchor(Gum.Wireframe.Anchor.Left);

            var removeButton = new Button { Text = "×", Width = 20 };
            headerPanel.AddChild(removeButton);
            removeButton.Anchor(Gum.Wireframe.Anchor.Right);
            removeButton.Push += (_, _) => this.RemoveFromRoot();

            var bodyStack = new StackPanel() { Spacing = 10 };
            InnerStackPanel.AddChild(bodyStack);

            bodyStack.AddChild(new Label() { Text = "Body Type:" });

            var bodyTypeComboBox = new ComboBox();
            bodyStack.AddChild(bodyTypeComboBox);
            bodyTypeComboBox.Items.Add(BodyType.Static);
            bodyTypeComboBox.Items.Add(BodyType.Kinematic);
            bodyTypeComboBox.Items.Add(BodyType.Dynamic);
            bodyTypeComboBox.SelectedObject = body.BodyType;
            bodyTypeComboBox.SelectionChanged += (_, _) =>
            {
                try
                {
                    body.BodyType = (BodyType)bodyTypeComboBox.SelectedObject;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            };

            bodyStack.AddChild(new Label { Text = $"Position:" });

            var positionTextBox = new TextBox
            {
                Text = JsonConvert.SerializeObject(body.Position),
                Width = 0
            };
            bodyStack.AddChild(positionTextBox);
            positionTextBox.Visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            positionTextBox.LostFocus += (_, _) =>
            {
                try
                {
                    var newPosition = JsonConvert.DeserializeObject<Vector2>(positionTextBox.Text);
                    body.Position = newPosition;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            };

            var isEnabledCheckBox = new CheckBox() { Text = $"Is Enabled", IsChecked = body.Enabled };
            bodyStack.AddChild(isEnabledCheckBox);
            isEnabledCheckBox.Checked += (_, _) => body.Enabled = true;
            isEnabledCheckBox.Unchecked += (_, _) => body.Enabled = false;

            var isAwakeCheckBox = new CheckBox() { Text = $"Is Awake", IsChecked = body.Awake };
            bodyStack.AddChild(isAwakeCheckBox);
            isAwakeCheckBox.Checked += (_, _) => body.Awake = true;
            isAwakeCheckBox.Unchecked += (_, _) => body.Awake = false;

            foreach (var fixture in body.FixtureList)
            {
                AddFixtureControls(fixture);
            }
        }

        public StackPanel InnerStackPanel { get; }

        internal void AddFixtureControls(Fixture fixture)
        {
            var fixturePanel = new StackPanel();
            InnerStackPanel.AddChild(fixturePanel);

            fixturePanel.AddChild(new Label() { Text = "Density: " });

            var densityTextBox = new NumberBox() { Value = fixture.Shape.Density, TicksFrequency = 0.05f };
            fixturePanel.AddChild(densityTextBox);
            densityTextBox.ValueChanged += () =>
            {
                fixture.Shape.Density = (float)densityTextBox.Value;
                fixture.Body.ResetMassData();
            };

            fixturePanel.AddChild(new Label() { Text = "Friction: " });

            var frictionSlider = new NumberBox() { Minimum = 0, Value = fixture.Friction, TicksFrequency = 0.05f };
            fixturePanel.AddChild(frictionSlider);
            frictionSlider.ValueChanged += () =>
            {
                fixture.Friction = (float)frictionSlider.Value;
            };

            fixturePanel.AddChild(new Label() { Text = "Restitution: " });

            var restitutionSlider = new NumberBox() { Minimum = 0, Maximum = 1.0f, Value = fixture.Restitution, TicksFrequency = 0.05f };
            fixturePanel.AddChild(restitutionSlider);
            restitutionSlider.ValueChanged += () =>
            {
                fixture.Restitution = (float)restitutionSlider.Value;
            };
        }

        internal void AddPlayerControls(KeyboardController playerController)
        {
            var playerStack = new StackPanel();
            InnerStackPanel.AddChild(playerStack);

            playerStack.AddChild(new Label() { Text = "X-Movement Force:" });
            var horizontalMovementForceSlider = new NumberBox() { Minimum = 0, TicksFrequency = 0.5f };
            playerStack.AddChild(horizontalMovementForceSlider);
            horizontalMovementForceSlider.Value = playerController.HorizontalMovementForce;
            horizontalMovementForceSlider.ValueChanged += () 
                => playerController.HorizontalMovementForce = (float)horizontalMovementForceSlider.Value;

            playerStack.AddChild(new Label() { Text = "Max X-Velocity:" });
            var maxXSpeedNumberBox = new NumberBox() { Value = playerController.MaxHorizontalSpeed, Minimum = 0, TicksFrequency = 0.5f };
            playerStack.AddChild(maxXSpeedNumberBox);
            maxXSpeedNumberBox.Value = playerController.MaxHorizontalSpeed;
            maxXSpeedNumberBox.ValueChanged += ()
                => playerController.MaxHorizontalSpeed = (float)maxXSpeedNumberBox.Value;

            playerStack.AddChild(new Label() { Text = "Jump Force:" });
            var jumpForceTextBox = new NumberBox() { Value = playerController.JumpForce, Minimum = 0 };
            playerStack.AddChild(jumpForceTextBox);
            jumpForceTextBox.ValueChanged += () 
                => playerController.JumpForce = (float)jumpForceTextBox.Value;
            //public ushort MaxJumpTimeout { get; set; } = 4;
            //public ushort MaxZoomTimeout { get; set; } = 6;
        }
    }
}
