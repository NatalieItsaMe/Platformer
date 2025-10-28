using Gum.Forms.Controls;
using System;

namespace Platformer.UI
{
    internal class NumberBox : Panel
    {
        public double? Minimum { get; set; }
        public double? Maximum { get; set; }
        public double TicksFrequency { get; set; } = 1;
        public bool IsSnapToTickEnabled { get; set; }
        public string Format { get; set; } = "0.00";
        public Action ValueChanged { get; set; } = () => { };

        public double Value 
        { 
            get => _value; 
            set 
            {
                if (Maximum.HasValue && value >= Maximum.Value)
                    _value = Maximum.Value;
                else if (Minimum.HasValue && value <= Minimum.Value)
                    _value = Minimum.Value;
                else if (IsSnapToTickEnabled && value % TicksFrequency > 0.001)
                    _value = value - (value % TicksFrequency);
                else
                    _value = value;

                box.Text = _value.ToString(Format);
                ValueChanged();
            }
        }

        private readonly StackPanel stack;
        private readonly TextBox box;
        private readonly Button upButton;
        private readonly Button downButton;

        private double _value;

        public NumberBox()
        {
            stack = new StackPanel() { Orientation = Orientation.Horizontal };
            AddChild(stack);

            box = new TextBox() { Text = Value.ToString(Format) };
            stack.AddChild(box);
            box.LostFocus += (_, _) =>
            {
                if (double.TryParse(box.Text, out double newValue))
                    Value = newValue;
                else
                    box.Text = _value.ToString(Format);
            };

            upButton = new Button() { Text = "+" };
            stack.AddChild(upButton);
            upButton.Visual.Width = 32;
            upButton.Click += (_, _) =>
            {
                Value += TicksFrequency;
            };

            downButton = new Button() { Text = "-" };
            stack.AddChild(downButton);
            downButton.Visual.Width = 32;
            downButton.Click += (_, _) =>
            {
                Value -= TicksFrequency;
            };
        }

    }
}
